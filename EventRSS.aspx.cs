using System.Diagnostics;
using DotNetNuke.Entities.Users;
using System.Web.UI;
using Microsoft.VisualBasic;
using System.Collections;
using System.Web;
using DotNetNuke.Services.Localization;
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Globalization;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;

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
				int iDaysBefore = 0;
				int iDaysAfter = 0;
				int iMax = 0;
				int iOwnerID = 0;
				int iLocationID = 0;
				int iImportance = 0;
				ArrayList categoryIDs = new ArrayList();
				ArrayList locationIDs = new ArrayList();
				int iGroupId = -1;
				int iUserId = -1;
				string iCategoryName = "";
				string iLocationName = "";
				string iOwnerName = "";
				string txtPriority = "";
				if (!(HttpContext.Current.Request.QueryString["Mid"] == ""))
				{
					_moduleID = System.Convert.ToInt32(HttpContext.Current.Request.QueryString["mid"]);
				}
				else
				{
					Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(), true);
				}
				if (!(HttpContext.Current.Request.QueryString["tabid"] == ""))
				{
					_tabID = System.Convert.ToInt32(HttpContext.Current.Request.QueryString["tabid"]);
				}
				else
				{
					Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(), true);
				}
				
				string localResourceFile = TemplateSourceDirectory + "/" + Localization.LocalResourceDirectory + "/EventRSS.aspx.resx";
				
				if (!(HttpContext.Current.Request.QueryString["CategoryName"] == ""))
				{
					iCategoryName = HttpContext.Current.Request.QueryString["CategoryName"];
					PortalSecurity objSecurity = new PortalSecurity();
					iCategoryName = objSecurity.InputFilter(iCategoryName, PortalSecurity.FilterFlag.NoSQL);
				}
				if (!(HttpContext.Current.Request.QueryString["CategoryID"] == ""))
				{
					categoryIDs.Add(System.Convert.ToInt32(HttpContext.Current.Request.QueryString["CategoryID"]));
				}
				if (!(HttpContext.Current.Request.QueryString["LocationName"] == ""))
				{
					iLocationName = HttpContext.Current.Request.QueryString["LocationName"];
					PortalSecurity objSecurity = new PortalSecurity();
					iLocationName = objSecurity.InputFilter(iLocationName, PortalSecurity.FilterFlag.NoSQL);
				}
				if (!(HttpContext.Current.Request.QueryString["LocationID"] == ""))
				{
					locationIDs.Add(System.Convert.ToInt32(HttpContext.Current.Request.QueryString["LocationID"]));
				}
				if (!(HttpContext.Current.Request.QueryString["groupid"] == ""))
				{
					iGroupId = System.Convert.ToInt32(HttpContext.Current.Request.QueryString["groupid"]);
				}
				if (!(HttpContext.Current.Request.QueryString["DaysBefore"] == ""))
				{
					iDaysBefore = System.Convert.ToInt32(HttpContext.Current.Request.QueryString["DaysBefore"]);
				}
				if (!(HttpContext.Current.Request.QueryString["DaysAfter"] == ""))
				{
					iDaysAfter = System.Convert.ToInt32(HttpContext.Current.Request.QueryString["DaysAfter"]);
				}
				if (!(HttpContext.Current.Request.QueryString["MaxNumber"] == ""))
				{
					iMax = System.Convert.ToInt32(HttpContext.Current.Request.QueryString["MaxNumber"]);
				}
				if (!(HttpContext.Current.Request.QueryString["OwnerName"] == ""))
				{
					iOwnerName = HttpContext.Current.Request.QueryString["OwnerName"];
				}
				if (!(HttpContext.Current.Request.QueryString["OwnerID"] == ""))
				{
					iOwnerID = System.Convert.ToInt32(HttpContext.Current.Request.QueryString["OwnerID"]);
				}
				if (!(HttpContext.Current.Request.QueryString["LocationName"] == ""))
				{
					iLocationName = HttpContext.Current.Request.QueryString["LocationName"];
				}
				if (!(HttpContext.Current.Request.QueryString["LocationID"] == ""))
				{
					iLocationID = System.Convert.ToInt32(HttpContext.Current.Request.QueryString["LocationID"]);
				}
				if (!(HttpContext.Current.Request.QueryString["Priority"] == ""))
				{
					string iPriority = "";
					iPriority = HttpContext.Current.Request.QueryString["Priority"];
					string lHigh = "";
					string lMedium = "";
					string lLow = "";
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
					iImportance = System.Convert.ToInt32(HttpContext.Current.Request.QueryString["Importance"]);
				}
				
				PortalSettings portalSettings = (PortalSettings) (HttpContext.Current.Items["PortalSettings"]);
				_portalID = portalSettings.PortalId;
				_userinfo = (UserInfo) (HttpContext.Current.Items["UserInfo"]);
				if (!ReferenceEquals(portalSettings, null))
				{
					if (portalSettings.DefaultLanguage != "")
					{
						CultureInfo userculture = new CultureInfo(portalSettings.DefaultLanguage, false);
						System.Threading.Thread.CurrentThread.CurrentCulture = userculture;
					}
				}
				if (_userinfo.UserID > 0)
				{
					if (_userinfo.Profile.PreferredLocale != "")
					{
						CultureInfo userculture = new CultureInfo(_userinfo.Profile.PreferredLocale, false);
						System.Threading.Thread.CurrentThread.CurrentCulture = userculture;
					}
				}
				
				EventModuleSettings ems = new EventModuleSettings();
				_settings = ems.GetEventModuleSettings(_moduleID, localResourceFile);
				
				if (_settings.Enablecategories == EventModuleSettings.DisplayCategories.DoNotDisplay)
				{
					categoryIDs = _settings.ModuleCategoryIDs;
					iCategoryName = "";
				}
				if (!string.IsNullOrEmpty(iCategoryName))
				{
					EventCategoryController oCntrlEventCategory = new EventCategoryController();
					EventCategoryInfo oEventCategory = oCntrlEventCategory.EventCategoryGetByName(iCategoryName, _portalID);
					if (!ReferenceEquals(oEventCategory, null))
					{
						categoryIDs.Add(oEventCategory.Category);
					}
				}
				if (_settings.Enablelocations == EventModuleSettings.DisplayLocations.DoNotDisplay)
				{
					locationIDs = _settings.ModuleLocationIDs;
					iLocationName = "";
				}
				if (!string.IsNullOrEmpty(iLocationName))
				{
					EventLocationController oCntrlEventLocation = new EventLocationController();
					EventLocationInfo oEventLocation = oCntrlEventLocation.EventsLocationGetByName(iLocationName, _portalID);
					if (!ReferenceEquals(oEventLocation, null))
					{
						locationIDs.Add(oEventLocation.Location);
					}
				}
				
				if (!_settings.RSSEnable)
				{
					Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(), true);
				}
				
				if (_settings.SocialGroupModule == EventModuleSettings.SocialModule.UserProfile)
				{
					iUserId = _userinfo.UserID;
				}
				bool getSubEvents = _settings.MasterEvent;
				
				DateTime dtEndDate = default(DateTime);
				if (HttpContext.Current.Request.QueryString["DaysAfter"] == "" && 
					HttpContext.Current.Request.QueryString["DaysBefore"] == "")
				{
					iDaysAfter = _settings.RSSDays;
				}
				EventTimeZoneUtilities objEventTimeZoneUtilities = new EventTimeZoneUtilities();
				DateTime currDate = objEventTimeZoneUtilities.ConvertFromUTCToModuleTimeZone(DateTime.UtcNow, _settings.TimeZoneId);
				
				dtEndDate = DateAndTime.DateAdd(DateInterval.Day, iDaysAfter, currDate).Date;
				
				DateTime dtStartDate = default(DateTime);
				dtStartDate = DateAndTime.DateAdd(DateInterval.Day, System.Convert.ToDouble(- iDaysBefore), currDate).Date;
				
				string txtFeedRootTitle = "";
				string txtFeedRootDescription = "";
				string txtRSSDateField = "";
				txtFeedRootTitle = _settings.RSSTitle;
				txtFeedRootDescription = _settings.RSSDesc;
				txtRSSDateField = _settings.RSSDateField;
				
				Response.ContentType = "text/xml";
				Response.ContentEncoding = Encoding.UTF8;
				
				
				using (StringWriter sw = new StringWriter())
				{
					using (XmlTextWriter writer = new XmlTextWriter(sw))
					{
						//                Dim writer As XmlTextWriter = New XmlTextWriter(sw)
						writer.Formatting = Formatting.Indented;
						writer.Indentation = 4;
						
						writer.WriteStartElement("rss");
						writer.WriteAttributeString("version", "2.0");
						writer.WriteAttributeString("xmlns:wfw", "http://wellformedweb.org/CommentAPI/");
						writer.WriteAttributeString("xmlns:slash", "http://purl.org/rss/1.0/modules/slash/");
						writer.WriteAttributeString("xmlns:dc", "http://purl.org/dc/elements/1.1/");
						writer.WriteAttributeString("xmlns:trackback", "http://madskills.com/public/xml/rss/module/trackback/");
						writer.WriteAttributeString("xmlns:atom", "http://www.w3.org/2005/Atom");
						writer.WriteAttributeString("xmlns", NsPre, null, NsFull);
						
						writer.WriteStartElement("channel");
						
						writer.WriteStartElement("atom:link");
						writer.WriteAttributeString("href", HttpContext.Current.Request.Url.AbsoluteUri);
						writer.WriteAttributeString("rel", "self");
						writer.WriteAttributeString("type", "application/rss+xml");
						writer.WriteEndElement();
						
						writer.WriteElementString("title", portalSettings.PortalName + " - " + txtFeedRootTitle);
						
						if ((portalSettings.PortalAlias.HTTPAlias.IndexOf("http://", StringComparison.Ordinal) == -1) && (portalSettings.PortalAlias.HTTPAlias.IndexOf("https://", StringComparison.Ordinal) == -1))
						{
							writer.WriteElementString("link", DotNetNuke.Common.Globals.AddHTTP(portalSettings.PortalAlias.HTTPAlias));
						}
						else
						{
							writer.WriteElementString("link", portalSettings.PortalAlias.HTTPAlias);
						}
						
						writer.WriteElementString("description", txtFeedRootDescription);
						writer.WriteElementString("ttl", "60");
						
						EventInfoHelper objEventInfoHelper = new EventInfoHelper(_moduleID, _tabID, _portalID, _settings);
						ArrayList lstEvents = default(ArrayList);
						TokenReplaceControllerClass tcc = new TokenReplaceControllerClass(_moduleID, localResourceFile);
						string tmpTitle = _settings.Templates.txtRSSTitle;
						string tmpDescription = _settings.Templates.txtRSSDescription;
						if (categoryIDs.Count == 0)
						{
							categoryIDs.Add("-1");
						}
						if (locationIDs.Count == 0)
						{
							locationIDs.Add("-1");
						}
						
						lstEvents = objEventInfoHelper.GetEvents(dtStartDate, dtEndDate, getSubEvents, categoryIDs, locationIDs, iGroupId, iUserId);
						
						EventBase objEventBase = new EventBase();
						string displayTimeZoneId = objEventBase.GetDisplayTimeZoneId(_settings, _portalID);
						
						int rssCount = 0;
						foreach (EventInfo eventInfo in lstEvents)
						{
						    var objEvent = eventInfo;

							if (System.Convert.ToInt32(categoryIDs[0]) == 0 & objEvent.Category != System.Convert.ToInt32(categoryIDs[0]))
							{
								continue;
							}
							if (System.Convert.ToInt32(locationIDs[0]) == 0 & objEvent.Location != System.Convert.ToInt32(locationIDs[0]))
							{
								continue;
							}
							if (iOwnerID > 0 & objEvent.OwnerID != iOwnerID)
							{
								continue;
							}
							if (!string.IsNullOrEmpty(iOwnerName)&& objEvent.OwnerName != iOwnerName)
							{
								continue;
							}
							if (iLocationID > 0 & objEvent.Location != iLocationID)
							{
								continue;
							}
							if (!string.IsNullOrEmpty(iLocationName)&& objEvent.LocationName != iLocationName)
							{
								continue;
							}
							if (iImportance > 0 && (int)objEvent.Importance != iImportance)
							{
								continue;
							}
							if (!string.IsNullOrEmpty(txtPriority)&& objEvent.Importance.ToString() != txtPriority)
							{
								continue;
							}
							
							// If full enrollments should be hidden, ignore
							if (HideFullEvent(objEvent))
							{
								continue;
							}
							
							DateTime pubDate = default(DateTime);
							string pubTimeZoneId = "";
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
							string eventTitle = tcc.TokenReplaceEvent(objEvent, tmpTitle);
							writer.WriteElementString("title", eventTitle);
							
							string eventDescription = tcc.TokenReplaceEvent(objEvent, tmpDescription);
							string txtDescription = HttpUtility.HtmlDecode(eventDescription);
							writer.WriteElementString("description", txtDescription);
							
							string txtURL = objEventInfoHelper.DetailPageURL(objEvent);
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
							if ((int)objEvent.Importance != 2)
							{
								string strImportance = Localization.GetString(objEvent.Importance.ToString() + "Prio", localResourceFile);
								writer.WriteElementString("category", strImportance);
							}
							
							// specific event data
							writer.WriteElementString(NsPre, "AllDayEvent", null, objEvent.AllDayEvent.ToString());
							writer.WriteElementString(NsPre, "Approved", null, objEvent.Approved.ToString());
							writer.WriteElementString(NsPre, "Cancelled", null, objEvent.Cancelled.ToString());
							writer.WriteElementString(NsPre, "Category", null, objEvent.CategoryName);
							//writer.WriteElementString(NsPre, "Location", Nothing, objEvent.LocationName)
							writer.WriteElementString(NsPre, "DetailURL", null, objEvent.DetailURL);
							writer.WriteElementString(NsPre, "EventTimeBegin", null, objEvent.EventTimeBegin.ToString("yyyy-MM-dd HH:mm:ss"));
							writer.WriteElementString(NsPre, "EventTimeZoneId", null, objEvent.EventTimeZoneId);
							writer.WriteElementString(NsPre, "Duration", null, objEvent.Duration.ToString());
							writer.WriteElementString(NsPre, "ImageURL", null, objEvent.ImageURL);
							writer.WriteElementString(NsPre, "LocationName", null, objEvent.LocationName);
							writer.WriteElementString(NsPre, "OriginalDateBegin", null, objEvent.OriginalDateBegin.ToString("yyyy-MM-dd HH:mm:ss"));
							writer.WriteElementString(NsPre, "Signups", null, objEvent.Signups.ToString());
							writer.WriteElementString(NsPre, "OtherTimeZoneId", null, objEvent.OtherTimeZoneId);
							
							writer.WriteEndElement();
							
							rssCount++;
							if (iMax > 0 & rssCount == iMax)
							{
								break;
							}
						}
						
						writer.WriteEndElement();
						writer.WriteEndElement();
						
						Response.Write(sw.ToString());
					}
					
				}
				
				
			}
			
			private bool HideFullEvent(EventInfo objevent)
			{
				EventInfoHelper objEventInfoHelper = new EventInfoHelper(_moduleID, _tabID, _portalID, _settings);
				return objEventInfoHelper.HideFullEvent(objevent, _settings.Eventhidefullenroll, _userinfo.UserID, Request.IsAuthenticated);
			}
			
			private static string GetRFC822Date(DateTime date, string inTimeZoneId)
			{
				TimeZoneInfo inTimeZone = TimeZoneInfo.FindSystemTimeZoneById(inTimeZoneId);
				TimeSpan offset = inTimeZone.GetUtcOffset(date);
				string timeZone1 = "";
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
	

