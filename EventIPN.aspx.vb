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
Imports System.Net
Imports System.IO
Imports System.Text

Namespace DotNetNuke.Modules.Events
    Partial Class EventIPN
        Inherits Page

#Region "Private Area"
        Private _moduleID As Integer = -1
        Private _settings As EventModuleSettings
        Private _objEventSignups As New EventSignupsInfo
        Private ReadOnly _objCtlEventSignups As New EventSignupsController
        Private _objEventPpErrorLog As New EventPpErrorLogInfo
        Private ReadOnly _objCtlEventPpErrorLog As New EventPpErrorLogController
        Private _objEvent As New EventInfo
        Private ReadOnly _objCtlEventEvent As New EventController
        Private _strToSend, _txnID, _paymentStatus, _receiverEmail, _itemName, _
          _itemNumber, _quantity, _invoice, _custom, _
          _paymentGross, _payerEmail, _pendingReason, _paymentDate, _paymentFee, _
          _txnType, _firstName, _lastName, _addressStreet, _addressCity, _addressState, _
          _addressZip, _addressCountry, _addressStatus, _payerStatus, _paymentType, _
          _notifyVersion, _verifySign, _subscrDate, _period1, _period2, _period3, _
          _amount1, _amount2, _amount3, _recurring, _reattempt, _retryAt, _recurTimes, _
          _username, _password, _subscrID, _currency As String
        Private _localResourceFile As String
#End Region

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

