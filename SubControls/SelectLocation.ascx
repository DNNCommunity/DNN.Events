<%@ Control Language="vb" AutoEventWireup="false" Codebehind="SelectLocation.ascx.vb" Inherits="DotNetNuke.Modules.Events.SelectLocation" %>
<%@ Register Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" TagPrefix="dnn" %>

<div class="SelCategoryTR">
    <div class="SelCategory SubHead">
		<asp:label id="lblLocation" runat="server" resourcekey="lblLocation"></asp:label>
        <dnn:DnnComboBox ID="ddlLocations" runat="server" CheckBoxes="True" EnableCheckAllItemsCheckBox="true" 
		    AllowCustomText="False" DataValueField="Location" DataTextField="LocationName"  InputCssClass="CategoryFormat" DropDownCssClass="CategoryFormat" CssClass="SelectCategory">
		</dnn:DnnComboBox>
        <asp:Button ID="btnUpdate" runat="server" style="display:none" />
    </div>
</div>
