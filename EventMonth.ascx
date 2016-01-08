<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="EventMonth.ascx.vb" Inherits="DotNetNuke.Modules.Events.EventMonth" %>
<%@ Register TagPrefix="evt" Namespace="DotNetNuke.Modules.Events.ScheduleControl.MonthControl" Assembly="DotNetNuke.Modules.Events.ScheduleControl" %>
<%@ Register TagPrefix="evt" TagName="Category" Src="~/DesktopModules/Events/SubControls/SelectCategory.ascx" %>
<%@ Register TagPrefix="evt" TagName="Location" Src="~/DesktopModules/Events/SubControls/SelectLocation.ascx" %>
<%@ Register TagPrefix="evt" TagName="Icons" Src="~/DesktopModules/Events/SubControls/EventIcons.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web.Deprecated" Namespace="DotNetNuke.Web.UI.WebControls" %>

<dnn:DnnToolTipManager 
        ID="toolTipManager" runat="server"  HideEvent="LeaveTargetAndToolTip" Modal="False" EnableShadow="True" CssClass="Eventtooltip" ShowCallout="False" />
<div>
    <div class="EvtHdrLftCol"></div>
    <div class="EvtHdrMdlCol">
        <div>
            <asp:panel id="pnlDateControls" Runat="server" Visible="True" CssClass="EvtDateControls">
                <asp:LinkButton ID="lnkToday" runat="server" CssClass="CommandButton"> Today</asp:LinkButton>&nbsp;
                <dnn:DnnDatePicker id="dpGoToDate" AutoPostBack="true" runat="server" DateInput-CssClass="DateFormat" CssClass="DatePicker"></dnn:DnnDatePicker>
            </asp:panel>
        </div>
        <div style="text-align:center;">
            <evt:Category ID="SelectCategory" runat="server"></evt:Category>
            <evt:Location ID="SelectLocation" runat="server"></evt:Location>
        </div>
    </div>
    <div class="TopIconBar EvtHdrRgtCol" >
        <evt:Icons ID="EventIcons" runat="server"></evt:Icons>
    </div>
    <div style="clear:both;text-align:center;" >
        <evt:DNNCalendar ID="EventCalendar" runat="server" CssClass="Event" CellPadding="0" NextPrevFormat="FullMonth" DayStyle-VerticalAlign="Top"
            PrevMonthText=" " NextMonthText=" " ShowGridLines="False" SelectMonthText=" " SelectWeekText=" " >
            <TodayDayStyle HorizontalAlign="Center" CssClass="EventTodayDay" VerticalAlign="Top"></TodayDayStyle>
            <SelectorStyle HorizontalAlign="Center" CssClass="EventSelector" VerticalAlign="Top"></SelectorStyle>
            <DayStyle HorizontalAlign="Center" CssClass="EventDay" VerticalAlign="Top"></DayStyle>
            <NextPrevStyle HorizontalAlign="Center" CssClass="EventNextPrev" VerticalAlign="Top"></NextPrevStyle>
            <DayHeaderStyle HorizontalAlign="Center" CssClass="EventDayHeader" VerticalAlign="Top"></DayHeaderStyle>
            <SelectedDayStyle HorizontalAlign="Center" CssClass="EventSelectedDay" VerticalAlign="Top"></SelectedDayStyle>
            <TitleStyle HorizontalAlign="Center" CssClass="EventTitle" VerticalAlign="Middle"></TitleStyle>
            <WeekendDayStyle HorizontalAlign="Center" CssClass="EventWeekendDay" VerticalAlign="Top"></WeekendDayStyle>
            <OtherMonthDayStyle HorizontalAlign="Center" CssClass="EventOtherMonthDay" VerticalAlign="Top"></OtherMonthDayStyle>
        </evt:DNNCalendar>
    </div>
    <div class="BottomIconBar">
        <evt:Icons ID="EventIcons2" runat="server"></evt:Icons>
    </div>
</div>
