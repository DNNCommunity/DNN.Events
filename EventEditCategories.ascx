<%@ Control Language="C#" AutoEventWireup="true" Codebehind="EventEditCategories.ascx.cs" Inherits="DotNetNuke.Modules.Events.EventEditCategories" %>
<%@ Register Src="~/controls/LabelControl.ascx" TagName="Label" TagPrefix="dnn" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web.Deprecated" Namespace="DotNetNuke.Web.UI.WebControls" %>
<asp:Panel ID="pnlEventsModuleCategories" runat="server">
    <div class="dnnForm EventEditCategories">
        <div id="divDeleteError" runat="server" visible="false" class="dnnForm">
            <div class="dnnFormMessage dnnFormValidationSummary">
                <asp:Label ID="lblDeleteError" runat="server">Cannot delete, linked to module settings.</asp:Label>
            </div>
        </div>
        <div style="float: left; width: 50%;">
            <div class="dnnFormItem">
                <dnn:Label ID="lblCategoryCap" runat="server" cssclass="dnnFormRequired SubHead" ResourceKey="plCategory" Text="Category:"/>
                <asp:TextBox ID="txtCategoryName" runat="server" cssclass="dnnFormRequired NormalTextBox"></asp:TextBox>
                <asp:RequiredFieldValidator id="valRequiredName" runat="server" cssclass="dnnFormMessage dnnFormError" resourcekey="valRequiredName" ControlToValidate="txtCategoryName" ValidationGroup="CategoryUpdate"></asp:RequiredFieldValidator>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblColorCap" runat="server" CssClass="SubHead" ResourceKey="plColor" Text="Color:"/>
                <table cellpadding="0" cellspacing="0">
                    <tr>
                        <td style="width: 150px;">
                            <asp:TextBox ID="txtCategoryColor" runat="server" CssClass="NormalTextBox" style=""></asp:TextBox>
                        </td>
                        <td style="width: 20px;">
                            <span id="colorPicker1">
                                <dnn:DnnColorPicker ID="cpBackColor" runat="server" EnableCustomColor="True" ShowIcon="True" Overlay="False" AutoPostBack="False" OnClientColorChange="HandleColorChange"></dnn:DnnColorPicker>
                            </span>
                        </td>
                        <td style="width: 100%;">
                            <asp:CustomValidator ID="ValColor" runat="server" SetFocusOnError="true" cssclass="dnnFormMessage dnnFormError"
                                                 ControlToValidate="txtCategoryColor" ClientValidationFunction="ValidateColor" resourcekey="InvalidColor" ValidationGroup="CategoryUpdate"/>
                        </td>
                    </tr>
                </table>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblFontColorCap" runat="server" CssClass="SubHead" ResourceKey="plFontColor" Text="Font Color:"/>
                <table cellpadding="0" cellspacing="0">
                    <tr>
                        <td style="width: 150px;">
                            <asp:TextBox ID="txtCategoryFontColor" runat="server" CssClass="NormalTextBox"></asp:TextBox>
                        </td>
                        <td style="width: 20px;">
                            <span id="colorPicker2">
                                <dnn:DnnColorPicker ID="cpForeColor" runat="server" EnableCustomColor="True" ShowIcon="True" Overlay="False" AutoPostBack="False" OnClientColorChange="HandleColorFontChange"></dnn:DnnColorPicker>
                            </span>
                        </td>
                        <td style="width: 100%;">
                            <asp:CustomValidator ID="CalFontColor" runat="server" SetFocusOnError="true" cssclass="dnnFormMessage dnnFormError"
                                                 ControlToValidate="txtCategoryFontColor" ClientValidationFunction="ValidateColor" resourcekey="InvalidColor" ValidationGroup="CategoryUpdate"/>
                        </td>
                    </tr>
                </table>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblPreview" runat="server" CssClass="SubHead" ResourceKey="plPreview" Text="Preview:"/>
                <table id="previewpane" class="SubHead" style="text-align: center; vertical-align: top;" runat="server">
                    <tr>
                        <td style="width: 145px;">
                            <span id="lblPreviewCat" style="" runat="server"></span>
                        </td>
                    </tr>
                </table>
            </div>
            <ul class="dnnActions dnnClear">
                <li>
                    <asp:LinkButton OnClick="cmdAdd_Click" ID="cmdAdd" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdAdd" ValidationGroup="CategoryUpdate"/>
                </li>
                <li>
                    <asp:LinkButton OnClick="cmdUpdate_Click" ID="cmdUpdate" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdUpdate" Visible="false" ValidationGroup="CategoryUpdate"/>
                </li>
                <li>
                    <asp:LinkButton OnClick="returnButton_Click" ID="returnButton" runat="server" CssClass="dnnSecondaryAction" resourcekey="returnButton" CausesValidation="False"/>
                </li>
            </ul>
        </div>
        <div style="float: left; width: 50%;">
            <asp:DataGrid ID="GrdCategories" runat="server" AutoGenerateColumns="False" BorderStyle="Outset" BorderWidth="1px" CssClass="Normal"
                          DataKeyField="Category" GridLines="Horizontal" OnDeleteCommand="GrdCategories_DeleteCommand" OnItemCommand="GrdCategories_ItemCommand"
                          Width="250px">
                <EditItemStyle VerticalAlign="Bottom"/>
                <AlternatingItemStyle BackColor="WhiteSmoke"/>
                <ItemStyle VerticalAlign="Top"/>
                <HeaderStyle BackColor="Silver" Font-Bold="True"/>
                <Columns>
                    <asp:TemplateColumn>
                        <ItemStyle HorizontalAlign="Left"/>
                        <ItemTemplate>
                            <asp:ImageButton ID="DeleteButton" runat="server" IconKey="Delete" AlternateText="Delete" CausesValidation="false" CommandArgument="Delete"
                                             CommandName="Delete" resourcekey="DeleteButton"/>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="PortalID" Visible="False">
                        <ItemTemplate>
                            <asp:Label ID="Label2" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "PortalID") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Category" Visible="False">
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Category") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Category Name">
                        <ItemTemplate>
                            <asp:Panel runat="server" BackColor='<%#
                                        this.GetColor((string) DataBinder.Eval(Container.DataItem, "Color")) %>'>
                                <asp:LinkButton ID="lnkCategoryName" runat="server" forecolor='<%#
                                        this.GetColor((string) DataBinder.Eval(Container.DataItem, "FontColor")) %>' CommandArgument="Select"
                                                CommandName="Select" Text='<%# DataBinder.Eval(Container.DataItem, "CategoryName") %>'>
                                </asp:LinkButton>
                            </asp:Panel>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
            <asp:Label ID="lblEditMessage" runat="server" CssClass="SubHead" resourcekey="lblEditMessage">(Select Item Link to Edit)</asp:Label>
        </div>
    </div>
</asp:Panel>