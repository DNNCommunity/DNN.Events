<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="EventUserGrid.ascx.vb" Inherits="DotNetNuke.Modules.Events.EventUserGrid" %>
<%@ Register Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" TagPrefix="dnn" %>
<div class="SubHead" style="vertical-align:top">
    <select id="dropdownFilterItem" onchange="ChangedropdownFilterItem(event);" class="NormalTextBox" runat="server"></select>
    &nbsp;<asp:Label ID="lblStartswith" runat="server" Text="Starts with"></asp:Label>
    &nbsp;<asp:TextBox ID="txtFilterUsers" CssClass="NormalTextBox evtGridInput" runat="server"></asp:TextBox>
    &nbsp;<dnn:CommandButton ID="cmdRefreshList" runat="server" IconKey="Refresh" CssClass="CommandButton" ></dnn:CommandButton>
</div>
<div>
    <asp:GridView ID="gvUsersToEnroll" runat="server" AutoGenerateColumns="False" 
        DataKeyField="UserID" GridLines="None" Visible="False" CssClass="EditEnrollGrid" 
        Width="445px" AllowPaging="True" DataKeyNames="UserID">
        <AlternatingRowStyle CssClass="EditEnrollGridAlternate" />
        <PagerStyle CssClass="ListPager" />
        <Columns>
            <asp:TemplateField HeaderText="Select">
                <HeaderStyle CssClass="EditEnrollGridHeader"></HeaderStyle>
                <ItemStyle CssClass="EditEnrollSelect"></ItemStyle>
                <ItemTemplate>
                    <asp:CheckBox ID="chkSelectUser" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="UserName" HeaderText="Username" ReadOnly="True">
                <HeaderStyle CssClass="EditEnrollGridHeader"></HeaderStyle>
                <ItemStyle CssClass="EditEnrollUser"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField DataField="DisplayName" HeaderText="Displayname" ReadOnly="True">
                <HeaderStyle CssClass="EditEnrollGridHeader"></HeaderStyle>
                <ItemStyle CssClass="EditEnrollDisplay"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField DataField="email" HeaderText="Emailaddress" ReadOnly="True">
                <HeaderStyle CssClass="EditEnrollGridHeader"></HeaderStyle>
                <ItemStyle CssClass="EditEnrollEmail"></ItemStyle>
            </asp:BoundField>
        </Columns>
    </asp:GridView>
    <dnn:CommandButton ID="cmdSelectedAddUser" runat="server" CssClass="CommandButton" IconKey="Add" ></dnn:CommandButton>
    <br />
</div>