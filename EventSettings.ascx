<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EventSettings.ascx.cs" Inherits="DotNetNuke.Modules.Events.EventSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="URL" Src="~/controls/URLControl.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web.Deprecated" Namespace="DotNetNuke.Web.UI.WebControls" %>

<asp:Panel ID="pnlEventsModuleSettings" runat="server">
<div class="dnnForm EventSettings" id="EventSettings">
<asp:ValidationSummary ID="ValidationSummary1" runat="server" Width="100%" CssClass="dnnFormMessage dnnFormValidationSummary" DisplayMode="List"></asp:ValidationSummary>
<ul class="dnnAdminTabNav">
    <li>
        <a href="#GeneralSettings"><%= this.LocalizeString("GeneralSettings") %></a>
    </li>
    <li>
        <a href="#DisplaySettings"><%= this.LocalizeString("DisplaySettings") %></a>
    </li>
    <li>
        <a href="#EmailandReminderSettings"><%= this.LocalizeString("EmailandReminderSettings") %></a>
    </li>
    <%
        if (int.Parse(this.ddlSocialGroupModule.SelectedValue) != 3)
        {
    %><li>
        <a href="#EnrollandModSettings"><%= this.LocalizeString("EnrollandModSettings") %></a>
    </li><%
        }
         %>
    <%
        if (int.Parse(this.ddlSocialGroupModule.SelectedValue) == 1)
        {
    %><li>
        <a href="#SubcalendarSettings"><%= this.LocalizeString("SubcalendarSettings") %></a>
    </li><%
        }
         %>
    <%
        if (int.Parse(this.ddlSocialGroupModule.SelectedValue) != 3)
        {
    %><li>
        <a href="#SEOandSitemapSettings"><%= this.LocalizeString("SEOandSitemapSettings") %></a>
    </li><%
        }
         %>
    <li>
        <a href="#IntegrationSettings"><%= this.LocalizeString("IntegrationSettings") %></a>
    </li>
    <li>
        <a href="#TemplateSettings"><%= this.LocalizeString("TemplateSettings") %></a>
    </li>
</ul>
<div class="GeneralSettings dnnClear" id="GeneralSettings">
    <h2 class="dnnFormSectionHead" id="dnnPanel-AdminSettings">
        <a href="" class="dnnSectionExpanded"><%= this.LocalizeString("AdminSettings") %></a>
    </h2>
    <fieldset class="dnnClear">
        <div>
            <div class="dnnFormItem">
                <dnn:Label ID="plTimeInterval" Text="Edit Time Interval:" runat="server" ControlName="txtTimeInterval"></dnn:Label>
                <asp:DropDownList ID="ddlTimeInterval" runat="server" CssClass="NormalTextBox" Width="60px">
                    <asp:ListItem Value="30">30</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plAllowRecurring" Text="Event Height/Width:" runat="server" ControlName="txtEventWidth"></dnn:Label>
                <asp:CheckBox ID="chkAllowRecurring" runat="server" Checked="True"></asp:CheckBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plMaxRecurrences" runat="server" Text="Max Generated Occurences:" ControlName="txtMaxRecurrences"></dnn:Label>
                <asp:TextBox ID="txtMaxRecurrences" runat="server" CssClass="NormalTextBox" Width="48px">1000</asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plPreventConflicts" Text="Event Height/Width:" runat="server" ControlName="txtEventWidth"></dnn:Label>
                <asp:CheckBox ID="chkPreventConflicts" runat="server"></asp:CheckBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plLocationConflict" Text="Location Conflict:" runat="server" ControlName="chkLocationConflict"></dnn:Label>
                <asp:CheckBox ID="chkLocationConflict" runat="server"></asp:CheckBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plEnableSearch" Text="Event Height/Width:" runat="server" ControlName="txtEventWidth"></dnn:Label>
                <asp:CheckBox ID="chkEnableSearch" runat="server" Checked="True"></asp:CheckBox>
            </div>
            <%
                if (int.Parse(this.ddlSocialGroupModule.SelectedValue) == 1)
                {
            %>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblOwnerChangeAllowed" runat="server" Text="Owner Change Allowed" ControlName="lblOwnerChangeAllowed"></dnn:Label>
                    <asp:CheckBox ID="chkOwnerChangeAllowed" runat="server" Checked="False"></asp:CheckBox>
                </div>
            <%
                }
            %>
            <div class="dnnFormItem">
                <dnn:Label ID="plExpireEvents" runat="server" Text="Expire Events Older Than:" ControlName="txtExpireEvents"></dnn:Label>
                <asp:TextBox ID="txtExpireEvents" runat="server" CssClass="NormalTextBox" Width="32px"></asp:TextBox>&nbsp;
                <asp:Label ID="plDays" resourcekey="plDays" runat="server">days</asp:Label>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plPrivateMessage" runat="server" recourcekey="PrivateMessage" Text="Private Calendar Message:" ControlName="txtPrivateMessage"></dnn:Label>
                <asp:TextBox ID="txtPrivateMessage" runat="server" CssClass="NormalTextBox" Width="300px"></asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plModuleCategory" runat="server" recourcekey="ModuleCategory" Text="Filter Events by Category:" ControlName="ddlModuleCategories"></dnn:Label>
                <dnn:DnnComboBox ID="ddlModuleCategories" runat="server" width="150px" CheckBoxes="True" EnableCheckAllItemsCheckBox="true"
                                 AllowCustomText="False" DataValueField="Category" DataTextField="CategoryName">
                </dnn:DnnComboBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plModuleLocation" runat="server" recourcekey="ModuleLocation" Text="Filter Events by Location:" ControlName="ddlModuleLocations"></dnn:Label>
                <dnn:DnnComboBox ID="ddlModuleLocations" runat="server" width="150px" CheckBoxes="True" EnableCheckAllItemsCheckBox="true"
                                 AllowCustomText="False" DataValueField="Location" DataTextField="LocationName">
                </dnn:DnnComboBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblStandardEmail" runat="server" Text="Emails From:" ControlName="txtStandardEmail"></dnn:Label>
                <asp:TextBox ID="txtStandardEmail" runat="server" MaxLength="100" Wrap="False" CssClass="NormalTextBox" Width="300px"></asp:TextBox>
                <asp:RegularExpressionValidator ID="valStandardEmail" runat="server" ControlToValidate="txtStandardEmail"
                                                ErrorMessage="Valid Email Address required" resourcekey="valStandardEmail" cssclass="dnnFormMessage dnnFormError"
                                                ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*">
                </asp:RegularExpressionValidator>
            </div>
        </div>
    </fieldset>
    <h2 class="dnnFormSectionHead" id="dnnPanel-LookFeelSettings">
        <a href="" class=""><%= this.LocalizeString("LookFeelSettings") %></a>
    </h2>
    <fieldset class="dnnClear">
        <div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblTheme" runat="server" Text="Event Height/Width:" ControlName="txtEventWidth"></dnn:Label>
                <table cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <asp:RadioButton GroupName="Theme" ID="rbThemeStandard" runat="server" Text="Standard" Width="150px" style="white-space: nowrap"/>
                            <asp:DropDownList ID="ddlThemeStandard" runat="server" CssClass="NormalTextBox"/>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:RadioButton GroupName="Theme" ID="rbThemeCustom" runat="server" Text="Custom" Width="150px" style="white-space: nowrap"/>
                            <asp:DropDownList ID="ddlThemeCustom" runat="server" CssClass="NormalTextBox"/>
                        </td>
                    </tr>
                </table>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblDefaultView" runat="server" Text="Event Height/Width:" ControlName="txtEventWidth"></dnn:Label>
                <asp:DropDownList ID="ddlDefaultView" runat="server" CssClass="NormalTextBox" Width="178px">
                    <asp:ListItem Value="EventMonth.ascx">Month</asp:ListItem>
                    <asp:ListItem Value="EventWeek.ascx">Week</asp:ListItem>
                    <asp:ListItem Value="EventList.ascx">List</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblAllowedViews" recourcekey="lblAllowedViews" runat="server" Text=""></dnn:Label>
                <table cellpadding="0" cellspacing="0">
                    <tr>
                        <td style="white-space: nowrap">
                            <asp:CheckBox ID="chkMonthAllowed" runat="server" class="autoWidth"></asp:CheckBox>
                            <asp:CheckBox ID="chkWeekAllowed" runat="server" class="autoWidth"></asp:CheckBox>
                            <asp:CheckBox ID="chkListAllowed" runat="server" class="autoWidth"></asp:CheckBox>
                        </td>
                    </tr>
                </table>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblEnableContainerSkin" runat="server"></dnn:Label>
                <asp:CheckBox ID="chkEnableContainerSkin" runat="server" Checked="true"></asp:CheckBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblCategoriesEnable" runat="server" ControlName="ddlEnableCategories"></dnn:Label>
                <asp:DropDownList ID="ddlEnableCategories" runat="server" CssClass="NormalTextBox" Width="178px"/>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblRestrictCategories" runat="server" ControlName="chkRestrictCategories"></dnn:Label>
                <asp:CheckBox ID="chkRestrictCategories" runat="server" Checked="false"></asp:CheckBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblLocationsEnable" runat="server" ControlName="ddlEnablelocations"></dnn:Label>
                <asp:DropDownList ID="ddlEnableLocations" runat="server" CssClass="NormalTextBox" Width="178px"/>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblRestrictLocations" runat="server" ControlName="chkRestrictLocations"></dnn:Label>
                <asp:CheckBox ID="chkRestrictLocations" runat="server" Checked="false"></asp:CheckBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plEnableEventNav" Text="Enable Date Navigation Controls" runat="server" ControlName="txtEnableEventNav"></dnn:Label>
                <asp:CheckBox ID="chkEnableEventNav" runat="server"></asp:CheckBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plIconBar" runat="server" Text="Icon Bar:" ControlName="txtIconBar"></dnn:Label>
                <asp:RadioButtonList ID="rblIconBar" runat="server" RepeatDirection="Vertical" CssClass="dnnFormRadioButtons">
                    <asp:ListItem Value="Top" Selected="True" resourcekey="plIconBarTop"></asp:ListItem>
                    <asp:ListItem Value="Bottom" resourcekey="plIconBarBottom"></asp:ListItem>
                    <asp:ListItem Value="None" resourcekey="plIconBarNone"></asp:ListItem>
                </asp:RadioButtonList>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plHTMLEmail" runat="server" Text="HTML Email:" ControlName="rblHTMLEmail"></dnn:Label>
                <asp:RadioButtonList ID="rblHTMLEmail" runat="server" RepeatDirection="Vertical" CssClass="dnnFormRadioButtons">
                    <asp:ListItem Value="Html" Selected="True" resourcekey="plHTMLEmailHtml"></asp:ListItem>
                    <asp:ListItem Value="Auto" resourcekey="plHTMLEmailAuto"></asp:ListItem>
                    <asp:ListItem Value="Text" resourcekey="plHTMLEmailText"></asp:ListItem>
                </asp:RadioButtonList>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plWeekStart" runat="server"></dnn:Label>
                <asp:DropDownList ID="ddlWeekStart" runat="server" CssClass="NormalTextBox">
                </asp:DropDownList>
            </div>
        </div>
    </fieldset>
    <h2 class="dnnFormSectionHead" id="dnnPanel-TimeZoneSettings">
        <a href="" class=""><%= this.LocalizeString("TimeZoneSettings") %></a>
    </h2>
    <fieldset class="dnnClear">
        <div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblTimeZone" runat="server"></dnn:Label>
                <dnn:DnnTimeZoneComboBox ID="cboTimeZone" runat="server" CssClass="NormalTextBox"/>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plTZDisplay" runat="server" Text="Event Height/Width:" ControlName="txtEventWidth"></dnn:Label>
                <asp:CheckBox ID="chkTZDisplay" runat="server"></asp:CheckBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblPrimaryTimeZone" runat="server" Text="Primary Timezone:" ControlName="ddlPrimaryTimeZone"></dnn:Label>
                <asp:DropDownList ID="ddlPrimaryTimeZone" runat="server" CssClass="NormalTextBox" Width="178px"/>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblSecondaryTimeZone" runat="server" Text="Secondary Timezone:" ControlName="ddlSecondaryTimeZone"></dnn:Label>
                <asp:DropDownList ID="ddlSecondaryTimeZone" runat="server" CssClass="NormalTextBox" Width="178px"/>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plEnableEventTimeZones" runat="server" Text="Enable per event Timezones:" ControlName="chkEnableEventTimeZones"></dnn:Label>
                <asp:CheckBox ID="chkEnableEventTimeZones" runat="server"></asp:CheckBox>
            </div>
        </div>
    </fieldset>
