<%@ Control Language="C#" AutoEventWireup="true" Codebehind="EventMyEnrollments.ascx.cs" Inherits="DotNetNuke.Modules.Events.EventMyEnrollments" %>
<%@ Register TagPrefix="evt" TagName="Icons" Src="~/DesktopModules/Events/SubControls/EventIcons.ascx" %>
<div>
    <div class="EvtHdrLftCol"></div>
    <div class="EvtHdrMdlCol">
        <div id="divMessage" runat="server" visible="false" class="dnnFormMessage dnnFormWarning">
            <asp:Label ID="lblMessage" runat="server">No Events/Enrollments Found...</asp:Label>
        </div>
    </div>
    <div class="TopIconBar EvtHdrRgtCol">
        <evt:Icons ID="EventIcons" runat="server"></evt:Icons>
    </div>
    <div style="clear: both">
        <asp:DataGrid ID="grdEnrollment" runat="server" AutoGenerateColumns="False" CellPadding="2" CssClass="EnrollGrid"
                      DataKeyField="SignupID" GridLines="None" OnItemCommand="grdEnrollment_ItemCommand" Width="100%" >
            <EditItemStyle VerticalAlign="Bottom"></EditItemStyle>
            <AlternatingItemStyle CssClass="EnrollGridAlternate"></AlternatingItemStyle>
            <ItemStyle VerticalAlign="Top"></ItemStyle>
            <HeaderStyle Font-Bold="True" BackColor="Silver"></HeaderStyle>
            <Columns>
                <asp:TemplateColumn HeaderText="Select">
                    <HeaderStyle CssClass="EnrollGridHeader"></HeaderStyle>
                    <ItemStyle CssClass="EnrollSelect"></ItemStyle>
                    <ItemTemplate>
                        <asp:CheckBox ID="chkSelect" runat="server"></asp:CheckBox>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:BoundColumn DataField="EventTimeBegin" HeaderText="Start" DataFormatString="{0:g}">
                    <HeaderStyle CssClass="EnrollGridHeader"></HeaderStyle>
                    <ItemStyle CssClass="EnrollDate"></ItemStyle>
                </asp:BoundColumn>
                <asp:BoundColumn DataField="EventTimeEnd" HeaderText="End" DataFormatString="{0:g}">
                    <HeaderStyle CssClass="EnrollGridHeader"></HeaderStyle>
                    <ItemStyle CssClass="EnrollDate"></ItemStyle>
                </asp:BoundColumn>
                <asp:TemplateColumn HeaderText="Event">
                    <HeaderStyle CssClass="EnrollGridHeader"></HeaderStyle>
                    <ItemStyle CssClass="EnrollTitle"></ItemStyle>
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkEnrollEventName" CssClass="CommandButton" runat="server" CommandName="Select" CommandArgument="Select">
                            <%# DataBinder.Eval(Container.DataItem, "EventName") %>
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn headerText="Approved">
                    <HeaderStyle CssClass="EnrollGridHeader"></HeaderStyle>
                    <ItemStyle CssClass="EnrollApproved"></ItemStyle>
                    <ItemTemplate>
                        <asp:CheckBox ID="chkApproved" runat="server" Enabled="false" Checked='<%# DataBinder.Eval(Container.DataItem, "Approved") %>'/>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:BoundColumn DataField="Approved" Visible="false"></asp:BoundColumn>
                <asp:TemplateColumn headerText="Fee">
                    <HeaderStyle CssClass="EnrollGridHeader"></HeaderStyle>
                    <ItemStyle CssClass="EnrollFee"></ItemStyle>
                    <ItemTemplate>
                        <asp:Label id="lblAmount" runat="server"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:BoundColumn DataField="PayPalAmount" Visible="false"></asp:BoundColumn>
                <asp:BoundColumn DataField="NoEnrolees" HeaderText="Qty">
                    <HeaderStyle CssClass="EnrollGridHeader"></HeaderStyle>
                    <ItemStyle CssClass="EnrollNo"></ItemStyle>
                </asp:BoundColumn>
                <asp:TemplateColumn headerText="Total">
                    <HeaderStyle CssClass="EnrollGridHeader"></HeaderStyle>
                    <ItemStyle CssClass="EnrollFee"></ItemStyle>
                    <ItemTemplate>
                        <asp:Label id="lblTotal" runat="server"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
        </asp:DataGrid>
    </div>
    <ul class="dnnActions dnnClear">
        <li>
            <asp:LinkButton OnClick="returnButton_Click" ID="returnButton" runat="server" CssClass="dnnPrimaryAction" CausesValidation="False" resourcekey="returnButton"/>
        </li>
        <li>
            <asp:LinkButton OnClick="lnkSelectedDelete_Click" ID="lnkSelectedDelete" runat="server" resourcekey="lnkSelectedDelete" CssClass="dnnSecondaryAction"/>
        </li>
    </ul>
    <div class="BottomIconBar">
        <evt:Icons ID="EventIcons2" runat="server"></evt:Icons>
    </div>
</div>