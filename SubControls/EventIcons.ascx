<%@ Control Language="vb" AutoEventWireup="false" Codebehind="EventIcons.ascx.vb" Inherits="DotNetNuke.Modules.Events.EventIcons" %>
<div class="IconBar" style="text-align:center">
    <asp:Label ID="lblSubscribe" runat="server" CssClass="SubHead IconBarPadding" >Subscribe</asp:Label>
    <asp:ImageButton ID="btnSubscribe" runat="server" AlternateText="Subscribe" 
        IconKey="Unchecked" visible="false" cssClass="IconBarPadding"/>
    <asp:Image ID="imgBar"  runat="server"
        ImageUrl="~/DesktopModules/Events/Images/cal-bar.gif" visible="true" cssClass="IconBarPadding"/>
    <asp:Hyperlink ID="btnSettings" runat="server" AlternateText="Edit Settings" 
        IconKey="EditTab" visible="false"/>
    <asp:ImageButton ID="btnModerate" runat="server" AlternateText="Moderate Events" 
        ImageUrl="~/DesktopModules/Events/Images/moderate.gif" visible="false" />
    <asp:HyperLink ID="btnAdd" runat="server" AlternateText="Add Events" 
        ImageUrl="~/DesktopModules/Events/Images/cal-add.gif" visible="false" />
    <asp:ImageButton ID="btnMonth" runat="server" AlternateText="Month View" 
        ImageUrl="~/DesktopModules/Events/Images/cal-month.gif" visible="false" />
    <asp:ImageButton ID="btnWeek" runat="server" AlternateText="Week View" 
        ImageUrl="~/DesktopModules/Events/Images/cal-week.gif" visible="false" />
    <asp:ImageButton ID="btnList" runat="server" AlternateText="List View" 
        ImageUrl="~/DesktopModules/Events/Images/cal-list.gif" visible="false" />
    <asp:ImageButton ID="btnEnroll" runat="server" AlternateText="My Enrollments" 
        ImageUrl="~/DesktopModules/Events/Images/cal-enroll.gif" visible="false" />
    <asp:HyperLink ID="hypiCal" runat="server" visible="false" imageurl="~/DesktopModules/Events/Images/iCal.gif" />
    <asp:Hyperlink ID="btnRSS" runat="server" visible="false" ImageUrl="~/DesktopModules/Events/Images/rss.gif"  />
</div>