</div>
<div class="DisplaySettings dnnClear" id="DisplaySettings">
<h2 class="dnnFormSectionHead" id="dnnPanel-DetailSettings">
    <a href="" class="dnnSectionExpanded"><%= this.LocalizeString("DetailSettings") %></a>
</h2>
<fieldset class="dnnClear">
    <div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblEventDetailNewPage" runat="server"></dnn:Label>
            <asp:CheckBox ID="chkEventDetailNewPage" runat="server"></asp:CheckBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblDetailPageAllowed" runat="server" Text="Set Event Detail Page Allowed" ControlName="lblDetailPageAllowed"></dnn:Label>
            <asp:CheckBox ID="chkDetailPageAllowed" runat="server" Checked="False"></asp:CheckBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblEnrollmentPageAllowed" runat="server" Text="Set Enrollment Page Allowed" ControlName="lblEnrollmentPageAllowed"></dnn:Label>
            <asp:CheckBox ID="chkEnrollmentPageAllowed" runat="server" Checked="False"></asp:CheckBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblEnrollmentPageDefaultURL" runat="server" Text="Enrollment Page Default URL:" ControlName="txtEnrollmentPageDefaultURL"></dnn:Label>
            <asp:TextBox ID="txtEnrollmentPageDefaultURL" runat="server" CssClass="NormalTextBox" Width="300px" MaxLength="250"></asp:TextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblEnableEnrollPopup" runat="server" Text="Enable Enroll Popup Validation:" ControlName="lblEnableEnrollPopup"></dnn:Label>
            <asp:CheckBox ID="chkEnableEnrollPopup" runat="server" Checked="True"></asp:CheckBox>
        </div>
    </div>
</fieldset>
<h2 class="dnnFormSectionHead" id="dnnPanel-MonthSettings">
    <a href="" class=""><%= this.LocalizeString("MonthSettings") %></a>
</h2>
<fieldset class="dnnClear">
    <div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblMonthCellEvents" runat="server" Text="Enable Month View Cell Events"></dnn:Label>
            <asp:CheckBox ID="chkMonthCellEvents" runat="server"></asp:CheckBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plShowEventsAlways" Text="Show Events in Next/Prev Month" runat="server" ControlName="txtShowEventsAlways"></dnn:Label>
            <asp:CheckBox ID="chkShowEventsAlways" runat="server"></asp:CheckBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plFridayWeekend" Text="Event Height/Width:" runat="server" ControlName="txtEventWidth"></dnn:Label>
            <asp:CheckBox ID="chkFridayWeekend" runat="server"></asp:CheckBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plTimeInTitle" Text="Show Event Start Time in Title" runat="server" ControlName="txtTimeInTitle"></dnn:Label>
            <asp:CheckBox ID="chkTimeInTitle" runat="server"></asp:CheckBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblEventDayNewPage" runat="server" Text="Event Day New Page:" ControlName="txtEventDayNewPage"></dnn:Label>
            <asp:CheckBox ID="chkEventDayNewPage" runat="server"></asp:CheckBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblMonthDaySelect" runat="server" Text="Enable selectable day:" ControlName="chkMonthDaySelect"></dnn:Label>
            <asp:CheckBox ID="chkMonthDaySelect" Checked="false" runat="server"></asp:CheckBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblIconsMonth" runat="server" Text="Show Icons:"></dnn:Label>
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <asp:CheckBox ID="chkIconMonthPrio" Checked="true" runat="server" class="autoWidth"></asp:CheckBox>&nbsp;<asp:Image ID="imgIconMonthPrioHigh" ImageUrl="Images/HighPrio.gif" runat="server"></asp:Image>&nbsp;<asp:Image ID="imgIconMonthPrioLow" ImageUrl="Images/LowPrio.gif" runat="server"></asp:Image>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox ID="chkIconMonthRec" Checked="true" runat="server" class="autoWidth"></asp:CheckBox>&nbsp;<asp:Image ID="imgIconMonthRec" ImageUrl="Images/rec.gif" runat="server"></asp:Image>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox ID="chkIconMonthReminder" Checked="true" runat="server" class="autoWidth"></asp:CheckBox>&nbsp;<asp:Image ID="imgIconMonthReminder" ImageUrl="Images/bell.gif" runat="server"></asp:Image>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox ID="chkIconMonthEnroll" Checked="true" runat="server" class="autoWidth"></asp:CheckBox>&nbsp;<asp:Image ID="imgIconMonthEnroll" ImageUrl="Images/enroll.gif" runat="server"></asp:Image>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</fieldset>
<h2 class="dnnFormSectionHead" id="dnnPanel-WeekSettings">
    <a href="" class=""><%= this.LocalizeString("WeekSettings") %></a>
