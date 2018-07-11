<%@ Control Language="C#" AutoEventWireup="true" Codebehind="EventIcons.ascx.cs" Inherits="DotNetNuke.Modules.Events.EventIcons" %>
<div class="IconBar" style="text-align: center">
    <asp:Label ID="lblSubscribe" runat="server" CssClass="SubHead IconBarPadding">Subscribe</asp:Label>
    <asp:ImageButton ID="btnSubscribe" runat="server" AlternateText="Subscribe"
                     IconKey="Unchecked" visible="false" cssClass="IconBarPadding" OnClick="btnSubscribe_Click"/>
    <asp:Image ID="imgBar" runat="server"
               ImageUrl="~/DesktopModules/Events/Images/cal-bar.gif" visible="true" cssClass="IconBarPadding"/>
    <asp:Hyperlink ID="btnSettings" runat="server" AlternateText="Edit Settings"
                   IconKey="EditTab" visible="false"/>
    <asp:Hyperlink ID="btnCategories" runat="server" AlternateText="Edit Categories"
                   ImageUrl="~/DesktopModules/Events/Images/Categories.gif" visible="false"/>
    <asp:Hyperlink ID="btnLocations" runat="server" AlternateText="Edit Locations"
                   ImageUrl="~/DesktopModules/Events/Images/Locations.gif" visible="false"/>
    <asp:ImageButton ID="btnModerate" runat="server" AlternateText="Moderate Events"
                     ImageUrl="~/DesktopModules/Events/Images/moderate.gif" visible="false" OnClick="btnModerate_Click"/>
    <asp:HyperLink ID="btnAdd" runat="server" AlternateText="Add Events"
                   ImageUrl="~/DesktopModules/Events/Images/cal-add.gif" visible="false"/>
    <asp:ImageButton ID="btnMonth" runat="server" AlternateText="Month View"
                     ImageUrl="~/DesktopModules/Events/Images/cal-month.gif" visible="false" OnClick="btnMonth_Click"/>
    <asp:ImageButton ID="btnWeek" runat="server" AlternateText="Week View"
                     ImageUrl="~/DesktopModules/Events/Images/cal-week.gif" visible="false" OnClick="btnWeek_Click"/>
    <asp:ImageButton ID="btnList" runat="server" AlternateText="List View"
                     ImageUrl="~/DesktopModules/Events/Images/cal-list.gif" visible="false" OnClick="btnList_Click"/>
    <asp:ImageButton ID="btnEnroll" runat="server" AlternateText="My Enrollments"
                     ImageUrl="~/DesktopModules/Events/Images/cal-enroll.gif" visible="false" OnClick="btnEnroll_Click"/>
    <asp:HyperLink ID="hypiCal" runat="server" visible="false" imageurl="~/DesktopModules/Events/Images/iCal.gif"/>
    <asp:Hyperlink ID="btnRSS" runat="server" visible="false" ImageUrl="~/DesktopModules/Events/Images/rss.gif"/>
</div>