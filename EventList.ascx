<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EventList.ascx.cs" Inherits="DotNetNuke.Modules.Events.EventList" %>
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
        <asp:GridView ID="gvEvents" ClientIDMode="AutoID" AllowPaging="True" PageSize="25" AllowSorting="True" GridLines="None" runat="server" CssClass="ListDataGrid" Width="100%" AutoGenerateColumns="False" OnRowCreated="gvEvents_RowCreated" OnRowDataBound="gvEvents_RowDataBound" OnPageIndexChanging="gvEvents_PageIndexChanging" OnRowCommand="gvEvents_RowCommand" OnSorting="gvEvents_Sorting">
            <AlternatingRowStyle CssClass="ListAlternate"/>
            <RowStyle CssClass="ListNormal"/>
            <PagerStyle CssClass="ListPager"/>
            <Columns>
                <asp:TemplateField>
                    <HeaderStyle CssClass="ListHeader"></HeaderStyle>
                    <ItemStyle CssClass="ListEdit"></ItemStyle>
                    <ItemTemplate>
                        <asp:ImageButton ID="btnEventEdit" runat="server" IconKey="Edit" CausesValidation="false" CommandName="Edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "EventId") %>'
                                         visible='<%# DataBinder.Eval(Container.DataItem, "EditVisibility") %>'/>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField SortExpression="EventDateBegin" HeaderText="Event Start">
                    <HeaderStyle CssClass="ListHeader"></HeaderStyle>
                    <ItemStyle CssClass="ListDate"></ItemStyle>
                    <ItemTemplate>
                        <asp:Label ID="lblEventBegin" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "txtEventTimeBegin") %>' forecolor='<%# DataBinder.Eval(Container.DataItem, "CategoryFontColor") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField SortExpression="EventDateEnd" HeaderText="Event End">
                    <HeaderStyle CssClass="ListHeader"></HeaderStyle>
                    <ItemStyle CssClass="ListDate"></ItemStyle>
                    <ItemTemplate>
                        <asp:Label ID="lblEventEnd" Text='<%# DataBinder.Eval(Container.DataItem, "txtEventDateEnd") %>' runat="server" forecolor='<%# DataBinder.Eval(Container.DataItem, "CategoryFontColor") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField SortExpression="EventName" HeaderText="Title">
                    <HeaderStyle CssClass="ListHeader"></HeaderStyle>
                    <ItemStyle CssClass="ListTitle"></ItemStyle>
                    <ItemTemplate>
                        <%#Eval("Icons") %>
                        <asp:HyperLink CssClass="ListTitle" ID="lnkEvent" runat="Server" NavigateUrl='<%# DataBinder.Eval(Container.DataItem, "URL") %>' Target='<%# DataBinder.Eval(Container.DataItem, "Target") %>'
                                       Text='<%# DataBinder.Eval(Container.DataItem, "EventName") %>'>
                        </asp:HyperLink>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField >
                    <HeaderStyle CssClass="ListHeader"></HeaderStyle>
                    <ItemStyle Wrap="False" CssClass="ListLink"></ItemStyle>
                    <ItemTemplate>
                        <asp:Label ID="imgEvent" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ImageURL") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField SortExpression="Duration" HeaderText="Duration">
                    <HeaderStyle CssClass="ListHeader"></HeaderStyle>
                    <ItemStyle CssClass="ListDuration"></ItemStyle>
                    <ItemTemplate>
                        <asp:Label id="lblDuration" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "DisplayDuration") %>' forecolor='<%# DataBinder.Eval(Container.DataItem, "CategoryFontColor") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField SortExpression="CategoryName" HeaderText="Category">
                    <HeaderStyle CssClass="ListHeader"></HeaderStyle>
                    <ItemStyle CssClass="ListCategory"></ItemStyle>
                    <ItemTemplate>
                        <asp:Label id="lblCategory" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CategoryName") %>' forecolor='<%# DataBinder.Eval(Container.DataItem, "CategoryFontColor") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField SortExpression="LocationName" HeaderText="Location">
                    <HeaderStyle CssClass="ListHeader"></HeaderStyle>
                    <ItemStyle CssClass="ListLocation"></ItemStyle>
                    <ItemTemplate>
                        <asp:Label id="lblLocation" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "LocationName") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField SortExpression="CustomField1" HeaderText="CustomField1">
                    <HeaderStyle CssClass="ListHeader"></HeaderStyle>
                    <ItemStyle CssClass="ListCustomField1"></ItemStyle>
                    <ItemTemplate>
                        <asp:Label id="lblCustomField1" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CustomField1") %>' forecolor='<%# DataBinder.Eval(Container.DataItem, "CategoryFontColor") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField SortExpression="CustomField2" HeaderText="CustomField2">
                    <HeaderStyle CssClass="ListHeader"></HeaderStyle>
                    <ItemStyle CssClass="ListCustomField2"></ItemStyle>
                    <ItemTemplate>
                        <asp:Label id="lblCustomField2" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CustomField2") %>' forecolor='<%# DataBinder.Eval(Container.DataItem, "CategoryFontColor") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField SortExpression="Description" HeaderText="Description">
                    <HeaderStyle CssClass="ListHeader"></HeaderStyle>
                    <ItemStyle CssClass="ListDescription"></ItemStyle>
                    <ItemTemplate>
                        <asp:Label ID="lblDescription" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "DecodedDesc") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField SortExpression="RecurText" HeaderText="RecurText">
                    <HeaderStyle CssClass="ListHeader"></HeaderStyle>
                    <ItemStyle CssClass="ListRecurText"></ItemStyle>
                    <ItemTemplate>
                        <asp:Label ID="lblRecurText" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "RecurText") %>' forecolor='<%# DataBinder.Eval(Container.DataItem, "CategoryFontColor") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField SortExpression="RecurUntil" HeaderText="RecurUntil">
                    <HeaderStyle CssClass="ListHeader"></HeaderStyle>
                    <ItemStyle CssClass="ListRecurUntil"></ItemStyle>
                    <ItemTemplate>
                        <asp:Label ID="lblRecurUntil" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "RecurUntil") %>' forecolor='<%# DataBinder.Eval(Container.DataItem, "CategoryFontColor") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
    <div id="divNoEvents" runat="server" visible="false" class="dnnForm">
        <div class="dnnFormMessage dnnFormWarning">
            <asp:Label ID="lblNoEvents" runat="server" resourcekey="lblNoEvents">No events to display.</asp:Label>
        </div>
    </div>
    <div class="BottomIconBar">
        <evt:Icons ID="EventIcons2" runat="server"></evt:Icons>
    </div>
</div>