</h2>
<fieldset class="dnnClear">
    <div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblFullTimeScale" runat="server" Text="Week View Full Time Scale" ControlName="txtFullTimeScale"></dnn:Label>
            <asp:CheckBox ID="chkFullTimeScale" runat="server"></asp:CheckBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblIncludeEndValue" runat="server" Text="Include End Value" ControlName="txtIncludeEndValue"></dnn:Label>
            <asp:CheckBox ID="chkIncludeEndValue" runat="server"></asp:CheckBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblShowValueMarks" runat="server" Text="Week View Value Marks" ControlName="txtShowValueMarks"></dnn:Label>
            <asp:CheckBox ID="chkShowValueMarks" runat="server"></asp:CheckBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblIconsWeek" runat="server" Text="Show Icons:"></dnn:Label>
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <asp:CheckBox ID="chkIconWeekPrio" Checked="true" runat="server" class="autoWidth"></asp:CheckBox>&nbsp;<asp:Image ID="imgIconWEEKPrioHigh" ImageUrl="Images/HighPrio.gif" runat="server"></asp:Image>&nbsp;<asp:Image ID="imgIconWeekPrioLow" ImageUrl="Images/LowPrio.gif" runat="server"></asp:Image>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox ID="chkIconWeekRec" Checked="true" runat="server" class="autoWidth"></asp:CheckBox>&nbsp;<asp:Image ID="imgIconWeekRec" ImageUrl="Images/rec.gif" runat="server"></asp:Image><br/>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox ID="chkIconWeekReminder" Checked="true" runat="server" class="autoWidth"></asp:CheckBox>&nbsp;<asp:Image ID="imgIconWeekReminder" ImageUrl="Images/bell.gif" runat="server"></asp:Image>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox ID="chkIconWeekEnroll" Checked="true" runat="server" class="autoWidth"></asp:CheckBox>&nbsp;<asp:Image ID="imgIconWeekEnroll" ImageUrl="Images/enroll.gif" runat="server"></asp:Image>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</fieldset>
<h2 class="dnnFormSectionHead" id="dnnPanel-EventListSettings">
    <a href="" class=""><%= this.LocalizeString("EventListSettings") %></a>
</h2>
<fieldset class="dnnClear">
    <div>
        <div class="dnnFormItem">
            <dnn:Label ID="plListViewGrid" Text="Grid or Repeater:" runat="server" ControlName="rblListViewGrid"></dnn:Label>
            <asp:RadioButtonList ID="rblListViewGrid" runat="server" RepeatDirection="Horizontal" CssClass="dnnFormRadioButtons">
                <asp:ListItem Value="Grid" Selected="True" resourcekey="plGrid">Grid</asp:ListItem>
                <asp:ListItem Value="Repeater" resourcekey="plRepeater">Repeater</asp:ListItem>
            </asp:RadioButtonList>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plShowHeader" Text="Show Table Header:" runat="server" ControlName="rblShowHeader"></dnn:Label>
            <asp:RadioButtonList ID="rblShowHeader" runat="server" RepeatDirection="Horizontal" CssClass="dnnFormRadioButtons">
                <asp:ListItem Value="Yes" Selected="True" resourcekey="plYes">Yes</asp:ListItem>
                <asp:ListItem Value="No" resourcekey="plNo">No</asp:ListItem>
            </asp:RadioButtonList>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plSelectedDays" Text="Selected Days:" runat="server" ControlName="rblSelectionTypeDays"></dnn:Label>
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td style="white-space: nowrap; width: 28px;">
                        <input id="rblSelectionTypeDays" type="radio" value="DAYS" name="rblSelectionType" runat="server"/>
                    </td>
                    <td style="white-space: nowrap;">
                        <asp:TextBox ID="txtDaysBefore" runat="server" CssClass="NormalTextBox" Width="32px">1</asp:TextBox>&nbsp;<asp:Label ID="Label4" resourcekey="plDaysBeforeCurrent" runat="server" EnableViewState="False">days before current date</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <asp:TextBox ID="txtDaysAfter" runat="server" CssClass="NormalTextBox" Width="32px">7</asp:TextBox>&nbsp;<asp:Label ID="Label8" resourcekey="plDaysAfterCurrent" runat="server" EnableViewState="False">days after current date</asp:Label>
                    </td>
                </tr>
            </table>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plSelectedNumEvents" Text="Select Number of Events:" runat="server" ControlName="rblSelectionTypeEvents"></dnn:Label>
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td style="white-space: nowrap; width: 28px;">
                        <input id="rblSelectionTypeEvents" type="radio" value="EVENTS" name="rblSelectionType" runat="server" checked="True"/>
                    </td>
                    <td style="white-space: nowrap;">
                        <asp:TextBox ID="txtNumEvents" runat="server" CssClass="NormalTextBox" Width="32px">10</asp:TextBox>&nbsp;<asp:Label ID="Label6" resourcekey="plEventsCurrentDate" runat="server" EnableViewState="False">events from current date</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <asp:TextBox ID="txtEventDays" runat="server" CssClass="NormalTextBox" Width="40px">365</asp:TextBox>&nbsp;<asp:Label ID="Label10" resourcekey="plDays" runat="server" EnableViewState="False">days</asp:Label>
                    </td>
                </tr>
            </table>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plRestrictCategoriesToTimeFrame" runat="server" Text="Restrict Categories to Time Frame" ControlName="chkRestrictCategoriesToTimeFrame"></dnn:Label>
            <asp:CheckBox ID="chkRestrictCategoriesToTimeFrame" runat="server"></asp:CheckBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plRestrictLocationsToTimeFrame" runat="server" Text="Restrict Locations to Time Frame" ControlName="chkRestrictLocationsToTimeFrame"></dnn:Label>
            <asp:CheckBox ID="chkRestrictLocationsToTimeFrame" runat="server"></asp:CheckBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plEventsFields" Text="Select the Events Fields to Display:" runat="server" ControlName="tblListColumns"></dnn:Label>
            <table id="tblListColumns" cellspacing="0" cellpadding="2" border="0">
                <tr>
                    <td class="NormalBold" align="center">
                        <asp:Label ID="plAvailable" resourcekey="plAvailable" runat="server" EnableViewState="False">Available</asp:Label>
                    </td>
                    <td align="center">
                        &nbsp;
                    </td>
                    <td class="NormalBold" align="center">
                        <asp:Label ID="plSelected" resourcekey="plSelected" runat="server" EnableViewState="False">Selected</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td valign="top" align="center">
                        <asp:ListBox class="NormalTextBox" ID="lstAvailable" runat="server" Width="200px" SelectionMode="Multiple" Height="150px"></asp:ListBox>
                    </td>
                    <td valign="middle" align="center">
                        <table id="Table3" cellspacing="0" cellpadding="0" border="0">
                            <tr>
                                <td valign="top" align="center">
                                    <asp:LinkButton ID="cmdAdd" CssClass="CommandButton" runat="server" EnableViewState="False" Text="&nbsp;>&nbsp;" OnClick="cmdAdd_Click"></asp:LinkButton>
                                </td>
                            </tr>
                            <tr>
                                <td valign="top" align="center">
                                    <asp:LinkButton ID="cmdRemove" CssClass="CommandButton" runat="server" EnableViewState="False" Text="&nbsp;<&nbsp;" OnClick="cmdRemove_Click"></asp:LinkButton>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td valign="bottom" align="center">
                                    <asp:LinkButton ID="cmdAddAll" CssClass="CommandButton" runat="server" EnableViewState="False" Text="&nbsp;>>&nbsp;" OnClick="cmdAddAll_Click"></asp:LinkButton>
                                </td>
                            </tr>
                            <tr>
                                <td valign="bottom" align="center">
                                    <asp:LinkButton ID="cmdRemoveAll" CssClass="CommandButton" runat="server" EnableViewState="False" Text="&nbsp;<<&nbsp;" OnClick="cmdRemoveAll_Click"></asp:LinkButton>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td valign="top" align="center">
                        <asp:ListBox class="NormalTextBox" ID="lstAssigned" runat="server" Width="200px" SelectionMode="Multiple" Height="150px"></asp:ListBox>
                    </td>
                </tr>
            </table>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plListPageSize" Text="Page size:" runat="server"/>
            <asp:DropDownList ID="ddlPageSize" CssClass="NormalTextBox" Width="200px" runat="server">
                <asp:ListItem Text="10" Value="10"/>
                <asp:ListItem Text="20" Value="20"/>
                <asp:ListItem Text="50" Value="50"/>
                <asp:ListItem Text="100" Value="100"/>
            </asp:DropDownList>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plListDefaultSort" Text="Default sorting:" runat="server"/>
            <asp:DropDownList ID="ddlListSortedFieldDirection" CssClass="NormalTextBox" Width="200px" runat="server"/>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plListDefaultColumn" Text="Default sort column:" runat="server"/>
            <asp:DropDownList ID="ddlListDefaultColumn" CssClass="NormalTextBox" Width="200px" runat="server"/>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblCollapseRecurring" runat="server" Text="Collapse Recurring" ControlName="txtCollapseRecurring"></dnn:Label>
            <asp:CheckBox ID="chkCollapseRecurring" runat="server"></asp:CheckBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblIconsList" runat="server" Text="Show Icons:"></dnn:Label>
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <asp:CheckBox ID="chkIconListPrio" Checked="true" runat="server" class="autoWidth"></asp:CheckBox>&nbsp;<asp:Image ID="imgIconListPrioHigh" ImageUrl="Images/HighPrio.gif" runat="server"></asp:Image>
                        <asp:Image ID="imgIconListPrioLow" ImageUrl="Images/LowPrio.gif" runat="server"></asp:Image>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox ID="chkIconListRec" Checked="true" runat="server" class="autoWidth"></asp:CheckBox>&nbsp;<asp:Image ID="imgIconListRec" ImageUrl="Images/rec.gif" runat="server"></asp:Image>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox ID="chkIconListReminder" Checked="true" runat="server" class="autoWidth"></asp:CheckBox>&nbsp;<asp:Image ID="imgIconListReminder" ImageUrl="Images/bell.gif" runat="server"></asp:Image>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox ID="chkIconListEnroll" Checked="true" runat="server" class="autoWidth"></asp:CheckBox>&nbsp;<asp:Image ID="imgIconListEnroll" ImageUrl="Images/enroll.gif" runat="server"></asp:Image>
                    </td>
                </tr>
            </table>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plListViewTable" runat="server" Text="Repeater as Table" ControlName="chkListViewTable"></dnn:Label>
            <asp:CheckBox ID="chkListViewTable" runat="server"></asp:CheckBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plRptColumns" Text="Repeater Columns:" runat="server" ControlName="txtRptColumns"></dnn:Label>
            <asp:TextBox ID="txtRptColumns" runat="server" CssClass="NormalTextBox" Width="40px">1</asp:TextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plRptRows" Text="Repeater Rows:" runat="server" ControlName="txtRptRows"></dnn:Label>
            <asp:TextBox ID="txtRptRows" runat="server" CssClass="NormalTextBox" Width="40px">1</asp:TextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plListViewUseTime" runat="server" Text="Use Time in Filter" ControlName="chkListViewUseTime"></dnn:Label>
            <asp:CheckBox ID="chkListViewUseTime" runat="server"></asp:CheckBox>
        </div>
    </div>
