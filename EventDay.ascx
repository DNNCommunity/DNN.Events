<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EventDay.ascx.cs" Inherits="DotNetNuke.Modules.Events.EventDay" %>
<%@ Register TagPrefix="evt" TagName="Category" Src="~/DesktopModules/Events/SubControls/SelectCategory.ascx" %>
<%@ Register TagPrefix="evt" TagName="Location" Src="~/DesktopModules/Events/SubControls/SelectLocation.ascx" %>
<%@ Register TagPrefix="evt" TagName="Icons" Src="~/DesktopModules/Events/SubControls/EventIcons.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web.Deprecated" Namespace="DotNetNuke.Web.UI.WebControls" %>

<dnn:DnnToolTipManager
    ID="toolTipManager" runat="server" HideEvent="LeaveTargetAndToolTip" Modal="False" EnableShadow="True" CssClass="HTMLNtooltip" ShowCallout="False"/>
<asp:Panel ID="pnlEventsModuleDay" runat="server">
    <div style="text-align: center">
        <div class="EvtHdrLftCol"></div>
        <div class="EvtHdrMdlCol">
            <evt:Category ID="SelectCategory" runat="server" OnCategorySelectedChanged="SelectCategory_CategorySelected"></evt:Category>
            <evt:Location ID="SelectLocation" runat="server" OnLocationSelectedChanged="SelectLocation_LocationSelected"></evt:Location>
        </div>
        <div class="TopIconBar EvtHdrRgtCol">
            <evt:Icons ID="EventIcons" runat="server"></evt:Icons>
        </div>
        <div style="clear: both">
            <asp:DataGrid ID="lstEvents" ClientIDMode="AutoID" CellPadding="2" AutoGenerateColumns="False" GridLines="None" runat="server" ShowHeader="False"
                          CssClass="ListDataGrid" DataKeyField="EventID" Width="100%" OnItemDataBound="lstEvents_ItemDataBound" OnItemCommand="lstEvents_ItemCommand">
                <AlternatingItemStyle CssClass="ListAlternate"></AlternatingItemStyle>
                <Columns>
                    <asp:TemplateColumn>
                        <HeaderStyle CssClass="ListHeader"></HeaderStyle>
                        <ItemStyle CssClass="ListEdit"></ItemStyle>
                        <ItemTemplate>
                            <asp:ImageButton ID="btnEventEdit" runat="server" IconKey="Edit" CausesValidation="false" CommandName="Edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "EventID") %>'
                                             visible='<%# DataBinder.Eval(Container.DataItem, "EditVisibility") %>'/>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Event Start">
                        <HeaderStyle CssClass="ListHeader"></HeaderStyle>
                        <ItemStyle CssClass="ListDate"></ItemStyle>
                        <ItemTemplate>
                            <asp:Label ID="lblEventBegin" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "txtEventTimeBegin") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Event End">
                        <HeaderStyle CssClass="ListHeader"></HeaderStyle>
                        <ItemStyle CssClass="ListDate"></ItemStyle>
                        <ItemTemplate>
                            <asp:Label ID="lblEventEnd" Text='<%# DataBinder.Eval(Container.DataItem, "txtEventDateEnd") %>' runat="server">
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Title">
                        <HeaderStyle CssClass="ListHeader"></HeaderStyle>
                        <ItemStyle CssClass="ListTitle"></ItemStyle>
                        <ItemTemplate>
                            <asp:label ID="lblIcons" Text='<%# DataBinder.Eval(Container.DataItem, "Icons") %>' runat="server"></asp:label>
                            <asp:HyperLink ID="lnkEvent" CssClass="ListTitle" runat="Server" NavigateUrl='<%# DataBinder.Eval(Container.DataItem, "URL") %>' Target='<%# DataBinder.Eval(Container.DataItem, "Target") %>'
                                           Text='<%# DataBinder.Eval(Container.DataItem, "EventName") %>' forecolor='<%# DataBinder.Eval(Container.DataItem, "CategoryFontColor") %>'>
                            </asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn>
                        <HeaderStyle CssClass="ListHeader"></HeaderStyle>
                        <ItemStyle Wrap="False" CssClass="ListLink"></ItemStyle>
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ImageURL") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Duration">
                        <HeaderStyle CssClass="ListHeader"></HeaderStyle>
                        <ItemStyle CssClass="ListDuration"></ItemStyle>
                        <ItemTemplate>
                            <asp:Label id="lblDuration" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "DisplayDuration") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Category">
                        <HeaderStyle CssClass="ListHeader"></HeaderStyle>
                        <ItemStyle CssClass="ListCategory"></ItemStyle>
                        <ItemTemplate>
                            <asp:Label id="lblCategory" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CategoryName") %>' forecolor='<%# DataBinder.Eval(Container.DataItem, "CategoryFontColor") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Location">
                        <HeaderStyle CssClass="ListHeader"></HeaderStyle>
                        <ItemStyle CssClass="ListLocation"></ItemStyle>
                        <ItemTemplate>
                            <asp:Label id="lblLocation" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "LocationName") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="CustomField1">
                        <HeaderStyle CssClass="ListHeader"></HeaderStyle>
                        <ItemStyle CssClass="ListCustomField1"></ItemStyle>
                        <ItemTemplate>
                            <asp:Label id="lblCustomField1" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CustomField1") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="CustomField2">
                        <HeaderStyle CssClass="ListHeader"></HeaderStyle>
                        <ItemStyle CssClass="ListCustomField2"></ItemStyle>
                        <ItemTemplate>
                            <asp:Label id="lblCustomField2" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CustomField2") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Description" SortExpression="Description">
                        <HeaderStyle CssClass="ListHeader"></HeaderStyle>
                        <ItemStyle CssClass="ListDescription"></ItemStyle>
                        <ItemTemplate>
                            <asp:Label ID="lblDescription" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "DecodedDesc") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="RecurText">
                        <HeaderStyle CssClass="ListHeader"></HeaderStyle>
                        <ItemStyle CssClass="ListDescription"></ItemStyle>
                        <ItemTemplate>
                            <asp:Label ID="lblRecurText" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "RecurText") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="RecurUntil">
                        <HeaderStyle CssClass="ListHeader"></HeaderStyle>
                        <ItemStyle CssClass="ListDate"></ItemStyle>
                        <ItemTemplate>
                            <asp:Label ID="lblRecurUntil" Text='<%# DataBinder.Eval(Container.DataItem, "RecurUntil") %>' runat="server">
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
        </div>
        <div id="divMessage" runat="server" visible="false" class="dnnForm">
            <div class="dnnFormMessage dnnFormWarning">
                <asp:Label ID="lblMessage" runat="server" resourcekey="lblMessage">&nbsp;No Events for this day...</asp:Label>
            </div>
        </div>
        <div class="BottomIconBar">
            <evt:Icons ID="EventIcons2" runat="server"></evt:Icons>
        </div>
        <ul class="dnnActions dnnClear">
            <li>
                <asp:LinkButton OnClick="returnButton_Click" ID="returnButton" runat="server" CssClass="dnnPrimaryAction" IconKey="Lt" CausesValidation="False" resourcekey="returnButton"/>
            </li>
        </ul>
    </div>
</asp:Panel>