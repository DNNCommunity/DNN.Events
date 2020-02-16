<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EventDetails.ascx.cs" Inherits="DotNetNuke.Modules.Events.EventDetails" %>

<asp:Panel ID="pnlEventsModuleDetails" runat="server" class="DetailEvent">
    <div id="divMessage" runat="server">
        <asp:Label ID="lblMessage" runat="server"></asp:Label>
    </div>
    <div id="divEventDetails1" runat="server" class="DetailClear DetailEventDetails1"/>
    <div id="divReminder" Runat="server" class="DetailClear DetailReminderDiv">
        <div class="SubHead DetailReminderLeft">
            <div id="rem1" runat="server" visible="false">
                <asp:Image ID="imgNotify" runat="server" ImageUrl="Images/bell.gif"/>&nbsp;
                <asp:LinkButton ID="cmdNotify" CssClass="CommandButton" runat="server" BorderStyle="none" resourcekey="cmdNotify" ValidationGroup="EventEmail" OnClick="cmdNotify_Click">Notify Me for this Event @</asp:LinkButton><br/>
                <asp:CheckBox ID="chkReminderRec" resourcekey="chkReminderRec" Visible="false" runat="server" CssClass="SubHead" Text=""></asp:CheckBox><br/>
            </div>
        </div>
        <div class="SubHead DetailReminderRight">
            <div id="rem2" runat="server" visible="false">
                <asp:TextBox ID="txtUserEmail" CssClass="NormalTextBox" runat="server" Width="199px"></asp:TextBox>
                <asp:RegularExpressionValidator ID="valEmail" ValidationGroup="EventEmail" runat="server" resourcekey="valEmail" cssclass="dnnFormMessage dnnFormError"
                                                ControlToValidate="txtUserEmail" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" Display="Dynamic">
                </asp:RegularExpressionValidator>
                <asp:RequiredFieldValidator ID="valEmail2" ValidationGroup="EventEmail" runat="server" resourcekey="valEmail" cssclass="dnnFormMessage dnnFormError"
                                            ControlToValidate="txtUserEmail" Display="Dynamic">
                </asp:RequiredFieldValidator><br/>
                <asp:TextBox ID="txtReminderTime" CssClass="NormalTextBox" runat="server" MaxLength="3" Width="30" Font-Size="8pt">8</asp:TextBox>
                <asp:DropDownList ID="ddlReminderTimeMeasurement" runat="server" Width="90" Font-Size="8pt" CssClass="NormalTextBox">
                    <asp:ListItem Value="m" resourcekey="Minutes">Minutes</asp:ListItem>
                    <asp:ListItem Value="h" resourcekey="Hours" Selected="True">Hours</asp:ListItem>
                    <asp:ListItem Value="d" resourcekey="Days">Days</asp:ListItem>
                </asp:DropDownList> <asp:Label ID="lblTimeBefore" resourcekey="lblTimeBefore" runat="server"></asp:Label>
                <asp:RequiredFieldValidator ID="valReminderTime2" ValidationGroup="EventEmail" runat="server" cssclass="dnnFormMessage dnnFormError"
                                            ControlToValidate="txtReminderTime" Display="Dynamic">
                </asp:RequiredFieldValidator>
                <asp:RangeValidator id="valReminderTime" runat="server" ValidationGroup="EventEmail"
                                    ControlToValidate="txtReminderTime" Display="Dynamic" Type="Integer" cssclass="dnnFormMessage dnnFormError">
                </asp:RangeValidator>
            </div>
            <div id="rem3" runat="server" visible="false">
                <asp:Image ID="imgConfirmation" runat="server" ImageUrl="Images/bell.gif"/>
                <asp:Label ID="lblConfirmation" runat="server"></asp:Label>
            </div>
        </div>
    </div>
    <div id="divIcalendar" Runat="server" class="DetailClear DetailiCalDiv">
        <div class="SubHead DetailiCalLeft">
            <asp:Image ID="imgEmail" runat="server" IconKey="Email"/>&nbsp;
            <asp:LinkButton ID="cmdEmail" CssClass="CommandButton" runat="server" BorderStyle="none" resourcekey="cmdEmail" ValidationGroup="EventEmailiCal" OnClick="cmdEmail_Click">
            </asp:LinkButton>
        </div>
        <div class="SubHead DetailiCalRight">
            <div id="iCal1" runat="server">
                <asp:TextBox ID="txtUserEmailiCal" CssClass="NormalTextBox" runat="server" Width="199px"></asp:TextBox>
                <asp:RegularExpressionValidator ID="valEmailiCal" ValidationGroup="EventEmailiCal" runat="server" resourcekey="valEmail" cssclass="dnnFormMessage dnnFormError"
                                                ControlToValidate="txtUserEmailiCal" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" Display="Dynamic">
                </asp:RegularExpressionValidator>
                <asp:RequiredFieldValidator ID="valEmailiCal2" ValidationGroup="EventEmailiCal" runat="server" resourcekey="valEmail" cssclass="dnnFormMessage dnnFormError"
                                            ControlToValidate="txtUserEmailiCal" Display="Dynamic">
                </asp:RequiredFieldValidator><br/>
            </div>
        </div>
    </div>
    <div id="divEventDetails2" runat="server" class="DetailClear DetailEventDetails2"/>
    <div id="divEnrollment" Runat="server" class="DetailClear DetailEnrollDiv">
        <div class="SubHead DetailEnrollLeft">
            <div id="enroll1" runat="server" visible="false">
                <asp:Image ID="imgEnroll" runat="server" ImageUrl="Images/enroll.gif"/>&nbsp;
                <asp:LinkButton ID="cmdSignup" CssClass="CommandButton" runat="server" BorderStyle="none"
                                ValidationGroup="EventSignup" OnClick="cmdSignup_Click">Enroll for this Event?</asp:LinkButton>
            </div>
            <div id="enroll4" runat="server" visible="false">
                <asp:Image ID="imgEnrollTooLate" runat="server" ImageUrl="Images/EnrollFull.gif"/>&nbsp;
                <asp:Label ID="lblEnrollTooLate" runat="server">Event has started, it is no longer possible to enroll.</asp:Label>
            </div>
        </div>
        <div class="SubHead DetailEnrollRight">
            <div id="enroll3" runat="server" visible="false">
                <asp:Label ID="lblNoEnrolee" runat="server" resourcekey="lblNoEnrolee" class="DetailEnrollLabel" Width="100px">No. of Enrolees</asp:Label>&nbsp;
                <asp:TextBox ID="txtNoEnrolees" runat="server" CssClass="NormalTextBox" Font-Size="8pt" MaxLength="3" Width="30">1</asp:TextBox>&nbsp;
                <asp:Label ID="lblMaxNoEnrolees" runat="server">(Max 1)</asp:Label>&nbsp;
                <asp:RangeValidator ID="valNoEnrolees" runat="server"
                                    ControlToValidate="txtNoEnrolees" MinimumValue="1" MaximumValue="1" Display="Dynamic" Type="Integer" cssclass="dnnFormMessage dnnFormError"
                                    ValidationGroup="EventSignup">
                </asp:RangeValidator>
                <asp:RequiredFieldValidator ID="valNoEnrolees2" ValidationGroup="EventSignup" runat="server" cssclass="dnnFormMessage dnnFormError"
                                            ControlToValidate="txtNoEnrolees" Display="Dynamic">
                </asp:RequiredFieldValidator>
            </div>
            <div id="enroll5" runat="server" visible="false">
                <div>
                    <asp:Label ID="lblAnonEmail" runat="server" resourcekey="lblAnonEmail" class="DetailEnrollLabel" Width="100px">Email Address:</asp:Label>&nbsp;
                    <asp:TextBox ID="txtAnonEmail" CssClass="NormalTextBox" runat="server" Width="199px"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="valAnonEmail" ValidationGroup="EventSignup" runat="server" resourcekey="valEmail" cssclass="dnnFormMessage dnnFormError"
                                                    ControlToValidate="txtAnonEmail" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" Display="Dynamic">
                    </asp:RegularExpressionValidator>
                    <asp:RequiredFieldValidator ID="valAnonEmail2" ValidationGroup="EventSignup" runat="server" resourcekey="valEmail" cssclass="dnnFormMessage dnnFormError"
                                                ControlToValidate="txtAnonEmail" Display="Dynamic">
                    </asp:RequiredFieldValidator>
                </div>
                <div>
                    <asp:Label ID="lblAnonName" runat="server" resourcekey="lblAnonName" class="DetailEnrollLabel" Width="100px">Name:</asp:Label>&nbsp;
                    <asp:TextBox ID="txtAnonName" CssClass="NormalTextBox" runat="server" Width="199px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="valAnonName" ValidationGroup="EventSignup" runat="server" resourcekey="valAnonName" cssclass="dnnFormMessage dnnFormError"
                                                ControlToValidate="txtAnonName" Display="Dynamic">
                    </asp:RequiredFieldValidator>
                </div>
                <div>
                    <asp:Label ID="lblAnonTelephone" runat="server" resourcekey="lblAnonTelephone" class="DetailEnrollLabel" Width="100px">Name:</asp:Label>&nbsp;
                    <asp:TextBox ID="txtAnonTelephone" CssClass="NormalTextBox" runat="server" Width="199px"></asp:TextBox>
                </div>
            </div>
            <div id="enroll2" runat="server" visible="false">
                <asp:Image ID="imgSignup" runat="server" ImageUrl="Images/enroll.gif"/>&nbsp;
                <asp:Label ID="lblSignup" runat="server">You are not enrolled!</asp:Label>
            </div>
        </div>
    </div>
    <div id="divEventDetails3" runat="server" class="DetailClear DetailEventDetails3"/>
    <div id="divEnrollList" Runat="server" class="DetailClear DetailEnrollTableDiv">
        <div class="SubHead DetailEnrollTableLeft">
            <asp:Label ID="lblEnrolledUsers" resourcekey="lblEnrolledUsers" runat="server">Enrolled Users</asp:Label>
        </div>
        <div class="SubHead DetailEnrollTableRight">
            <asp:DataGrid ID="grdEnrollment" runat="server" AutoGenerateColumns="False" CssClass="DetailEnrollGrid"
                          DataKeyField="SignupID" GridLines="None" Visible="True" Width="100%">
                <EditItemStyle VerticalAlign="Bottom"></EditItemStyle>
                <AlternatingItemStyle CssClass="DetailEnrollGridAlternate"></AlternatingItemStyle>
                <ItemStyle VerticalAlign="Top"></ItemStyle>
                <HeaderStyle CssClass="DetailEnrollGridHeader"></HeaderStyle>
                <Columns>
                    <asp:BoundColumn DataField="EnrollUserName" HeaderText="EnrollUserName">
                        <ItemStyle CssClass="DetailEnrollUser"></ItemStyle>
                    </asp:BoundColumn>
                    <asp:BoundColumn DataField="EnrollDisplayName" HeaderText="EnrollDisplayName">
                        <ItemStyle CssClass="DetailEnrollDisplay"></ItemStyle>
                    </asp:BoundColumn>
                    <asp:BoundColumn DataField="EnrollEmail" HeaderText="EnrollEmail">
                        <ItemStyle CssClass="DetailEnrollEmail"></ItemStyle>
                    </asp:BoundColumn>
                    <asp:BoundColumn DataField="EnrollPhone" HeaderText="EnrollPhone">
                        <ItemStyle CssClass="DetailEnrollPhone"></ItemStyle>
                    </asp:BoundColumn>
                    <asp:TemplateColumn headerText="EnrollApproved">
                        <ItemStyle CssClass="DetailEnrollApproved"></ItemStyle>
                        <ItemTemplate>
                            <asp:CheckBox ID="chkEnrollApproved" runat="server" Enabled="false" Checked='<%# DataBinder.Eval(Container.DataItem, "EnrollApproved") %>'/>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:BoundColumn DataField="EnrollNo" HeaderText="EnrollNo">
                        <ItemStyle CssClass="DetailEnrollNo"></ItemStyle>
                    </asp:BoundColumn>
                </Columns>
            </asp:DataGrid>
        </div>
        <asp:LinkButton ID="cmdvEventSignups" CssClass="" runat="server" resourcekey="cmdvEventSignups" causesvalidation="False" OnClick="cmdvEventSignups_Click"/>
    </div>
    <div id="divEventDetails4" runat="server" class="DetailClear DetailEventDetails4"/>
    <div class="DetailClear"></div>
    <ul class="DetailCommands dnnActions dnnClear">
        <li>
            <asp:LinkButton OnClick="returnButton_Click" ID="returnButton" CssClass="dnnPrimaryAction" runat="server" resourcekey="returnButton" causesvalidation="False"/>
        </li>
        <li>
            <asp:hyperlink ID="editButton" CssClass="dnnSecondaryAction" runat="server" resourcekey="editButton" causesvalidation="False"/>
        </li>
        <li>
            <asp:hyperlink ID="editSeriesButton" CssClass="dnnSecondaryAction" runat="server" resourcekey="editSeriesButton" causesvalidation="False" Visible="False"/>
        </li>
        <li>
            <asp:LinkButton OnClick="deleteButton_Click" ID="deleteButton" CssClass="dnnSecondaryAction" runat="server" resourcekey="deleteButton" causesvalidation="False"/>
        </li>
        <li>
            <asp:LinkButton OnClick="deleteSeriesButton_Click" ID="deleteSeriesButton" CssClass="dnnSecondaryAction" runat="server" resourcekey="deleteSeriesButton" causesvalidation="False" Visible="False"/>
        </li>
        <li class="cmdRight">
            <asp:HyperLink OnPreRender="cmdPrint_PreRender" ID="cmdPrint" runat="server" CssClass="dnnSecondaryAction" IconKey="Print"> </asp:HyperLink>
        </li>
        <li class="cmdRight">
            <asp:LinkButton OnClick="cmdvEventSeries_Click" ID="cmdvEventSeries" CssClass="dnnSecondaryAction" runat="server" resourcekey="cmdvEventExportSeries" causesvalidation="False"/>
        </li>
        <li class="cmdRight">
            <asp:LinkButton OnClick="cmdvEvent_Click" ID="cmdvEvent" CssClass="dnnSecondaryAction" runat="server" resourcekey="cmdvEventExport" causesvalidation="False"/>
        </li>
    </ul>
</asp:Panel>