<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EventRpt.ascx.cs" Inherits="DotNetNuke.Modules.Events.EventRpt" %>
<%@ Register TagPrefix="evt" TagName="Category" Src="~/DesktopModules/Events/SubControls/SelectCategory.ascx" %>
<%@ Register TagPrefix="evt" TagName="Location" Src="~/DesktopModules/Events/SubControls/SelectLocation.ascx" %>
<%@ Register TagPrefix="evt" TagName="Icons" Src="~/DesktopModules/Events/SubControls/EventIcons.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web.Deprecated" Namespace="DotNetNuke.Web.UI.WebControls" %>

<dnn:DnnToolTipManager
    ID="toolTipManager" runat="server" HideEvent="LeaveTargetAndToolTip" Modal="False" EnableShadow="True" CssClass="Eventtooltip" ShowCallout="False"/>
<div>
    <div class="EvtHdrLftCol">
        <evt:Category ID="SelectCategory" runat="server" OnCategorySelectedChanged="SelectCategory_CategorySelected"></evt:Category>
    </div>
    <div class="EvtHdrMdlCol">
        <evt:Location ID="SelectLocation" runat="server" OnLocationSelectedChanged="SelectLocation_LocationSelected"></evt:Location>
    </div>
    <div class="TopIconBar EvtHdrRgtCol">
        <evt:Icons ID="EventIcons" runat="server"></evt:Icons>
    </div>
    <div style="clear: both">
        <div id="RptTable" class="RptTable">
            <div style="padding: 0">
                <asp:Repeater ID="rptEvents" runat="server" OnItemDataBound="rptEvents_ItemDataBound">
                    <HeaderTemplate>
                        <asp:Literal ID="rptHeader" runat="server"></asp:Literal>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Literal ID="rptBody" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Literal ID="rptFooter" runat="server"></asp:Literal>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
            <div id="rptTRPager" runat="server" visible="false">
                <asp:Repeater ID="rptPager" runat="server" OnItemDataBound="rptPages_ItemDataBound" OnItemCommand="rptPages_ItemCommand">
                    <HeaderTemplate>
                        <table id="rptPagerHeader" class="RptPagerHeader">
                        <tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <td>
                            <asp:LinkButton ID="cmdPage" CommandName="Page" CommandArgument="<%# Container.DataItem %>" runat="server" CssClass="RptPagerPage"><%# Container.DataItem %></asp:LinkButton>
                        </td>
                    </ItemTemplate>
                    <FooterTemplate>
                        </tr>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
    <div class="BottomIconBar">
        <evt:Icons ID="EventIcons2" runat="server"></evt:Icons>
    </div>
</div>