</fieldset>
<h2 class="dnnFormSectionHead" id="dnnPanel-CustomFields">
    <a href="" class=""><%= this.LocalizeString("CustomFields") %></a>
</h2>
<fieldset class="dnnClear">
    <div>
        <div class="dnnFormItem">
            <dnn:Label ID="plCustomField1" Text="Display Custom Field 1" runat="server" ControlName="chkCustomField1"></dnn:Label>
            <asp:CheckBox ID="chkCustomField1" runat="server"></asp:CheckBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plCustomField2" Text="Display Custom Field 2" runat="server" ControlName="chkCustomField2"></dnn:Label>
            <asp:CheckBox ID="chkCustomField2" runat="server"></asp:CheckBox>
        </div>
    </div>
</fieldset>
<h2 class="dnnFormSectionHead" id="dnnPanel-TooltipSettings">
    <a href="" class=""><%= this.LocalizeString("TooltipSettings") %></a>
</h2>
<fieldset class="dnnClear">
    <div>
        <div class="dnnFormItem">
            <dnn:Label ID="plToolTipMonth" Text="Display Month View Tooltip:" runat="server" ControlName="chkToolTipMonth"></dnn:Label>
            <asp:CheckBox ID="chkToolTipMonth" runat="server" Checked="True"></asp:CheckBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plToolTipWeek" Text="Display Week View Tooltip:" runat="server" ControlName="chkToolTipWeek"></dnn:Label>
            <asp:CheckBox ID="chkToolTipWeek" runat="server" Checked="True"></asp:CheckBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plToolTipDay" Text="Display Day View Tooltip:" runat="server" ControlName="chkToolTipDay"></dnn:Label>
            <asp:CheckBox ID="chkToolTipDay" runat="server" Checked="True"></asp:CheckBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plToolTipList" Text="Display List View Tooltip:" runat="server" ControlName="chkToolTipList"></dnn:Label>
            <asp:CheckBox ID="chkToolTipList" runat="server" Checked="True"></asp:CheckBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plTooltipLength" Text="Tooltip Length:" runat="server" ControlName="txtTooltipLength"></dnn:Label>
            <asp:TextBox ID="txtTooltipLength" runat="server" CssClass="NormalTextBox" Width="60px">10000</asp:TextBox>
        </div>
    </div>
</fieldset>
<h2 class="dnnFormSectionHead" id="dnnPanel-ImageSettings">
    <a href="" class=""><%= this.LocalizeString("ImageSettings") %></a>
</h2>
<fieldset class="dnnClear">
    <div>
        <div class="dnnFormItem">
            <dnn:Label ID="plImageEnabled" runat="server" Text="Event Height/Width:" ControlName="txtEventWidth"></dnn:Label>
            <asp:CheckBox ID="chkImageEnabled" runat="server" Checked="True"></asp:CheckBox>
        </div>
        <div id="divImageEnabled" runat="server">
            <div class="dnnFormItem">
                <dnn:Label ID="plEventImageMonth" runat="server" Text=""></dnn:Label>
                <asp:CheckBox ID="chkEventImageMonth" runat="server"></asp:CheckBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plEventImageWeek" runat="server" Text=""></dnn:Label>
                <asp:CheckBox ID="chkEventImageWeek" runat="server"></asp:CheckBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plMaxThumbWidth" Text="Max Thumbnail Width:" runat="server" ControlName="txtMaxThumbWidth"></dnn:Label>
                <asp:TextBox ID="txtMaxThumbWidth" runat="server" CssClass="NormalTextBox" Width="60px">125</asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plMaxThumbHeight" Text="Max Thumbnail Height:" runat="server" ControlName="txtMaxThumbHeight"></dnn:Label>
                <asp:TextBox ID="txtMaxThumbHeight" runat="server" CssClass="NormalTextBox" Width="60px">125</asp:TextBox>
            </div>
        </div>
    </div>
