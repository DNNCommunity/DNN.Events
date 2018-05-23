<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SelectLocation.ascx.cs" Inherits="DotNetNuke.Modules.Events.SelectLocation" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web.Deprecated" Namespace="DotNetNuke.Web.UI.WebControls" %>

<div class="SelCategoryTR">
    <div class="SelCategory SubHead">
        <asp:label id="lblLocation" runat="server" resourcekey="lblLocation"></asp:label>
        <dnn:DnnComboBox ID="ddlLocations" runat="server" CheckBoxes="True" EnableCheckAllItemsCheckBox="true"
                         AllowCustomText="False" DataValueField="Location" DataTextField="LocationName" InputCssClass="CategoryFormat" DropDownCssClass="CategoryFormat" CssClass="SelectCategory">
        </dnn:DnnComboBox>
        <asp:Button ID="btnUpdate" runat="server" style="display: none" OnClick="btnUpdate_Click"/>
    </div>
</div>