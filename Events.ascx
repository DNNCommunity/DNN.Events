<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Events.ascx.cs" Inherits="DotNetNuke.Modules.Events.Events" %>
<asp:Panel ID="pnlEventsModule" runat="server">
    <asp:PlaceHolder ID="phMain" runat="server"></asp:PlaceHolder>
</asp:Panel>
<div style="text-align: center;">
    <asp:Label ID="lblModuleSettings" runat="server" resourcekey="lblModuleSettings" cssclass="dnnFormMessage dnnFormWarning" Visible="False">Please update module settings...contact Portal Admin.</asp:Label>
</div>