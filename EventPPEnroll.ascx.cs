using DotNetNuke.Services.Exceptions;
using System.Diagnostics;
using DotNetNuke.Framework;
using Microsoft.VisualBasic;
using System.Web;
using DotNetNuke.Services.Localization;
using System;
using System.Globalization;


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
    [DNNtc.ModuleControlProperties("PPEnroll", "Event PayPal Enrollment", DNNtc.ControlType.View, "https://dnnevents.codeplex.com/documentation", false, true)]
    public partial class EventPPEnroll : EventBase
    {

        #region  Web Form Designer Generated Code

        //This call is required by the Web Form Designer.
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {

        }

        private void Page_Init(System.Object sender, EventArgs e)
        {
            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            InitializeComponent();
        }

        #endregion

        #region Private Area
        private int _itemID = -1;
        private int _noEnrol = 1;
        private EventController _objCtlEvent = new EventController();
        private EventSignupsInfo _objEventSignups = new EventSignupsInfo();
        private EventSignupsController _objCtlEventSignups = new EventSignupsController();
        private string _anonEmail = null;
        private string _anonName = null;
        private string _anonTelephone = null;

        private enum MessageLevel : int
        {
            DNNSuccess = 1,
            DNNInformation,
            DNNWarning,
            DNNError
        }

        #endregion

        #region Event Handlers
        private void Page_Load(System.Object sender, EventArgs e)
        {
            try
            {
                double dblTotal = 0;
                string strCurrency = "";

                if (!(ReferenceEquals(Request.Params["ItemID"], null)))
                {
                    _itemID = int.Parse(Request.Params["ItemID"]);
                }
                if (!(ReferenceEquals(Request.Params["AnonEmail"], null)))
                {
                    _anonEmail = HttpUtility.UrlDecode(Request.Params["AnonEmail"]);
                }
                if (!(ReferenceEquals(Request.Params["AnonName"], null)))
                {
                    _anonName = HttpUtility.UrlDecode(Request.Params["AnonName"]);
                }
                if (!(ReferenceEquals(Request.Params["AnonPhone"], null)))
                {
                    _anonTelephone = HttpUtility.UrlDecode(Request.Params["AnonPhone"]);
                    if (_anonTelephone == "0")
                    {
                        _anonTelephone = "";
                    }
                }

                // Set the selected theme
                SetTheme(pnlEventsModulePayPal);

                divMessage.Attributes.Add("style", "display:none;");

                if (!(Page.IsPostBack))
                {
                    if (_itemID != -1)
                    {
                        if (!(ReferenceEquals(Request.Params["NoEnrol"], null)))
                        {
                            _noEnrol = int.Parse(Request.Params["NoEnrol"]);
                        }

                        EventInfo objEvent = default(EventInfo);
                        objEvent = _objCtlEvent.EventsGet(_itemID, ModuleId);

                        //  Compute Dates/Times (for recurring)
                        DateTime startdate = objEvent.EventTimeBegin;
                        SelectedDate = startdate.Date;
                        DateTime d = default(DateTime);
                        d = startdate.Date.AddMinutes(objEvent.EventTimeBegin.TimeOfDay.TotalMinutes);
                        lblStartDate.Text = d.ToLongDateString() + " " + d.ToShortTimeString();
                        lblEventName.Text = objEvent.EventName;
                        lblDescription.Text = Server.HtmlDecode(objEvent.EventDesc);
                        lblFee.Text = string.Format("{0:#0.00}", objEvent.EnrollFee);
                        lblNoEnrolees.Text = Convert.ToString(_noEnrol);
                        lblPurchase.Text = Localization.GetString("lblPurchase", LocalResourceFile);
                        lblPurchase.Visible = true;
                    }
                    else if (!(ReferenceEquals(Request.Params["signupid"], null)))
                    {
                        // Get EventSignup
                        _objEventSignups = new EventSignupsInfo();
                        int signupID = System.Convert.ToInt32(Request.Params["signupid"]);
                        _objEventSignups = _objCtlEventSignups.EventsSignupsGet(signupID, ModuleId, false);
                        lblStartDate.Text = _objEventSignups.EventTimeBegin.ToLongDateString() + " " + _objEventSignups.EventTimeBegin.ToShortTimeString();
                        lblEventName.Text = _objEventSignups.EventName;
                        // Get Related Event
                        EventInfo objEvent = default(EventInfo);
                        objEvent = _objCtlEvent.EventsGet(_objEventSignups.EventID, _objEventSignups.ModuleID);
                        lblDescription.Text = Server.HtmlDecode(objEvent.EventDesc);
                        lblFee.Text = string.Format("{0:#0.00}", objEvent.EnrollFee);
                        lblNoEnrolees.Text = Convert.ToString(_objEventSignups.NoEnrolees);
                        if (Request.Params["status"].ToLower() == "enrolled")
                        {
                            // User has been successfully enrolled for this event (paid enrollment)
                            ShowMessage(Localization.GetString("lblComplete", LocalResourceFile), MessageLevel.DNNSuccess);
                            lblPurchase.Visible = false;
                            cmdPurchase.Visible = false;
                            cancelButton.Visible = false;
                            cmdReturn.Visible = true;
                        }
                        else if (Request.Params["status"].ToLower() == "cancelled")
                        {
                            // User has been cancelled paid enrollment
                            ShowMessage(Localization.GetString("lblCancel", LocalResourceFile), MessageLevel.DNNWarning);
                            lblPurchase.Visible = false;
                            cmdPurchase.Visible = false;
                            cancelButton.Visible = false;
                            cmdReturn.Visible = true;
                            // Make sure we delete the signup
                            DeleteEnrollment(signupID, objEvent.ModuleID, objEvent.EventID);

                            // Mail users
                            if (Settings.SendEnrollMessageCancelled)
                            {
                                EventEmailInfo objEventEmailInfo = new EventEmailInfo();
                                EventEmails objEventEmail = new EventEmails(PortalId, ModuleId, LocalResourceFile, ((PageBase)Page).PageCulture.Name);
                                objEventEmailInfo.TxtEmailSubject = Settings.Templates.txtEnrollMessageSubject;
                                objEventEmailInfo.TxtEmailBody = Settings.Templates.txtEnrollMessageCancelled;
                                objEventEmailInfo.TxtEmailFrom = Settings.StandardEmail;
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
                                objEventEmailInfo.UserIDs.Add(objEvent.OwnerID);
                                objEventEmail.SendEmails(objEventEmailInfo, objEvent, _objEventSignups);
                            }
                        }
                    }
                    else // security violation attempt to access item not related to this Module
                    {
                        Response.Redirect(GetSocialNavigateUrl(), true);
                    }
                }

                dblTotal = Conversion.Val(lblFee.Text) * Conversion.Val(lblNoEnrolees.Text);
                lblTotal.Text = Strings.Format(dblTotal, "#,##0.00");
                strCurrency = PortalSettings.Currency;
                lblFeeCurrency.Text = strCurrency;
                lblTotalCurrency.Text = strCurrency;
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion

        #region Control Events
        protected void cmdPurchase_Click(System.Object sender, EventArgs e)
        {
            EventInfo objEvent = default(EventInfo);

            try
            {
                if (Page.IsValid)
                {
                    objEvent = _objCtlEvent.EventsGet(_itemID, ModuleId);
                    // User wants to purchase event, create Event Signup Record
                    _objEventSignups = new EventSignupsInfo();

                    //Just in case the user has clicked back and has now clicked Purchase again!!
                    EventSignupsInfo objEventSignupsChk = default(EventSignupsInfo);
                    if (ReferenceEquals(_anonEmail, null))
                    {
                        objEventSignupsChk = _objCtlEventSignups.EventsSignupsGetUser(objEvent.EventID, UserId, objEvent.ModuleID);
                    }
                    else
                    {
                        objEventSignupsChk = _objCtlEventSignups.EventsSignupsGetAnonUser(objEvent.EventID, _anonEmail, objEvent.ModuleID);
                    }
                    if (!ReferenceEquals(objEventSignupsChk, null))
                    {
                        _objEventSignups.SignupID = objEventSignupsChk.SignupID;
                    }
                    _objEventSignups.EventID = objEvent.EventID;
                    _objEventSignups.ModuleID = objEvent.ModuleID;
                    if (ReferenceEquals(_anonEmail, null))
                    {
                        _objEventSignups.UserID = UserId;
                        _objEventSignups.AnonEmail = null;
                        _objEventSignups.AnonName = null;
                        _objEventSignups.AnonTelephone = null;
                        _objEventSignups.AnonCulture = null;
                        _objEventSignups.AnonTimeZoneId = null;
                    }
                    else
                    {
                        Security.PortalSecurity objSecurity = new Security.PortalSecurity();
                        _objEventSignups.UserID = -1;
                        _objEventSignups.AnonEmail = objSecurity.InputFilter(_anonEmail, Security.PortalSecurity.FilterFlag.NoScripting);
                        _objEventSignups.AnonName = objSecurity.InputFilter(_anonName, Security.PortalSecurity.FilterFlag.NoScripting);
                        _objEventSignups.AnonTelephone = objSecurity.InputFilter(_anonTelephone, Security.PortalSecurity.FilterFlag.NoScripting);
                        _objEventSignups.AnonCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
                        _objEventSignups.AnonTimeZoneId = GetDisplayTimeZoneId();
                    }
                    _objEventSignups.PayPalStatus = "none";
                    _objEventSignups.PayPalReason = "PayPal call initiated...";
                    _objEventSignups.PayPalPaymentDate = DateTime.UtcNow;
                    _objEventSignups.Approved = false;
                    _objEventSignups.NoEnrolees = int.Parse(lblNoEnrolees.Text);

                    _objEventSignups = CreateEnrollment(_objEventSignups, objEvent);

                    if (!ReferenceEquals(objEventSignupsChk, null))
                    {
                        _objEventSignups = _objCtlEventSignups.EventsSignupsGet(objEventSignupsChk.SignupID, objEventSignupsChk.ModuleID, false);
                    }

                    // Mail users
                    if (Settings.SendEnrollMessagePaying)
                    {
                        EventEmailInfo objEventEmailInfo = new EventEmailInfo();
                        EventEmails objEventEmail = new EventEmails(PortalId, ModuleId, LocalResourceFile, ((PageBase)Page).PageCulture.Name);
                        objEventEmailInfo.TxtEmailSubject = Settings.Templates.txtEnrollMessageSubject;
                        objEventEmailInfo.TxtEmailBody = Settings.Templates.txtEnrollMessagePaying;
                        objEventEmailInfo.TxtEmailFrom = Settings.StandardEmail;
                        if (ReferenceEquals(_anonEmail, null))
                        {
                            objEventEmailInfo.UserEmails.Add(PortalSettings.UserInfo.Email);
                            objEventEmailInfo.UserLocales.Add(PortalSettings.UserInfo.Profile.PreferredLocale);
                            objEventEmailInfo.UserTimeZoneIds.Add(PortalSettings.UserInfo.Profile.PreferredTimeZone.Id);
                        }
                        else
                        {
                            objEventEmailInfo.UserEmails.Add(_objEventSignups.AnonEmail);
                            objEventEmailInfo.UserLocales.Add(_objEventSignups.AnonCulture);
                            objEventEmailInfo.UserTimeZoneIds.Add(_objEventSignups.AnonTimeZoneId);
                        }
                        objEventEmailInfo.UserIDs.Add(objEvent.OwnerID);
                        objEventEmail.SendEmails(objEventEmailInfo, objEvent, _objEventSignups);
                    }

                    // build PayPal URL
                    string ppurl = Settings.Paypalurl + "/cgi-bin/webscr?cmd=_xclick&business=";

                    int socialGroupId = GetUrlGroupId();

                    EventInfoHelper objEventInfoHelper = new EventInfoHelper(ModuleId, TabId, PortalId, Settings);
                    string returnURL = "";
                    if (socialGroupId > 0)
                    {
                        returnURL = objEventInfoHelper.AddSkinContainerControls(DotNetNuke.Common.Globals.NavigateURL(TabId, "PPEnroll", "Mid=" + System.Convert.ToString(ModuleId), "signupid=" + System.Convert.ToString(_objEventSignups.SignupID), "status=enrolled", "groupid=" + socialGroupId.ToString()), "?");
                    }
                    else
                    {
                        returnURL = objEventInfoHelper.AddSkinContainerControls(DotNetNuke.Common.Globals.NavigateURL(TabId, "PPEnroll", "Mid=" + System.Convert.ToString(ModuleId), "signupid=" + System.Convert.ToString(_objEventSignups.SignupID), "status=enrolled"), "?");
                    }
                    if (returnURL.IndexOf("://") + 1 == 0)
                    {
                        returnURL = DotNetNuke.Common.Globals.AddHTTP(DotNetNuke.Common.Globals.GetDomainName(Request)) + returnURL;
                    }
                    string cancelURL = "";
                    if (socialGroupId > 0)
                    {
                        cancelURL = objEventInfoHelper.AddSkinContainerControls(DotNetNuke.Common.Globals.NavigateURL(TabId, "PPEnroll", "Mid=" + System.Convert.ToString(ModuleId), "signupid=" + System.Convert.ToString(_objEventSignups.SignupID), "status=cancelled", "groupid=" + socialGroupId.ToString()), "?");
                    }
                    else
                    {
                        cancelURL = objEventInfoHelper.AddSkinContainerControls(DotNetNuke.Common.Globals.NavigateURL(TabId, "PPEnroll", "Mid=" + System.Convert.ToString(ModuleId), "signupid=" + System.Convert.ToString(_objEventSignups.SignupID), "status=cancelled"), "?");
                    }
                    if (cancelURL.IndexOf("://") + 1 == 0)
                    {
                        cancelURL = DotNetNuke.Common.Globals.AddHTTP(DotNetNuke.Common.Globals.GetDomainName(Request)) + cancelURL;
                    }
                    string strPayPalURL = "";
                    strPayPalURL = ppurl + DotNetNuke.Common.Globals.HTTPPOSTEncode(objEvent.PayPalAccount);
                    strPayPalURL = strPayPalURL + "&item_name=" + DotNetNuke.Common.Globals.HTTPPOSTEncode(objEvent.ModuleTitle + " - " + lblEventName.Text + " ( " + lblFee.Text + " " + lblFeeCurrency.Text + " )");
                    strPayPalURL = strPayPalURL + "&item_number=" + DotNetNuke.Common.Globals.HTTPPOSTEncode(System.Convert.ToString(_objEventSignups.SignupID));
                    strPayPalURL = strPayPalURL + "&quantity=" + DotNetNuke.Common.Globals.HTTPPOSTEncode(Convert.ToString(_objEventSignups.NoEnrolees));
                    strPayPalURL = strPayPalURL + "&custom=" + DotNetNuke.Common.Globals.HTTPPOSTEncode(System.Convert.ToDateTime(lblStartDate.Text).ToShortDateString());

                    // Make sure currency is in correct format
                    double dblFee = double.Parse(lblFee.Text);
                    CultureInfo uiculture = System.Threading.Thread.CurrentThread.CurrentCulture;
                    System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                    strPayPalURL = strPayPalURL + "&amount=" + DotNetNuke.Common.Globals.HTTPPOSTEncode(Strings.Format(dblFee, "#,##0.00"));
                    System.Threading.Thread.CurrentThread.CurrentCulture = uiculture;

                    strPayPalURL = strPayPalURL + "&currency_code=" + DotNetNuke.Common.Globals.HTTPPOSTEncode(lblTotalCurrency.Text);
                    strPayPalURL = strPayPalURL + "&return=" + returnURL;
                    strPayPalURL = strPayPalURL + "&cancel_return=" + cancelURL;
                    strPayPalURL = strPayPalURL + "&notify_url=" + DotNetNuke.Common.Globals.HTTPPOSTEncode(DotNetNuke.Common.Globals.AddHTTP(DotNetNuke.Common.Globals.GetDomainName(Request)) + "/DesktopModules/Events/EventIPN.aspx");
                    strPayPalURL = strPayPalURL + "&undefined_quantity=&no_note=1&no_shipping=1";
                    //strPayPalURL = strPayPalURL & "&undefined_quantity=&no_note=1&no_shipping=1&rm=2"

                    // redirect to PayPal
                    Response.Redirect(strPayPalURL, true);
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cancelButton_Click(System.Object sender, EventArgs e)
        {
            try
            {
                Response.Redirect(GetSocialNavigateUrl(), true);
            }
            catch (Exception) //Module failed to load
            {
                //ProcessModuleLoadException(Me, exc)
            }
        }

        protected void cmdReturn_Click(System.Object sender, EventArgs e)
        {
            try
            {
                Response.Redirect(GetSocialNavigateUrl(), true);
            }
            catch (Exception) //Module failed to load
            {
                //ProcessModuleLoadException(Me, exc)
            }
        }

        #endregion
        #region Support Functions
        private void ShowMessage(string msg, MessageLevel messageLevel)
        {
            lblMessage.Text = msg;

            //Hide the rest of the form fields.
            divMessage.Attributes.Add("style", "display:block;");

            switch (messageLevel)
            {
                case MessageLevel.DNNSuccess:
                    divMessage.Attributes.Add("class", "dnnFormMessage dnnFormSuccess");
                    break;
                case MessageLevel.DNNInformation:
                    divMessage.Attributes.Add("class", "dnnFormMessage dnnFormInfo");
                    break;
                case MessageLevel.DNNWarning:
                    divMessage.Attributes.Add("class", "dnnFormMessage dnnFormWarning");
                    break;
                case MessageLevel.DNNError:
                    divMessage.Attributes.Add("class", "dnnFormMessage dnnFormValidationSummary");
                    break;
            }
        }

        #endregion


    }

}