</fieldset>
</div>
<div class="EmailandReminderSettings dnnClear" id="EmailandReminderSettings">
    <h2 class="dnnFormSectionHead" id="dnnPanel-EventEmailSettings">
        <a href="" class="dnnSectionExpanded"><%= this.LocalizeString("EventEmailSettings") %></a>
    </h2>
    <fieldset class="dnnClear">
        <div>
            <%
                if (int.Parse(this.ddlSocialGroupModule.SelectedValue) == 1)
                {
            %>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblNewEventEmail" runat="server" Text="Send New Event Email" ControlName="lblNewEventEmail"></dnn:Label>
                    <table cellspacing="0" cellpadding="0" border="0">
                        <tr>
                            <td>
                                <asp:RadioButtonList ID="rblNewEventEmail" runat="server" RepeatDirection="Vertical" CssClass="dnnFormRadioButtons dnnRadioButton">
                                    <asp:ListItem Value="Never" Selected="True" resourcekey="plNewEventEmailNever"></asp:ListItem>
                                    <asp:ListItem Value="Subscribe" resourcekey="plNewEventEmailSubscribe"></asp:ListItem>
                                    <asp:ListItem Value="Role" resourcekey="plNewEventEmailRole"></asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                            <td class="SubHead" valign="bottom">
                                <asp:DropDownList ID="ddNewEventEmailRoles" AutoPostBack="False" runat="server" Width="300px">
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                </div>
            <%
                }
            %>
            <div class="dnnFormItem">
                <dnn:Label ID="plNewPerEventEmail" Text="Allow New Per Event Email:" runat="server" ControlName="plNewPerEventEmail"></dnn:Label>
                <asp:CheckBox ID="chkNewPerEventEmail" runat="server" Checked="False"></asp:CheckBox>
            </div>
        </div>
    </fieldset>
    <h2 class="dnnFormSectionHead" id="dnnPanel-NotificationSettings">
        <a href="" class=""><%= this.LocalizeString("NotificationSettings") %></a>
    </h2>
    <fieldset class="dnnClear">
        <div>
            <div class="dnnFormItem">
                <dnn:Label ID="plShowEventNotify" Text="Allow Event Reminders" runat="server" ControlName="plShowEventNotify"></dnn:Label>
                <asp:CheckBox ID="chkEventNotify" runat="server" Checked="True"></asp:CheckBox>
            </div>
            <div id="divEventNotify" runat="server">
                <div class="dnnFormItem">
                    <dnn:Label ID="lblNotifyAnon" runat="server" Text="Remind Anonymous" ControlName="lblNotifyAnon"></dnn:Label>
                    <asp:CheckBox ID="chkNotifyAnon" runat="server" Checked="True"></asp:CheckBox>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblSendReminderDefault" runat="server" Text="Reminder Default Value" ControlName="lblSendReminderDefault"></dnn:Label>
                    <asp:CheckBox ID="chkSendReminderDefault" runat="server" Checked="False"></asp:CheckBox>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblReminderFrom" runat="server" Text="Reminder Email From:" ControlName="txtReminderFrom"></dnn:Label>
                    <asp:TextBox ID="txtReminderFrom" runat="server" MaxLength="100" Wrap="False" CssClass="NormalTextBox" Width="300px"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="valReminderFrom" runat="server" ControlToValidate="txtReminderFrom"
                                                    ErrorMessage="Valid Email Address required" resourcekey="valReminderFrom" cssclass="dnnFormMessage dnnFormError"
                                                    ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*">
                    </asp:RegularExpressionValidator>
                </div>
            </div>
        </div>
    </fieldset>
