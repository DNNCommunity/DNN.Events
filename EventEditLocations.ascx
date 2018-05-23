<%@ Control Language="C#" AutoEventWireup="true" Codebehind="EventEditLocations.ascx.cs" Inherits="DotNetNuke.Modules.Events.EventEditLocations" %>
<%@ Register Src="~/controls/LabelControl.ascx" TagName="Label" TagPrefix="dnn" %>
<%@ Register Assembly="CountryListBox" Namespace="DotNetNuke.UI.WebControls" TagPrefix="dnn2" %>

<asp:Panel ID="pnlEventsModuleLocations" runat="server">
    <div class="dnnForm EventEditLocations">
        <div style="float: left; width: 50%;">
            <div class="dnnFormItem">
                <dnn:Label ID="lblLocationCap" runat="server" cssclass="dnnFormRequired SubHead" ResourceKey="plLocation" Text="Location"></dnn:Label>
                <asp:TextBox ID="txtLocationName" runat="server" cssclass="dnnFormRequired NormalTextBox" Width="200px"></asp:TextBox>
                <asp:RequiredFieldValidator id="valRequiredName" runat="server" cssclass="dnnFormMessage dnnFormError" resourcekey="valRequiredName" ControlToValidate="txtLocationName" ValidationGroup="EventEditLocation"></asp:RequiredFieldValidator>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblMapURLCap" runat="server" CssClass="SubHead" ResourceKey="plMapURL" Text="Map URL"/>
                <asp:TextBox ID="txtMapURL" runat="server" CssClass="NormalTextBox" Width="200px"></asp:TextBox>
            </div>
            <%--        <div class="dnnFormItem">
            <dnn:Address id="AddressLocation" runat="server"></dnn:Address>
        </div>--%>
            <div class="dnnFormItem">
                <dnn:Label ID="lblStreetCap" runat="server" CssClass="SubHead" ResourceKey="plStreet" Text="Street"/>
                <asp:TextBox ID="txtStreet" runat="server" CssClass="NormalTextBox" Width="200px"></asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblPostalCodeCap" runat="server" CssClass="SubHead" ResourceKey="plPostalCode" Text="Postal Code"/>
                <asp:TextBox ID="txtPostalCode" runat="server" CssClass="NormalTextBox" Width="200px"></asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblCityCap" runat="server" CssClass="SubHead" ResourceKey="plCity" Text="City"/>
                <asp:TextBox ID="txtCity" runat="server" CssClass="NormalTextBox" Width="200px"></asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblCountryCap" runat="server" CssClass="SubHead" ResourceKey="plCountry" Text="Country"/>
                <dnn2:CountryListBox ID="cboCountry" runat="server" GeoIPFile="" TestIP="" LocalhostCountryCode=""
                                     DataValueField="Value" DataTextField="Text" AutoPostBack="true"
                                     OnSelectedIndexChanged="OnCountryIndexChanged">
                </dnn2:CountryListBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblRegionCap" runat="server" CssClass="SubHead" ResourceKey="plRegion" Text="Region"/>
                <asp:DropDownList ID="cboRegion" runat="server" DataValueField="EntryID" DataTextField="Text"></asp:DropDownList>
                <asp:TextBox ID="txtRegion" runat="server" CssClass="NormalTextBox" Width="200px"></asp:TextBox>
            </div>
            <ul class="dnnActions dnnClear">
                <li>
                    <asp:LinkButton OnClick="cmdAdd_Click" ID="cmdAdd" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdAdd" ValidationGroup="EventEditLocation"/>
                </li>
                <li>
                    <asp:LinkButton OnClick="cmdUpdate_Click" ID="cmdUpdate" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdUpdate" Visible="false" ValidationGroup="EventEditLocation"/>
                </li>
                <li>
                    <asp:LinkButton OnClick="returnButton_Click" ID="returnButton" CssClass="dnnSecondaryAction" runat="server" resourcekey="returnButton" CausesValidation="False"/>
                </li>
            </ul>
        </div>
        <div style="float: left; width: 50%;">
            <asp:DataGrid ID="GrdLocations" runat="server" AutoGenerateColumns="False" BorderStyle="Outset" BorderWidth="1px" CssClass="Normal"
                          DataKeyField="Location" GridLines="Horizontal" OnDeleteCommand="GrdLocations_DeleteCommand" OnItemCommand="GrdLocations_ItemCommand"
                          Width="250px">
                <EditItemStyle VerticalAlign="Bottom"></EditItemStyle>
                <AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
                <ItemStyle VerticalAlign="Top"></ItemStyle>
                <HeaderStyle Font-Bold="True" BackColor="Silver"></HeaderStyle>
                <Columns>
                    <asp:TemplateColumn>
                        <ItemStyle HorizontalAlign="Left"></ItemStyle>
                        <ItemTemplate>
                            <asp:ImageButton ID="DeleteButton" runat="server" IconKey="Delete" CommandArgument="Delete" CommandName="Delete" AlternateText="Delete" resourcekey="DeleteButton"
                                             CausesValidation="false">
                            </asp:ImageButton>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn Visible="False" HeaderText="PortalID">
                        <ItemTemplate>
                            <asp:Label ID="Label2" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "PortalID") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn Visible="False" HeaderText="Location">
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Location") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Location Name">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkLocationName" CommandName="Select" CommandArgument="Select" Text='<%# DataBinder.Eval(Container.DataItem, "LocationName") %>'
                                            runat="server">
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Map URL">
                        <ItemTemplate>
                            <asp:HyperLink ID="lnkMapURL" Target="_blank" NavigateUrl='<%# DataBinder.Eval(Container.DataItem, "MapURL") %>' runat="server">
                                <%# DataBinder.Eval(Container.DataItem, "MapURL") %>
                            </asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
            <asp:Label ID="lblEditMessage" CssClass="SubHead" runat="server" resourcekey="lblEditMessage">(Select Item Link to Edit)</asp:Label>
        </div>
    </div>
</asp:Panel>