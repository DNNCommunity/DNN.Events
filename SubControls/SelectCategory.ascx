<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SelectCategory.ascx.cs" Inherits="DotNetNuke.Modules.Events.SelectCategory" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web.Deprecated" Namespace="DotNetNuke.Web.UI.WebControls" %>

<div class="SelCategoryTR">
    <div class="SelCategory SubHead">
        <asp:label id="lblCategory" runat="server" resourcekey="lblCategory"></asp:label>
        <dnn:DnnComboBox ID="ddlCategories" runat="server" CheckBoxes="True" EnableCheckAllItemsCheckBox="true"
                         AllowCustomText="False" DataValueField="Category" DataTextField="CategoryName" InputCssClass="CategoryFormat" DropDownCssClass="CategoryFormat" CssClass="SelectCategory">
        </dnn:DnnComboBox>
        <asp:Button ID="btnUpdate" runat="server" style="display: none" OnClick="btnUpdate_Click"/>
    </div>
</div>