</div>
<%
    if (int.Parse(this.ddlSocialGroupModule.SelectedValue) != 3)
    {
%>
    <div class="EnrollandModSettings dnnClear" id="EnrollandModSettings">
    <h2 class="dnnFormSectionHead" id="dnnPanel-EnrollmentSettings">
        <a href="" class="dnnSectionExpanded"><%= this.LocalizeString("EnrollmentSettings") %></a>
    </h2>
    <fieldset class="dnnClear">
    <div>
    <div class="dnnFormItem">
        <dnn:Label ID="plEnrollment" Text="" runat="server" ControlName="chkEventSignup"></dnn:Label>
        <asp:CheckBox ID="chkEventSignup" runat="server" Checked="True"></asp:CheckBox>
    </div>
    <div id="divEventSignup" runat="server">
    <div class="dnnFormItem">
        <dnn:Label ID="plEventSignupAllowPaid" Text="" runat="server" ControlName="chkEventSignupAllowPaid"></dnn:Label>
        <asp:CheckBox ID="chkEventSignupAllowPaid" runat="server" Checked="True"></asp:CheckBox>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="plAllowAnonEnroll" Text="" runat="server" ControlName="chkAllowAnonEnroll"></dnn:Label>
        <asp:CheckBox ID="chkAllowAnonEnroll" runat="server" Checked="False"></asp:CheckBox>
    </div>
    <div id="divEventSignupAllowPaid" runat="server">
        <div class="dnnFormItem">
            <dnn:Label ID="plPayPal" Text="PayPal Account (paid enrollments):" runat="server" ControlName="txtPayPalAccount"></dnn:Label>
            <asp:TextBox ID="txtPayPalAccount" runat="server" CssClass="NormalTextBox" Wrap="False" MaxLength="100" Width="300px"></asp:TextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plPayPalURL" Text="PayPal Account (paid enrollments):" runat="server" ControlName="txtPayPalAccount"></dnn:Label>
            <asp:TextBox ID="txtPayPalURL" runat="server" CssClass="NormalTextBox" Width="300px">https://www.paypal.com</asp:TextBox>
        </div>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="plDefaultEnrollView" Text="Display Enroll List by Default:" runat="server" ControlName="chkDefaultEnrollView"></dnn:Label>
        <asp:CheckBox ID="chkDefaultEnrollView" runat="server" Checked="False"></asp:CheckBox>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="plHideFullEnroll" Text="Hide Full enrolled events:" runat="server" ControlName="chkHideFullEnroll"></dnn:Label>
        <asp:CheckBox ID="chkHideFullEnroll" runat="server" Checked="False"></asp:CheckBox>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="plMaxNoEnrolees" Text="Allow Multiple Enrolees:" runat="server" ControlName="txtMaxNoEnrolees"></dnn:Label>
        <asp:TextBox ID="txtMaxNoEnrolees" runat="server" CssClass="NormalTextBox" Width="40px">1</asp:TextBox>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="plCancelDays" Text="Cancel Before Days:" runat="server" ControlName="txtCancelDays"></dnn:Label>
        <asp:TextBox ID="txtCancelDays" runat="server" CssClass="NormalTextBox" Width="40px">0</asp:TextBox>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="plEnrolListSortDirection" Text="My Enrolments Sorting:" runat="server"/>
        <asp:DropDownList ID="ddlEnrolListSortDirection" CssClass="NormalTextBox" Width="200px" runat="server"/>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="plEnrolListDaysBefore" Text="My Enrolments Days Before:" runat="server" ControlName="txtEnrolListDaysBefore"></dnn:Label>
        <asp:TextBox ID="txtEnrolListDaysBefore" runat="server" CssClass="NormalTextBox" Width="40px">0</asp:TextBox>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="plEnrolListDaysAfter" Text="My Enrolments Days After:" runat="server" ControlName="txtEnrolListDaysAfter"></dnn:Label>
        <asp:TextBox ID="txtEnrolListDaysAfter" runat="server" CssClass="NormalTextBox" Width="40px">0</asp:TextBox>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="plEnrollColumns" Text="Select the User Fields to display:" runat="server" ControlName="tblEnrollColumns"></dnn:Label>
        <table id="tblEnrollColumns" cellspacing="0" cellpadding="2" border="0" style="width: 400px">
            <tr align="center">
                <td align="left" class="NormalBold" style="width: 30%">
                    <asp:Label ID="plEnVisibility" resourcekey="plEnVisibility" runat="server" EnableViewState="False">Visibility</asp:Label>
                </td>
                <td style="width: 15%" class="NormalBold">
                    <asp:Label ID="plEnNone" resourcekey="plEnNone" runat="server" EnableViewState="False">None</asp:Label>
                </td>
                <td style="width: 15%" class="NormalBold">
                    <asp:Label ID="plEnEditors" resourcekey="plEnEditors" runat="server" EnableViewState="False">Editors</asp:Label>
                </td>
                <td style="width: 15%" class="NormalBold">
                    <asp:Label ID="plEnViewers" resourcekey="plEnViewers" runat="server" EnableViewState="False">Viewers</asp:Label>
                </td>
                <td style="width: 15%" class="NormalBold">
                    <asp:Label ID="plEnAnon" resourcekey="plEnAnon" runat="server" EnableViewState="False">All</asp:Label>
                </td>
            </tr>
            <tr align="center" class="inputCenter">
                <td align="left" class="NormalBold">
                    <asp:Label ID="plEnUser" resourcekey="plEnUser" runat="server" EnableViewState="False">User Name</asp:Label>
                </td>
                <td>
                    <input id="rblEnUserNone" type="radio" name="rblEnUser" runat="server"/>
                </td>
                <td>
                    <input id="rblEnUserEdit" type="radio" name="rblEnUser" runat="server"/>
                </td>
                <td>
                    <input id="rblEnUserView" type="radio" name="rblEnUser" runat="server"/>
                </td>
                <td>
                    <input id="rblEnUserAnon" type="radio" name="rblEnUser" runat="server"/>
                </td>
            </tr>
            <tr align="center" class="inputCenter">
                <td align="left" class="NormalBold">
                    <asp:Label ID="plEnDisp" resourcekey="plEnDisp" runat="server" EnableViewState="False">Display Name</asp:Label>
                </td>
                <td>
                    <input id="rblEnDispNone" type="radio" name="rblEnDisp" runat="server"/>
                </td>
                <td>
                    <input id="rblEnDispEdit" type="radio" name="rblEnDisp" runat="server"/>
                </td>
                <td>
                    <input id="rblEnDispView" type="radio" name="rblEnDisp" runat="server"/>
                </td>
                <td>
                    <input id="rblEnDispAnon" type="radio" name="rblEnDisp" runat="server"/>
                </td>
            </tr>
            <tr align="center" class="inputCenter">
                <td align="left" class="NormalBold">
                    <asp:Label ID="plEnEmail" resourcekey="plEnEmail" runat="server" EnableViewState="False">Email Address</asp:Label>
                </td>
                <td>
                    <input id="rblEnEmailNone" type="radio" name="rblEnEmail" runat="server"/>
                </td>
                <td>
                    <input id="rblEnEmailEdit" type="radio" name="rblEnEmail" runat="server"/>
                </td>
                <td>
                    <input id="rblEnEmailView" type="radio" name="rblEnEmail" runat="server"/>
                </td>
                <td>
                    <input id="rblEnEmailAnon" type="radio" name="rblEnEmail" runat="server"/>
                </td>
            </tr>
            <tr align="center" class="inputCenter">
                <td align="left" class="NormalBold">
                    <asp:Label ID="plEnPhone" resourcekey="plEnPhone" runat="server" EnableViewState="False">Phone No</asp:Label>
                </td>
                <td>
                    <input id="rblEnPhoneNone" type="radio" name="rblEnPhone" runat="server"/>
                </td>
                <td>
                    <input id="rblEnPhoneEdit" type="radio" name="rblEnPhone" runat="server"/>
                </td>
                <td>
                    <input id="rblEnPhoneView" type="radio" name="rblEnPhone" runat="server"/>
                </td>
                <td>
                    <input id="rblEnPhoneAnon" type="radio" name="rblEnPhone" runat="server"/>
                </td>
            </tr>
            <tr align="center" class="inputCenter">
                <td align="left" class="NormalBold">
                    <asp:Label ID="plEnApprove" resourcekey="plEnApprove" runat="server" EnableViewState="False">Approved</asp:Label>
                </td>
                <td>
                    <input id="rblEnApproveNone" type="radio" name="rblEnApprove" runat="server"/>
                </td>
                <td>
                    <input id="rblEnApproveEdit" type="radio" name="rblEnApprove" runat="server"/>
                </td>
                <td>
                    <input id="rblEnApproveView" type="radio" name="rblEnApprove" runat="server"/>
                </td>
                <td>
                    <input id="rblEnApproveAnon" type="radio" name="rblEnApprove" runat="server"/>
                </td>
            </tr>
            <tr align="center" class="inputCenter">
                <td align="left" class="NormalBold">
                    <asp:Label ID="plEnNo" resourcekey="plEnNo" runat="server" EnableViewState="False">No. Enrolled</asp:Label>
                </td>
                <td>
                    <input id="rblEnNoNone" type="radio" name="rblEnNo" runat="server"/>
                </td>
                <td>
                    <input id="rblEnNoEdit" type="radio" name="rblEnNo" runat="server"/>
                </td>
                <td>
                    <input id="rblEnNoView" type="radio" name="rblEnNo" runat="server"/>
                </td>
                <td>
                    <input id="rblEnNoAnon" type="radio" name="rblEnNo" runat="server"/>
                </td>
            </tr>
        </table>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="plEnrollEmailsToSend" Text="Select Emails To Send:" runat="server" ControlName="txtEnrollEmailsToSend"></dnn:Label>
        <table id="tblEnrollEmailsToSend" cellspacing="0" cellpadding="2" border="0" style="width: 350px">
            <tr>
                <td align="left" class="NormalBold">
                    <asp:Label ID="lblEnrollMessageApproved" resourcekey="txtEnrollMessageApprovedMsgName" runat="server" EnableViewState="False">EnrollMessageApproved</asp:Label>
                </td>
                <td align="left" class="NormalBold" style="width: 20%">
                    <asp:checkbox ID="chkEnrollMessageApproved" runat="server" Checked="true"></asp:checkbox>
                </td>
            </tr>
            <tr>
                <td align="left" class="NormalBold">
                    <asp:Label ID="lblEnrollMessageWaiting" resourcekey="txtEnrollMessageWaitingMsgName" runat="server" EnableViewState="False">EnrollMessageWaiting</asp:Label>
                </td>
                <td align="left" class="NormalBold">
                    <asp:checkbox ID="chkEnrollMessageWaiting" runat="server" Checked="true"></asp:checkbox>
                </td>
            </tr>
            <tr>
                <td align="left" class="NormalBold">
                    <asp:Label ID="lblEnrollMessageDenied" resourcekey="txtEnrollMessageDeniedName" runat="server" EnableViewState="False">EnrollMessageDenied</asp:Label>
                </td>
                <td align="left" class="NormalBold">
                    <asp:checkbox ID="chkEnrollMessageDenied" runat="server" Checked="true"></asp:checkbox>
                </td>
            </tr>
            <tr>
                <td align="left" class="NormalBold">
                    <asp:Label ID="lblEnrollMessageAdded" resourcekey="txtEnrollMessageAddedName" runat="server" EnableViewState="False">EnrollMessageAdded</asp:Label>
                </td>
                <td align="left" class="NormalBold">
                    <asp:checkbox ID="chkEnrollMessageAdded" runat="server" Checked="true"></asp:checkbox>
                </td>
            </tr>
            <tr>
                <td align="left" class="NormalBold">
                    <asp:Label ID="lblEnrollMessageDeleted" resourcekey="txtEnrollMessageDeletedName" runat="server" EnableViewState="False">EnrollMessageDeleted</asp:Label>
                </td>
                <td align="left" class="NormalBold">
                    <asp:checkbox ID="chkEnrollMessageDeleted" runat="server" Checked="true"></asp:checkbox>
                </td>
            </tr>
            <tr>
                <td align="left" class="NormalBold">
                    <asp:Label ID="lblEnrollMessagePaying" resourcekey="txtEnrollMessagePayingName" runat="server" EnableViewState="False">EnrollMessagePaying</asp:Label>
                </td>
                <td align="left" class="NormalBold">
                    <asp:checkbox ID="chkEnrollMessagePaying" runat="server" Checked="true"></asp:checkbox>
                </td>
            </tr>
            <tr>
                <td align="left" class="NormalBold">
                    <asp:Label ID="lblEnrollMessagePending" resourcekey="txtEnrollMessagePendingName" runat="server" EnableViewState="False">EnrollMessagePending</asp:Label>
                </td>
                <td align="left" class="NormalBold">
                    <asp:checkbox ID="chkEnrollMessagePending" runat="server" Checked="true"></asp:checkbox>
                </td>
            </tr>
            <tr>
                <td align="left" class="NormalBold">
                    <asp:Label ID="lblEnrollMessagePaid" resourcekey="txtEnrollMessagePaidName" runat="server" EnableViewState="False">EnrollMessagePaid</asp:Label>
                </td>
                <td align="left" class="NormalBold">
                    <asp:checkbox ID="chkEnrollMessagePaid" runat="server" Checked="true"></asp:checkbox>
                </td>
            </tr>
            <tr>
                <td align="left" class="NormalBold">
                    <asp:Label ID="lblEnrollMessageIncorrect" resourcekey="txtEnrollMessageIncorrectName" runat="server" EnableViewState="False">EnrollMessageIncorrect</asp:Label>
                </td>
                <td align="left" class="NormalBold">
                    <asp:checkbox ID="chkEnrollMessageIncorrect" runat="server" Checked="true"></asp:checkbox>
                </td>
            </tr>
            <tr>
                <td align="left" class="NormalBold">
                    <asp:Label ID="lblEnrollMessageCancelled" resourcekey="txtEnrollMessageCancelledName" runat="server" EnableViewState="False">EnrollMessageCancelled</asp:Label>
                </td>
                <td align="left" class="NormalBold">
                    <asp:checkbox ID="chkEnrollMessageCancelled" runat="server" Checked="true"></asp:checkbox>
                </td>
            </tr>
        </table>
    </div>
    </div>
    </div>
    </fieldset>
    <h2 class="dnnFormSectionHead" id="dnnPanel-ModerationSettings">
        <a href="" class=""><%= this.LocalizeString("ModerationSettings") %></a>
    </h2>
    <fieldset class="dnnClear">
        <div>
            <div class="dnnFormItem">
                <dnn:Label ID="plModerateAll" Text="Moderate ALL Event/Enrollment Changes" runat="server" ControlName="chkModerateAll"></dnn:Label>
                <asp:CheckBox ID="chkModerateAll" runat="server"></asp:CheckBox>
            </div>
        </div>
    </fieldset>
    </div>
<%
    }