#Region "Event Handlers"
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
            Try
                _localResourceFile = TemplateSourceDirectory & "/" & Localization.LocalResourceDirectory & "/EventIPN.resx"

                Dim sPpnMessages As String = ""             '  Payment message
                ' assign posted variables to local variables
                _receiverEmail = Request.Params("receiver_email")
                _itemName = Request.Params("item_name")
                _itemNumber = Request.Params("item_number")
                _quantity = Request.Params("quantity")
                _invoice = Request.Params("invoice")
                _custom = Request.Params("custom")
                _paymentStatus = Request.Params("payment_status")
                _currency = Request.Params("mc_currency")
                _pendingReason = Request.Params("pending_reason")
                _paymentDate = Request.Params("payment_date")
                _paymentFee = Request.Params("mc_fee")
                _paymentGross = Request.Params("mc_gross")
                _txnID = Request.Params("txn_id")
                _txnType = Request.Params("txn_type")
                _firstName = Request.Params("first_name")
                _lastName = Request.Params("last_name")
                _addressStreet = Request.Params("address_street")
                _addressCity = Request.Params("address_city")
                _addressState = Request.Params("address_state")
                _addressZip = Request.Params("address_zip")
                _addressCountry = Request.Params("address_country")
                _addressStatus = Request.Params("address_status")
                _payerEmail = Request.Params("payer_email")
                _payerStatus = Request.Params("payer_status")
                _paymentType = Request.Params("payment_type")
                _notifyVersion = Request.Params("notify_version")
                _verifySign = Request.Params("verify_sign")
                _subscrDate = Request.Params("subscr_date")   'Start date or cancellation date depending on whether transaction is "subscr_signup" or "subscr_cancel"
                _period1 = Request.Params("period1")           '(optional) Trial subscription interval in days, weeks, months, years (example: a 4 day interval is "period1: 4 d")
                _period2 = Request.Params("period2")           '(optional) Trial subscription interval in days, weeks, months, years
                _period3 = Request.Params("period3")           'Regular subscription interval in days, weeks, months, years
                _amount1 = Request.Params("amount1")           '(optional) Amount of payment for trial period1
                _amount2 = Request.Params("amount2")           '(optional) Amount of payment for trial period2
                _amount3 = Request.Params("amount3")           'Amount of payment for regular period3
                _recurring = Request.Params("recurring")       'Indicates whether regular rate recurs (1 is yes, 0 is no)
                _reattempt = Request.Params("reattempt")       'Indicates whether reattempts should occur upon payment failures (1 is yes, 0 is no)
                _retryAt = Request.Params("retry_at")         'Date we will retry failed subscription payment
                _recurTimes = Request.Params("recur_times")   'How many payment installments will occur at the regular rate
                _username = Request.Params("username")         '(optional) Username generated by PayPal and given to subscriber to access the subscription
                _password = Request.Params("password")         '(optional) Password generated by PayPal and given to subscriber to access the subscription (password will be hashed).
                _subscrID = Request.Params("subscr_id")       '(optional) ID generated by PayPal for the subscriber
                _strToSend = Request.Form.ToString()

                ' Create the string to post back to PayPal system to validate
                _strToSend &= "&cmd=_notify-validate"

                ' Get the Event Signup
                _objEventSignups = _objCtlEventSignups.EventsSignupsGet(CType(_itemNumber, Integer), 0, True)

                ' Get Module Settings
                _moduleID = _objEventSignups.ModuleID
                Dim ems As New EventModuleSettings
                _settings = ems.GetEventModuleSettings(_moduleID, _localResourceFile)

                'Initialize the WebRequest.
                Dim webURL As String
                webURL = _settings.Paypalurl & "/cgi-bin/webscr"

                'Send PayPal Verification Response
                Dim myRequest As HttpWebRequest = CType(HttpWebRequest.Create(webURL), HttpWebRequest)
                myRequest.AllowAutoRedirect = False
                myRequest.Method = "POST"
                myRequest.ContentType = "application/x-www-form-urlencoded"

                'Create post stream
                Dim requestStream As Stream = myRequest.GetRequestStream()
                Dim someBytes() As Byte = Encoding.UTF8.GetBytes(_strToSend)
                requestStream.Write(someBytes, 0, someBytes.Length)
                requestStream.Close()

                'Send request and get response
                Dim myResponse As HttpWebResponse = CType(myRequest.GetResponse(), HttpWebResponse)
                If myResponse.StatusCode = HttpStatusCode.OK Then

                    'Obtain a 'Stream' object associated with the response object.
                    Dim receiveStream As Stream = myResponse.GetResponseStream()
                    Dim encode As Encoding = System.Text.Encoding.GetEncoding("utf-8")

                    'Pipe the stream to a higher level stream reader with the required encoding format. 
                    Dim readStream As StreamReader = New StreamReader(receiveStream, encode)

                    'Read result
                    Dim result As String = readStream.ReadLine()
                    If result = "INVALID" Then
                        MailUsTheOrder("PPIPN: Status came back as INVALID!", False)
                    ElseIf result = "VERIFIED" Then

                        Select Case (_paymentStatus)
                            Case "Completed"        'The payment has been completed and the funds are successfully in your account balance
                                Select Case (_txnType)
                                    Case "web_accept", "cart"
                                        '"web_accept": The payment was sent by your customer via the Web Accept feature.
                                        '"cart": This payment was sent by your customer via the Shopping Cart feature
                                        sPpnMessages = sPpnMessages & "PPIPN: This payment was sent by your customer via the Shopping Cart feature" & Environment.NewLine
                                    Case "send_money"       'This payment was sent by your customer from the PayPal website, imports the "Send Money" tab
                                        sPpnMessages = sPpnMessages & "PPIPN: This payment was sent by your customer from the PayPal website" & Environment.NewLine
                                    Case "subscr_signup"    'This IPN is for a subscription sign-up
                                        sPpnMessages = sPpnMessages & "PPIPN: This IPN is for a subscription sign-up" & Environment.NewLine
                                    Case "subscr_cancel"    'This IPN is for a subscription cancellation
                                        sPpnMessages = sPpnMessages & "PPIPN: Subscription cancellation." & Environment.NewLine
                                    Case "subscr_failed"    'This IPN is for a subscription payment failure
                                        sPpnMessages = sPpnMessages & "PPIPN: Subscription failed." & Environment.NewLine
                                    Case "subscr_payment"   'This IPN is for a subscription payment
                                        sPpnMessages = sPpnMessages & "PPIPN: This IPN is for a subscription payment" & Environment.NewLine
                                    Case "subscr_eot"       'This IPN is for a subscription's end of term
                                        sPpnMessages = sPpnMessages & "PPIPN:  Subscription end of term." & Environment.NewLine
                                End Select
                                Select Case (_addressStatus)
                                    Case "confirmed"    'Customer provided a Confirmed Address
                                    Case "unconfirmed"  'Customer provided an Unconfirmed Address
                                End Select
                                Select Case (_payerStatus)
                                    Case "verified"         'Customer has a Verified U.S. PayPal account
                                    Case "unverified"       'Customer has an Unverified U.S. PayPal account
                                    Case "intl_verified"    'Customer has a Verified International PayPal account
                                    Case "intl_unverified"  'Customer has an Unverified International PayPal account
                                End Select
                                Select Case (_paymentType)
                                    Case "echeck"       'This payment was funded with an eCheck
                                        sPpnMessages = sPpnMessages & "PPIPN: Payment was funded with an eCheck." & Environment.NewLine
                                    Case "instant"      'This payment was funded with PayPal balance, credit card, or Instant Transfer
                                        sPpnMessages = sPpnMessages & "PPIPN: This payment was funded with PayPal balance, credit card, or Instant Transfer" & Environment.NewLine
                                End Select
                                MailUsTheOrder(sPpnMessages, True)
                            Case "Pending"          'The payment is pending - see the "pending reason" variable below for more information. Watch: You will receive another instant payment notification when the payment becomes "completed", "failed", or "denied"
                                Select Case (_pendingReason)
                                    Case "echeck"       'The payment is pending because it was made by an eCheck, which has not yet cleared
                                    Case "intl"         'The payment is pending because you, the merchant, hold an international account and do not have a withdrawal mechanism. You must manually accept or deny this payment from your Account Overview
                                    Case "verify"       'The payment is pending because you, the merchant, are not yet verified. You must verify your account before you can accept this payment
                                    Case "address"      'The payment is pending because your customer did not include a confirmed shipping address and you, the merchant, have your Payment Receiving Preferences set such that you want to manually accept or deny each of these payments. To change your preference, go to the "Preferences" section of your "Profile"
                                    Case "upgrade"      'The payment is pending because it was made via credit card and you, the merchant, must upgrade your account to Business or Premier status in order to receive the funds
                                    Case "unilateral"   'The payment is pending because it was made to an email address that is not yet registered or confirmed
                                    Case "other"        'The payment is pending for an "other" reason. For more information, contact customer service
                                End Select
                                MailUsTheOrder("PPIPN: Order is waiting to be processed.")
                            Case "Failed"          'The payment has failed. This will only happen if the payment was made from your customer's bank account
                                MailUsTheOrder("PPIPN: This only happens if the payment was made from our customer's bank account.")
                            Case "Denied"          'You, the merchant, denied the payment. This will only happen if the payment was previously pending due to one of the "pending reasons"
                                MailUsTheOrder("PPIPN: We denied this payment.")
                        End Select
                    End If
                End If
                'Close the response to free resources.
                myResponse.Close()        'If it is "OK"
            Catch exc As Exception
                LogException(New ModuleLoadException("EventIPN, Paypal Exception: " & exc.Message & " at: " & exc.Source))
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub
#End Region

