<%@ Control Language="vb" AutoEventWireup="false" Codebehind="SelectCategory.ascx.vb" Inherits="DotNetNuke.Modules.Events.SelectCategory" %>
<%@ Register Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" TagPrefix="dnn" %>

<div class="SelCategoryTR">
    <div class="SelCategory SubHead">
		<asp:label id="lblCategory" runat="server" resourcekey="lblCategory"></asp:label>
        <dnn:DnnComboBox ID="ddlCategories" runat="server" CheckBoxes="True" EnableCheckAllItemsCheckBox="true" 
		    AllowCustomText="False" DataValueField="Category" DataTextField="CategoryName"  InputCssClass="CategoryFormat" DropDownCssClass="CategoryFormat" CssClass="SelectCategory">
		</dnn:DnnComboBox>
        <asp:Button ID="btnUpdate" runat="server" style="display:none" />
    </div>
</div>

