#region Copyright

// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2018
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
//

#endregion


namespace DotNetNuke.Modules.Events
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.UI;
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Localization;
    using global::Components;

    public partial class EventIPN : Page
    {
        #region Event Handlers

        private void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this._localResourceFile = this.TemplateSourceDirectory + "/" + Localization.LocalResourceDirectory +
                                          "/EventIPN.resx";

                var sPpnMessages = ""; //  Payment message
                // assign posted variables to local variables
                this._receiverEmail = this.Request.Params["receiver_email"];
                this._itemName = this.Request.Params["item_name"];
                this._itemNumber = this.Request.Params["item_number"];
                this._quantity = this.Request.Params["quantity"];
                this._invoice = this.Request.Params["invoice"];
                this._custom = this.Request.Params["custom"];
                this._paymentStatus = this.Request.Params["payment_status"];
                this._currency = this.Request.Params["mc_currency"];
                this._pendingReason = this.Request.Params["pending_reason"];
                this._paymentDate = this.Request.Params["payment_date"];
                this._paymentFee = this.Request.Params["mc_fee"];
                this._paymentGross = this.Request.Params["mc_gross"];
                this._txnID = this.Request.Params["txn_id"];
                this._txnType = this.Request.Params["txn_type"];
                this._firstName = this.Request.Params["first_name"];
                this._lastName = this.Request.Params["last_name"];
                this._addressStreet = this.Request.Params["address_street"];
                this._addressCity = this.Request.Params["address_city"];
                this._addressState = this.Request.Params["address_state"];
                this._addressZip = this.Request.Params["address_zip"];
                this._addressCountry = this.Request.Params["address_country"];
                this._addressStatus = this.Request.Params["address_status"];
                this._payerEmail = this.Request.Params["payer_email"];
                this._payerStatus = this.Request.Params["payer_status"];
                this._paymentType = this.Request.Params["payment_type"];
                this._notifyVersion = this.Request.Params["notify_version"];
                this._verifySign = this.Request.Params["verify_sign"];
                this._subscrDate =
                    this.Request.Params
                        ["subscr_date"]; //Start date or cancellation date depending on whether transaction is "subscr_signup" or "subscr_cancel"
                this._period1 =
                    this.Request.Params
                        ["period1"]; //(optional) Trial subscription interval in days, weeks, months, years (example: a 4 day interval is "period1: 4 d")
                this._period2 =
                    this.Request.Params
                        ["period2"]; //(optional) Trial subscription interval in days, weeks, months, years
                this._period3 =
                    this.Request.Params["period3"]; //Regular subscription interval in days, weeks, months, years
                this._amount1 = this.Request.Params["amount1"]; //(optional) Amount of payment for trial period1
                this._amount2 = this.Request.Params["amount2"]; //(optional) Amount of payment for trial period2
                this._amount3 = this.Request.Params["amount3"]; //Amount of payment for regular period3
                this._recurring =
                    this.Request.Params["recurring"]; //Indicates whether regular rate recurs (1 is yes, 0 is no)
                this._reattempt =
                    this.Request.Params
                        ["reattempt"]; //Indicates whether reattempts should occur upon payment failures (1 is yes, 0 is no)
                this._retryAt = this.Request.Params["retry_at"]; //Date we will retry failed subscription payment
                this._recurTimes =
                    this.Request.Params["recur_times"]; //How many payment installments will occur at the regular rate
                this._username =
                    this.Request.Params
                        ["username"]; //(optional) Username generated by PayPal and given to subscriber to access the subscription
                this._password =
                    this.Request.Params
                        ["password"]; //(optional) Password generated by PayPal and given to subscriber to access the subscription (password will be hashed).
                this._subscrID =
                    this.Request.Params["subscr_id"]; //(optional) ID generated by PayPal for the subscriber
                this._strToSend = this.Request.Form.ToString();

                // Create the string to post back to PayPal system to validate
                this._strToSend += "&cmd=_notify-validate";

                // Get the Event Signup
                this._objEventSignups =
                    this._objCtlEventSignups.EventsSignupsGet(Convert.ToInt32(this._itemNumber), 0, true);

                // Get Module Settings
                this._moduleID = this._objEventSignups.ModuleID;
                this._settings = EventModuleSettings.GetEventModuleSettings(this._moduleID, this._localResourceFile);

                //Initialize the WebRequest.
                var webURL = "";
                webURL = this._settings.Paypalurl + "/cgi-bin/webscr";

                //Send PayPal Verification Response
                var myRequest = (HttpWebRequest) WebRequest.Create(webURL);
                myRequest.AllowAutoRedirect = false;
                myRequest.Method = "POST";
                myRequest.ContentType = "application/x-www-form-urlencoded";

                //Create post stream
                var requestStream = myRequest.GetRequestStream();
                var someBytes = Encoding.UTF8.GetBytes(this._strToSend);
                requestStream.Write(someBytes, 0, someBytes.Length);
                requestStream.Close();

                //Send request and get response
                var myResponse = (HttpWebResponse) myRequest.GetResponse();
                if (myResponse.StatusCode == HttpStatusCode.OK)
                {
                    //Obtain a 'Stream' object associated with the response object.
                    var receiveStream = myResponse.GetResponseStream();
                    var encode = Encoding.GetEncoding("utf-8");

                    //Pipe the stream to a higher level stream reader with the required encoding format.
                    var readStream = new StreamReader(receiveStream, encode);

                    //Read result
                    var result = readStream.ReadLine();
                    if (result == "INVALID")
                    {
                        this.MailUsTheOrder("PPIPN: Status came back as INVALID!", false);
                    }
                    else if (result == "VERIFIED")
                    {
                        switch (this._paymentStatus)
                        {
                            case "Completed"
                            : //The payment has been completed and the funds are successfully in your account balance
                                switch (this._txnType)
                                {
                                    case "web_accept":
                                    case "cart":
                                        //"web_accept": The payment was sent by your customer via the Web Accept feature.
                                        //"cart": This payment was sent by your customer via the Shopping Cart feature
                                        sPpnMessages =
                                            sPpnMessages +
                                            "PPIPN: This payment was sent by your customer via the Shopping Cart feature" +
                                            Environment.NewLine;
                                        break;
                                    case "send_money"
                                    : //This payment was sent by your customer from the PayPal website, imports the "Send Money" tab
                                        sPpnMessages =
                                            sPpnMessages +
                                            "PPIPN: This payment was sent by your customer from the PayPal website" +
                                            Environment.NewLine;
                                        break;
                                    case "subscr_signup": //This IPN is for a subscription sign-up
                                        sPpnMessages =
                                            sPpnMessages + "PPIPN: This IPN is for a subscription sign-up" +
                                            Environment.NewLine;
                                        break;
                                    case "subscr_cancel": //This IPN is for a subscription cancellation
                                        sPpnMessages =
                                            sPpnMessages + "PPIPN: Subscription cancellation." + Environment.NewLine;
                                        break;
                                    case "subscr_failed": //This IPN is for a subscription payment failure
                                        sPpnMessages =
                                            sPpnMessages + "PPIPN: Subscription failed." + Environment.NewLine;
                                        break;
                                    case "subscr_payment": //This IPN is for a subscription payment
                                        sPpnMessages =
                                            sPpnMessages + "PPIPN: This IPN is for a subscription payment" +
                                            Environment.NewLine;
                                        break;
                                    case "subscr_eot": //This IPN is for a subscription's end of term
                                        sPpnMessages =
                                            sPpnMessages + "PPIPN:  Subscription end of term." + Environment.NewLine;
                                        break;
                                }
                                switch (this._addressStatus)
                                {
                                    case "confirmed": //Customer provided a Confirmed Address
                                        break;
                                    case "unconfirmed": //Customer provided an Unconfirmed Address
                                        break;
                                }
                                switch (this._payerStatus)
                                {
                                    case "verified": //Customer has a Verified U.S. PayPal account
                                        break;
                                    case "unverified": //Customer has an Unverified U.S. PayPal account
                                        break;
                                    case "intl_verified": //Customer has a Verified International PayPal account
                                        break;
                                    case "intl_unverified": //Customer has an Unverified International PayPal account
                                        break;
                                }
                                switch (this._paymentType)
                                {
                                    case "echeck": //This payment was funded with an eCheck
                                        sPpnMessages =
                                            sPpnMessages + "PPIPN: Payment was funded with an eCheck." +
                                            Environment.NewLine;
                                        break;
                                    case "instant"
                                    : //This payment was funded with PayPal balance, credit card, or Instant Transfer
                                        sPpnMessages =
                                            sPpnMessages +
                                            "PPIPN: This payment was funded with PayPal balance, credit card, or Instant Transfer" +
                                            Environment.NewLine;
                                        break;
                                }
                                this.MailUsTheOrder(sPpnMessages, true);
                                break;
                            case "Pending"
                            : //The payment is pending - see the "pending reason" variable below for more information. Watch: You will receive another instant payment notification when the payment becomes "completed", "failed", or "denied"
                                switch (this._pendingReason)
                                {
                                    case "echeck"
                                    : //The payment is pending because it was made by an eCheck, which has not yet cleared
                                        break;
                                    case "intl"
                                    : //The payment is pending because you, the merchant, hold an international account and do not have a withdrawal mechanism. You must manually accept or deny this payment from your Account Overview
                                        break;
                                    case "verify"
                                    : //The payment is pending because you, the merchant, are not yet verified. You must verify your account before you can accept this payment
                                        break;
                                    case "address"
                                    : //The payment is pending because your customer did not include a confirmed shipping address and you, the merchant, have your Payment Receiving Preferences set such that you want to manually accept or deny each of these payments. To change your preference, go to the "Preferences" section of your "Profile"
                                        break;
                                    case "upgrade"
                                    : //The payment is pending because it was made via credit card and you, the merchant, must upgrade your account to Business or Premier status in order to receive the funds
                                        break;
                                    case "unilateral"
                                    : //The payment is pending because it was made to an email address that is not yet registered or confirmed
                                        break;
                                    case "other"
                                    : //The payment is pending for an "other" reason. For more information, contact customer service
                                        break;
                                }
                                this.MailUsTheOrder("PPIPN: Order is waiting to be processed.");
                                break;
                            case "Failed"
                            : //The payment has failed. This will only happen if the payment was made from your customer's bank account
                                this.MailUsTheOrder(
                                    "PPIPN: This only happens if the payment was made from our customer's bank account.");
                                break;
                            case "Denied"
                            : //You, the merchant, denied the payment. This will only happen if the payment was previously pending due to one of the "pending reasons"
                                this.MailUsTheOrder("PPIPN: We denied this payment.");
                                break;
                        }
                    }
                }
                //Close the response to free resources.
                myResponse.Close(); //If it is "OK"
            }
            catch (Exception exc)
            {
                Exceptions.LogException(
                    new ModuleLoadException("EventIPN, Paypal Exception: " + exc.Message + " at: " + exc.Source));
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion

        #region Private Area

        private int _moduleID = -1;
        private EventModuleSettings _settings;
        private EventSignupsInfo _objEventSignups = new EventSignupsInfo();
        private readonly EventSignupsController _objCtlEventSignups = new EventSignupsController();
        private EventPpErrorLogInfo _objEventPpErrorLog = new EventPpErrorLogInfo();
        private readonly EventPpErrorLogController _objCtlEventPpErrorLog = new EventPpErrorLogController();
        private EventInfo _objEvent = new EventInfo();
        private readonly EventController _objCtlEventEvent = new EventController();
        private string _strToSend;
        private string _txnID;
        private string _paymentStatus;
        private string _receiverEmail;
        private string _itemName;
        private string _itemNumber;
        private string _quantity;
        private string _invoice;
        private string _custom;
        private string _paymentGross;
        private string _payerEmail;
        private string _pendingReason;
        private string _paymentDate;
        private string _paymentFee;
        private string _txnType;
        private string _firstName;
        private string _lastName;
        private string _addressStreet;
        private string _addressCity;
        private string _addressState;
        private string _addressZip;
        private string _addressCountry;
        private string _addressStatus;
        private string _payerStatus;
        private string _paymentType;
        private string _notifyVersion;
        private string _verifySign;
        private string _subscrDate;
        private string _period1;
        private string _period2;
        private string _period3;
        private string _amount1;
        private string _amount2;
        private string _amount3;
        private string _recurring;
        private string _reattempt;
        private string _retryAt;
        private string _recurTimes;
        private string _username;
        private string _password;
        private string _subscrID;
        private string _currency;
        private string _localResourceFile;

        #endregion

        #region  Web Form Designer Generated Code

        //This call is required by the Web Form Designer.
        [DebuggerStepThrough]
        private void InitializeComponent()
        { }

        private void Page_Init(object sender, EventArgs e)
        {
            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            this.InitializeComponent();
        }

        #endregion

        #region Helper Routines

        private void MailUsTheOrder(string tagMsg, bool sendToUser = true)
        {
            // ********* RWJS - Seems to add no value, and would have always returned nothing *********
            //			InitializeSettings(Item_number)

            var sMessage = "";
            var sEmail = "";
            var strNewLine = Environment.NewLine;
            if (this._settings.HTMLEmail == "html")
            {
                strNewLine = "<br />";
            }
            sMessage = tagMsg + strNewLine
                       + "Transaction ID:   " + this._txnID + strNewLine
                       + "Transaction Type: " + this._txnType + strNewLine
                       + "Payment Type:     " + this._paymentType + strNewLine
                       + "Payment Status:   " + this._paymentStatus + strNewLine
                       + "Pending Reason:   " + this._pendingReason + strNewLine
                       + "Payment Date:     " + this._paymentDate + strNewLine
                       + "Receiver Email:   " + this._receiverEmail + strNewLine
                       + "Invoice:          " + this._invoice + strNewLine
                       + "Item Number:      " + this._itemNumber + strNewLine
                       + "Item Name:        " + this._itemName + strNewLine
                       + "Quantity:         " + this._quantity + strNewLine
                       + "Custom:           " + this._custom + strNewLine
                       + "Payment Currency: " + this._currency + strNewLine
                       + "Payment Gross:    " + this._paymentGross + strNewLine
                       + "Payment Fee:      " + this._paymentFee + strNewLine
                       + "Payer Email:      " + this._payerEmail + strNewLine
                       + "First Name:       " + this._firstName + strNewLine
                       + "Last Name:        " + this._lastName + strNewLine
                       + "Street Address:   " + this._addressStreet + strNewLine
                       + "City:             " + this._addressCity + strNewLine
                       + "State:            " + this._addressState + strNewLine
                       + "Zip Code:         " + this._addressZip + strNewLine
                       + "Country:          " + this._addressCountry + strNewLine
                       + "Address Status:   " + this._addressStatus + strNewLine
                       + "Payer Status:     " + this._payerStatus + strNewLine
                       + "Verify Sign:      " + this._verifySign + strNewLine
                       + "Subscriber Date:  " + this._subscrDate + strNewLine
                       + "Period 1:         " + this._period1 + strNewLine
                       + "Period 2:         " + this._period2 + strNewLine
                       + "Period 3:         " + this._period3 + strNewLine
                       + "Amount 1:         " + this._amount1 + strNewLine
                       + "Amount 2:         " + this._amount2 + strNewLine
                       + "Amount 3:         " + this._amount3 + strNewLine
                       + "Recurring:        " + this._recurring + strNewLine
                       + "Reattempt:        " + this._reattempt + strNewLine
                       + "Retry At:         " + this._retryAt + strNewLine
                       + "Recur Times:      " + this._recurTimes + strNewLine
                       + "UserName:         " + this._username + strNewLine
                       + "Password:         " + this._password + strNewLine
                       + "Subscriber ID:    " + this._subscrID + strNewLine
                       + "Notify Version:   "
                       + this._notifyVersion + strNewLine;


            var portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
            sEmail = this._settings.StandardEmail.Trim();
            sMessage = sMessage;
            try
            {
                var sSystemPrice = Convert.ToDecimal(this._paymentGross);

                // Also verify that Gross Payment is what we logged as the Fee ("payment_gross" field )
                this._objEventSignups =
                    this._objCtlEventSignups.EventsSignupsGet(Convert.ToInt32(this._itemNumber), 0, true);
                this._objEvent =
                    this._objCtlEventEvent.EventsGet(this._objEventSignups.EventID, this._objEventSignups.ModuleID);
                var sPPPrice = this._objEvent.EnrollFee * this._objEventSignups.NoEnrolees;


                var objEventEmailInfo = new EventEmailInfo();
                var objEventEmail = new EventEmails(this._objEvent.PortalID, this._objEventSignups.ModuleID,
                                                    this._localResourceFile);
                objEventEmailInfo.TxtEmailSubject = this._settings.Templates.txtEnrollMessageSubject;
                objEventEmailInfo.TxtEmailFrom = sEmail;
                if (sendToUser)
                {
                    if (this._objEventSignups.UserID > -1)
                    {
                        objEventEmailInfo.UserIDs.Add(this._objEventSignups.UserID);
                    }
                    else
                    {
                        objEventEmailInfo.UserEmails.Add(this._objEventSignups.AnonEmail);
                        objEventEmailInfo.UserLocales.Add(this._objEventSignups.AnonCulture);
                        objEventEmailInfo.UserTimeZoneIds.Add(this._objEventSignups.AnonTimeZoneId);
                    }
                }
                objEventEmailInfo.UserIDs.Add(this._objEvent.OwnerID);
                var objEventEmailInfo2 = new EventEmailInfo();
                objEventEmailInfo2.TxtEmailFrom = sEmail;
                objEventEmailInfo2.UserEmails.Add(this._objEvent.PayPalAccount);
                objEventEmailInfo2.UserLocales.Add("");
                objEventEmailInfo2.UserTimeZoneIds.Add("");

                if (sPPPrice == sSystemPrice)
                {
                    //we're ok
                    objEventEmailInfo2.TxtEmailSubject =
                        "Sale of: " + this._itemName + " from " + portalSettings.PortalName;
                    objEventEmailInfo2.TxtEmailBody = sMessage;
                    if (this._paymentStatus.ToUpper() == "COMPLETED")
                    {
                        this.SavePayPalInfo(true, this._objEvent);

                        // Mail users
                        if (this._settings.SendEnrollMessagePaid)
                        {
                            objEventEmailInfo.TxtEmailBody = this._settings.Templates.txtEnrollMessagePaid;
                            objEventEmail.SendEmails(objEventEmailInfo, this._objEvent, this._objEventSignups, true);
                        }
                    }
                    else
                    {
                        this.SavePayPalInfo(false, this._objEvent);

                        // Mail users
                        if (this._settings.SendEnrollMessagePending)
                        {
                            objEventEmailInfo.TxtEmailBody = this._settings.Templates.txtEnrollMessagePending;
                            objEventEmail.SendEmails(objEventEmailInfo, this._objEvent, this._objEventSignups, true);
                        }
                    }
                }
                else
                {
                    //someone is trying to rip us off.
                    objEventEmailInfo2.TxtEmailSubject = "Failed Price Matchup Check: " + this._itemName + " from " +
                                                         portalSettings.PortalName;
                    objEventEmailInfo2.TxtEmailBody =
                        "There was an incorrect match between actual price and price paid. The following transaction information is provided below:" +
                        strNewLine +
                        strNewLine + "The purchasing email is: " + this._payerEmail + strNewLine +
                        "User ID: " + this._custom + strNewLine +
                        "Transaction Type: " + this._txnType + strNewLine +
                        "Transaction ID: " + this._txnID + strNewLine +
                        "Item Number: " + this._itemNumber + strNewLine +
                        "PayPal Paid: " + sSystemPrice + strNewLine +
                        "Actual Price: " + sPPPrice + strNewLine +
                        strNewLine + "TRANSACTION DETAILS: " + strNewLine + strNewLine + sMessage;

                    this.SavePayPalErrorLogInfo();

                    // Mail users
                    if (this._settings.SendEnrollMessageIncorrect)
                    {
                        objEventEmailInfo.TxtEmailBody = this._settings.Templates.txtEnrollMessageIncorrect;
                        objEventEmail.SendEmails(objEventEmailInfo, this._objEvent, this._objEventSignups, true);
                    }
                }
                objEventEmail.SendEmails(objEventEmailInfo2, this._objEvent, this._objEventSignups, true);
            }
            catch (Exception exc)
            {
                Exceptions.LogException(
                    new ModuleLoadException("EventIPN, Paypal Exception: " + exc.Message + " at: " + exc.Source));
                Exceptions.ProcessModuleLoadException(this, exc);
                var localResourceFile = this.TemplateSourceDirectory + "/" + Localization.LocalResourceDirectory +
                                        "/EventIPN.resx";
                var objEventEmailInfo = new EventEmailInfo();
                var objEventEmail = new EventEmails(this._objEvent.PortalID, this._objEventSignups.ModuleID,
                                                    localResourceFile);
                objEventEmailInfo.TxtEmailSubject = "Sale of: " + this._itemName + " from " + portalSettings.PortalName;
                objEventEmailInfo.TxtEmailFrom = this._settings.StandardEmail.Trim();
                objEventEmailInfo.UserEmails.Add(this._objEvent.PayPalAccount);
                objEventEmailInfo.UserLocales.Add("");
                objEventEmailInfo.UserTimeZoneIds.Add("");

                objEventEmailInfo.TxtEmailBody =
                    "There was a failure of the item purchase module. The following transaction information is provided below:" +
                    strNewLine +
                    strNewLine + "The purchasing email is: " + this._payerEmail + strNewLine +
                    strNewLine + "User ID: " + this._custom + strNewLine +
                    strNewLine + "Transaction Type: " + this._txnType + strNewLine +
                    strNewLine + "Transaction ID: " + this._txnID + strNewLine +
                    strNewLine + "Error Code: " + exc.Message + strNewLine + exc.Source;

                objEventEmail.SendEmails(objEventEmailInfo, this._objEvent, this._objEventSignups, true);
            }
        }

        private void SavePayPalInfo(bool approved, EventInfo objEvent)
        {
            this._objEventSignups = this._objCtlEventSignups.EventsSignupsGet(int.Parse(this._itemNumber), 0, true);
            this._objEventSignups.Approved = approved;
            this._objEventSignups.PayPalAddress = this._addressStreet;
            this._objEventSignups.PayPalCity = this._addressCity;
            this._objEventSignups.PayPalCountry = this._addressCountry;
            this._objEventSignups.PayPalAmount = Convert.ToDecimal(this._paymentGross);
            this._objEventSignups.PayPalCurrency = this._currency;
            this._objEventSignups.PayPalFee = Convert.ToDecimal(this._paymentFee);
            this._objEventSignups.PayPalFirstName = this._firstName;
            this._objEventSignups.PayPalLastName = this._lastName;
            this._objEventSignups.PayPalPayerEmail = this._payerEmail;
            this._objEventSignups.PayPalPayerID = this._subscrID;
            this._objEventSignups.PayPalPayerStatus = this._payerStatus;
            this._objEventSignups.PayPalPaymentDate = DateTime.UtcNow;
            this._objEventSignups.PayPalReason = this._pendingReason;
            this._objEventSignups.PayPalRecieverEmail = this._receiverEmail;
            this._objEventSignups.PayPalState = this._addressState;
            this._objEventSignups.PayPalStatus = this._paymentStatus.ToLower();
            this._objEventSignups.PayPalTransID = this._txnID;
            //objEventSignups.PayPalUserEmail = pay.IPNUserEmail
            this._objEventSignups.PayPalZip = this._addressZip;
            var eventsBase = new EventBase();
            eventsBase.CreateEnrollment(this._objEventSignups, objEvent, this._settings);
        }

        private void SavePayPalErrorLogInfo()
        {
            this._objEventPpErrorLog = new EventPpErrorLogInfo();
            this._objEventPpErrorLog.SignupID = Convert.ToInt32(this._itemNumber);
            //objEventPPErrorLog.Approved = False
            this._objEventPpErrorLog.PayPalAddress = this._addressStreet;
            this._objEventPpErrorLog.PayPalCity = this._addressCity;
            this._objEventPpErrorLog.PayPalCountry = this._addressCountry;
            this._objEventPpErrorLog.PayPalAmount = Convert.ToDouble(this._paymentGross);
            this._objEventPpErrorLog.PayPalCurrency = this._currency;
            this._objEventPpErrorLog.PayPalFee = Convert.ToDouble(this._paymentFee);
            this._objEventPpErrorLog.PayPalFirstName = this._firstName;
            this._objEventPpErrorLog.PayPalLastName = this._lastName;
            this._objEventPpErrorLog.PayPalPayerEmail = this._payerEmail;
            this._objEventPpErrorLog.PayPalPayerID = this._subscrID;
            this._objEventPpErrorLog.PayPalPayerStatus = this._payerStatus;
            //objEventPPErrorLog.PayPalPaymentDate = CType(Payment_date, Date)
            this._objEventPpErrorLog.PayPalPaymentDate = DateTime.Now;
            this._objEventPpErrorLog.PayPalReason = this._pendingReason;
            this._objEventPpErrorLog.PayPalRecieverEmail = this._receiverEmail;
            this._objEventPpErrorLog.PayPalState = this._addressState;
            this._objEventPpErrorLog.PayPalStatus = this._paymentStatus.ToLower();
            this._objEventPpErrorLog.PayPalTransID = this._txnID;
            //objEventPPErrorLog.PayPalUserEmail = pay.IPNUserEmail
            this._objEventPpErrorLog.PayPalZip = this._addressZip;
            this._objEventPpErrorLog = this._objCtlEventPpErrorLog.EventsPpErrorLogAdd(this._objEventPpErrorLog);
        }

        #endregion
    }
}