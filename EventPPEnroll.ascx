<%@ Control Language="C#" AutoEventWireup="true" Codebehind="EventPPEnroll.ascx.cs" Inherits="DotNetNuke.Modules.Events.EventPPEnroll" %>
<%@ Register Src="~/controls/LabelControl.ascx" TagName="Label" TagPrefix="dnn" %>
<asp:Panel ID="pnlEventsModulePayPal" runat="server">
    <div id="PPEnroll" class="PPEnroll dnnForm" runat="server">
        <div id="divMessage" runat="server">
            <asp:Label ID="lblMessage" runat="server"></asp:Label>
        </div>
        <fieldset>
            <div class="dnnFormItem">
                <dnn:Label ID="lblEventNameCap" runat="server" ResourceKey="plEventName" Text="Event Name"></dnn:Label>
                <asp:Label ID="lblEventName" runat="server" CssClass="Normal"></asp:Label>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblEventStart" runat="server" ResourceKey="plEventStart" Text="Event Date/Time"></dnn:Label>
                <asp:Label ID="lblStartDate" runat="server" Width="300px" CssClass="Normal"></asp:Label>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblDescriptionCap" runat="server" ResourceKey="plDescription" Text="Description"></dnn:Label>
                <asp:Label ID="lblDescription" runat="server" CssClass="Normal"></asp:Label>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblFeeCap" runat="server" ResourceKey="plFee" Text="Event Fee"></dnn:Label>
                <asp:Label ID="lblFee" runat="server" CssClass="Normal"></asp:Label>
                <asp:Label ID="lblFeeCurrency" CssClass="NormalBold" runat="server"></asp:Label>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblNoEnroled" runat="server" ResourceKey="plNoEnrolees" Text="No. Enrolees"></dnn:Label>
                <asp:Label ID="lblNoEnrolees" runat="server" CssClass="Normal"></asp:Label>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblTotalCharges" runat="server" ResourceKey="plTotal" Text="Total Charges"></dnn:Label>
                <asp:Label ID="lblTotal" runat="server" CssClass="Normal"></asp:Label>
                <asp:Label ID="lblTotalCurrency" CssClass="NormalBold" runat="server"></asp:Label>
            </div>
            <div class="dnnFormItem">
                <asp:Panel ID="Panel1" runat="server" CssClass="SubHead" Width="100%">
                    <asp:Label ID="lblPurchase" runat="server">Pressing the "Purchase" link below will take you to the PayPal secure payment form. You will then be able to approve the payment and, after completion, will be returned back to the Event Enrollment confirmation form.</asp:Label>
                </asp:Panel>
            </div>
        </fieldset>
        <ul class="dnnActions dnnClear">
            <li>
                <asp:LinkButton OnClick="cmdReturn_Click" ID="cmdReturn" runat="server" resourcekey="cmdReturn" CssClass="dnnPrimaryAction" CausesValidation="False" Visible="False"/>
            </li>
            <li>
                <asp:LinkButton OnClick="cmdPurchase_Click" ID="cmdPurchase" runat="server" resourcekey="cmdPurchase" CssClass="dnnPrimaryAction"/>
            </li>
            <li>
                <asp:LinkButton OnClick="cancelButton_Click" ID="cancelButton" runat="server" resourcekey="cancelButton" CssClass="dnnSecondaryAction" CausesValidation="False"/>
            </li>
        </ul>
    </div>
</asp:Panel>