#Region "Helper Routines"
        Private Sub MailUsTheOrder(ByVal tagMsg As String, Optional ByVal sendToUser As Boolean = True)
            ' ********* RWJS - Seems to add no value, and would have always returned nothing *********
            '			InitializeSettings(Item_number)

            Dim sMessage As String
            Dim sEmail As String
            Dim strNewLine As String = Environment.NewLine
            If _settings.HTMLEmail = "html" Then
                strNewLine = "<br />"
            End If
            sMessage = tagMsg & strNewLine _
                & "Transaction ID:   " & _txnID & strNewLine _
                & "Transaction Type: " & _txnType & strNewLine _
                & "Payment Type:     " & _paymentType & strNewLine _
                & "Payment Status:   " & _paymentStatus & strNewLine _
                & "Pending Reason:   " & _pendingReason & strNewLine _
                & "Payment Date:     " & _paymentDate & strNewLine _
                & "Receiver Email:   " & _receiverEmail & strNewLine _
                & "Invoice:          " & _invoice & strNewLine _
                & "Item Number:      " & _itemNumber & strNewLine _
                & "Item Name:        " & _itemName & strNewLine _
                & "Quantity:         " & _quantity & strNewLine _
                & "Custom:           " & _custom & strNewLine _
                & "Payment Currency: " & _currency & strNewLine _
                & "Payment Gross:    " & _paymentGross & strNewLine _
                & "Payment Fee:      " & _paymentFee & strNewLine _
                & "Payer Email:      " & _payerEmail & strNewLine _
                & "First Name:       " & _firstName & strNewLine _
                & "Last Name:        " & _lastName & strNewLine _
                & "Street Address:   " & _addressStreet & strNewLine _
                & "City:             " & _addressCity & strNewLine _
                & "State:            " & _addressState & strNewLine _
                & "Zip Code:         " & _addressZip & strNewLine _
                & "Country:          " & _addressCountry & strNewLine _
                & "Address Status:   " & _addressStatus & strNewLine _
                & "Payer Status:     " & _payerStatus & strNewLine _
                & "Verify Sign:      " & _verifySign & strNewLine _
                & "Subscriber Date:  " & _subscrDate & strNewLine _
                & "Period 1:         " & _period1 & strNewLine _
                & "Period 2:         " & _period2 & strNewLine _
                & "Period 3:         " & _period3 & strNewLine _
                & "Amount 1:         " & _amount1 & strNewLine _
                & "Amount 2:         " & _amount2 & strNewLine _
                & "Amount 3:         " & _amount3 & strNewLine _
                & "Recurring:        " & _recurring & strNewLine _
                & "Reattempt:        " & _reattempt & strNewLine _
                & "Retry At:         " & _retryAt & strNewLine _
                & "Recur Times:      " & _recurTimes & strNewLine _
                & "UserName:         " & _username & strNewLine _
                & "Password:         " & _password & strNewLine _
                & "Subscriber ID:    " & _subscrID & strNewLine _
                & "Notify Version:   " & _notifyVersion & strNewLine


            Dim portalSettings As Entities.Portals.PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), Entities.Portals.PortalSettings)
            sEmail = _settings.StandardEmail.Trim
            sMessage = sMessage
            Try
                Dim sSystemPrice As Decimal = CType(_paymentGross, Decimal)

                ' Also verify that Gross Payment is what we logged as the Fee ("payment_gross" field )
                _objEventSignups = _objCtlEventSignups.EventsSignupsGet(CType(_itemNumber, Integer), 0, True)
                _objEvent = _objCtlEventEvent.EventsGet(_objEventSignups.EventID, _objEventSignups.ModuleID)
                Dim sPPPrice As Decimal = _objEvent.EnrollFee * _objEventSignups.NoEnrolees


                Dim objEventEmailInfo As New EventEmailInfo
                Dim objEventEmail As New EventEmails(_objEvent.PortalID, _objEventSignups.ModuleID, _localResourceFile)
                objEventEmailInfo.TxtEmailSubject = _settings.Templates.txtEnrollMessageSubject
                objEventEmailInfo.TxtEmailFrom() = sEmail
                If sendToUser Then
                    If _objEventSignups.UserID > -1 Then
                        objEventEmailInfo.UserIDs.Add(_objEventSignups.UserID)
                    Else
                        objEventEmailInfo.UserEmails.Add(_objEventSignups.AnonEmail)
                        objEventEmailInfo.UserLocales.Add(_objEventSignups.AnonCulture)
                        objEventEmailInfo.UserTimeZoneIds.Add(_objEventSignups.AnonTimeZoneId)
                    End If
                End If
                objEventEmailInfo.UserIDs.Add(_objEvent.OwnerID)
                Dim objEventEmailInfo2 As New EventEmailInfo
                objEventEmailInfo2.TxtEmailFrom() = sEmail
                objEventEmailInfo2.UserEmails.Add(_objEvent.PayPalAccount)
                objEventEmailInfo2.UserLocales.Add("")
                objEventEmailInfo2.UserTimeZoneIds.Add("")

                If sPPPrice = sSystemPrice Then
                    'we're ok
                    objEventEmailInfo2.TxtEmailSubject = "Sale of: " & _itemName & " from " & portalSettings.PortalName
                    objEventEmailInfo2.TxtEmailBody = sMessage
                    If _paymentStatus.ToUpper = "COMPLETED" Then
                        SavePayPalInfo(True, _objEvent)

                        ' Mail users
                        If _settings.SendEnrollMessagePaid Then
                            objEventEmailInfo.TxtEmailBody = _settings.Templates.txtEnrollMessagePaid
                            objEventEmail.SendEmails(objEventEmailInfo, _objEvent, _objEventSignups, True)
                        End If
                    Else
                        SavePayPalInfo(False, _objEvent)

                        ' Mail users
                        If _settings.SendEnrollMessagePending Then
                            objEventEmailInfo.TxtEmailBody = _settings.Templates.txtEnrollMessagePending
                            objEventEmail.SendEmails(objEventEmailInfo, _objEvent, _objEventSignups, True)
                        End If
                    End If
                Else
                    'someone is trying to rip us off.
                    objEventEmailInfo2.TxtEmailSubject = "Failed Price Matchup Check: " & _itemName & " from " & portalSettings.PortalName
                    objEventEmailInfo2.TxtEmailBody = "There was an incorrect match between actual price and price paid. The following transaction information is provided below:" & strNewLine & _
                        strNewLine & "The purchasing email is: " & _payerEmail & strNewLine & _
                        "User ID: " & _custom & strNewLine & _
                        "Transaction Type: " & _txnType & strNewLine & _
                        "Transaction ID: " & _txnID & strNewLine & _
                        "Item Number: " & _itemNumber & strNewLine & _
                        "PayPal Paid: " & sSystemPrice.ToString & strNewLine & _
                        "Actual Price: " & sPPPrice.ToString & strNewLine & _
                        strNewLine & "TRANSACTION DETAILS: " & strNewLine & strNewLine & sMessage

                    SavePayPalErrorLogInfo()

                    ' Mail users
                    If _settings.SendEnrollMessageIncorrect Then
                        objEventEmailInfo.TxtEmailBody = _settings.Templates.txtEnrollMessageIncorrect
                        objEventEmail.SendEmails(objEventEmailInfo, _objEvent, _objEventSignups, True)
                    End If
                End If
                objEventEmail.SendEmails(objEventEmailInfo2, _objEvent, _objEventSignups, True)
            Catch exc As Exception
                LogException(New ModuleLoadException("EventIPN, Paypal Exception: " & exc.Message & " at: " & exc.Source))
                ProcessModuleLoadException(Me, exc)
                Dim localResourceFile As String = TemplateSourceDirectory & "/" & Localization.LocalResourceDirectory & "/EventIPN.resx"
                Dim objEventEmailInfo As New EventEmailInfo
                Dim objEventEmail As New EventEmails(_objEvent.PortalID, _objEventSignups.ModuleID, localResourceFile)
                objEventEmailInfo.TxtEmailSubject = "Sale of: " & _itemName & " from " & portalSettings.PortalName
                objEventEmailInfo.TxtEmailFrom() = _settings.StandardEmail.Trim
                objEventEmailInfo.UserEmails.Add(_objEvent.PayPalAccount)
                objEventEmailInfo.UserLocales.Add("")
                objEventEmailInfo.UserTimeZoneIds.Add("")

                objEventEmailInfo.TxtEmailBody = "There was a failure of the item purchase module. The following transaction information is provided below:" & strNewLine & _
                     strNewLine & "The purchasing email is: " & _payerEmail & strNewLine & _
                     strNewLine & "User ID: " & _custom & strNewLine & _
                     strNewLine & "Transaction Type: " & _txnType & strNewLine & _
                     strNewLine & "Transaction ID: " & _txnID & strNewLine & _
                     strNewLine & "Error Code: " & exc.Message & strNewLine & exc.Source

                objEventEmail.SendEmails(objEventEmailInfo, _objEvent, _objEventSignups, True)
            End Try
        End Sub

        Private Sub SavePayPalInfo(ByVal approved As Boolean, ByVal objEvent As EventInfo)
            _objEventSignups = _objCtlEventSignups.EventsSignupsGet(CInt(_itemNumber), 0, True)
            _objEventSignups.Approved = approved
            _objEventSignups.PayPalAddress = _addressStreet
            _objEventSignups.PayPalCity = _addressCity
            _objEventSignups.PayPalCountry = _addressCountry
            _objEventSignups.PayPalAmount = CType(_paymentGross, Decimal)
            _objEventSignups.PayPalCurrency = _currency
            _objEventSignups.PayPalFee = CType(_paymentFee, Decimal)
            _objEventSignups.PayPalFirstName = _firstName
            _objEventSignups.PayPalLastName = _lastName
            _objEventSignups.PayPalPayerEmail = _payerEmail
            _objEventSignups.PayPalPayerID = _subscrID
            _objEventSignups.PayPalPayerStatus = _payerStatus
            _objEventSignups.PayPalPaymentDate = DateTime.UtcNow
            _objEventSignups.PayPalReason = _pendingReason
            _objEventSignups.PayPalRecieverEmail = _receiverEmail
            _objEventSignups.PayPalState = _addressState
            _objEventSignups.PayPalStatus = _paymentStatus.ToLower()
            _objEventSignups.PayPalTransID = _txnID
            'objEventSignups.PayPalUserEmail = pay.IPNUserEmail
            _objEventSignups.PayPalZip = _addressZip
            Dim eventsBase As New EventBase
            eventsBase.CreateEnrollment(_objEventSignups, objEvent, _settings)
        End Sub

        Private Sub SavePayPalErrorLogInfo()
            _objEventPpErrorLog = New EventPpErrorLogInfo
            _objEventPpErrorLog.SignupID = CType(_itemNumber, Integer)
            'objEventPPErrorLog.Approved = False
            _objEventPpErrorLog.PayPalAddress = _addressStreet
            _objEventPpErrorLog.PayPalCity = _addressCity
            _objEventPpErrorLog.PayPalCountry = _addressCountry
            _objEventPpErrorLog.PayPalAmount = CType(_paymentGross, Double)
            _objEventPpErrorLog.PayPalCurrency = _currency
            _objEventPpErrorLog.PayPalFee = CType(_paymentFee, Double)
            _objEventPpErrorLog.PayPalFirstName = _firstName
            _objEventPpErrorLog.PayPalLastName = _lastName
            _objEventPpErrorLog.PayPalPayerEmail = _payerEmail
            _objEventPpErrorLog.PayPalPayerID = _subscrID
            _objEventPpErrorLog.PayPalPayerStatus = _payerStatus
            'objEventPPErrorLog.PayPalPaymentDate = CType(Payment_date, Date)
            _objEventPpErrorLog.PayPalPaymentDate = DateTime.Now
            _objEventPpErrorLog.PayPalReason = _pendingReason
            _objEventPpErrorLog.PayPalRecieverEmail = _receiverEmail
            _objEventPpErrorLog.PayPalState = _addressState
            _objEventPpErrorLog.PayPalStatus = _paymentStatus.ToLower()
            _objEventPpErrorLog.PayPalTransID = _txnID
            'objEventPPErrorLog.PayPalUserEmail = pay.IPNUserEmail
            _objEventPpErrorLog.PayPalZip = _addressZip
            _objEventPpErrorLog = _objCtlEventPpErrorLog.EventsPpErrorLogAdd(_objEventPpErrorLog)
        End Sub
#End Region

    End Class
End Namespace