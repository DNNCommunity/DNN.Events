'
' DotNetNuke® - http://www.dnnsoftware.com
' Copyright (c) 2002-2013
' by DNNCorp
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'

Imports System.Globalization

Namespace DotNetNuke.Modules.Events

    <DNNtc.ModuleControlProperties("PPEnroll", "Event PayPal Enrollment", DNNtc.ControlType.View, "https://dnnevents.codeplex.com/documentation", False, True)> _
    Partial Class EventPPEnroll
        Inherits EventBase

#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()
        End Sub

#End Region

#Region "Private Area"
        Private _itemID As Integer = -1
        Private _noEnrol As Integer = 1
        Private ReadOnly _objCtlEvent As New EventController
        Private _objEventSignups As New EventSignupsInfo
        Private ReadOnly _objCtlEventSignups As New EventSignupsController
        Private _anonEmail As String = Nothing
        Private _anonName As String = Nothing
        Private _anonTelephone As String = Nothing

        Private Enum MessageLevel As Integer
            DNNSuccess = 1
            DNNInformation
            DNNWarning
            DNNError
        End Enum

#End Region

#Region "Event Handlers"
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
            Try
                Dim dblTotal As Double
                Dim strCurrency As String

                If Not (Request.Params("ItemID") Is Nothing) Then
                    _itemID = Int32.Parse(Request.Params("ItemID"))
                End If
                If Not (Request.Params("AnonEmail") Is Nothing) Then
                    _anonEmail = HttpUtility.UrlDecode(Request.Params("AnonEmail"))
                End If
                If Not (Request.Params("AnonName") Is Nothing) Then
                    _anonName = HttpUtility.UrlDecode(Request.Params("AnonName"))
                End If
                If Not (Request.Params("AnonPhone") Is Nothing) Then
                    _anonTelephone = HttpUtility.UrlDecode(Request.Params("AnonPhone"))
                    If _anonTelephone = "0" Then
                        _anonTelephone = ""
                    End If
                End If

                ' Set the selected theme 
                SetTheme(pnlEventsModulePayPal)

                divMessage.Attributes.Add("style", "display:none;")

                If Not (Page.IsPostBack) Then
                    If _itemID <> -1 Then
                        If Not (Request.Params("NoEnrol") Is Nothing) Then
                            _noEnrol = Int32.Parse(Request.Params("NoEnrol"))
                        End If

                        Dim objEvent As EventInfo
                        objEvent = _objCtlEvent.EventsGet(_itemID, ModuleId)

                        '  Compute Dates/Times (for recurring)
                        Dim startdate As DateTime = objEvent.EventTimeBegin
                        SelectedDate = startdate.Date
                        Dim d As DateTime
                        d = startdate.Date.AddMinutes(objEvent.EventTimeBegin.TimeOfDay.TotalMinutes)
                        lblStartDate.Text = d.ToLongDateString + " " + d.ToShortTimeString
                        lblEventName.Text = objEvent.EventName
                        lblDescription.Text = Server.HtmlDecode(objEvent.EventDesc)
                        lblFee.Text = String.Format("{0:#0.00}", objEvent.EnrollFee)
                        lblNoEnrolees.Text = CStr(_noEnrol)
                        lblPurchase.Text = Localization.GetString("lblPurchase", LocalResourceFile)
                        lblPurchase.Visible = True
                    ElseIf Not (Request.Params("signupid") Is Nothing) Then
                        ' Get EventSignup
                        _objEventSignups = New EventSignupsInfo
                        Dim signupID As Integer = CType(Request.Params("signupid"), Integer)
                        _objEventSignups = _objCtlEventSignups.EventsSignupsGet(signupID, ModuleId, False)
                        lblStartDate.Text = _objEventSignups.EventTimeBegin.ToLongDateString + " " + _objEventSignups.EventTimeBegin.ToShortTimeString
                        lblEventName.Text = _objEventSignups.EventName
                        ' Get Related Event
                        Dim objEvent As EventInfo
                        objEvent = _objCtlEvent.EventsGet(_objEventSignups.EventID, _objEventSignups.ModuleID)
                        lblDescription.Text = Server.HtmlDecode(objEvent.EventDesc)
                        lblFee.Text = String.Format("{0:#0.00}", objEvent.EnrollFee)
                        lblNoEnrolees.Text = CStr(_objEventSignups.NoEnrolees)
                        If Request.Params("status").ToLower = "enrolled" Then
                            ' User has been successfully enrolled for this event (paid enrollment)
                            ShowMessage(Localization.GetString("lblComplete", LocalResourceFile), MessageLevel.DNNSuccess)
                            lblPurchase.Visible = False
                            cmdPurchase.Visible = False
                            cancelButton.Visible = False
                            cmdReturn.Visible = True
                        ElseIf Request.Params("status").ToLower = "cancelled" Then
                            ' User has been cancelled paid enrollment
                            ShowMessage(Localization.GetString("lblCancel", LocalResourceFile), MessageLevel.DNNWarning)
                            lblPurchase.Visible = False
                            cmdPurchase.Visible = False
                            cancelButton.Visible = False
                            cmdReturn.Visible = True
                            ' Make sure we delete the signup
                            DeleteEnrollment(signupID, objEvent.ModuleID, objEvent.EventID)

                            ' Mail users
                            If Settings.SendEnrollMessageCancelled Then
                                Dim objEventEmailInfo As New EventEmailInfo
                                Dim objEventEmail As New EventEmails(PortalId, ModuleId, LocalResourceFile, CType(Page, PageBase).PageCulture.Name)
                                objEventEmailInfo.TxtEmailSubject = Settings.Templates.txtEnrollMessageSubject
                                objEventEmailInfo.TxtEmailBody = Settings.Templates.txtEnrollMessageCancelled
                                objEventEmailInfo.TxtEmailFrom() = Settings.StandardEmail
                                If _objEventSignups.UserID > -1 Then
                                    objEventEmailInfo.UserIDs.Add(_objEventSignups.UserID)
                                Else
                                    objEventEmailInfo.UserEmails.Add(_objEventSignups.AnonEmail)
                                    objEventEmailInfo.UserLocales.Add(_objEventSignups.AnonCulture)
                                    objEventEmailInfo.UserTimeZoneIds.Add(_objEventSignups.AnonTimeZoneId)
                                End If
                                objEventEmailInfo.UserIDs.Add(objEvent.OwnerID)
                                objEventEmail.SendEmails(objEventEmailInfo, objEvent, _objEventSignups)
                            End If
                        End If
                    Else ' security violation attempt to access item not related to this Module
                        Response.Redirect(GetSocialNavigateUrl(), True)
                    End If
                End If

                dblTotal = Val(lblFee.Text) * Val(lblNoEnrolees.Text)
                lblTotal.Text = Format(dblTotal, "#,##0.00")
                strCurrency = PortalSettings.Currency
                lblFeeCurrency.Text = strCurrency
                lblTotalCurrency.Text = strCurrency
            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