%>
<%
    if (int.Parse(this.ddlSocialGroupModule.SelectedValue) == 1)
    {
%>
    <div class="SubcalendarSettings dnnClear" id="SubcalendarSettings">
        <fieldset class="dnnClear">
            <div>
                <div class="dnnFormItem">
                    <dnn:Label ID="plMaster" Text="Master Event" runat="server" ControlName="chkMasterEvent"></dnn:Label>
                    <asp:CheckBox ID="chkMasterEvent" runat="server" AutoPostBack="True" OnCheckedChanged="chkMasterEvent_CheckedChanged"></asp:CheckBox>
                </div>
                <div id="divMasterEvent" runat="server">
                    <div class="dnnFormItem">
                        <dnn:Label ID="lblAddSubModuleName" runat="server" Text="Master Event" ControlName="chkMasterEvent"></dnn:Label>
                        <asp:CheckBox ID="chkAddSubModuleName" runat="server" Checked="True"></asp:CheckBox>
                    </div>
                    <div class="dnnFormItem">
                        <dnn:Label ID="lblEnforceSubCalPerms" runat="server" Text="Enforce View Permissions" ControlName="chkEnforceSubCalPerms"></dnn:Label>
                        <asp:CheckBox ID="chkEnforceSubCalPerms" runat="server" Checked="True"></asp:CheckBox>
                    </div>
                    <div class="dnnFormItem">
                        <dnn:Label ID="plSubEvent" Text="Add/Remove Sub-Calendars:" runat="server" ControlName="ddlPortalEvents"></dnn:Label>
                        <table cellspacing="0" cellpadding="2" border="0">
                            <tr>
                                <td class="NormalBold" align="center">
                                    <asp:Label ID="plAvailableCals" resourcekey="plAvailableCals" runat="server" EnableViewState="False">Available</asp:Label>
                                </td>
                                <td align="center">
                                    &nbsp;
                                </td>
                                <td class="NormalBold" align="center">
                                    <asp:Label ID="plSelectedCals" resourcekey="plSelectedCals" runat="server" EnableViewState="False">Selected</asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td valign="top" align="center">
                                    <asp:ListBox class="NormalTextBox" ID="lstAvailableCals" runat="server" Width="200px" SelectionMode="Multiple" Height="150px"></asp:ListBox>
                                </td>
                                <td valign="middle" align="center">
                                    <table id="Table2" cellspacing="0" cellpadding="0" border="0">
                                        <tr>
                                            <td valign="top" align="center">
                                                <asp:LinkButton ID="cmdAddCals" CssClass="CommandButton" runat="server" EnableViewState="False" Text="&nbsp;>&nbsp;" OnClick="cmdAddCals_Click"></asp:LinkButton>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top" align="center">
                                                <asp:LinkButton ID="cmdRemoveCals" CssClass="CommandButton" runat="server" EnableViewState="False" Text="&nbsp;<&nbsp;" OnClick="cmdRemoveCals_Click"></asp:LinkButton>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="bottom" align="center">
                                                <asp:LinkButton ID="cmdAddAllCals" CssClass="CommandButton" runat="server" EnableViewState="False" Text="&nbsp;>>&nbsp;" OnClick="cmdAddAllCals_Click"></asp:LinkButton>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="bottom" align="center">
                                                <asp:LinkButton ID="cmdRemoveAllCals" CssClass="CommandButton" runat="server" EnableViewState="False" Text="&nbsp;<<&nbsp;" OnClick="cmdRemoveAllCals_Click"></asp:LinkButton>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td valign="top" align="center">
                                    <asp:ListBox class="NormalTextBox" ID="lstAssignedCals" runat="server" Width="200px" SelectionMode="Multiple" Height="150px"></asp:ListBox>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
        </fieldset>
    </div>
<%
    }
%>
<%
    if (!(int.Parse(this.ddlSocialGroupModule.SelectedValue) == 3))
    {
%>
    <div class="SEOandSitemapSettings dnnClear" id="SEOandSitemapSettings">
        <h2 class="dnnFormSectionHead" id="dnnPanel-SEOSettings">
            <a href="" class="dnnSectionExpanded"><%= this.LocalizeString("SEOSettings") %></a>
        </h2>
        <fieldset class="dnnClear">
            <div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblEnableSEO" runat="server" Text="Enable SEO:" ControlName="lblEnableSEO"></dnn:Label>
                    <asp:CheckBox ID="chkEnableSEO" runat="server" Checked="True"></asp:CheckBox>
                </div>
                <div id="divSEOEnable" runat="server">
                    <div class="dnnFormItem">
                        <dnn:Label ID="lblSEODescriptionLength" Text="Description Length:" runat="server" ControlName="lblSEODescriptionLength"></dnn:Label>
                        <asp:TextBox ID="txtSEODescriptionLength" runat="server" CssClass="NormalTextBox" Width="40px">256</asp:TextBox>
                    </div>
                </div>
            </div>
        </fieldset>
        <h2 class="dnnFormSectionHead" id="dnnPanel-SitemapSettings">
            <a href="" class=""><%= this.LocalizeString("SitemapSettings") %></a>
        </h2>
        <fieldset class="dnnClear">
            <div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblEnableSitemap" runat="server" Text="Enable Sitemap:" ControlName="lblEnableSitemap"></dnn:Label>
                    <asp:CheckBox ID="chkEnableSitemap" runat="server" Checked="True"></asp:CheckBox>
                </div>
                <div id="divSitemapEnable" runat="server">
                    <div class="dnnFormItem">
                        <dnn:Label ID="lblSitemapPriority" Text="Sitemap Priority:" runat="server" ControlName="lblSitemapPriority"></dnn:Label>
                        <asp:TextBox ID="txtSitemapPriority" runat="server" CssClass="NormalTextBox" Width="40px">0.5</asp:TextBox>
                    </div>
                    <div class="dnnFormItem">
                        <dnn:Label ID="lblSitemapDaysBefore" Text="Days Before:" runat="server" ControlName="lblSitemapDaysBefore"></dnn:Label>
                        <asp:TextBox ID="txtSitemapDaysBefore" runat="server" CssClass="NormalTextBox" Width="40px">365</asp:TextBox>
                    </div>
                    <div class="dnnFormItem">
                        <dnn:Label ID="lblSitemapDaysAfter" Text="Days After:" runat="server" ControlName="lblSitemapDaysAfter"></dnn:Label>
                        <asp:TextBox ID="txtSitemapDaysAfter" runat="server" CssClass="NormalTextBox" Width="40px">365</asp:TextBox>
                    </div>
                </div>
            </div>
        </fieldset>
    </div>
<%
    }
