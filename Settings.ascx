<%@ Control Language="vb" AutoEventWireup="false" Codebehind="Settings.ascx.vb" Inherits="DotNetNuke.Modules.Events.Settings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnFormMessage dnnFormInfo">
    <asp:Label ID="lblSettingsMoved" resourcekey="lblSettingsMoved" runat="server">Settings can now be edited via the Edit Event option in the action menu</asp:Label>
</div>
<div id="tblMain" style="width:100%;" visible="false" class="dnnForm EventModuleSettings" >
    <div id="divUpgrade" runat="server">
        <div class="dnnFormMessage dnnFormValidationSummary" style="width:300px">
            <dnn:Label ID="plUpgrade" Text="Upgrade:" runat="server" ControlName="cmdUpgrade"></dnn:Label>
        </div>
        <div style="white-space:nowrap;margin-left:2em;">
            <asp:LinkButton ID="cmdUpgrade" CssClass="dnnPrimaryAction" runat="server" EnableViewState="False" Text="Re-start Upgrade"></asp:LinkButton>
        </div>
        <div id="divRetry" class="dnnFormMessage dnnFormWarning" visible="false" style="width:300px" runat="server">
            <dnn:Label ID="plRetry" Text="Retry" runat="server" ControlName="cmdUpgrade"></dnn:Label>
        </div>
    </div>
</div>
