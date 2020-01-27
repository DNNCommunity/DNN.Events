<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EventWeek.ascx.cs" Inherits="DotNetNuke.Modules.Events.EventWeek" %>
<%@ Register TagPrefix="evt" Namespace="DotNetNuke.Modules.Events.ScheduleControl" Assembly="DotNetNuke.Modules.Events.ScheduleControl" %>
<%@ Register TagPrefix="evt" TagName="Category" Src="~/DesktopModules/Events/SubControls/SelectCategory.ascx" %>
<%@ Register TagPrefix="evt" TagName="Location" Src="~/DesktopModules/Events/SubControls/SelectLocation.ascx" %>
<%@ Register TagPrefix="evt" TagName="Icons" Src="~/DesktopModules/Events/SubControls/EventIcons.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web.Deprecated" Namespace="DotNetNuke.Web.UI.WebControls" %>

<dnn:DnnToolTipManager
    ID="toolTipManager" runat="server" HideEvent="LeaveTargetAndToolTip" Modal="False" EnableShadow="True" CssClass="HTMLNtooltip" ShowCallout="False"/>
<div>
    <div class="EvtHdrLftCol"></div>
    <div class="EvtHdrMdlCol">
        <div>
            <asp:panel id="pnlDateControls" Runat="server" Visible="True" CssClass="EvtDateControls">
                <asp:LinkButton ID="lnkToday" runat="server" CssClass="CommandButton" OnClick="lnkToday_Click"> Today</asp:LinkButton>&nbsp;
                <dnn:DnnDatePicker id="dpGoToDate" OnSelectedDateChanged="dpGoToDate_SelectedDateChanged" AutoPostBack="true" runat="server" DateInput-CssClass="DateFormat" CssClass="DatePicker"></dnn:DnnDatePicker>
            </asp:panel>
        </div>
        <div style="text-align: center;">
            <evt:Category id="SelectCategory" runat="server"></evt:Category>
            <evt:Location ID="SelectLocation" runat="server" OnLocationSelectedChanged="SelectLocation_LocationSelected"></evt:Location>
        </div>
    </div>
    <div class="TopIconBar EvtHdrRgtCol">
        <evt:Icons ID="EventIcons" runat="server"></evt:Icons>
    </div>
    <div style="clear: both; text-align: center;">
        <table cellspacing="0" cellpadding="0" width="100%" border="0">
            <tr class="WeekHeader">
                <td style="white-space: nowrap;" align="center">
                    <asp:LinkButton ID="lnkPrev" CssClass="WeekNextPrev" runat="server" OnClick="lnkPrev_Click">&lt;&lt;</asp:LinkButton>
                </td>
                <td style="white-space: nowrap; width: 66%" align="center">
                    <asp:Label ID="lblWeekOf" CssClass="WeekOfTitle" runat="server"></asp:Label>
                </td>
                <td style="white-space: nowrap;" align="center">
                    <asp:LinkButton ID="lnkNext" CssClass="WeekNextPrev" runat="server" OnClick="lnkNext_Click">&gt;&gt;</asp:LinkButton>
                </td>
            </tr>
        </table>
        <evt:ScheduleCalendar ID="schWeek" runat="server" Weeks="1"
                              StartDate="2007-01-14"
                              TimeScaleInterval="30" StartTimeField="StartTime" EndTimeField="EndTime"
                              Layout="Vertical" GridLines="None" StartDay="Sunday"
                              TimeFieldsContainDate="True"
                              CssClass="WeekTable" OnItemDataBound="schWeek_ItemDataBound">
            <ItemTemplate>
                <%#Eval("Icons") %>
                <asp:HyperLink ID="lnkEvent" runat="Server" Text='<%# DataBinder.Eval(Container.DataItem, "Task") %>'
                               NavigateUrl='<%# DataBinder.Eval(Container.DataItem, "URL") %>' Target='<%# DataBinder.Eval(Container.DataItem, "Target") %>'>
                </asp:HyperLink>
            </ItemTemplate>
            <DateStyle CssClass="WeekTitle"></DateStyle>
            <ItemStyle CssClass="WeekItem"></ItemStyle>
            <BackgroundStyle CssClass="WeekBackground"></BackgroundStyle>
            <TimeTemplate>
                <%# Container.DataItem.ToShortTimeString() %>
            </TimeTemplate>
            <TimeStyle CssClass="WeekRangeheader" Wrap="False"></TimeStyle>
        </evt:ScheduleCalendar>
    </div>
    <div class="BottomIconBar">
        <evt:Icons ID="EventIcons2" runat="server"></evt:Icons>
    </div>
</div>