%>
<div class="IntegrationSettings dnnClear" id="IntegrationSettings">
    <h2 class="dnnFormSectionHead" id="dnnPanel-RSSSettings">
        <a href="" class="dnnSectionExpanded"><%= this.LocalizeString("RSSSettings") %></a>
    </h2>
    <fieldset class="dnnClear">
        <div>
            <div class="dnnFormItem">
                <dnn:Label ID="plRSSEnable" Text="Enable RSS:" runat="server" ControlName="chkRSSEnable"></dnn:Label>
                <asp:CheckBox ID="chkRSSEnable" runat="server"></asp:CheckBox>
            </div>
            <div id="divRSSEnable" runat="server">
                <div class="dnnFormItem">
                    <dnn:Label ID="plRSSDateField" Text="Date to use:" runat="server" ControlName="ddlRSSDateField"></dnn:Label>
                    <asp:DropDownList ID="ddlRSSDateField" CssClass="NormalTextBox" Width="200px" runat="server"/>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="plRSSDays" Text="Days to include:" runat="server" ControlName="txtRSSDays"></dnn:Label>
                    <asp:TextBox ID="txtRSSDays" runat="server" CssClass="NormalTextBox" Width="40px">365</asp:TextBox>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="plRSSTitle" Text="Feed Title:" runat="server" ControlName="txtRSSTitle"></dnn:Label>
                    <asp:TextBox ID="txtRSSTitle" runat="server" CssClass="NormalTextBox" width="300px">Enter title</asp:TextBox>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="plRSSDesc" Text="Feed Description:" runat="server" ControlName="txtRSSDesc"></dnn:Label>
                    <asp:TextBox ID="txtRSSDesc" runat="server" CssClass="NormalTextBox" width="300px">Enter description</asp:TextBox>
                </div>
            </div>
        </div>
    </fieldset>
    <h2 class="dnnFormSectionHead" id="dnnPanel-vCalSettings">
        <a href="" class=""><%= this.LocalizeString("vCalSettings") %></a>
    </h2>
    <fieldset class="dnnClear">
        <div>
            <div class="dnnFormItem">
                <dnn:Label ID="plExportOwnerEmail" Text="Owner Email Address Export:" runat="server" ControlName="chkExportOwnerEmail"></dnn:Label>
                <asp:CheckBox ID="chkExportOwnerEmail" runat="server" Checked="False"></asp:CheckBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plExportAnonOwnerEmail" Text="Owner Email for Unregistered User:" runat="server" ControlName="chkExportAnonOwnerEmail"></dnn:Label>
                <asp:CheckBox ID="chkExportAnonOwnerEmail" runat="server" Checked="False"></asp:CheckBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="pliCalOnIconBar" Text="Show iCal icon on icon bar:" runat="server" ControlName="chkiCalOnIconBar"></dnn:Label>
                <asp:CheckBox ID="chkiCalOnIconBar" runat="server" Checked="False"></asp:CheckBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="pliCalEmailEnable" Text="Enable iCal Emailing:" runat="server" ControlName="chkiCalEmailEnable"></dnn:Label>
                <asp:CheckBox ID="chkiCalEmailEnable" runat="server" Checked="False"></asp:CheckBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="pliCalURLinLocation" Text="Show URL in Location:" runat="server" ControlName="chkiCalURLinLocation"></dnn:Label>
                <asp:CheckBox ID="chkiCalURLinLocation" runat="server" Checked="True"></asp:CheckBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="pliCalIncludeCalname" Text="Include Calname in .ics:" runat="server" ControlName="chkiCalIncludeCalname"></dnn:Label>
                <asp:CheckBox ID="chkiCalIncludeCalname" runat="server" Checked="True"></asp:CheckBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lbliCalDaysBefore" Text="Days Before:" runat="server" ControlName="lbliCalDaysBefore"></dnn:Label>
                <asp:TextBox ID="txtiCalDaysBefore" runat="server" CssClass="NormalTextBox" Width="40px">365</asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lbliCalDaysAfter" Text="Days After:" runat="server" ControlName="lbliCalDaysAfter"></dnn:Label>
                <asp:TextBox ID="txtiCalDaysAfter" runat="server" CssClass="NormalTextBox" Width="40px">365</asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="pliCalURLAppend" Text="URL to Append:" runat="server" ControlName="txtiCalURLAppend"></dnn:Label>
                <asp:TextBox ID="txtiCalURLAppend" runat="server" CssClass="NormalTextBox" Width="400px">Enter title</asp:TextBox>
            </div>
            <div id="diviCalEventImage" runat="server">
                <div class="dnnFormItem">
                    <dnn:Label id="lbliCalDisplayImage" Text="Enable Default Image:" runat="server"></dnn:Label>
                    <asp:CheckBox id="chkiCalDisplayImage" runat="server" cssclass="SubHead"></asp:CheckBox>
                </div>
                <div id="diviCalDisplayImage" runat="server">
                    <div class="dnnFormItem">
                        <dnn:Label id="lbliCalDefaultImage" Text="Default Image:" runat="server"></dnn:Label>
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <dnn:URL id="ctliCalDefaultImage" runat="server" width="300" showfiles="True" showurls="True" shownewwindow="False" showtrack="False"
                                             showlog="False" urltype="F" showtabs="False">
                                    </dnn:URL>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="pliCalURL" Text="iCal URL:" runat="server" ControlName="pliCalURL"></dnn:Label>
                <asp:Textbox ID="lbliCalURL" runat="server" TextMode="MultiLine" ReadOnly="true" CssClass="SubHead" Width="400px">iCal URL</asp:Textbox>
            </div>
        </div>
    </fieldset>
    <h2 class="dnnFormSectionHead" id="dnnPanel-SocialSettings">
        <a href="" class=""><%= this.LocalizeString("SocialSettings") %></a>
    </h2>
    <fieldset class="dnnClear">
        <div>
            <div class="dnnFormItem">
                <dnn:Label ID="plSocialGroupModule" runat="server" ControlName="ddlSocialGroupModule"></dnn:Label>
                <asp:DropDownList ID="ddlSocialGroupModule" runat="server" CssClass="NormalTextBox" Width="178px"/>
            </div>
            <div class="dnnFormItem">
                <dnn:Label id="lblSocialUserPrivate" Text="Private User Calendars:" runat="server"></dnn:Label>
                <asp:CheckBox id="chkSocialUserPrivate" runat="server" cssclass="SubHead"></asp:CheckBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblSocialGroupSecurity" runat="server" ControlName="ddlSocialGroupSecurity"></dnn:Label>
                <asp:DropDownList ID="ddlSocialGroupSecurity" runat="server" CssClass="NormalTextBox" Width="178px"/>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblFBAdmins" Text="Facebook Admins:" runat="server" ControlName="txtFBAdmins"></dnn:Label>
                <asp:TextBox ID="txtFBAdmins" runat="server" CssClass="NormalTextBox" Width="400px"></asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblFBAppID" Text="Facebook App_ID:" runat="server" ControlName="txtFBAppID"></dnn:Label>
                <asp:TextBox ID="txtFBAppID" runat="server" CssClass="NormalTextBox" Width="400px"></asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label id="lblJournalIntegration" Text="Enable Journal Integration:" runat="server"></dnn:Label>
                <asp:CheckBox id="chkJournalIntegration" runat="server" cssclass="SubHead"></asp:CheckBox>
            </div>
        </div>
    </fieldset>
</div>
<div class="TemplateSettings dnnClear" id="TemplateSettings">
    <fieldset class="dnnClear">
        <div>
            <div class="dnnFormItem">
                <dnn:Label ID="plTemplates" runat="server" Text="Event Templates" ControlName="ddlTemplates"></dnn:Label>
                <a runat="server" id="lnkTemplatesHelp" target="_blank">Help</a>
                <asp:DropDownList ID="ddlTemplates" runat="server" AutoPostBack="true" CssClass="NormalTextBox" OnSelectedIndexChanged="ddlTemplates_SelectedIndexChanged">
                </asp:DropDownList>
            </div>
            <div>
                <asp:TextBox ID="txtEventTemplate" runat="server" TextMode="MultiLine" Width="95%" Height="300px">
                </asp:TextBox>
            </div>
            <div>
                <dnn:CommandButton ID="cmdUpdateTemplate" IconKey="Save" ResourceKey="cmdUpdateTemplate" runat="server" CssClass="CommandButton" EnableViewState="False" />&nbsp;
                <dnn:CommandButton ID="cmdResetTemplate" IconKey="Restore" ResourceKey="cmdResetTemplate" runat="server" CssClass="CommandButton" EnableViewState="False" />&nbsp;
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Label ID="lblTemplateUpdated" runat="server" CssClass="SubHead" Visible="false">Template Updated</asp:Label>
            </div>
        </div>
    </fieldset>
</div>
<ul class="dnnActions dnnClear">
    <li>
        <asp:LinkButton OnClick="updateButton_Click" ID="updateButton" ResourceKey="updateButton" runat="server" CssClass="dnnPrimaryAction"/>
    </li>
    <li>
        <asp:LinkButton OnClick="cancelButton_Click" ID="cancelButton" ResourceKey="cancelButton" runat="server" CssClass="dnnSecondaryAction" CausesValidation="False"/>
    </li>
</ul>
</div>
</asp:Panel>

<script language="javascript" type="text/javascript">

    /*globals jQuery, window, Sys */
    (function($, sys) {
        function setupEditEvents() {
            $('#EventSettings').dnnTabs().dnnPanels();
        }

        $(document).ready(function() {
            setupEditEvents();
            sys.WebForms.PageRequestManager.getInstance().add_endRequest(function() {
                setupEditEvents();
            });
        });
    }(jQuery, window.Sys));
</script>