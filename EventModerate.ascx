<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EventModerate.ascx.cs" Inherits="DotNetNuke.Modules.Events.EventModerate" Explicit="True" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<asp:Panel ID="pnlEventsModuleModerate" runat="server">
    <div class="dnnForm EventModerate">
        <div class="dnnFormItem">
            <dnn:Label ID="lblModerateView" runat="server"></dnn:Label>
            <asp:RadioButtonList ID="rbModerate" runat="server" RepeatDirection="Vertical" CssClass="dnnFormRadioButtons" ToolTip="Select Events to Moderate Events or Enrollment to Moderate Event Enrollment" AutoPostBack="True" OnSelectedIndexChanged="rbModerate_SelectedIndexChanged">
                <asp:ListItem resourcekey="EventsListItem" Value="Events" Selected="True">Events</asp:ListItem>
                <asp:ListItem resourcekey="EnrollmentListItem" Value="Enrollment">Enrollment</asp:ListItem>
            </asp:RadioButtonList>
        </div>
        <div>
            <asp:Panel ID="pnlEmail" runat="server" Width="100%" BorderWidth="2px" BorderStyle="Outset" CssClass="SubHead">
                <div>
                    <asp:CheckBox ID="chkEmail" runat="server" CssClass="SubHead" resourcekey="chkEmail"></asp:CheckBox>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblEmailFrom" runat="server"></dnn:Label>
                    <asp:TextBox ID="txtEmailFrom" runat="server" Width="250px" CssClass="NormalTextBox" MaxLength="100" Wrap="False"></asp:TextBox>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblNotifyEmailSubject" runat="server"></dnn:Label>
                    <asp:TextBox ID="txtEmailSubject" runat="server" Width="500px" CssClass="NormalTextBox"></asp:TextBox>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblNotifyEmailMessage" runat="server"></dnn:Label>
                    <asp:TextBox ID="txtEmailMessage" runat="server" Width="500px" ToolTip="Message to be emailed to the Requesting User." CssClass="NormalTextBox" TextMode="MultiLine" Height="100px"></asp:TextBox>
                </div>
            </asp:Panel>
        </div>
        <div>
            <asp:Panel ID="pnlGrid" runat="server" Width="100%" BorderWidth="2px" BorderStyle="Outset" CssClass="SubHead" style="margin-top: 2px">
                <asp:DataGrid ID="grdRecurEvents" runat="server" Width="100%" BorderWidth="0px" CssClass="Normal" GridLines="Horizontal" AutoGenerateColumns="False" OnItemCommand="grdRecurEvents_ItemCommand" DataKeyField="RecurMasterID">
                    <EditItemStyle VerticalAlign="Bottom"></EditItemStyle>
                    <AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
                    <ItemStyle VerticalAlign="Top"></ItemStyle>
                    <HeaderStyle Font-Bold="True" BackColor="Silver"></HeaderStyle>
                    <Columns>
                        <asp:TemplateColumn HeaderText="RecurAction">
                            <ItemTemplate>
                                <asp:RadioButtonList ID="rbEventRecurAction" runat="server" RepeatDirection="Horizontal" Font-Names="Verdana" Font-Size="7pt">
                                    <asp:ListItem resourcekey="Approve" Value="Approve">Approve</asp:ListItem>
                                    <asp:ListItem resourcekey="Deny" Value="Deny">Deny</asp:ListItem>
                                </asp:RadioButtonList>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:BoundColumn DataField="DTStart" HeaderText="Date" DataFormatString="{0:d}"></asp:BoundColumn>
                        <asp:BoundColumn DataField="DTStart" HeaderText="Time" DataFormatString="{0:t}"></asp:BoundColumn>
                        <asp:TemplateColumn HeaderText="Event">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkRecurDownload" CssClass="CommandButton" runat="server" CommandName="Select" CommandArgument="Select">
                                    <%# DataBinder.Eval(Container.DataItem, "EventName") %>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:BoundColumn DataField="FirstEventID" Visible="false"></asp:BoundColumn>
                    </Columns>
                </asp:DataGrid>
                <asp:DataGrid ID="grdEvents" runat="server" Width="100%" BorderWidth="0px" CssClass="Normal" GridLines="Horizontal" AutoGenerateColumns="False" OnItemCommand="grdEvents_ItemCommand" DataKeyField="EventID">
                    <EditItemStyle VerticalAlign="Bottom"></EditItemStyle>
                    <AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
                    <ItemStyle VerticalAlign="Top"></ItemStyle>
                    <HeaderStyle Font-Bold="True" BackColor="Silver"></HeaderStyle>
                    <Columns>
                        <asp:TemplateColumn HeaderText="SingleAction">
                            <ItemTemplate>
                                <asp:RadioButtonList ID="rbEventAction" runat="server" RepeatDirection="Horizontal" Font-Names="Verdana" Font-Size="7pt">
                                    <asp:ListItem resourcekey="Approve" Value="Approve">Approve</asp:ListItem>
                                    <asp:ListItem resourcekey="Deny" Value="Deny">Deny</asp:ListItem>
                                </asp:RadioButtonList>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:BoundColumn DataField="EventTimeBegin" HeaderText="Date" DataFormatString="{0:d}"></asp:BoundColumn>
                        <asp:BoundColumn DataField="EventTimeBegin" HeaderText="Time" DataFormatString="{0:t}"></asp:BoundColumn>
                        <asp:TemplateColumn HeaderText="Event">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkDownload" CssClass="CommandButton" runat="server" CommandName="Select" CommandArgument="Select">
                                    <%# DataBinder.Eval(Container.DataItem, "EventName") %>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid>
                <asp:DataGrid ID="grdEnrollment" runat="server" Width="100%" BorderWidth="0px" CssClass="Normal" GridLines="Horizontal" AutoGenerateColumns="False" OnItemCommand="grdEnrollment_ItemCommand" DataKeyField="SignupID" Visible="False">
                    <EditItemStyle VerticalAlign="Bottom"></EditItemStyle>
                    <AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
                    <ItemStyle VerticalAlign="Top"></ItemStyle>
                    <HeaderStyle Font-Bold="True" BackColor="Silver"></HeaderStyle>
                    <Columns>
                        <asp:TemplateColumn HeaderText="Action">
                            <ItemTemplate>
                                <asp:RadioButtonList ID="rbEnrollAction" runat="server" RepeatDirection="Horizontal" Font-Names="Verdana" Font-Size="7pt">
                                    <asp:ListItem resourcekey="Approve" Value="Approve">Approve</asp:ListItem>
                                    <asp:ListItem resourcekey="Deny" Value="Deny">Deny</asp:ListItem>
                                </asp:RadioButtonList>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:BoundColumn DataField="EventTimeBegin" HeaderText="Date" DataFormatString="{0:d}"></asp:BoundColumn>
                        <asp:BoundColumn DataField="EventTimeBegin" HeaderText="Time" DataFormatString="{0:t}"></asp:BoundColumn>
                        <asp:TemplateColumn HeaderText="Event">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEnrollEventName" CssClass="CommandButton" CommandArgument="Select" CommandName="Select" runat="server">
                                    <%# DataBinder.Eval(Container.DataItem, "EventName") %>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn>
                            <ItemTemplate>
                                <asp:ImageButton ID="btnUserEmail" runat="server" IconKey="Email" CausesValidation="false" CommandName="User" CommandArgument="Select" Visible='<%# DataBinder.Eval(Container.DataItem, "EmailVisible") %>'/>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:BoundColumn DataField="UserName" HeaderText="User"></asp:BoundColumn>
                        <asp:BoundColumn DataField="NoEnrolees" HeaderText="Qty"></asp:BoundColumn>
                    </Columns>
                </asp:DataGrid>
            </asp:Panel>
        </div>
        <div class="SubHead">
            <asp:Label ID="lblMessage" runat="server" CssClass="SubHead">Note: Deny will delete Event/Signup Entries from the Database!</asp:Label>
        </div>
        <ul class="dnnActions dnnClear">
            <li>
                <asp:LinkButton OnClick="returnButton_Click" ID="returnButton" CssClass="dnnPrimaryAction" runat="server" resourcekey="returnButton" CausesValidation="False"/>
            </li>
            <li>
                <asp:LinkButton OnClick="cmdUpdateSelected_Click" ID="cmdUpdateSelected" CssClass="dnnSecondaryAction" runat="server" resourcekey="cmdUpdateSelected" CausesValidation="False"/>
            </li>
            <li>
                <asp:LinkButton OnClick="cmdSelectApproveAll_Click" ID="cmdSelectApproveAll" CssClass="dnnSecondaryAction" runat="server" resourcekey="cmdSelectApproveAll" CausesValidation="False"/>
            </li>
            <li>
                <asp:LinkButton OnClick="cmdSelectDenyAll_Click" ID="cmdSelectDenyAll" CssClass="dnnSecondaryAction" runat="server" resourcekey="cmdSelectDenyAll" CausesValidation="False"/>
            </li>
            <li>
                <asp:LinkButton OnClick="cmdUnmarkAll_Click" ID="cmdUnmarkAll" CssClass="dnnSecondaryAction" runat="server" resourcekey="cmdUnmarkAll" CausesValidation="False"/>
            </li>
        </ul>
    </div>
</asp:Panel>