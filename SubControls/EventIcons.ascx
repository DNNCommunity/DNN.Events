<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EventIcons.ascx.cs" Inherits="DotNetNuke.Modules.Events.EventIcons" %>
<div class="IconBar" style="text-align: center">
    <asp:Label ID="lblSubscribe" runat="server" CssClass="SubHead IconBarPadding">Subscribe</asp:Label>
    <asp:ImageButton ID="btnSubscribe" runat="server" AlternateText="Subscribe"
        IconKey="Unchecked" Visible="false" CssClass="IconBarPadding" OnClick="btnSubscribe_Click" />
    <asp:Image ID="imgBar" runat="server"
        ImageUrl="~/DesktopModules/Events/Images/cal-bar.gif" Visible="true" CssClass="IconBarPadding" />
    <asp:HyperLink ID="btnSettings" runat="server" AlternateText="Edit Settings"
        IconKey="EditTab" Visible="false" />
    <asp:HyperLink ID="btnCategories" runat="server" AlternateText="Edit Categories"
        ImageUrl="~/DesktopModules/Events/Images/Categories.gif" Visible="false" />
    <asp:HyperLink ID="btnLocations" runat="server" AlternateText="Edit Locations"
        ImageUrl="~/DesktopModules/Events/Images/Locations.gif" Visible="false" />
    <asp:ImageButton ID="btnModerate" runat="server" AlternateText="Moderate Events"
        ImageUrl="~/DesktopModules/Events/Images/moderate.gif" Visible="false" OnClick="btnModerate_Click" />
    <asp:HyperLink ID="btnAdd" runat="server" AlternateText="Add Events"
        ImageUrl="~/DesktopModules/Events/Images/cal-add.gif" Visible="false" />
    <asp:ImageButton ID="btnMonth" runat="server" AlternateText="Month View"
        ImageUrl="~/DesktopModules/Events/Images/cal-month.gif" Visible="false" OnClick="btnMonth_Click" />
    <asp:ImageButton ID="btnWeek" runat="server" AlternateText="Week View"
        ImageUrl="~/DesktopModules/Events/Images/cal-week.gif" Visible="false" OnClick="btnWeek_Click" />
    <asp:ImageButton ID="btnList" runat="server" AlternateText="List View"
        ImageUrl="~/DesktopModules/Events/Images/cal-list.gif" Visible="false" OnClick="btnList_Click" />
    <asp:ImageButton ID="btnEnroll" runat="server" AlternateText="My Enrollments"
        ImageUrl="~/DesktopModules/Events/Images/cal-enroll.gif" Visible="false" OnClick="btnEnroll_Click" />
    <asp:HyperLink ID="hypiCal" Text="Download iCal calendar file" runat="server" Visible="false" ImageUrl="~/DesktopModules/Events/Images/iCal.gif" />
    <asp:HyperLink ID="btnRSS" Text="Show RSS feed" runat="server" Visible="false" ImageUrl="~/DesktopModules/Events/Images/rss.gif" />
</div>
