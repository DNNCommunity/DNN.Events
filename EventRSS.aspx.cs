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
    using System.Collections;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Web;
    using System.Web.UI;
    using System.Xml;
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Entities.Users;
    using DotNetNuke.Security;
    using DotNetNuke.Services.Localization;
    using global::Components;
    using Microsoft.VisualBasic;
    using Globals = DotNetNuke.Common.Globals;

    public partial class EventRSS : Page
    {
        #region Private Variables

        private int _moduleID;
        private int _tabID;
        private int _portalID;
        private EventModuleSettings _settings;
        private UserInfo _userinfo;
        private const string NsPre = "e";
        private const string NsFull = "http://www.dnnsoftware.com/dnnevents";

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

        #region Event Handlers

        private void Page_Load(object sender, EventArgs e)
        {
            var iDaysBefore = 0;
            var iDaysAfter = 0;
            var iMax = 0;
            var iOwnerID = 0;
            var iLocationID = 0;
            var iImportance = 0;
            var categoryIDs = new ArrayList();
            var locationIDs = new ArrayList();
            var iGroupId = -1;
            var iUserId = -1;
            var iCategoryName = "";
            var iLocationName = "";
            var iOwnerName = "";
            var txtPriority = "";
            if (!(HttpContext.Current.Request.QueryString["Mid"] == ""))
            {
                this._moduleID = Convert.ToInt32(HttpContext.Current.Request.QueryString["mid"]);
            }
            else
            {
                this.Response.Redirect(Globals.NavigateURL(), true);
            }
            if (!(HttpContext.Current.Request.QueryString["tabid"] == ""))
            {
                this._tabID = Convert.ToInt32(HttpContext.Current.Request.QueryString["tabid"]);
            }
            else
            {
                this.Response.Redirect(Globals.NavigateURL(), true);
            }

            var localResourceFile = this.TemplateSourceDirectory + "/" + Localization.LocalResourceDirectory +
                                    "/EventRSS.aspx.resx";

            if (!(HttpContext.Current.Request.QueryString["CategoryName"] == ""))
            {
                iCategoryName = HttpContext.Current.Request.QueryString["CategoryName"];
                var objSecurity = new PortalSecurity();
                iCategoryName = objSecurity.InputFilter(iCategoryName, PortalSecurity.FilterFlag.NoSQL);
            }
            if (!(HttpContext.Current.Request.QueryString["CategoryID"] == ""))
            {
                categoryIDs.Add(Convert.ToInt32(HttpContext.Current.Request.QueryString["CategoryID"]));
            }
            if (!(HttpContext.Current.Request.QueryString["LocationName"] == ""))
            {
                iLocationName = HttpContext.Current.Request.QueryString["LocationName"];
                var objSecurity = new PortalSecurity();
                iLocationName = objSecurity.InputFilter(iLocationName, PortalSecurity.FilterFlag.NoSQL);
            }
            if (!(HttpContext.Current.Request.QueryString["LocationID"] == ""))
            {
                locationIDs.Add(Convert.ToInt32(HttpContext.Current.Request.QueryString["LocationID"]));
            }
            if (!(HttpContext.Current.Request.QueryString["groupid"] == ""))
            {
                iGroupId = Convert.ToInt32(HttpContext.Current.Request.QueryString["groupid"]);
            }
            if (!(HttpContext.Current.Request.QueryString["DaysBefore"] == ""))
            {
                iDaysBefore = Convert.ToInt32(HttpContext.Current.Request.QueryString["DaysBefore"]);
            }
            if (!(HttpContext.Current.Request.QueryString["DaysAfter"] == ""))
            {
                iDaysAfter = Convert.ToInt32(HttpContext.Current.Request.QueryString["DaysAfter"]);
            }
            if (!(HttpContext.Current.Request.QueryString["MaxNumber"] == ""))
            {
                iMax = Convert.ToInt32(HttpContext.Current.Request.QueryString["MaxNumber"]);
            }
            if (!(HttpContext.Current.Request.QueryString["OwnerName"] == ""))
            {
                iOwnerName = HttpContext.Current.Request.QueryString["OwnerName"];
            }
            if (!(HttpContext.Current.Request.QueryString["OwnerID"] == ""))
            {
                iOwnerID = Convert.ToInt32(HttpContext.Current.Request.QueryString["OwnerID"]);
            }
            if (!(HttpContext.Current.Request.QueryString["LocationName"] == ""))
            {
                iLocationName = HttpContext.Current.Request.QueryString["LocationName"];
            }
            if (!(HttpContext.Current.Request.QueryString["LocationID"] == ""))
            {
                iLocationID = Convert.ToInt32(HttpContext.Current.Request.QueryString["LocationID"]);
            }
            if (!(HttpContext.Current.Request.QueryString["Priority"] == ""))
            {
                var iPriority = "";
                iPriority = HttpContext.Current.Request.QueryString["Priority"];
                var lHigh = "";
                var lMedium = "";
                var lLow = "";
                lHigh = Localization.GetString("High", localResourceFile);
                lMedium = Localization.GetString("Normal", localResourceFile);
                lLow = Localization.GetString("Low", localResourceFile);

                txtPriority = "Medium";
                if (iPriority == lHigh)
                {
                    txtPriority = "High";
                }
                else if (iPriority == lMedium)
                {
                    txtPriority = "Medium";
                }
                else if (iPriority == lLow)
                {
                    txtPriority = "Low";
                }
                else if (iPriority == "High")
                {
                    txtPriority = "High";
                }
                else if (iPriority == "Normal")
                {
                    txtPriority = "Medium";
                }
                else if (iPriority == "Low")
                {
                    txtPriority = "Low";
                }
            }
            if (!(HttpContext.Current.Request.QueryString["Importance"] == ""))
            {
                iImportance = Convert.ToInt32(HttpContext.Current.Request.QueryString["Importance"]);
            }

            var portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
            this._portalID = portalSettings.PortalId;
            this._userinfo = (UserInfo) HttpContext.Current.Items["UserInfo"];
            if (!ReferenceEquals(portalSettings, null))
            {
                if (portalSettings.DefaultLanguage != "")
                {
                    var userculture = new CultureInfo(portalSettings.DefaultLanguage, false);
                    Thread.CurrentThread.CurrentCulture = userculture;
                }
            }
            if (this._userinfo.UserID > 0)
            {
                if (this._userinfo.Profile.PreferredLocale != "")
                {
                    var userculture = new CultureInfo(this._userinfo.Profile.PreferredLocale, false);
                    Thread.CurrentThread.CurrentCulture = userculture;
                }
            }

            this._settings = EventModuleSettings.GetEventModuleSettings(this._moduleID, localResourceFile);

            if (this._settings.Enablecategories == EventModuleSettings.DisplayCategories.DoNotDisplay)
            {
                categoryIDs = this._settings.ModuleCategoryIDs;
                iCategoryName = "";
            }
            if (!string.IsNullOrEmpty(iCategoryName))
            {
                var oCntrlEventCategory = new EventCategoryController();
                var oEventCategory = oCntrlEventCategory.EventCategoryGetByName(iCategoryName, this._portalID);
                if (!ReferenceEquals(oEventCategory, null))
                {
                    categoryIDs.Add(oEventCategory.Category);
                }
            }
            if (this._settings.Enablelocations == EventModuleSettings.DisplayLocations.DoNotDisplay)
            {
                locationIDs = this._settings.ModuleLocationIDs;
                iLocationName = "";
            }
            if (!string.IsNullOrEmpty(iLocationName))
            {
                var oCntrlEventLocation = new EventLocationController();
                var oEventLocation = oCntrlEventLocation.EventsLocationGetByName(iLocationName, this._portalID);
                if (!ReferenceEquals(oEventLocation, null))
                {
                    locationIDs.Add(oEventLocation.Location);
                }
            }

            if (!this._settings.RSSEnable)
            {
                this.Response.Redirect(Globals.NavigateURL(), true);
            }

            if (this._settings.SocialGroupModule == EventModuleSettings.SocialModule.UserProfile)
            {
                iUserId = this._userinfo.UserID;
            }
            var getSubEvents = this._settings.MasterEvent;

            var dtEndDate = default(DateTime);
            if (HttpContext.Current.Request.QueryString["DaysAfter"] == "" &&
                HttpContext.Current.Request.QueryString["DaysBefore"] == "")
            {
                iDaysAfter = this._settings.RSSDays;
            }
            var objEventTimeZoneUtilities = new EventTimeZoneUtilities();
            var currDate =
                objEventTimeZoneUtilities.ConvertFromUTCToModuleTimeZone(DateTime.UtcNow, this._settings.TimeZoneId);

            dtEndDate = DateAndTime.DateAdd(DateInterval.Day, iDaysAfter, currDate).Date;

            var dtStartDate = default(DateTime);
            dtStartDate = DateAndTime.DateAdd(DateInterval.Day, Convert.ToDouble(-iDaysBefore), currDate).Date;

            var txtFeedRootTitle = "";
            var txtFeedRootDescription = "";
            var txtRSSDateField = "";
            txtFeedRootTitle = this._settings.RSSTitle;
            txtFeedRootDescription = this._settings.RSSDesc;
            txtRSSDateField = this._settings.RSSDateField;

            this.Response.ContentType = "text/xml";
            this.Response.ContentEncoding = Encoding.UTF8;


            using (var sw = new StringWriter())
            {
                using (var writer = new XmlTextWriter(sw))
                {
                    //                Dim writer As XmlTextWriter = New XmlTextWriter(sw)
                    writer.Formatting = Formatting.Indented;
                    writer.Indentation = 4;

                    writer.WriteStartElement("rss");
                    writer.WriteAttributeString("version", "2.0");
                    writer.WriteAttributeString("xmlns:wfw", "http://wellformedweb.org/CommentAPI/");
                    writer.WriteAttributeString("xmlns:slash", "http://purl.org/rss/1.0/modules/slash/");
                    writer.WriteAttributeString("xmlns:dc", "http://purl.org/dc/elements/1.1/");
                    writer.WriteAttributeString("xmlns:trackback",
                                                "http://madskills.com/public/xml/rss/module/trackback/");
                    writer.WriteAttributeString("xmlns:atom", "http://www.w3.org/2005/Atom");
                    writer.WriteAttributeString("xmlns", NsPre, null, NsFull);

                    writer.WriteStartElement("channel");

                    writer.WriteStartElement("atom:link");
                    writer.WriteAttributeString("href", HttpContext.Current.Request.Url.AbsoluteUri);
                    writer.WriteAttributeString("rel", "self");
                    writer.WriteAttributeString("type", "application/rss+xml");
                    writer.WriteEndElement();

                    writer.WriteElementString("title", portalSettings.PortalName + " - " + txtFeedRootTitle);

                    if (portalSettings.PortalAlias.HTTPAlias.IndexOf("http://", StringComparison.Ordinal) == -1 &&
                        portalSettings.PortalAlias.HTTPAlias.IndexOf("https://", StringComparison.Ordinal) == -1)
                    {
                        writer.WriteElementString("link", Globals.AddHTTP(portalSettings.PortalAlias.HTTPAlias));
                    }
                    else
                    {
                        writer.WriteElementString("link", portalSettings.PortalAlias.HTTPAlias);
                    }

                    writer.WriteElementString("description", txtFeedRootDescription);
                    writer.WriteElementString("ttl", "60");

                    var objEventInfoHelper =
                        new EventInfoHelper(this._moduleID, this._tabID, this._portalID, this._settings);
                    var lstEvents = default(ArrayList);
                    var tcc = new TokenReplaceControllerClass(this._moduleID, localResourceFile);
                    var tmpTitle = this._settings.Templates.txtRSSTitle;
                    var tmpDescription = this._settings.Templates.txtRSSDescription;
                    if (categoryIDs.Count == 0)
                    {
                        categoryIDs.Add("-1");
                    }
                    if (locationIDs.Count == 0)
                    {
                        locationIDs.Add("-1");
                    }

                    lstEvents = objEventInfoHelper.GetEvents(dtStartDate, dtEndDate, getSubEvents, categoryIDs,
                                                             locationIDs, iGroupId, iUserId);

                    var objEventBase = new EventBase();
                    var displayTimeZoneId = objEventBase.GetDisplayTimeZoneId(this._settings, this._portalID);

                    var rssCount = 0;
                    foreach (EventInfo eventInfo in lstEvents)
                    {
                        var objEvent = eventInfo;

                        if ((Convert.ToInt32(categoryIDs[0]) == 0) &
                            (objEvent.Category != Convert.ToInt32(categoryIDs[0])))
                        {
                            continue;
                        }
                        if ((Convert.ToInt32(locationIDs[0]) == 0) &
                            (objEvent.Location != Convert.ToInt32(locationIDs[0])))
                        {
                            continue;
                        }
                        if ((iOwnerID > 0) & (objEvent.OwnerID != iOwnerID))
                        {
                            continue;
                        }
                        if (!string.IsNullOrEmpty(iOwnerName) && objEvent.OwnerName != iOwnerName)
                        {
                            continue;
                        }
                        if ((iLocationID > 0) & (objEvent.Location != iLocationID))
                        {
                            continue;
                        }
                        if (!string.IsNullOrEmpty(iLocationName) && objEvent.LocationName != iLocationName)
                        {
                            continue;
                        }
                        if (iImportance > 0 && (int) objEvent.Importance != iImportance)
                        {
                            continue;
                        }
                        if (!string.IsNullOrEmpty(txtPriority) && objEvent.Importance.ToString() != txtPriority)
                        {
                            continue;
                        }

                        // If full enrollments should be hidden, ignore
                        if (this.HideFullEvent(objEvent))
                        {
                            continue;
                        }

                        var pubDate = default(DateTime);
                        var pubTimeZoneId = "";
                        switch (txtRSSDateField)
                        {
                            case "UPDATEDDATE":
                                pubDate = objEvent.LastUpdatedAt;
                                pubTimeZoneId = objEvent.OtherTimeZoneId;
                                break;
                            case "CREATIONDATE":
                                pubDate = objEvent.CreatedDate;
                                pubTimeZoneId = objEvent.OtherTimeZoneId;
                                break;
                            case "EVENTDATE":
                                pubDate = objEvent.EventTimeBegin;
                                pubTimeZoneId = objEvent.EventTimeZoneId;
                                break;
                        }

                        objEvent = objEventInfoHelper.ConvertEventToDisplayTimeZone(objEvent, displayTimeZoneId);

                        writer.WriteStartElement("item");
                        var eventTitle = tcc.TokenReplaceEvent(objEvent, tmpTitle);
                        writer.WriteElementString("title", eventTitle);

                        var eventDescription = tcc.TokenReplaceEvent(objEvent, tmpDescription);
                        var txtDescription = HttpUtility.HtmlDecode(eventDescription);
                        writer.WriteElementString("description", txtDescription);

                        var txtURL = objEventInfoHelper.DetailPageURL(objEvent);
                        writer.WriteElementString("link", txtURL);
                        writer.WriteElementString("guid", txtURL);

                        writer.WriteElementString("pubDate", GetRFC822Date(pubDate, pubTimeZoneId));

                        writer.WriteElementString("dc:creator", objEvent.OwnerName);

                        if (objEvent.Category > 0 && !ReferenceEquals(objEvent.Category, null))
                        {
                            writer.WriteElementString("category", objEvent.CategoryName);
                        }
                        if (objEvent.Location > 0 && !ReferenceEquals(objEvent.Location, null))
                        {
                            writer.WriteElementString("category", objEvent.LocationName);
                        }
                        if ((int) objEvent.Importance != 2)
                        {
                            var strImportance = Localization.GetString(objEvent.Importance + "Prio", localResourceFile);
                            writer.WriteElementString("category", strImportance);
                        }

                        // specific event data
                        writer.WriteElementString(NsPre, "AllDayEvent", null, objEvent.AllDayEvent.ToString());
                        writer.WriteElementString(NsPre, "Approved", null, objEvent.Approved.ToString());
                        writer.WriteElementString(NsPre, "Cancelled", null, objEvent.Cancelled.ToString());
                        writer.WriteElementString(NsPre, "Category", null, objEvent.CategoryName);
                        //writer.WriteElementString(NsPre, "Location", Nothing, objEvent.LocationName)
                        writer.WriteElementString(NsPre, "DetailURL", null, objEvent.DetailURL);
                        writer.WriteElementString(NsPre, "EventTimeBegin", null,
                                                  objEvent.EventTimeBegin.ToString("yyyy-MM-dd HH:mm:ss"));
                        writer.WriteElementString(NsPre, "EventTimeZoneId", null, objEvent.EventTimeZoneId);
                        writer.WriteElementString(NsPre, "Duration", null, objEvent.Duration.ToString());
                        writer.WriteElementString(NsPre, "ImageURL", null, objEvent.ImageURL);
                        writer.WriteElementString(NsPre, "LocationName", null, objEvent.LocationName);
                        writer.WriteElementString(NsPre, "OriginalDateBegin", null,
                                                  objEvent.OriginalDateBegin.ToString("yyyy-MM-dd HH:mm:ss"));
                        writer.WriteElementString(NsPre, "Signups", null, objEvent.Signups.ToString());
                        writer.WriteElementString(NsPre, "OtherTimeZoneId", null, objEvent.OtherTimeZoneId);

                        writer.WriteEndElement();

                        rssCount++;
                        if ((iMax > 0) & (rssCount == iMax))
                        {
                            break;
                        }
                    }

                    writer.WriteEndElement();
                    writer.WriteEndElement();

                    this.Response.Write(sw.ToString());
                }
            }
        }

        private bool HideFullEvent(EventInfo objevent)
        {
            var objEventInfoHelper = new EventInfoHelper(this._moduleID, this._tabID, this._portalID, this._settings);
            return objEventInfoHelper.HideFullEvent(objevent, this._settings.Eventhidefullenroll, this._userinfo.UserID,
                                                    this.Request.IsAuthenticated);
        }

        private static string GetRFC822Date(DateTime date, string inTimeZoneId)
        {
            var inTimeZone = TimeZoneInfo.FindSystemTimeZoneById(inTimeZoneId);
            var offset = inTimeZone.GetUtcOffset(date);
            var timeZone1 = "";
            if (offset.Hours >= 0)
            {
                timeZone1 = "+";
            }
            else
            {
                timeZone1 = "";
            }
            timeZone1 += offset.Hours.ToString("00") + offset.Minutes.ToString("00");
            return date.ToString("ddd, dd MMM yyyy HH:mm:ss " + timeZone1, CultureInfo.InvariantCulture);
        }

        #endregion
    }
}