#Region "Control Events"
        Private Sub cmdPurchase_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles cmdPurchase.Click
            Dim objEvent As EventInfo

            Try
                If Page.IsValid Then
                    objEvent = _objCtlEvent.EventsGet(_itemID, ModuleId)
                    ' User wants to purchase event, create Event Signup Record
                    _objEventSignups = New EventSignupsInfo

                    'Just in case the user has clicked back and has now clicked Purchase again!!
                    Dim objEventSignupsChk As EventSignupsInfo
                    If _anonEmail Is Nothing Then
                        objEventSignupsChk = _objCtlEventSignups.EventsSignupsGetUser(objEvent.EventID, UserId, objEvent.ModuleID)
                    Else
                        objEventSignupsChk = _objCtlEventSignups.EventsSignupsGetAnonUser(objEvent.EventID, _anonEmail, objEvent.ModuleID)
                    End If
                    If Not objEventSignupsChk Is Nothing Then
                        _objEventSignups.SignupID = objEventSignupsChk.SignupID
                    End If
                    _objEventSignups.EventID = objEvent.EventID
                    _objEventSignups.ModuleID = objEvent.ModuleID
                    If _anonEmail Is Nothing Then
                        _objEventSignups.UserID = UserId
                        _objEventSignups.AnonEmail = Nothing
                        _objEventSignups.AnonName = Nothing
                        _objEventSignups.AnonTelephone = Nothing
                        _objEventSignups.AnonCulture = Nothing
                        _objEventSignups.AnonTimeZoneId = Nothing
                    Else
                        Dim objSecurity As New Security.PortalSecurity
                        _objEventSignups.UserID = -1
                        _objEventSignups.AnonEmail = objSecurity.InputFilter(_anonEmail, Security.PortalSecurity.FilterFlag.NoScripting)
                        _objEventSignups.AnonName = objSecurity.InputFilter(_anonName, Security.PortalSecurity.FilterFlag.NoScripting)
                        _objEventSignups.AnonTelephone = objSecurity.InputFilter(_anonTelephone, Security.PortalSecurity.FilterFlag.NoScripting)
                        _objEventSignups.AnonCulture = Threading.Thread.CurrentThread.CurrentCulture.Name
                        _objEventSignups.AnonTimeZoneId = GetDisplayTimeZoneId()
                    End If
                    _objEventSignups.PayPalStatus = "none"
                    _objEventSignups.PayPalReason = "PayPal call initiated..."
                    _objEventSignups.PayPalPaymentDate = DateTime.UtcNow
                    _objEventSignups.Approved = False
                    _objEventSignups.NoEnrolees = CInt(lblNoEnrolees.Text)

                    _objEventSignups = CreateEnrollment(_objEventSignups, objEvent)

                    If Not objEventSignupsChk Is Nothing Then
                        _objEventSignups = _objCtlEventSignups.EventsSignupsGet(objEventSignupsChk.SignupID, objEventSignupsChk.ModuleID, False)
                    End If

                    ' Mail users
                    If Settings.SendEnrollMessagePaying Then
                        Dim objEventEmailInfo As New EventEmailInfo
                        Dim objEventEmail As New EventEmails(PortalId, ModuleId, LocalResourceFile, CType(Page, PageBase).PageCulture.Name)
                        objEventEmailInfo.TxtEmailSubject = Settings.Templates.txtEnrollMessageSubject
                        objEventEmailInfo.TxtEmailBody = Settings.Templates.txtEnrollMessagePaying
                        objEventEmailInfo.TxtEmailFrom() = Settings.StandardEmail
                        If _anonEmail Is Nothing Then
                            objEventEmailInfo.UserEmails.Add(PortalSettings.UserInfo.Email)
                            objEventEmailInfo.UserLocales.Add(PortalSettings.UserInfo.Profile.PreferredLocale)
                            objEventEmailInfo.UserTimeZoneIds.Add(PortalSettings.UserInfo.Profile.PreferredTimeZone.Id)
                        Else
                            objEventEmailInfo.UserEmails.Add(_objEventSignups.AnonEmail)
                            objEventEmailInfo.UserLocales.Add(_objEventSignups.AnonCulture)
                            objEventEmailInfo.UserTimeZoneIds.Add(_objEventSignups.AnonTimeZoneId)
                        End If
                        objEventEmailInfo.UserIDs.Add(objEvent.OwnerID)
                        objEventEmail.SendEmails(objEventEmailInfo, objEvent, _objEventSignups)
                    End If

                    ' build PayPal URL
                    Dim ppurl As String = Settings.Paypalurl & "/cgi-bin/webscr?cmd=_xclick&business="

                    Dim socialGroupId As Integer = GetUrlGroupId()

                    Dim objEventInfoHelper As New EventInfoHelper(ModuleId, TabId, PortalId, Settings)
                    Dim returnURL As String
                    If socialGroupId > 0 Then
                        returnURL = objEventInfoHelper.AddSkinContainerControls(NavigateURL(TabId, "PPEnroll", "Mid=" & ModuleId, "signupid=" & CType(_objEventSignups.SignupID, String), "status=enrolled", "groupid=" & socialGroupId.ToString), "?")
                    Else
                        returnURL = objEventInfoHelper.AddSkinContainerControls(NavigateURL(TabId, "PPEnroll", "Mid=" & ModuleId, "signupid=" & CType(_objEventSignups.SignupID, String), "status=enrolled"), "?")
                    End If
                    If InStr(1, returnURL, "://") = 0 Then
                        returnURL = AddHTTP(GetDomainName(Request)) & returnURL
                    End If
                    Dim cancelURL As String
                    If socialGroupId > 0 Then
                        cancelURL = objEventInfoHelper.AddSkinContainerControls(NavigateURL(TabId, "PPEnroll", "Mid=" & ModuleId, "signupid=" & CType(_objEventSignups.SignupID, String), "status=cancelled", "groupid=" & socialGroupId.ToString), "?")
                    Else
                        cancelURL = objEventInfoHelper.AddSkinContainerControls(NavigateURL(TabId, "PPEnroll", "Mid=" & ModuleId, "signupid=" & CType(_objEventSignups.SignupID, String), "status=cancelled"), "?")
                    End If
                    If InStr(1, cancelURL, "://") = 0 Then
                        cancelURL = AddHTTP(GetDomainName(Request)) & cancelURL
                    End If
                    Dim strPayPalURL As String
                    strPayPalURL = ppurl & HTTPPOSTEncode(objEvent.PayPalAccount)
                    strPayPalURL = strPayPalURL & "&item_name=" & HTTPPOSTEncode(objEvent.ModuleTitle & " - " & lblEventName.Text & " ( " & lblFee.Text & " " & lblFeeCurrency.Text & " )")
                    strPayPalURL = strPayPalURL & "&item_number=" & HTTPPOSTEncode(CType(_objEventSignups.SignupID, String))
                    strPayPalURL = strPayPalURL & "&quantity=" & HTTPPOSTEncode(CStr(_objEventSignups.NoEnrolees))
                    strPayPalURL = strPayPalURL & "&custom=" & HTTPPOSTEncode(CType(lblStartDate.Text, DateTime).ToShortDateString)

                    ' Make sure currency is in correct format
                    Dim dblFee As Double = CDbl(lblFee.Text)
                    Dim uiculture As CultureInfo = Threading.Thread.CurrentThread.CurrentCulture
                    Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture
                    strPayPalURL = strPayPalURL & "&amount=" & HTTPPOSTEncode(Format(dblFee, "#,##0.00"))
                    Threading.Thread.CurrentThread.CurrentCulture = uiculture

                    strPayPalURL = strPayPalURL & "&currency_code=" & HTTPPOSTEncode(lblTotalCurrency.Text)
                    strPayPalURL = strPayPalURL & "&return=" & returnURL
                    strPayPalURL = strPayPalURL & "&cancel_return=" & cancelURL
                    strPayPalURL = strPayPalURL & "&notify_url=" & HTTPPOSTEncode(AddHTTP(GetDomainName(Request)) & "/DesktopModules/Events/EventIPN.aspx")
                    strPayPalURL = strPayPalURL & "&undefined_quantity=&no_note=1&no_shipping=1"
                    'strPayPalURL = strPayPalURL & "&undefined_quantity=&no_note=1&no_shipping=1&rm=2"

                    ' redirect to PayPal
                    Response.Redirect(strPayPalURL, True)
                End If
            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub cancelButton_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles cancelButton.Click
            Try
                Response.Redirect(GetSocialNavigateUrl(), True)
            Catch exc As Exception 'Module failed to load
                'ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub cmdReturn_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles cmdReturn.Click
            Try
                Response.Redirect(GetSocialNavigateUrl(), True)
            Catch exc As Exception 'Module failed to load
                'ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region
#Region "Support Functions"
        Private Sub ShowMessage(ByVal msg As String, ByVal messageLevel As MessageLevel)
            lblMessage.Text = msg

            'Hide the rest of the form fields.
            divMessage.Attributes.Add("style", "display:block;")

            Select Case MessageLevel
                Case MessageLevel.DNNSuccess
                    divMessage.Attributes.Add("class", "dnnFormMessage dnnFormSuccess")
                Case MessageLevel.DNNInformation
                    divMessage.Attributes.Add("class", "dnnFormMessage dnnFormInfo")
                Case MessageLevel.DNNWarning
                    divMessage.Attributes.Add("class", "dnnFormMessage dnnFormWarning")
                Case MessageLevel.DNNError
                    divMessage.Attributes.Add("class", "dnnFormMessage dnnFormValidationSummary")
            End Select
        End Sub

#End Region


    End Class

End Namespace
