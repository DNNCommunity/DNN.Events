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
    using System.Globalization;
    using System.Threading;
    using System.Web;
    using DotNetNuke.Framework;
    using DotNetNuke.Security;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Localization;
    using global::Components;
    using Microsoft.VisualBasic;
    using Globals = DotNetNuke.Common.Globals;

    [DNNtc.ModuleControlProperties("PPEnroll", "Event PayPal Enrollment", DNNtc.ControlType.View, "https://dnnevents.codeplex.com/documentation", false, true)]
    public partial class EventPPEnroll : EventBase
    {
        #region Event Handlers

        private void Page_Load(object sender, EventArgs e)
        {
            try
            {
                double dblTotal = 0;
                var strCurrency = "";

                if (!ReferenceEquals(this.Request.Params["ItemID"], null))
                {
                    this._itemID = int.Parse(this.Request.Params["ItemID"]);
                }
                if (!ReferenceEquals(this.Request.Params["AnonEmail"], null))
                {
                    this._anonEmail = HttpUtility.UrlDecode(this.Request.Params["AnonEmail"]);
                }
                if (!ReferenceEquals(this.Request.Params["AnonName"], null))
                {
                    this._anonName = HttpUtility.UrlDecode(this.Request.Params["AnonName"]);
                }
                if (!ReferenceEquals(this.Request.Params["AnonPhone"], null))
                {
                    this._anonTelephone = HttpUtility.UrlDecode(this.Request.Params["AnonPhone"]);
                    if (this._anonTelephone == "0")
                    {
                        this._anonTelephone = "";
                    }
                }

                // Set the selected theme
                this.SetTheme(this.pnlEventsModulePayPal);

                this.divMessage.Attributes.Add("style", "display:none;");

                if (!this.Page.IsPostBack)
                {
                    if (this._itemID != -1)
                    {
                        if (!ReferenceEquals(this.Request.Params["NoEnrol"], null))
                        {
                            this._noEnrol = int.Parse(this.Request.Params["NoEnrol"]);
                        }

                        var objEvent = default(EventInfo);
                        objEvent = this._objCtlEvent.EventsGet(this._itemID, this.ModuleId);

                        //  Compute Dates/Times (for recurring)
                        var startdate = objEvent.EventTimeBegin;
                        this.SelectedDate = startdate.Date;
                        var d = default(DateTime);
                        d = startdate.Date.AddMinutes(objEvent.EventTimeBegin.TimeOfDay.TotalMinutes);
                        this.lblStartDate.Text = d.ToLongDateString() + " " + d.ToShortTimeString();
                        this.lblEventName.Text = objEvent.EventName;
                        this.lblDescription.Text = this.Server.HtmlDecode(objEvent.EventDesc);
                        this.lblFee.Text = string.Format("{0:#0.00}", objEvent.EnrollFee);
                        this.lblNoEnrolees.Text = Convert.ToString(this._noEnrol);
                        this.lblPurchase.Text = Localization.GetString("lblPurchase", this.LocalResourceFile);
                        this.lblPurchase.Visible = true;
                    }
                    else if (!ReferenceEquals(this.Request.Params["signupid"], null))
                    {
                        // Get EventSignup
                        this._objEventSignups = new EventSignupsInfo();
                        var signupID = Convert.ToInt32(this.Request.Params["signupid"]);
                        this._objEventSignups =
                            this._objCtlEventSignups.EventsSignupsGet(signupID, this.ModuleId, false);
                        this.lblStartDate.Text = this._objEventSignups.EventTimeBegin.ToLongDateString() + " " +
                                                 this._objEventSignups.EventTimeBegin.ToShortTimeString();
                        this.lblEventName.Text = this._objEventSignups.EventName;
                        // Get Related Event
                        var objEvent = default(EventInfo);
                        objEvent = this._objCtlEvent.EventsGet(this._objEventSignups.EventID,
                                                               this._objEventSignups.ModuleID);
                        this.lblDescription.Text = this.Server.HtmlDecode(objEvent.EventDesc);
                        this.lblFee.Text = string.Format("{0:#0.00}", objEvent.EnrollFee);
                        this.lblNoEnrolees.Text = Convert.ToString(this._objEventSignups.NoEnrolees);
                        if (this.Request.Params["status"].ToLower() == "enrolled")
                        {
                            // User has been successfully enrolled for this event (paid enrollment)
                            this.ShowMessage(Localization.GetString("lblComplete", this.LocalResourceFile),
                                             MessageLevel.DNNSuccess);
                            this.lblPurchase.Visible = false;
                            this.cmdPurchase.Visible = false;
                            this.cancelButton.Visible = false;
                            this.cmdReturn.Visible = true;
                        }
                        else if (this.Request.Params["status"].ToLower() == "cancelled")
                        {
                            // User has been cancelled paid enrollment
                            this.ShowMessage(Localization.GetString("lblCancel", this.LocalResourceFile),
                                             MessageLevel.DNNWarning);
                            this.lblPurchase.Visible = false;
                            this.cmdPurchase.Visible = false;
                            this.cancelButton.Visible = false;
                            this.cmdReturn.Visible = true;
                            // Make sure we delete the signup
                            this.DeleteEnrollment(signupID, objEvent.ModuleID, objEvent.EventID);

                            // Mail users
                            if (this.Settings.SendEnrollMessageCancelled)
                            {
                                var objEventEmailInfo = new EventEmailInfo();
                                var objEventEmail =
                                    new EventEmails(this.PortalId, this.ModuleId, this.LocalResourceFile,
                                                    ((PageBase) this.Page).PageCulture.Name);
                                objEventEmailInfo.TxtEmailSubject = this.Settings.Templates.txtEnrollMessageSubject;
                                objEventEmailInfo.TxtEmailBody = this.Settings.Templates.txtEnrollMessageCancelled;
                                objEventEmailInfo.TxtEmailFrom = this.Settings.StandardEmail;
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
                                objEventEmailInfo.UserIDs.Add(objEvent.OwnerID);
                                objEventEmail.SendEmails(objEventEmailInfo, objEvent, this._objEventSignups);
                            }
                        }
                    }
                    else // security violation attempt to access item not related to this Module
                    {
                        this.Response.Redirect(this.GetSocialNavigateUrl(), true);
                    }
                }

                dblTotal = Conversion.Val(this.lblFee.Text) * Conversion.Val(this.lblNoEnrolees.Text);
                this.lblTotal.Text = Strings.Format(dblTotal, "#,##0.00");
                strCurrency = this.PortalSettings.Currency;
                this.lblFeeCurrency.Text = strCurrency;
                this.lblTotalCurrency.Text = strCurrency;
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion

        #region Support Functions

        private void ShowMessage(string msg, MessageLevel messageLevel)
        {
            this.lblMessage.Text = msg;

            //Hide the rest of the form fields.
            this.divMessage.Attributes.Add("style", "display:block;");

            switch (messageLevel)
            {
                case MessageLevel.DNNSuccess:
                    this.divMessage.Attributes.Add("class", "dnnFormMessage dnnFormSuccess");
                    break;
                case MessageLevel.DNNInformation:
                    this.divMessage.Attributes.Add("class", "dnnFormMessage dnnFormInfo");
                    break;
                case MessageLevel.DNNWarning:
                    this.divMessage.Attributes.Add("class", "dnnFormMessage dnnFormWarning");
                    break;
                case MessageLevel.DNNError:
                    this.divMessage.Attributes.Add("class", "dnnFormMessage dnnFormValidationSummary");
                    break;
            }
        }

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

        #region Private Area

        private int _itemID = -1;
        private int _noEnrol = 1;
        private readonly EventController _objCtlEvent = new EventController();
        private EventSignupsInfo _objEventSignups = new EventSignupsInfo();
        private readonly EventSignupsController _objCtlEventSignups = new EventSignupsController();
        private string _anonEmail;
        private string _anonName;
        private string _anonTelephone;

        private enum MessageLevel
        {
            DNNSuccess = 1,
            DNNInformation,
            DNNWarning,
            DNNError
        }

        #endregion

        #region Control Events

        protected void cmdPurchase_Click(object sender, EventArgs e)
        {
            var objEvent = default(EventInfo);

            try
            {
                if (this.Page.IsValid)
                {
                    objEvent = this._objCtlEvent.EventsGet(this._itemID, this.ModuleId);
                    // User wants to purchase event, create Event Signup Record
                    this._objEventSignups = new EventSignupsInfo();

                    //Just in case the user has clicked back and has now clicked Purchase again!!
                    var objEventSignupsChk = default(EventSignupsInfo);
                    if (ReferenceEquals(this._anonEmail, null))
                    {
                        objEventSignupsChk =
                            this._objCtlEventSignups.EventsSignupsGetUser(
                                objEvent.EventID, this.UserId, objEvent.ModuleID);
                    }
                    else
                    {
                        objEventSignupsChk =
                            this._objCtlEventSignups.EventsSignupsGetAnonUser(
                                objEvent.EventID, this._anonEmail, objEvent.ModuleID);
                    }
                    if (!ReferenceEquals(objEventSignupsChk, null))
                    {
                        this._objEventSignups.SignupID = objEventSignupsChk.SignupID;
                    }
                    this._objEventSignups.EventID = objEvent.EventID;
                    this._objEventSignups.ModuleID = objEvent.ModuleID;
                    if (ReferenceEquals(this._anonEmail, null))
                    {
                        this._objEventSignups.UserID = this.UserId;
                        this._objEventSignups.AnonEmail = null;
                        this._objEventSignups.AnonName = null;
                        this._objEventSignups.AnonTelephone = null;
                        this._objEventSignups.AnonCulture = null;
                        this._objEventSignups.AnonTimeZoneId = null;
                    }
                    else
                    {
                        var objSecurity = new PortalSecurity();
                        this._objEventSignups.UserID = -1;
                        this._objEventSignups.AnonEmail =
                            objSecurity.InputFilter(this._anonEmail, PortalSecurity.FilterFlag.NoScripting);
                        this._objEventSignups.AnonName =
                            objSecurity.InputFilter(this._anonName, PortalSecurity.FilterFlag.NoScripting);
                        this._objEventSignups.AnonTelephone =
                            objSecurity.InputFilter(this._anonTelephone, PortalSecurity.FilterFlag.NoScripting);
                        this._objEventSignups.AnonCulture = Thread.CurrentThread.CurrentCulture.Name;
                        this._objEventSignups.AnonTimeZoneId = this.GetDisplayTimeZoneId();
                    }
                    this._objEventSignups.PayPalStatus = "none";
                    this._objEventSignups.PayPalReason = "PayPal call initiated...";
                    this._objEventSignups.PayPalPaymentDate = DateTime.UtcNow;
                    this._objEventSignups.Approved = false;
                    this._objEventSignups.NoEnrolees = int.Parse(this.lblNoEnrolees.Text);

                    this._objEventSignups = this.CreateEnrollment(this._objEventSignups, objEvent);

                    if (!ReferenceEquals(objEventSignupsChk, null))
                    {
                        this._objEventSignups =
                            this._objCtlEventSignups.EventsSignupsGet(objEventSignupsChk.SignupID,
                                                                      objEventSignupsChk.ModuleID, false);
                    }

                    // Mail users
                    if (this.Settings.SendEnrollMessagePaying)
                    {
                        var objEventEmailInfo = new EventEmailInfo();
                        var objEventEmail = new EventEmails(this.PortalId, this.ModuleId, this.LocalResourceFile,
                                                            ((PageBase) this.Page).PageCulture.Name);
                        objEventEmailInfo.TxtEmailSubject = this.Settings.Templates.txtEnrollMessageSubject;
                        objEventEmailInfo.TxtEmailBody = this.Settings.Templates.txtEnrollMessagePaying;
                        objEventEmailInfo.TxtEmailFrom = this.Settings.StandardEmail;
                        if (ReferenceEquals(this._anonEmail, null))
                        {
                            objEventEmailInfo.UserEmails.Add(this.PortalSettings.UserInfo.Email);
                            objEventEmailInfo.UserLocales.Add(this.PortalSettings.UserInfo.Profile.PreferredLocale);
                            objEventEmailInfo.UserTimeZoneIds.Add(this.PortalSettings.UserInfo.Profile.PreferredTimeZone
                                                                      .Id);
                        }
                        else
                        {
                            objEventEmailInfo.UserEmails.Add(this._objEventSignups.AnonEmail);
                            objEventEmailInfo.UserLocales.Add(this._objEventSignups.AnonCulture);
                            objEventEmailInfo.UserTimeZoneIds.Add(this._objEventSignups.AnonTimeZoneId);
                        }
                        objEventEmailInfo.UserIDs.Add(objEvent.OwnerID);
                        objEventEmail.SendEmails(objEventEmailInfo, objEvent, this._objEventSignups);
                    }

                    // build PayPal URL
                    var ppurl = this.Settings.Paypalurl + "/cgi-bin/webscr?cmd=_xclick&business=";

                    var socialGroupId = this.GetUrlGroupId();

                    var objEventInfoHelper =
                        new EventInfoHelper(this.ModuleId, this.TabId, this.PortalId, this.Settings);
                    var returnURL = "";
                    if (socialGroupId > 0)
                    {
                        returnURL = objEventInfoHelper.AddSkinContainerControls(
                            Globals.NavigateURL(this.TabId, "PPEnroll", "Mid=" + Convert.ToString(this.ModuleId),
                                                "signupid=" + Convert.ToString(this._objEventSignups.SignupID),
                                                "status=enrolled", "groupid=" + socialGroupId), "?");
                    }
                    else
                    {
                        returnURL = objEventInfoHelper.AddSkinContainerControls(
                            Globals.NavigateURL(this.TabId, "PPEnroll", "Mid=" + Convert.ToString(this.ModuleId),
                                                "signupid=" + Convert.ToString(this._objEventSignups.SignupID),
                                                "status=enrolled"), "?");
                    }
                    if (returnURL.IndexOf("://") + 1 == 0)
                    {
                        returnURL = Globals.AddHTTP(Globals.GetDomainName(this.Request)) + returnURL;
                    }
                    var cancelURL = "";
                    if (socialGroupId > 0)
                    {
                        cancelURL = objEventInfoHelper.AddSkinContainerControls(
                            Globals.NavigateURL(this.TabId, "PPEnroll", "Mid=" + Convert.ToString(this.ModuleId),
                                                "signupid=" + Convert.ToString(this._objEventSignups.SignupID),
                                                "status=cancelled", "groupid=" + socialGroupId), "?");
                    }
                    else
                    {
                        cancelURL = objEventInfoHelper.AddSkinContainerControls(
                            Globals.NavigateURL(this.TabId, "PPEnroll", "Mid=" + Convert.ToString(this.ModuleId),
                                                "signupid=" + Convert.ToString(this._objEventSignups.SignupID),
                                                "status=cancelled"), "?");
                    }
                    if (cancelURL.IndexOf("://") + 1 == 0)
                    {
                        cancelURL = Globals.AddHTTP(Globals.GetDomainName(this.Request)) + cancelURL;
                    }
                    var strPayPalURL = "";
                    strPayPalURL = ppurl + Globals.HTTPPOSTEncode(objEvent.PayPalAccount);
                    strPayPalURL = strPayPalURL + "&item_name=" +
                                   Globals.HTTPPOSTEncode(objEvent.ModuleTitle + " - " + this.lblEventName.Text +
                                                          " ( " + this.lblFee.Text + " " + this.lblFeeCurrency.Text +
                                                          " )");
                    strPayPalURL = strPayPalURL + "&item_number=" +
                                   Globals.HTTPPOSTEncode(Convert.ToString(this._objEventSignups.SignupID));
                    strPayPalURL = strPayPalURL + "&quantity=" +
                                   Globals.HTTPPOSTEncode(Convert.ToString(this._objEventSignups.NoEnrolees));
                    strPayPalURL = strPayPalURL + "&custom=" +
                                   Globals.HTTPPOSTEncode(
                                       Convert.ToDateTime(this.lblStartDate.Text).ToShortDateString());

                    // Make sure currency is in correct format
                    var dblFee = double.Parse(this.lblFee.Text);
                    var uiculture = Thread.CurrentThread.CurrentCulture;
                    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                    strPayPalURL = strPayPalURL + "&amount=" +
                                   Globals.HTTPPOSTEncode(Strings.Format(dblFee, "#,##0.00"));
                    Thread.CurrentThread.CurrentCulture = uiculture;

                    strPayPalURL = strPayPalURL + "&currency_code=" +
                                   Globals.HTTPPOSTEncode(this.lblTotalCurrency.Text);
                    strPayPalURL = strPayPalURL + "&return=" + returnURL;
                    strPayPalURL = strPayPalURL + "&cancel_return=" + cancelURL;
                    strPayPalURL = strPayPalURL + "&notify_url=" +
                                   Globals.HTTPPOSTEncode(Globals.AddHTTP(Globals.GetDomainName(this.Request)) +
                                                          "/DesktopModules/Events/EventIPN.aspx");
                    strPayPalURL = strPayPalURL + "&undefined_quantity=&no_note=1&no_shipping=1";
                    //strPayPalURL = strPayPalURL & "&undefined_quantity=&no_note=1&no_shipping=1&rm=2"

                    // redirect to PayPal
                    this.Response.Redirect(strPayPalURL, true);
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cancelButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.Response.Redirect(this.GetSocialNavigateUrl(), true);
            }
            catch (Exception) //Module failed to load
            {
                //ProcessModuleLoadException(Me, exc)
            }
        }

        protected void cmdReturn_Click(object sender, EventArgs e)
        {
            try
            {
                this.Response.Redirect(this.GetSocialNavigateUrl(), true);
            }
            catch (Exception) //Module failed to load
            {
                //ProcessModuleLoadException(Me, exc)
            }
        }

        #endregion
    }
}