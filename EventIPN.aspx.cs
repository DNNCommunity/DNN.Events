using DotNetNuke.Services.Exceptions;
using System.Diagnostics;
using System.Web.UI;
using System.Web;
using DotNetNuke.Services.Localization;
using System;
using System.Net;
using System.IO;
using System.Text;


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
		public partial class EventIPN : Page
		{
			
#region Private Area
			private int _moduleID = -1;
			private EventModuleSettings _settings;
			private EventSignupsInfo _objEventSignups = new EventSignupsInfo();
			private EventSignupsController _objCtlEventSignups = new EventSignupsController();
			private EventPpErrorLogInfo _objEventPpErrorLog = new EventPpErrorLogInfo();
			private EventPpErrorLogController _objCtlEventPpErrorLog = new EventPpErrorLogController();
			private EventInfo _objEvent = new EventInfo();
			private EventController _objCtlEventEvent = new EventController();
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
			[DebuggerStepThrough()]private void InitializeComponent()
			{
				
			}
			
			private void Page_Init(System.Object sender, EventArgs e)
			{
				//CODEGEN: This method call is required by the Web Form Designer
				//Do not modify it using the code editor.
				InitializeComponent();
			}
			
#endregion
			
#region Event Handlers
			private void Page_Load(System.Object sender, EventArgs e)
			{
				try
				{
					_localResourceFile = TemplateSourceDirectory + "/" + Localization.LocalResourceDirectory + "/EventIPN.resx";
					
					string sPpnMessages = ""; //  Payment message
					// assign posted variables to local variables
					_receiverEmail = Request.Params["receiver_email"];
					_itemName = Request.Params["item_name"];
					_itemNumber = Request.Params["item_number"];
					_quantity = Request.Params["quantity"];
					_invoice = Request.Params["invoice"];
					_custom = Request.Params["custom"];
					_paymentStatus = Request.Params["payment_status"];
					_currency = Request.Params["mc_currency"];
					_pendingReason = Request.Params["pending_reason"];
					_paymentDate = Request.Params["payment_date"];
					_paymentFee = Request.Params["mc_fee"];
					_paymentGross = Request.Params["mc_gross"];
					_txnID = Request.Params["txn_id"];
					_txnType = Request.Params["txn_type"];
					_firstName = Request.Params["first_name"];
					_lastName = Request.Params["last_name"];
					_addressStreet = Request.Params["address_street"];
					_addressCity = Request.Params["address_city"];
					_addressState = Request.Params["address_state"];
					_addressZip = Request.Params["address_zip"];
					_addressCountry = Request.Params["address_country"];
					_addressStatus = Request.Params["address_status"];
					_payerEmail = Request.Params["payer_email"];
					_payerStatus = Request.Params["payer_status"];
					_paymentType = Request.Params["payment_type"];
					_notifyVersion = Request.Params["notify_version"];
					_verifySign = Request.Params["verify_sign"];
					_subscrDate = Request.Params["subscr_date"]; //Start date or cancellation date depending on whether transaction is "subscr_signup" or "subscr_cancel"
					_period1 = Request.Params["period1"]; //(optional) Trial subscription interval in days, weeks, months, years (example: a 4 day interval is "period1: 4 d")
					_period2 = Request.Params["period2"]; //(optional) Trial subscription interval in days, weeks, months, years
					_period3 = Request.Params["period3"]; //Regular subscription interval in days, weeks, months, years
					_amount1 = Request.Params["amount1"]; //(optional) Amount of payment for trial period1
					_amount2 = Request.Params["amount2"]; //(optional) Amount of payment for trial period2
					_amount3 = Request.Params["amount3"]; //Amount of payment for regular period3
					_recurring = Request.Params["recurring"]; //Indicates whether regular rate recurs (1 is yes, 0 is no)
					_reattempt = Request.Params["reattempt"]; //Indicates whether reattempts should occur upon payment failures (1 is yes, 0 is no)
					_retryAt = Request.Params["retry_at"]; //Date we will retry failed subscription payment
					_recurTimes = Request.Params["recur_times"]; //How many payment installments will occur at the regular rate
					_username = Request.Params["username"]; //(optional) Username generated by PayPal and given to subscriber to access the subscription
					_password = Request.Params["password"]; //(optional) Password generated by PayPal and given to subscriber to access the subscription (password will be hashed).
					_subscrID = Request.Params["subscr_id"]; //(optional) ID generated by PayPal for the subscriber
					_strToSend = Request.Form.ToString();
					
					// Create the string to post back to PayPal system to validate
					_strToSend += "&cmd=_notify-validate";
					
					// Get the Event Signup
					_objEventSignups = _objCtlEventSignups.EventsSignupsGet(System.Convert.ToInt32(_itemNumber), 0, true);
					
					// Get Module Settings
					_moduleID = _objEventSignups.ModuleID;
					EventModuleSettings ems = new EventModuleSettings();
					_settings = ems.GetEventModuleSettings(_moduleID, _localResourceFile);
					
					//Initialize the WebRequest.
					string webURL = "";
					webURL = _settings.Paypalurl + "/cgi-bin/webscr";
					
					//Send PayPal Verification Response
					HttpWebRequest myRequest = (HttpWebRequest) (HttpWebRequest.Create(webURL));
					myRequest.AllowAutoRedirect = false;
					myRequest.Method = "POST";
					myRequest.ContentType = "application/x-www-form-urlencoded";
					
					//Create post stream
					Stream requestStream = myRequest.GetRequestStream();
					byte[] someBytes = Encoding.UTF8.GetBytes(_strToSend);
					requestStream.Write(someBytes, 0, someBytes.Length);
					requestStream.Close();
					
					//Send request and get response
					HttpWebResponse myResponse = (HttpWebResponse) (myRequest.GetResponse());
					if (myResponse.StatusCode == HttpStatusCode.OK)
					{
						
						//Obtain a 'Stream' object associated with the response object.
						Stream receiveStream = myResponse.GetResponseStream();
						Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
						
						//Pipe the stream to a higher level stream reader with the required encoding format.
						StreamReader readStream = new StreamReader(receiveStream, encode);
						
						//Read result
						string result = readStream.ReadLine();
						if (result == "INVALID")
						{
							MailUsTheOrder("PPIPN: Status came back as INVALID!", false);
						}
						else if (result == "VERIFIED")
						{
							
							switch (_paymentStatus)
							{
								case "Completed": //The payment has been completed and the funds are successfully in your account balance
									switch (_txnType)
									{
										case "web_accept":
										case "cart":
											//"web_accept": The payment was sent by your customer via the Web Accept feature.
											//"cart": This payment was sent by your customer via the Shopping Cart feature
											sPpnMessages = sPpnMessages + "PPIPN: This payment was sent by your customer via the Shopping Cart feature" + Environment.NewLine;
											break;
										case "send_money": //This payment was sent by your customer from the PayPal website, imports the "Send Money" tab
											sPpnMessages = sPpnMessages + "PPIPN: This payment was sent by your customer from the PayPal website" + Environment.NewLine;
											break;
										case "subscr_signup": //This IPN is for a subscription sign-up
											sPpnMessages = sPpnMessages + "PPIPN: This IPN is for a subscription sign-up" + Environment.NewLine;
											break;
										case "subscr_cancel": //This IPN is for a subscription cancellation
											sPpnMessages = sPpnMessages + "PPIPN: Subscription cancellation." + Environment.NewLine;
											break;
										case "subscr_failed": //This IPN is for a subscription payment failure
											sPpnMessages = sPpnMessages + "PPIPN: Subscription failed." + Environment.NewLine;
											break;
										case "subscr_payment": //This IPN is for a subscription payment
											sPpnMessages = sPpnMessages + "PPIPN: This IPN is for a subscription payment" + Environment.NewLine;
											break;
										case "subscr_eot": //This IPN is for a subscription's end of term
											sPpnMessages = sPpnMessages + "PPIPN:  Subscription end of term." + Environment.NewLine;
											break;
									}
									switch (_addressStatus)
									{
										case "confirmed": //Customer provided a Confirmed Address
											break;
										case "unconfirmed": //Customer provided an Unconfirmed Address
											break;
									}
									switch (_payerStatus)
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
									switch (_paymentType)
									{
										case "echeck": //This payment was funded with an eCheck
											sPpnMessages = sPpnMessages + "PPIPN: Payment was funded with an eCheck." + Environment.NewLine;
											break;
										case "instant": //This payment was funded with PayPal balance, credit card, or Instant Transfer
											sPpnMessages = sPpnMessages + "PPIPN: This payment was funded with PayPal balance, credit card, or Instant Transfer" + Environment.NewLine;
											break;
									}
									MailUsTheOrder(sPpnMessages, true);
									break;
								case "Pending": //The payment is pending - see the "pending reason" variable below for more information. Watch: You will receive another instant payment notification when the payment becomes "completed", "failed", or "denied"
									switch (_pendingReason)
									{
										case "echeck": //The payment is pending because it was made by an eCheck, which has not yet cleared
											break;
										case "intl": //The payment is pending because you, the merchant, hold an international account and do not have a withdrawal mechanism. You must manually accept or deny this payment from your Account Overview
											break;
										case "verify": //The payment is pending because you, the merchant, are not yet verified. You must verify your account before you can accept this payment
											break;
										case "address": //The payment is pending because your customer did not include a confirmed shipping address and you, the merchant, have your Payment Receiving Preferences set such that you want to manually accept or deny each of these payments. To change your preference, go to the "Preferences" section of your "Profile"
											break;
										case "upgrade": //The payment is pending because it was made via credit card and you, the merchant, must upgrade your account to Business or Premier status in order to receive the funds
											break;
										case "unilateral": //The payment is pending because it was made to an email address that is not yet registered or confirmed
											break;
										case "other": //The payment is pending for an "other" reason. For more information, contact customer service
											break;
									}
									MailUsTheOrder("PPIPN: Order is waiting to be processed.");
									break;
								case "Failed": //The payment has failed. This will only happen if the payment was made from your customer's bank account
									MailUsTheOrder("PPIPN: This only happens if the payment was made from our customer's bank account.");
									break;
								case "Denied": //You, the merchant, denied the payment. This will only happen if the payment was previously pending due to one of the "pending reasons"
									MailUsTheOrder("PPIPN: We denied this payment.");
									break;
							}
						}
					}
					//Close the response to free resources.
					myResponse.Close(); //If it is "OK"
				}
				catch (Exception exc)
				{
					Exceptions.LogException(new ModuleLoadException("EventIPN, Paypal Exception: " + exc.Message + " at: " + exc.Source));
					Exceptions.ProcessModuleLoadException(this, exc);
				}
			}
#endregion
			
#region Helper Routines
			private void MailUsTheOrder(string tagMsg, bool sendToUser = true)
			{
				// ********* RWJS - Seems to add no value, and would have always returned nothing *********
				//			InitializeSettings(Item_number)
				
				string sMessage = "";
				string sEmail = "";
				string strNewLine = Environment.NewLine;
				if (_settings.HTMLEmail == "html")
				{
					strNewLine = "<br />";
				}
				sMessage = tagMsg + strNewLine 
					+ "Transaction ID:   " + _txnID + strNewLine 
					+ "Transaction Type: " + _txnType + strNewLine 
					+ "Payment Type:     " + _paymentType + strNewLine 
					+ "Payment Status:   " + _paymentStatus + strNewLine 
					+ "Pending Reason:   " + _pendingReason + strNewLine 
					+ "Payment Date:     " + _paymentDate + strNewLine 
					+ "Receiver Email:   " + _receiverEmail + strNewLine 
					+ "Invoice:          " + _invoice + strNewLine 
					+ "Item Number:      " + _itemNumber + strNewLine 
					+ "Item Name:        " + _itemName + strNewLine 
					+ "Quantity:         " + _quantity + strNewLine 
					+ "Custom:           " + _custom + strNewLine 
					+ "Payment Currency: " + _currency + strNewLine 
					+ "Payment Gross:    " + _paymentGross + strNewLine 
					+ "Payment Fee:      " + _paymentFee + strNewLine 
					+ "Payer Email:      " + _payerEmail + strNewLine 
					+ "First Name:       " + _firstName + strNewLine 
					+ "Last Name:        " + _lastName + strNewLine 
					+ "Street Address:   " + _addressStreet + strNewLine 
					+ "City:             " + _addressCity + strNewLine 
					+ "State:            " + _addressState + strNewLine 
					+ "Zip Code:         " + _addressZip + strNewLine 
					+ "Country:          " + _addressCountry + strNewLine 
					+ "Address Status:   " + _addressStatus + strNewLine 
					+ "Payer Status:     " + _payerStatus + strNewLine 
					+ "Verify Sign:      " + _verifySign + strNewLine 
					+ "Subscriber Date:  " + _subscrDate + strNewLine 
					+ "Period 1:         " + _period1 + strNewLine 
					+ "Period 2:         " + _period2 + strNewLine 
					+ "Period 3:         " + _period3 + strNewLine 
					+ "Amount 1:         " + _amount1 + strNewLine 
					+ "Amount 2:         " + _amount2 + strNewLine 
					+ "Amount 3:         " + _amount3 + strNewLine 
					+ "Recurring:        " + _recurring + strNewLine 
					+ "Reattempt:        " + _reattempt + strNewLine 
					+ "Retry At:         " + _retryAt + strNewLine 
					+ "Recur Times:      " + _recurTimes + strNewLine 
					+ "UserName:         " + _username + strNewLine 
					+ "Password:         " + _password + strNewLine 
					+ "Subscriber ID:    " + _subscrID + strNewLine 
					+ "Notify Version:   "
				+ _notifyVersion + strNewLine;
				
				
				Entities.Portals.PortalSettings portalSettings = (Entities.Portals.PortalSettings) (HttpContext.Current.Items["PortalSettings"]);
				sEmail = _settings.StandardEmail.Trim();
				sMessage = sMessage;
				try
				{
					decimal sSystemPrice = System.Convert.ToDecimal(_paymentGross);
					
					// Also verify that Gross Payment is what we logged as the Fee ("payment_gross" field )
					_objEventSignups = _objCtlEventSignups.EventsSignupsGet(System.Convert.ToInt32(_itemNumber), 0, true);
					_objEvent = _objCtlEventEvent.EventsGet(_objEventSignups.EventID, _objEventSignups.ModuleID);
					decimal sPPPrice = _objEvent.EnrollFee * _objEventSignups.NoEnrolees;
					
					
					EventEmailInfo objEventEmailInfo = new EventEmailInfo();
					EventEmails objEventEmail = new EventEmails(_objEvent.PortalID, _objEventSignups.ModuleID, _localResourceFile);
					objEventEmailInfo.TxtEmailSubject = _settings.Templates.txtEnrollMessageSubject;
					objEventEmailInfo.TxtEmailFrom = sEmail;
					if (sendToUser)
					{
						if (_objEventSignups.UserID > -1)
						{
							objEventEmailInfo.UserIDs.Add(_objEventSignups.UserID);
						}
						else
						{
							objEventEmailInfo.UserEmails.Add(_objEventSignups.AnonEmail);
							objEventEmailInfo.UserLocales.Add(_objEventSignups.AnonCulture);
							objEventEmailInfo.UserTimeZoneIds.Add(_objEventSignups.AnonTimeZoneId);
						}
					}
					objEventEmailInfo.UserIDs.Add(_objEvent.OwnerID);
					EventEmailInfo objEventEmailInfo2 = new EventEmailInfo();
					objEventEmailInfo2.TxtEmailFrom = sEmail;
					objEventEmailInfo2.UserEmails.Add(_objEvent.PayPalAccount);
					objEventEmailInfo2.UserLocales.Add("");
					objEventEmailInfo2.UserTimeZoneIds.Add("");
					
					if (sPPPrice == sSystemPrice)
					{
						//we're ok
						objEventEmailInfo2.TxtEmailSubject = "Sale of: " + _itemName + " from " + portalSettings.PortalName;
						objEventEmailInfo2.TxtEmailBody = sMessage;
						if (_paymentStatus.ToUpper() == "COMPLETED")
						{
							SavePayPalInfo(true, _objEvent);
							
							// Mail users
							if (_settings.SendEnrollMessagePaid)
							{
								objEventEmailInfo.TxtEmailBody = _settings.Templates.txtEnrollMessagePaid;
								objEventEmail.SendEmails(objEventEmailInfo, _objEvent, _objEventSignups, true);
							}
						}
						else
						{
							SavePayPalInfo(false, _objEvent);
							
							// Mail users
							if (_settings.SendEnrollMessagePending)
							{
								objEventEmailInfo.TxtEmailBody = _settings.Templates.txtEnrollMessagePending;
								objEventEmail.SendEmails(objEventEmailInfo, _objEvent, _objEventSignups, true);
							}
						}
					}
					else
					{
						//someone is trying to rip us off.
						objEventEmailInfo2.TxtEmailSubject = "Failed Price Matchup Check: " + _itemName + " from " + portalSettings.PortalName;
						objEventEmailInfo2.TxtEmailBody = "There was an incorrect match between actual price and price paid. The following transaction information is provided below:" + strNewLine + 
							strNewLine + "The purchasing email is: " + _payerEmail + strNewLine + 
							"User ID: " + _custom + strNewLine + 
							"Transaction Type: " + _txnType + strNewLine + 
							"Transaction ID: " + _txnID + strNewLine + 
							"Item Number: " + _itemNumber + strNewLine + 
							"PayPal Paid: " + sSystemPrice.ToString() + strNewLine + 
							"Actual Price: " + sPPPrice.ToString() + strNewLine + 
							strNewLine + "TRANSACTION DETAILS: " + strNewLine + strNewLine + sMessage;
						
						SavePayPalErrorLogInfo();
						
						// Mail users
						if (_settings.SendEnrollMessageIncorrect)
						{
							objEventEmailInfo.TxtEmailBody = _settings.Templates.txtEnrollMessageIncorrect;
							objEventEmail.SendEmails(objEventEmailInfo, _objEvent, _objEventSignups, true);
						}
					}
					objEventEmail.SendEmails(objEventEmailInfo2, _objEvent, _objEventSignups, true);
				}
				catch (Exception exc)
				{
					Exceptions.LogException(new ModuleLoadException("EventIPN, Paypal Exception: " + exc.Message + " at: " + exc.Source));
					Exceptions.ProcessModuleLoadException(this, exc);
					string localResourceFile = TemplateSourceDirectory + "/" + Localization.LocalResourceDirectory + "/EventIPN.resx";
					EventEmailInfo objEventEmailInfo = new EventEmailInfo();
					EventEmails objEventEmail = new EventEmails(_objEvent.PortalID, _objEventSignups.ModuleID, localResourceFile);
					objEventEmailInfo.TxtEmailSubject = "Sale of: " + _itemName + " from " + portalSettings.PortalName;
					objEventEmailInfo.TxtEmailFrom = _settings.StandardEmail.Trim();
					objEventEmailInfo.UserEmails.Add(_objEvent.PayPalAccount);
					objEventEmailInfo.UserLocales.Add("");
					objEventEmailInfo.UserTimeZoneIds.Add("");
					
					objEventEmailInfo.TxtEmailBody = "There was a failure of the item purchase module. The following transaction information is provided below:" + strNewLine + 
						strNewLine + "The purchasing email is: " + _payerEmail + strNewLine + 
						strNewLine + "User ID: " + _custom + strNewLine + 
						strNewLine + "Transaction Type: " + _txnType + strNewLine + 
						strNewLine + "Transaction ID: " + _txnID + strNewLine + 
						strNewLine + "Error Code: " + exc.Message + strNewLine + exc.Source;
					
					objEventEmail.SendEmails(objEventEmailInfo, _objEvent, _objEventSignups, true);
				}
			}
			
			private void SavePayPalInfo(bool approved, EventInfo objEvent)
			{
				_objEventSignups = _objCtlEventSignups.EventsSignupsGet(int.Parse(_itemNumber), 0, true);
				_objEventSignups.Approved = approved;
				_objEventSignups.PayPalAddress = _addressStreet;
				_objEventSignups.PayPalCity = _addressCity;
				_objEventSignups.PayPalCountry = _addressCountry;
				_objEventSignups.PayPalAmount = System.Convert.ToDecimal(_paymentGross);
				_objEventSignups.PayPalCurrency = _currency;
				_objEventSignups.PayPalFee = System.Convert.ToDecimal(_paymentFee);
				_objEventSignups.PayPalFirstName = _firstName;
				_objEventSignups.PayPalLastName = _lastName;
				_objEventSignups.PayPalPayerEmail = _payerEmail;
				_objEventSignups.PayPalPayerID = _subscrID;
				_objEventSignups.PayPalPayerStatus = _payerStatus;
				_objEventSignups.PayPalPaymentDate = DateTime.UtcNow;
				_objEventSignups.PayPalReason = _pendingReason;
				_objEventSignups.PayPalRecieverEmail = _receiverEmail;
				_objEventSignups.PayPalState = _addressState;
				_objEventSignups.PayPalStatus = _paymentStatus.ToLower();
				_objEventSignups.PayPalTransID = _txnID;
				//objEventSignups.PayPalUserEmail = pay.IPNUserEmail
				_objEventSignups.PayPalZip = _addressZip;
				EventBase eventsBase = new EventBase();
				eventsBase.CreateEnrollment(_objEventSignups, objEvent, _settings);
			}
			
			private void SavePayPalErrorLogInfo()
			{
				_objEventPpErrorLog = new EventPpErrorLogInfo();
				_objEventPpErrorLog.SignupID = System.Convert.ToInt32(_itemNumber);
				//objEventPPErrorLog.Approved = False
				_objEventPpErrorLog.PayPalAddress = _addressStreet;
				_objEventPpErrorLog.PayPalCity = _addressCity;
				_objEventPpErrorLog.PayPalCountry = _addressCountry;
				_objEventPpErrorLog.PayPalAmount = System.Convert.ToDouble(_paymentGross);
				_objEventPpErrorLog.PayPalCurrency = _currency;
				_objEventPpErrorLog.PayPalFee = System.Convert.ToDouble(_paymentFee);
				_objEventPpErrorLog.PayPalFirstName = _firstName;
				_objEventPpErrorLog.PayPalLastName = _lastName;
				_objEventPpErrorLog.PayPalPayerEmail = _payerEmail;
				_objEventPpErrorLog.PayPalPayerID = _subscrID;
				_objEventPpErrorLog.PayPalPayerStatus = _payerStatus;
				//objEventPPErrorLog.PayPalPaymentDate = CType(Payment_date, Date)
				_objEventPpErrorLog.PayPalPaymentDate = DateTime.Now;
				_objEventPpErrorLog.PayPalReason = _pendingReason;
				_objEventPpErrorLog.PayPalRecieverEmail = _receiverEmail;
				_objEventPpErrorLog.PayPalState = _addressState;
				_objEventPpErrorLog.PayPalStatus = _paymentStatus.ToLower();
				_objEventPpErrorLog.PayPalTransID = _txnID;
				//objEventPPErrorLog.PayPalUserEmail = pay.IPNUserEmail
				_objEventPpErrorLog.PayPalZip = _addressZip;
				_objEventPpErrorLog = _objCtlEventPpErrorLog.EventsPpErrorLogAdd(_objEventPpErrorLog);
			}
#endregion
			
		}
	}

