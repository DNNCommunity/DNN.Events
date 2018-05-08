using Microsoft.VisualBasic;
using System.Web.UI.WebControls;
using System.Collections;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Localization;
using System;
using DotNetNuke.Entities.Modules;


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
		
#region EventModuleSettings
		[Serializable()]
			public class EventModuleSettings
			{
			public enum TimeZones : int
			{
				UserTZ = 1,
				ModuleTZ,
				PortalTZ
			}
			
			public enum CategoriesSelected : int
			{
				All = 1,
				Some,
				None
			}
			
			public enum DisplayCategories : int
			{
				DoNotDisplay = 1,
				SingleSelect,
				MultiSelect
			}
			
			public enum LocationsSelected : int
			{
				All = 1,
				Some,
				None
			}
			
			public enum DisplayLocations : int
			{
				DoNotDisplay = 1,
				SingleSelect,
				MultiSelect
			}
			
			public enum SocialModule : int
			{
				No = 1,
				SocialGroup,
				UserProfile
			}
			
			public enum SocialGroupPrivacy : int
			{
				OpenToAll = 1,
				EditByGroup,
				PrivateToGroup
			}
			
#region  Private Members
			private Hashtable _allsettings;
			private int _moduleID = -1;
			private string _localresourcefile = null;
			
			private string _version = "";
			private bool _ownerchangeallowed = false;
			private bool _eventsignup = false;
			private bool _eventsignupallowpaid = true;
			private string _timeinterval = "30";
			private string _timeZone = null;
			private string _timeZoneId = null;
			private bool _enableEventTimeZones = false;
			private TimeZones _primaryTimeZone = TimeZones.UserTZ;
			private TimeZones _secondaryTimeZone = TimeZones.PortalTZ;
			private bool _eventtooltipmonth = true;
			private bool _eventtooltipweek = true;
			private bool _eventtooltipday = true;
			private bool _eventtooltiplist = true;
			private int _eventtooltiplength = 10000;
			private bool _monthcellnoevents = false;
			// ReSharper disable FieldCanBeMadeReadOnly.Local
			private bool _disablecategories = false;
			// ReSharper restore FieldCanBeMadeReadOnly.Local
			private DisplayCategories _enablecategories = (DisplayCategories) 0;
			private bool _restrictcategories = false;
			private bool _restrictcategoriestotimeframe = false;
			// ReSharper disable FieldCanBeMadeReadOnly.Local
			private bool _disablelocations = false;
			// ReSharper restore FieldCanBeMadeReadOnly.Local
			private DisplayLocations _enablelocations = (DisplayLocations) 0;
			private bool _restrictlocations = false;
			private bool _restrictlocationstotimeframe = false;
			private bool _enablecontainerskin = true;
			private bool _eventdetailnewpage = false;
			private string _maxrecurrences = "1000";
			private bool _enableenrollpopup = true;
			private bool _eventdaynewpage = false;
			private string _defaultView = "EventMonth.ascx";
			private bool _eventnotify = true;
			private bool _notifyanon = false;
			private bool _sendreminderdefault = false;
			private string _neweventemails = "Never";
			private int _neweventemailrole = -1;
			private bool _newpereventemail = false;
			private bool _tzdisplay = false;
			private string _paypalurl = "https://www.paypal.com";
			private bool _disableEventnav = false;
			private bool _collapserecurring = false;
			private bool _fulltimescale = false;
			private bool _includeendvalue = true;
			private bool _showvaluemarks = false;
			private bool _eventimage = true;
			private bool _allowreoccurring = true;
			private bool _eventsearch = true;
			private bool _preventconflicts = false;
			private bool _locationconflict = false;
			private bool _showEventsAlways = false;
			private bool _timeintitle = false;
			private bool _monthdayselect = false;
			private bool _masterEvent = false;
			private bool _addsubmodulename = true;
			private bool _enforcesubcalperms = true;
			private bool _fridayweekend = false;
			private bool _eventdefaultenrollview = false;
			private string _paypalaccount = null;
			private bool _moderateall = false;
			private string _reminderfrom = null;
			private string _eventsListSelectType = "EVENTS";
			private int _eventsListNumEvents = 10;
			private int _eventsListEventDays = 365;
			private int _eventsListPageSize = 10;
			private string _eventsListSortDirection = "ASC";
			private bool _rssEnable = false;
			private string _rssDateField = "UPDATEDDATE";
			private int _rssDays = 365;
			private string _rssTitle = null;
			private string _rssDesc = null;
			private string _expireevents = "";
			private bool _exportowneremail = false;
			private bool _exportanonowneremail = false;
			private bool _monthAllowed = true;
			private bool _weekAllowed = true;
			private bool _listAllowed = true;
			private bool _eventImageMonth = true;
			private bool _eventImageWeek = false;
			private string _iconBar = "TOP";
			private string _enrollEditFields = "03;05;";
			private string _enrollViewFields = "";
			private string _enrollAnonFields = "02;06;";
			private bool _eventhidefullenroll = false;
			private int _maxnoenrolees = 1;
			private int _enrolcanceldays = 0;
			private bool _detailPageAllowed = false;
			private bool _enrollmentPageAllowed = false;
			private string _enrollmentPageDefaultUrl = "";
			private bool _eventsCustomField1 = false;
			private bool _eventsCustomField2 = false;
			private bool _iconMonthPrio = true;
			private bool _iconMonthRec = true;
			private bool _iconMonthReminder = true;
			private bool _iconMonthEnroll = true;
			private bool _iconWeekPrio = true;
			private bool _iconWeekRec = true;
			private bool _iconWeekReminder = true;
			private bool _iconWeekEnroll = true;
			private bool _iconListPrio = true;
			private bool _iconListRec = true;
			private bool _iconListReminder = true;
			private bool _iconListEnroll = true;
			private string _privateMessage = "";
			private string _eventTheme = "";
			private string _eventThemeDefault = "0,MinimalExtropy,";
			private string _eventsListFields = "BD;ED;EN";
			private string _eventsListShowHeader = "Yes";
			private int _eventsListBeforeDays = 1;
			private int _eventsListAfterDays = 7;
			private string _recurDummy = null;
			private EventTemplates _templates;
			private const int ModuleCategoryID = -1;
			private ArrayList _moduleCategoryIDs = null;
			private CategoriesSelected _moduleCategoriesSelected = CategoriesSelected.All;
			private const int ModuleLocationID = -1;
			private ArrayList _moduleLocationIDs = null;
			private LocationsSelected _moduleLocationsSelected = LocationsSelected.All;
			private string _eventsListSortColumn = "EventDateBegin";
			private string _htmlEmail = "html";
			private int _iCalDaysBefore = 365;
			private int _iCalDaysAfter = 365;
			private string _icalURLAppend = "";
			private string _icalDefaultImage = "";
			private bool _icalOnIconBar = false;
			private bool _icalEmailEnable = false;
			private bool _iIcalURLInLocation = true;
			private bool _iIcalIncludeCalname = false;
			private bool _enableSEO = true;
			private int _seoDescriptionLength = 256;
			private bool _enableSitemap = false;
			private float _siteMapPriority = (float) (0.5F);
			private int _siteMapDaysBefore = 365;
			private int _siteMapDaysAfter = 365;
			private bool _listViewGrid = true;
			private bool _listViewTable = true;
			private int _rptColumns = 1;
			private int _rptRows = 5;
			private System.Web.UI.WebControls.FirstDayOfWeek _weekStart = System.Web.UI.WebControls.FirstDayOfWeek.Default;
			private bool _listViewUseTime = false;
			private string _standardEmail = null;
			private string _fbAdmins = "";
			private string _fbAppID = "";
			private int _maxThumbWidth = 125;
			private int _maxThumbHeight = 125;
			private bool _sendEnrollMessageApproved = true;
			private bool _sendEnrollMessageWaiting = true;
			private bool _sendEnrollMessageDenied = true;
			private bool _sendEnrollMessageAdded = true;
			private bool _sendEnrollMessageDeleted = true;
			private bool _sendEnrollMessagePaying = true;
			private bool _sendEnrollMessagePending = true;
			private bool _sendEnrollMessagePaid = true;
			private bool _sendEnrollMessageIncorrect = true;
			private bool _sendEnrollMessageCancelled = true;
			private bool _allowanonenroll = false;
			private SocialModule _socialGroupModule = SocialModule.No;
			private SortDirection _enrolListSortDirection = SortDirection.Descending;
			private int _enrolListDaysBefore = (365 * 4) + 1;
			private int _enrolListDaysAfter = (365 * 4) + 1;
			private bool _journalIntegration = true;
			private bool _socialUserPrivate = true;
			private SocialGroupPrivacy _socialGroupSecurity = SocialGroupPrivacy.EditByGroup;
			
#endregion
			
#region  Constructors
			
			public EventModuleSettings()
			{
			}
			
			public EventModuleSettings(int moduleId, string localResourceFile)
			{
				_moduleID = moduleId;
				ModuleController mc = new ModuleController();
				_allsettings = (Hashtable) (mc.GetModuleSettings(_moduleID));
				_localresourcefile = localResourceFile;
				
				// Set default correct for those where the basic default is affected by upgrade
				UpdateDefaults();
				
				string temp_variable = Version;
				ReadValue(_allsettings, "version", ref temp_variable);
				Version = temp_variable;
				string temp_variable2 = Timeinterval;
				ReadValue(_allsettings, "timeinterval", ref temp_variable2);
				Timeinterval = temp_variable2;
				string temp_variable3 = TimeZone;
				ReadValue(_allsettings, "TimeZone", ref temp_variable3);
				TimeZone = temp_variable3;
				string temp_variable4 = TimeZoneId;
				ReadValue(_allsettings, "TimeZoneId", ref temp_variable4);
				TimeZoneId = temp_variable4;
				bool temp_variable5 = EnableEventTimeZones;
				ReadValue(_allsettings, "EnableEventTimeZones", ref temp_variable5);
				EnableEventTimeZones = temp_variable5;
				TimeZones temp_variable6 = PrimaryTimeZone;
				ReadValue(_allsettings, "PrimaryTimeZone", ref temp_variable6);
				PrimaryTimeZone = temp_variable6;
				TimeZones temp_variable7 = SecondaryTimeZone;
				ReadValue(_allsettings, "SecondaryTimeZone", ref temp_variable7);
				SecondaryTimeZone = temp_variable7;
				bool temp_variable8 = Eventtooltipmonth;
				ReadValue(_allsettings, "eventtooltipmonth", ref temp_variable8);
				Eventtooltipmonth = temp_variable8;
				bool temp_variable9 = Eventtooltipweek;
				ReadValue(_allsettings, "eventtooltipweek", ref temp_variable9);
				Eventtooltipweek = temp_variable9;
				bool temp_variable10 = Eventtooltipday;
				ReadValue(_allsettings, "eventtooltipday", ref temp_variable10);
				Eventtooltipday = temp_variable10;
				bool temp_variable11 = Eventtooltiplist;
				ReadValue(_allsettings, "eventtooltiplist", ref temp_variable11);
				Eventtooltiplist = temp_variable11;
				int temp_variable12 = Eventtooltiplength;
				ReadValue(_allsettings, "eventtooltiplength", ref temp_variable12);
				Eventtooltiplength = temp_variable12;
				bool temp_variable13 = Monthcellnoevents;
				ReadValue(_allsettings, "monthcellnoevents", ref temp_variable13);
				Monthcellnoevents = temp_variable13;
				ReadValue(_allsettings, "disablecategories", ref _disablecategories);
				DisplayCategories temp_variable14 = Enablecategories;
				ReadValue(_allsettings, "enablecategories", ref temp_variable14);
				Enablecategories = temp_variable14;
				bool temp_variable15 = Restrictcategories;
				ReadValue(_allsettings, "restrictcategories", ref temp_variable15);
				Restrictcategories = temp_variable15;
				bool temp_variable16 = RestrictCategoriesToTimeFrame;
				ReadValue(_allsettings, "restrictcategoriestotimeframe", ref temp_variable16);
				RestrictCategoriesToTimeFrame = temp_variable16;
				ReadValue(_allsettings, "disablelocations", ref _disablelocations);
				DisplayLocations temp_variable17 = Enablelocations;
				ReadValue(_allsettings, "enablelocations", ref temp_variable17);
				Enablelocations = temp_variable17;
				bool temp_variable18 = Restrictlocations;
				ReadValue(_allsettings, "restrictlocations", ref temp_variable18);
				Restrictlocations = temp_variable18;
				bool temp_variable19 = RestrictLocationsToTimeFrame;
				ReadValue(_allsettings, "restrictlocationstotimeframe", ref temp_variable19);
				RestrictLocationsToTimeFrame = temp_variable19;
				bool temp_variable20 = Enablecontainerskin;
				ReadValue(_allsettings, "enablecontainerskin", ref temp_variable20);
				Enablecontainerskin = temp_variable20;
				bool temp_variable21 = Eventdetailnewpage;
				ReadValue(_allsettings, "eventdetailnewpage", ref temp_variable21);
				Eventdetailnewpage = temp_variable21;
				string temp_variable22 = Maxrecurrences;
				ReadValue(_allsettings, "maxrecurrences", ref temp_variable22);
				Maxrecurrences = temp_variable22;
				bool temp_variable23 = Enableenrollpopup;
				ReadValue(_allsettings, "enableenrollpopup", ref temp_variable23);
				Enableenrollpopup = temp_variable23;
				bool temp_variable24 = Eventdaynewpage;
				ReadValue(_allsettings, "eventdaynewpage", ref temp_variable24);
				Eventdaynewpage = temp_variable24;
				string temp_variable25 = DefaultView;
				ReadValue(_allsettings, "DefaultView", ref temp_variable25);
				DefaultView = temp_variable25;
				bool temp_variable26 = Eventnotify;
				ReadValue(_allsettings, "Eventnotify", ref temp_variable26);
				Eventnotify = temp_variable26;
				bool temp_variable27 = Notifyanon;
				ReadValue(_allsettings, "notifyanon", ref temp_variable27);
				Notifyanon = temp_variable27;
				bool temp_variable28 = Sendreminderdefault;
				ReadValue(_allsettings, "sendreminderdefault", ref temp_variable28);
				Sendreminderdefault = temp_variable28;
				string temp_variable29 = Neweventemails;
				ReadValue(_allsettings, "neweventemails", ref temp_variable29);
				Neweventemails = temp_variable29;
				int temp_variable30 = Neweventemailrole;
				ReadValue(_allsettings, "neweventemailrole", ref temp_variable30);
				Neweventemailrole = temp_variable30;
				bool temp_variable31 = Newpereventemail;
				ReadValue(_allsettings, "newpereventemail", ref temp_variable31);
				Newpereventemail = temp_variable31;
				bool temp_variable32 = Tzdisplay;
				ReadValue(_allsettings, "tzdisplay", ref temp_variable32);
				Tzdisplay = temp_variable32;
				string temp_variable33 = Paypalurl;
				ReadValue(_allsettings, "paypalurl", ref temp_variable33);
				Paypalurl = temp_variable33;
				bool temp_variable34 = DisableEventnav;
				ReadValue(_allsettings, "disableEventnav", ref temp_variable34);
				DisableEventnav = temp_variable34;
				bool temp_variable35 = Collapserecurring;
				ReadValue(_allsettings, "collapserecurring", ref temp_variable35);
				Collapserecurring = temp_variable35;
				bool temp_variable36 = Fulltimescale;
				ReadValue(_allsettings, "fulltimescale", ref temp_variable36);
				Fulltimescale = temp_variable36;
				bool temp_variable37 = Includeendvalue;
				ReadValue(_allsettings, "includeendvalue", ref temp_variable37);
				Includeendvalue = temp_variable37;
				bool temp_variable38 = Showvaluemarks;
				ReadValue(_allsettings, "showvaluemarks", ref temp_variable38);
				Showvaluemarks = temp_variable38;
				bool temp_variable39 = Eventimage;
				ReadValue(_allsettings, "eventimage", ref temp_variable39);
				Eventimage = temp_variable39;
				bool temp_variable40 = Allowreoccurring;
				ReadValue(_allsettings, "allowreoccurring", ref temp_variable40);
				Allowreoccurring = temp_variable40;
				bool temp_variable41 = Eventsearch;
				ReadValue(_allsettings, "Eventsearch", ref temp_variable41);
				Eventsearch = temp_variable41;
				bool temp_variable42 = Preventconflicts;
				ReadValue(_allsettings, "preventconflicts", ref temp_variable42);
				Preventconflicts = temp_variable42;
				bool temp_variable43 = Locationconflict;
				ReadValue(_allsettings, "locationconflict", ref temp_variable43);
				Locationconflict = temp_variable43;
				bool temp_variable44 = ShowEventsAlways;
				ReadValue(_allsettings, "showEventsAlways", ref temp_variable44);
				ShowEventsAlways = temp_variable44;
				bool temp_variable45 = Timeintitle;
				ReadValue(_allsettings, "timeintitle", ref temp_variable45);
				Timeintitle = temp_variable45;
				bool temp_variable46 = Monthdayselect;
				ReadValue(_allsettings, "monthdayselect", ref temp_variable46);
				Monthdayselect = temp_variable46;
				bool temp_variable47 = MasterEvent;
				ReadValue(_allsettings, "masterEvent", ref temp_variable47);
				MasterEvent = temp_variable47;
				bool temp_variable48 = Addsubmodulename;
				ReadValue(_allsettings, "addsubmodulename", ref temp_variable48);
				Addsubmodulename = temp_variable48;
				bool temp_variable49 = Enforcesubcalperms;
				ReadValue(_allsettings, "enforcesubcalperms", ref temp_variable49);
				Enforcesubcalperms = temp_variable49;
				bool temp_variable50 = Eventsignup;
				ReadValue(_allsettings, "eventsignup", ref temp_variable50);
				Eventsignup = temp_variable50;
				bool temp_variable51 = Eventsignupallowpaid;
				ReadValue(_allsettings, "eventsignupallowpaid", ref temp_variable51);
				Eventsignupallowpaid = temp_variable51;
				bool temp_variable52 = Fridayweekend;
				ReadValue(_allsettings, "fridayweekend", ref temp_variable52);
				Fridayweekend = temp_variable52;
				bool temp_variable53 = Eventdefaultenrollview;
				ReadValue(_allsettings, "eventdefaultenrollview", ref temp_variable53);
				Eventdefaultenrollview = temp_variable53;
				string temp_variable54 = Paypalaccount;
				ReadValue(_allsettings, "paypalaccount", ref temp_variable54);
				Paypalaccount = temp_variable54;
				bool temp_variable55 = Moderateall;
				ReadValue(_allsettings, "moderateall", ref temp_variable55);
				Moderateall = temp_variable55;
				string temp_variable56 = Reminderfrom;
				ReadValue(_allsettings, "reminderfrom", ref temp_variable56);
				Reminderfrom = temp_variable56;
				string temp_variable57 = EventsListSelectType;
				ReadValue(_allsettings, "EventsListSelectType", ref temp_variable57);
				EventsListSelectType = temp_variable57;
				int temp_variable58 = EventsListNumEvents;
				ReadValue(_allsettings, "EventsListNumEvents", ref temp_variable58);
				EventsListNumEvents = temp_variable58;
				int temp_variable59 = EventsListEventDays;
				ReadValue(_allsettings, "EventsListEventDays", ref temp_variable59);
				EventsListEventDays = temp_variable59;
				int temp_variable60 = EventsListPageSize;
				ReadValue(_allsettings, "EventsListPageSize", ref temp_variable60);
				EventsListPageSize = temp_variable60;
				string temp_variable61 = EventsListSortDirection;
				ReadValue(_allsettings, "EventsListSortDirection", ref temp_variable61);
				EventsListSortDirection = temp_variable61;
				bool temp_variable62 = RSSEnable;
				ReadValue(_allsettings, "RSSEnable", ref temp_variable62);
				RSSEnable = temp_variable62;
				string temp_variable63 = RSSDateField;
				ReadValue(_allsettings, "RSSDateField", ref temp_variable63);
				RSSDateField = temp_variable63;
				int temp_variable64 = RSSDays;
				ReadValue(_allsettings, "RSSDays", ref temp_variable64);
				RSSDays = temp_variable64;
				string temp_variable65 = RSSTitle;
				ReadValue(_allsettings, "RSSTitle", ref temp_variable65);
				RSSTitle = temp_variable65;
				string temp_variable66 = RSSDesc;
				ReadValue(_allsettings, "RSSDesc", ref temp_variable66);
				RSSDesc = temp_variable66;
				string temp_variable67 = Expireevents;
				ReadValue(_allsettings, "expireevents", ref temp_variable67);
				Expireevents = temp_variable67;
				bool temp_variable68 = Exportowneremail;
				ReadValue(_allsettings, "exportowneremail", ref temp_variable68);
				Exportowneremail = temp_variable68;
				bool temp_variable69 = Exportanonowneremail;
				ReadValue(_allsettings, "exportanonowneremail", ref temp_variable69);
				Exportanonowneremail = temp_variable69;
				bool temp_variable70 = Ownerchangeallowed;
				ReadValue(_allsettings, "ownerchangeallowed", ref temp_variable70);
				Ownerchangeallowed = temp_variable70;
				bool temp_variable71 = MonthAllowed;
				ReadValue(_allsettings, "MonthAllowed", ref temp_variable71);
				MonthAllowed = temp_variable71;
				bool temp_variable72 = WeekAllowed;
				ReadValue(_allsettings, "WeekAllowed", ref temp_variable72);
				WeekAllowed = temp_variable72;
				bool temp_variable73 = ListAllowed;
				ReadValue(_allsettings, "ListAllowed", ref temp_variable73);
				ListAllowed = temp_variable73;
				bool temp_variable74 = EventImageMonth;
				ReadValue(_allsettings, "EventImageMonth", ref temp_variable74);
				EventImageMonth = temp_variable74;
				bool temp_variable75 = EventImageWeek;
				ReadValue(_allsettings, "EventImageWeek", ref temp_variable75);
				EventImageWeek = temp_variable75;
				string temp_variable76 = IconBar;
				ReadValue(_allsettings, "IconBar", ref temp_variable76);
				IconBar = temp_variable76;
				string temp_variable77 = EnrollEditFields;
				ReadValue(_allsettings, "EnrollEditFields", ref temp_variable77);
				EnrollEditFields = temp_variable77;
				string temp_variable78 = EnrollViewFields;
				ReadValue(_allsettings, "EnrollViewFields", ref temp_variable78);
				EnrollViewFields = temp_variable78;
				string temp_variable79 = EnrollAnonFields;
				ReadValue(_allsettings, "EnrollAnonFields", ref temp_variable79);
				EnrollAnonFields = temp_variable79;
				bool temp_variable80 = Eventhidefullenroll;
				ReadValue(_allsettings, "eventhidefullenroll", ref temp_variable80);
				Eventhidefullenroll = temp_variable80;
				int temp_variable81 = Maxnoenrolees;
				ReadValue(_allsettings, "maxnoenrolees", ref temp_variable81);
				Maxnoenrolees = temp_variable81;
				int temp_variable82 = Enrolcanceldays;
				ReadValue(_allsettings, "enrolcanceldays", ref temp_variable82);
				Enrolcanceldays = temp_variable82;
				bool temp_variable83 = DetailPageAllowed;
				ReadValue(_allsettings, "DetailPageAllowed", ref temp_variable83);
				DetailPageAllowed = temp_variable83;
				bool temp_variable84 = EnrollmentPageAllowed;
				ReadValue(_allsettings, "EnrollmentPageAllowed", ref temp_variable84);
				EnrollmentPageAllowed = temp_variable84;
				string temp_variable85 = EnrollmentPageDefaultUrl;
				ReadValue(_allsettings, "EnrollmentPageDefaultUrl", ref temp_variable85);
				EnrollmentPageDefaultUrl = temp_variable85;
				bool temp_variable86 = EventsCustomField1;
				ReadValue(_allsettings, "EventsCustomField1", ref temp_variable86);
				EventsCustomField1 = temp_variable86;
				bool temp_variable87 = EventsCustomField2;
				ReadValue(_allsettings, "EventsCustomField2", ref temp_variable87);
				EventsCustomField2 = temp_variable87;
				bool temp_variable88 = IconMonthPrio;
				ReadValue(_allsettings, "IconMonthPrio", ref temp_variable88);
				IconMonthPrio = temp_variable88;
				bool temp_variable89 = IconMonthRec;
				ReadValue(_allsettings, "IconMonthRec", ref temp_variable89);
				IconMonthRec = temp_variable89;
				bool temp_variable90 = IconMonthReminder;
				ReadValue(_allsettings, "IconMonthReminder", ref temp_variable90);
				IconMonthReminder = temp_variable90;
				bool temp_variable91 = IconMonthEnroll;
				ReadValue(_allsettings, "IconMonthEnroll", ref temp_variable91);
				IconMonthEnroll = temp_variable91;
				bool temp_variable92 = IconWeekPrio;
				ReadValue(_allsettings, "IconWeekPrio", ref temp_variable92);
				IconWeekPrio = temp_variable92;
				bool temp_variable93 = IconWeekRec;
				ReadValue(_allsettings, "IconWeekRec", ref temp_variable93);
				IconWeekRec = temp_variable93;
				bool temp_variable94 = IconWeekReminder;
				ReadValue(_allsettings, "IconWeekReminder", ref temp_variable94);
				IconWeekReminder = temp_variable94;
				bool temp_variable95 = IconWeekEnroll;
				ReadValue(_allsettings, "IconWeekEnroll", ref temp_variable95);
				IconWeekEnroll = temp_variable95;
				bool temp_variable96 = IconListPrio;
				ReadValue(_allsettings, "IconListPrio", ref temp_variable96);
				IconListPrio = temp_variable96;
				bool temp_variable97 = IconListRec;
				ReadValue(_allsettings, "IconListRec", ref temp_variable97);
				IconListRec = temp_variable97;
				bool temp_variable98 = IconListReminder;
				ReadValue(_allsettings, "IconListReminder", ref temp_variable98);
				IconListReminder = temp_variable98;
				bool temp_variable99 = IconListEnroll;
				ReadValue(_allsettings, "IconListEnroll", ref temp_variable99);
				IconListEnroll = temp_variable99;
				string temp_variable100 = PrivateMessage;
				ReadValue(_allsettings, "PrivateMessage", ref temp_variable100);
				PrivateMessage = temp_variable100;
				string temp_variable101 = EventTheme;
				ReadValue(_allsettings, "EventTheme", ref temp_variable101);
				EventTheme = temp_variable101;
				string temp_variable102 = EventThemeDefault;
				ReadValue(_allsettings, "EventThemeDefault", ref temp_variable102);
				EventThemeDefault = temp_variable102;
				string temp_variable103 = EventsListFields;
				ReadValue(_allsettings, "EventsListFields", ref temp_variable103);
				EventsListFields = temp_variable103;
				string temp_variable104 = EventsListShowHeader;
				ReadValue(_allsettings, "EventsListShowHeader", ref temp_variable104);
				EventsListShowHeader = temp_variable104;
				int temp_variable105 = EventsListBeforeDays;
				ReadValue(_allsettings, "EventsListBeforeDays", ref temp_variable105);
				EventsListBeforeDays = temp_variable105;
				int temp_variable106 = EventsListAfterDays;
				ReadValue(_allsettings, "EventsListAfterDays", ref temp_variable106);
				EventsListAfterDays = temp_variable106;
				string temp_variable107 = RecurDummy;
				ReadValue(_allsettings, "RecurDummy", ref temp_variable107);
				RecurDummy = temp_variable107;
				//ReadValue(_allsettings, "modulecategoryid", ref ModuleCategoryID);
				ReadValue(_allsettings, "modulecategoryids", ModuleCategoryIDs);
				//ReadValue(_allsettings, "modulelocationid", ref ModuleLocationID);
				ReadValue(_allsettings, "modulelocationids", ModuleLocationIDs);
				string temp_variable108 = EventsListSortColumn;
				ReadValue(_allsettings, "eventslistsortcolumn", ref temp_variable108);
				EventsListSortColumn = temp_variable108;
                string temp_variable109 = HTMLEmail;
                ReadValue(_allsettings, "HTMLEmail", ref temp_variable109);
                HTMLEmail = temp_variable109;
				int temp_variable110 = IcalDaysBefore;
				ReadValue(_allsettings, "iCalDaysBefore", ref temp_variable110);
				IcalDaysBefore = temp_variable110;
				int temp_variable111 = IcalDaysAfter;
				ReadValue(_allsettings, "iCalDaysAfter", ref temp_variable111);
				IcalDaysAfter = temp_variable111;
				string temp_variable112 = IcalURLAppend;
				ReadValue(_allsettings, "iCalURLAppend", ref temp_variable112);
				IcalURLAppend = temp_variable112;
				string temp_variable113 = IcalDefaultImage;
				ReadValue(_allsettings, "iCalDefaultImage", ref temp_variable113);
				IcalDefaultImage = temp_variable113;
				bool temp_variable114 = IcalURLInLocation;
				ReadValue(_allsettings, "iCalURLinLocation", ref temp_variable114);
				IcalURLInLocation = temp_variable114;
				bool temp_variable115 = IcalIncludeCalname;
				ReadValue(_allsettings, "iCalIncludeCalname", ref temp_variable115);
				IcalIncludeCalname = temp_variable115;
				bool temp_variable116 = EnableSEO;
				ReadValue(_allsettings, "enableSEO", ref temp_variable116);
				EnableSEO = temp_variable116;
				int temp_variable117 = SEODescriptionLength;
				ReadValue(_allsettings, "SEODescriptionLength", ref temp_variable117);
				SEODescriptionLength = temp_variable117;
				bool temp_variable118 = EnableSitemap;
				ReadValue(_allsettings, "enableSitemap", ref temp_variable118);
				EnableSitemap = temp_variable118;
				float temp_variable119 = SiteMapPriority;
				ReadValue(_allsettings, "siteMapPriority", ref temp_variable119);
				SiteMapPriority = temp_variable119;
				int temp_variable120 = SiteMapDaysBefore;
				ReadValue(_allsettings, "siteMapDaysBefore", ref temp_variable120);
				SiteMapDaysBefore = temp_variable120;
				int temp_variable121 = SiteMapDaysAfter;
				ReadValue(_allsettings, "siteMapDaysAfter", ref temp_variable121);
				SiteMapDaysAfter = temp_variable121;
				bool temp_variable122 = ListViewGrid;
				ReadValue(_allsettings, "ListViewGrid", ref temp_variable122);
				ListViewGrid = temp_variable122;
				bool temp_variable123 = ListViewTable;
				ReadValue(_allsettings, "ListViewTable", ref temp_variable123);
				ListViewTable = temp_variable123;
				int temp_variable124 = RptColumns;
				ReadValue(_allsettings, "rptColumns", ref temp_variable124);
				RptColumns = temp_variable124;
				int temp_variable125 = RptRows;
				ReadValue(_allsettings, "rptRows", ref temp_variable125);
				RptRows = temp_variable125;
				System.Web.UI.WebControls.FirstDayOfWeek temp_variable126 = WeekStart;
				ReadValue(_allsettings, "WeekStart", ref temp_variable126);
				WeekStart = temp_variable126;
				bool temp_variable127 = ListViewUseTime;
				ReadValue(_allsettings, "ListViewUseTime", ref temp_variable127);
				ListViewUseTime = temp_variable127;
				bool temp_variable128 = IcalOnIconBar;
				ReadValue(_allsettings, "iCalOnIconBar", ref temp_variable128);
				IcalOnIconBar = temp_variable128;
				bool temp_variable129 = IcalEmailEnable;
				ReadValue(_allsettings, "iCalEmailEnable", ref temp_variable129);
				IcalEmailEnable = temp_variable129;
				string temp_variable130 = StandardEmail;
				ReadValue(_allsettings, "StandardEmail", ref temp_variable130);
				StandardEmail = temp_variable130;
				string temp_variable131 = FBAdmins;
				ReadValue(_allsettings, "FBAdmins", ref temp_variable131);
				FBAdmins = temp_variable131;
				string temp_variable132 = FBAppID;
				ReadValue(_allsettings, "FBAppID", ref temp_variable132);
				FBAppID = temp_variable132;
				int temp_variable133 = MaxThumbWidth;
				ReadValue(_allsettings, "MaxThumbWidth", ref temp_variable133);
				MaxThumbWidth = temp_variable133;
				int temp_variable134 = MaxThumbHeight;
				ReadValue(_allsettings, "MaxThumbHeight", ref temp_variable134);
				MaxThumbHeight = temp_variable134;
				bool temp_variable135 = SendEnrollMessageApproved;
				ReadValue(_allsettings, "SendEnrollMessageApproved", ref temp_variable135);
				SendEnrollMessageApproved = temp_variable135;
				bool temp_variable136 = SendEnrollMessageWaiting;
				ReadValue(_allsettings, "SendEnrollMessageWaiting", ref temp_variable136);
				SendEnrollMessageWaiting = temp_variable136;
				bool temp_variable137 = SendEnrollMessageDenied;
				ReadValue(_allsettings, "SendEnrollMessageDenied", ref temp_variable137);
				SendEnrollMessageDenied = temp_variable137;
				bool temp_variable138 = SendEnrollMessageAdded;
				ReadValue(_allsettings, "SendEnrollMessageAdded", ref temp_variable138);
				SendEnrollMessageAdded = temp_variable138;
				bool temp_variable139 = SendEnrollMessageDeleted;
				ReadValue(_allsettings, "SendEnrollMessageDeleted", ref temp_variable139);
				SendEnrollMessageDeleted = temp_variable139;
				bool temp_variable140 = SendEnrollMessagePaying;
				ReadValue(_allsettings, "SendEnrollMessagePaying", ref temp_variable140);
				SendEnrollMessagePaying = temp_variable140;
				bool temp_variable141 = SendEnrollMessagePending;
				ReadValue(_allsettings, "SendEnrollMessagePending", ref temp_variable141);
				SendEnrollMessagePending = temp_variable141;
				bool temp_variable142 = SendEnrollMessagePaid;
				ReadValue(_allsettings, "SendEnrollMessagePaid", ref temp_variable142);
				SendEnrollMessagePaid = temp_variable142;
				bool temp_variable143 = SendEnrollMessageIncorrect;
				ReadValue(_allsettings, "SendEnrollMessageIncorrect", ref temp_variable143);
				SendEnrollMessageIncorrect = temp_variable143;
				bool temp_variable144 = SendEnrollMessageCancelled;
				ReadValue(_allsettings, "SendEnrollMessageCancelled", ref temp_variable144);
				SendEnrollMessageCancelled = temp_variable144;
				bool temp_variable145 = AllowAnonEnroll;
				ReadValue(_allsettings, "allowanonenroll", ref temp_variable145);
				AllowAnonEnroll = temp_variable145;
				SocialModule temp_variable146 = SocialGroupModule;
				ReadValue(_allsettings, "SocialGroupModule", ref temp_variable146);
				SocialGroupModule = temp_variable146;
				bool temp_variable147 = SocialUserPrivate;
				ReadValue(_allsettings, "SocialUserPrivate", ref temp_variable147);
				SocialUserPrivate = temp_variable147;
				SocialGroupPrivacy temp_variable148 = SocialGroupSecurity;
				ReadValue(_allsettings, "SSocialGroupSecurity", ref temp_variable148);
				SocialGroupSecurity = temp_variable148;
				SortDirection temp_variable149 = EnrolListSortDirection;
				ReadValue(_allsettings, "EnrolListSortDirection", ref temp_variable149);
				EnrolListSortDirection = temp_variable149;
				int temp_variable150 = EnrolListDaysBefore;
				ReadValue(_allsettings, "EnrolListDaysBefore", ref temp_variable150);
				EnrolListDaysBefore = temp_variable150;
				int temp_variable151 = EnrolListDaysAfter;
				ReadValue(_allsettings, "EnrolListDaysAfter", ref temp_variable151);
				EnrolListDaysAfter = temp_variable151;
				bool temp_variable152 = JournalIntegration;
				ReadValue(_allsettings, "JournalIntegration", ref temp_variable152);
				JournalIntegration = temp_variable152;
				Templates = new EventTemplates(_moduleID, _allsettings, _localresourcefile);
			}
			
#endregion
			
#region  Public Methods
			
			public EventModuleSettings GetEventModuleSettings(int moduleID, string localResourceFile)
			{
				_moduleID = moduleID;
				if (_moduleID > 0)
				{
					string cacheKey = "EventsSettings" + moduleID.ToString();
					EventModuleSettings bs = default(EventModuleSettings);
					bs = (EventModuleSettings) (DataCache.GetCache(cacheKey));
					if (ReferenceEquals(bs, null))
					{
						bs = new EventModuleSettings(_moduleID, localResourceFile);
						if (!ReferenceEquals(localResourceFile, null))
						{
							DataCache.SetCache(cacheKey, bs);
						}
					}
					return bs;
				}
				return null;
			}
			
			
			public void SaveSettings(int moduleId)
			{
				
				ModuleController objModules = new ModuleController();
				DesktopModuleInfo objDesktopModule = default(DesktopModuleInfo);
				objDesktopModule = DesktopModuleController.GetDesktopModuleByModuleName("DNN_Events", 0);
				objModules.UpdateModuleSetting(moduleId, "version", objDesktopModule.Version);
				
				objModules.UpdateModuleSetting(moduleId, "timeinterval", Timeinterval.ToString());
				objModules.UpdateModuleSetting(moduleId, "TimeZoneId", TimeZoneId);
				objModules.UpdateModuleSetting(moduleId, "EnableEventTimeZones", EnableEventTimeZones.ToString());
				objModules.UpdateModuleSetting(moduleId, "PrimaryTimeZone", (System.Convert.ToInt32(PrimaryTimeZone)).ToString());
				objModules.UpdateModuleSetting(moduleId, "SecondaryTimeZone", (System.Convert.ToInt32(SecondaryTimeZone)).ToString());
				objModules.UpdateModuleSetting(moduleId, "eventtooltipmonth", Eventtooltipmonth.ToString());
				objModules.UpdateModuleSetting(moduleId, "eventtooltipweek", Eventtooltipweek.ToString());
				objModules.UpdateModuleSetting(moduleId, "eventtooltipday", Eventtooltipday.ToString());
				objModules.UpdateModuleSetting(moduleId, "eventtooltiplist", Eventtooltiplist.ToString());
				objModules.UpdateModuleSetting(moduleId, "eventtooltiplength", Eventtooltiplength.ToString());
				objModules.UpdateModuleSetting(moduleId, "monthcellnoevents", Monthcellnoevents.ToString());
				objModules.UpdateModuleSetting(moduleId, "enablecategories", (System.Convert.ToInt32(Enablecategories)).ToString());
				objModules.UpdateModuleSetting(moduleId, "restrictcategories", Restrictcategories.ToString());
				objModules.UpdateModuleSetting(moduleId, "restrictcategoriestotimeframe", RestrictCategoriesToTimeFrame.ToString());
				objModules.UpdateModuleSetting(moduleId, "enablelocations", (System.Convert.ToInt32(Enablelocations)).ToString());
				objModules.UpdateModuleSetting(moduleId, "restrictlocations", Restrictlocations.ToString());
				objModules.UpdateModuleSetting(moduleId, "restrictlocationstotimeframe", RestrictLocationsToTimeFrame.ToString());
				objModules.UpdateModuleSetting(moduleId, "enablecontainerskin", Enablecontainerskin.ToString());
				objModules.UpdateModuleSetting(moduleId, "eventdetailnewpage", Eventdetailnewpage.ToString());
				objModules.UpdateModuleSetting(moduleId, "maxrecurrences", Maxrecurrences.ToString());
				objModules.UpdateModuleSetting(moduleId, "enableenrollpopup", Enableenrollpopup.ToString());
				objModules.UpdateModuleSetting(moduleId, "eventdaynewpage", Eventdaynewpage.ToString());
				objModules.UpdateModuleSetting(moduleId, "DefaultView", DefaultView.ToString());
				objModules.UpdateModuleSetting(moduleId, "Eventnotify", Eventnotify.ToString());
				objModules.UpdateModuleSetting(moduleId, "notifyanon", Notifyanon.ToString());
				objModules.UpdateModuleSetting(moduleId, "sendreminderdefault", Sendreminderdefault.ToString());
				objModules.UpdateModuleSetting(moduleId, "neweventemails", Neweventemails.ToString());
				objModules.UpdateModuleSetting(moduleId, "neweventemailrole", Neweventemailrole.ToString());
				objModules.UpdateModuleSetting(moduleId, "newpereventemail", Newpereventemail.ToString());
				objModules.UpdateModuleSetting(moduleId, "tzdisplay", Tzdisplay.ToString());
				objModules.UpdateModuleSetting(moduleId, "paypalurl", Paypalurl.ToString());
				objModules.UpdateModuleSetting(moduleId, "disableEventnav", DisableEventnav.ToString());
				objModules.UpdateModuleSetting(moduleId, "collapserecurring", Collapserecurring.ToString());
				objModules.UpdateModuleSetting(moduleId, "fulltimescale", Fulltimescale.ToString());
				objModules.UpdateModuleSetting(moduleId, "includeendvalue", Includeendvalue.ToString());
				objModules.UpdateModuleSetting(moduleId, "showvaluemarks", Showvaluemarks.ToString());
				objModules.UpdateModuleSetting(moduleId, "eventimage", Eventimage.ToString());
				objModules.UpdateModuleSetting(moduleId, "allowreoccurring", Allowreoccurring.ToString());
				objModules.UpdateModuleSetting(moduleId, "Eventsearch", Eventsearch.ToString());
				objModules.UpdateModuleSetting(moduleId, "preventconflicts", Preventconflicts.ToString());
				objModules.UpdateModuleSetting(moduleId, "locationconflict", Locationconflict.ToString());
				objModules.UpdateModuleSetting(moduleId, "showEventsAlways", ShowEventsAlways.ToString());
				objModules.UpdateModuleSetting(moduleId, "timeintitle", Timeintitle.ToString());
				objModules.UpdateModuleSetting(moduleId, "monthdayselect", Monthdayselect.ToString());
				objModules.UpdateModuleSetting(moduleId, "masterEvent", MasterEvent.ToString());
				objModules.UpdateModuleSetting(moduleId, "addsubmodulename", Addsubmodulename.ToString());
				objModules.UpdateModuleSetting(moduleId, "enforcesubcalperms", Enforcesubcalperms.ToString());
				objModules.UpdateModuleSetting(moduleId, "eventsignup", Eventsignup.ToString());
				objModules.UpdateModuleSetting(moduleId, "eventsignupallowpaid", Eventsignupallowpaid.ToString());
				objModules.UpdateModuleSetting(moduleId, "fridayweekend", Fridayweekend.ToString());
				objModules.UpdateModuleSetting(moduleId, "eventdefaultenrollview", Eventdefaultenrollview.ToString());
				objModules.UpdateModuleSetting(moduleId, "paypalaccount", Paypalaccount.ToString());
				objModules.UpdateModuleSetting(moduleId, "moderateall", Moderateall.ToString());
				objModules.UpdateModuleSetting(moduleId, "reminderfrom", Reminderfrom.ToString());
				objModules.UpdateModuleSetting(moduleId, "EventsListSelectType", EventsListSelectType.ToString());
				objModules.UpdateModuleSetting(moduleId, "EventsListNumEvents", EventsListNumEvents.ToString());
				objModules.UpdateModuleSetting(moduleId, "EventsListEventDays", EventsListEventDays.ToString());
				objModules.UpdateModuleSetting(moduleId, "EventsListPageSize", EventsListPageSize.ToString());
				objModules.UpdateModuleSetting(moduleId, "EventsListSortDirection", EventsListSortDirection.ToString());
				objModules.UpdateModuleSetting(moduleId, "RSSEnable", RSSEnable.ToString());
				objModules.UpdateModuleSetting(moduleId, "RSSDateField", RSSDateField.ToString());
				objModules.UpdateModuleSetting(moduleId, "RSSDays", RSSDays.ToString());
				objModules.UpdateModuleSetting(moduleId, "RSSTitle", RSSTitle.ToString());
				objModules.UpdateModuleSetting(moduleId, "RSSDesc", RSSDesc.ToString());
				objModules.UpdateModuleSetting(moduleId, "expireevents", Expireevents.ToString());
				objModules.UpdateModuleSetting(moduleId, "exportowneremail", Exportowneremail.ToString());
				objModules.UpdateModuleSetting(moduleId, "exportanonowneremail", Exportanonowneremail.ToString());
				objModules.UpdateModuleSetting(moduleId, "ownerchangeallowed", Ownerchangeallowed.ToString());
				objModules.UpdateModuleSetting(moduleId, "MonthAllowed", MonthAllowed.ToString());
				objModules.UpdateModuleSetting(moduleId, "WeekAllowed", WeekAllowed.ToString());
				objModules.UpdateModuleSetting(moduleId, "ListAllowed", ListAllowed.ToString());
				objModules.UpdateModuleSetting(moduleId, "EventImageMonth", EventImageMonth.ToString());
				objModules.UpdateModuleSetting(moduleId, "EventImageWeek", EventImageWeek.ToString());
				objModules.UpdateModuleSetting(moduleId, "IconBar", IconBar.ToString());
				objModules.UpdateModuleSetting(moduleId, "EnrollEditFields", EnrollEditFields.ToString());
				objModules.UpdateModuleSetting(moduleId, "EnrollViewFields", EnrollViewFields.ToString());
				objModules.UpdateModuleSetting(moduleId, "EnrollAnonFields", EnrollAnonFields.ToString());
				objModules.UpdateModuleSetting(moduleId, "eventhidefullenroll", Eventhidefullenroll.ToString());
				objModules.UpdateModuleSetting(moduleId, "maxnoenrolees", Maxnoenrolees.ToString());
				objModules.UpdateModuleSetting(moduleId, "enrolcanceldays", Enrolcanceldays.ToString());
				objModules.UpdateModuleSetting(moduleId, "DetailPageAllowed", DetailPageAllowed.ToString());
				objModules.UpdateModuleSetting(moduleId, "EnrollmentPageAllowed", EnrollmentPageAllowed.ToString());
				objModules.UpdateModuleSetting(moduleId, "EnrollmentPageDefaultUrl", EnrollmentPageDefaultUrl.ToString());
				objModules.UpdateModuleSetting(moduleId, "EventsCustomField1", EventsCustomField1.ToString());
				objModules.UpdateModuleSetting(moduleId, "EventsCustomField2", EventsCustomField2.ToString());
				objModules.UpdateModuleSetting(moduleId, "IconMonthPrio", IconMonthPrio.ToString());
				objModules.UpdateModuleSetting(moduleId, "IconMonthRec", IconMonthRec.ToString());
				objModules.UpdateModuleSetting(moduleId, "IconMonthReminder", IconMonthReminder.ToString());
				objModules.UpdateModuleSetting(moduleId, "IconMonthEnroll", IconMonthEnroll.ToString());
				objModules.UpdateModuleSetting(moduleId, "IconWeekPrio", IconWeekPrio.ToString());
				objModules.UpdateModuleSetting(moduleId, "IconWeekRec", IconWeekRec.ToString());
				objModules.UpdateModuleSetting(moduleId, "IconWeekReminder", IconWeekReminder.ToString());
				objModules.UpdateModuleSetting(moduleId, "IconWeekEnroll", IconWeekEnroll.ToString());
				objModules.UpdateModuleSetting(moduleId, "IconListPrio", IconListPrio.ToString());
				objModules.UpdateModuleSetting(moduleId, "IconListRec", IconListRec.ToString());
				objModules.UpdateModuleSetting(moduleId, "IconListReminder", IconListReminder.ToString());
				objModules.UpdateModuleSetting(moduleId, "IconListEnroll", IconListEnroll.ToString());
				objModules.UpdateModuleSetting(moduleId, "PrivateMessage", PrivateMessage.ToString());
				objModules.UpdateModuleSetting(moduleId, "EventTheme", EventTheme.ToString());
				// Hard coded value, not set anywhere
				// .UpdateModuleSetting(ModuleId, "EventThemeDefault", EventThemeDefault.ToString)
				objModules.UpdateModuleSetting(moduleId, "EventsListFields", EventsListFields.ToString());
				objModules.UpdateModuleSetting(moduleId, "EventsListShowHeader", EventsListShowHeader.ToString());
				objModules.UpdateModuleSetting(moduleId, "EventsListBeforeDays", EventsListBeforeDays.ToString());
				objModules.UpdateModuleSetting(moduleId, "EventsListAfterDays", EventsListAfterDays.ToString());
				if (!ReferenceEquals(RecurDummy, null))
				{
					objModules.UpdateModuleSetting(moduleId, "RecurDummy", RecurDummy.ToString());
				}
				objModules.UpdateModuleSetting(moduleId, "modulecategoryids", string.Join(",", (string[]) (ModuleCategoryIDs.ToArray(typeof(string)))));
				objModules.UpdateModuleSetting(moduleId, "modulelocationids", string.Join(",", (string[]) (ModuleLocationIDs.ToArray(typeof(string)))));
				objModules.UpdateModuleSetting(moduleId, "eventslistsortcolumn", EventsListSortColumn.ToString());
				objModules.UpdateModuleSetting(moduleId, "HTMLEmail", HTMLEmail.ToString());
				objModules.UpdateModuleSetting(moduleId, "iCalDaysBefore", IcalDaysBefore.ToString());
				objModules.UpdateModuleSetting(moduleId, "iCalDaysAfter", IcalDaysAfter.ToString());
				objModules.UpdateModuleSetting(moduleId, "iCalURLAppend", IcalURLAppend.ToString());
				objModules.UpdateModuleSetting(moduleId, "iCalDefaultImage", IcalDefaultImage.ToString());
				objModules.UpdateModuleSetting(moduleId, "iCalURLinLocation", IcalURLInLocation.ToString());
				objModules.UpdateModuleSetting(moduleId, "iCalIncludeCalname", IcalIncludeCalname.ToString());
				objModules.UpdateModuleSetting(moduleId, "enableSEO", EnableSEO.ToString());
				objModules.UpdateModuleSetting(moduleId, "SEODescriptionLength", SEODescriptionLength.ToString());
				objModules.UpdateModuleSetting(moduleId, "enableSitemap", EnableSitemap.ToString());
				objModules.UpdateModuleSetting(moduleId, "siteMapPriority", SiteMapPriority.ToString());
				objModules.UpdateModuleSetting(moduleId, "siteMapDaysBefore", SiteMapDaysBefore.ToString());
				objModules.UpdateModuleSetting(moduleId, "siteMapDaysAfter", SiteMapDaysAfter.ToString());
				objModules.UpdateModuleSetting(moduleId, "ListViewGrid", ListViewGrid.ToString());
				objModules.UpdateModuleSetting(moduleId, "ListViewTable", ListViewTable.ToString());
				objModules.UpdateModuleSetting(moduleId, "rptColumns", RptColumns.ToString());
				objModules.UpdateModuleSetting(moduleId, "rptRows", RptRows.ToString());
				objModules.UpdateModuleSetting(moduleId, "WeekStart", (System.Convert.ToInt32(WeekStart)).ToString());
				objModules.UpdateModuleSetting(moduleId, "ListViewUseTime", ListViewUseTime.ToString());
				objModules.UpdateModuleSetting(moduleId, "iCalOnIconBar", IcalOnIconBar.ToString());
				objModules.UpdateModuleSetting(moduleId, "iCalEmailEnable", IcalEmailEnable.ToString());
				objModules.UpdateModuleSetting(moduleId, "StandardEmail", StandardEmail.ToString());
				objModules.UpdateModuleSetting(moduleId, "FBAdmins", FBAdmins.ToString());
				objModules.UpdateModuleSetting(moduleId, "FBAppID", FBAppID.ToString());
				objModules.UpdateModuleSetting(moduleId, "MaxThumbHeight", MaxThumbHeight.ToString());
				objModules.UpdateModuleSetting(moduleId, "MaxThumbWidth", MaxThumbWidth.ToString());
				objModules.UpdateModuleSetting(moduleId, "SendEnrollMessageApproved", SendEnrollMessageApproved.ToString());
				objModules.UpdateModuleSetting(moduleId, "SendEnrollMessageWaiting", SendEnrollMessageWaiting.ToString());
				objModules.UpdateModuleSetting(moduleId, "SendEnrollMessageDenied", SendEnrollMessageDenied.ToString());
				objModules.UpdateModuleSetting(moduleId, "SendEnrollMessageAdded", SendEnrollMessageAdded.ToString());
				objModules.UpdateModuleSetting(moduleId, "SendEnrollMessageDeleted", SendEnrollMessageDeleted.ToString());
				objModules.UpdateModuleSetting(moduleId, "SendEnrollMessagePaying", SendEnrollMessagePaying.ToString());
				objModules.UpdateModuleSetting(moduleId, "SendEnrollMessagePending", SendEnrollMessagePending.ToString());
				objModules.UpdateModuleSetting(moduleId, "SendEnrollMessagePaid", SendEnrollMessagePaid.ToString());
				objModules.UpdateModuleSetting(moduleId, "SendEnrollMessageIncorrect", SendEnrollMessageIncorrect.ToString());
				objModules.UpdateModuleSetting(moduleId, "SendEnrollMessageCancelled", SendEnrollMessageCancelled.ToString());
				objModules.UpdateModuleSetting(moduleId, "allowanonenroll", AllowAnonEnroll.ToString());
				objModules.UpdateModuleSetting(moduleId, "SocialGroupModule", (System.Convert.ToInt32(SocialGroupModule)).ToString());
				objModules.UpdateModuleSetting(moduleId, "SocialUserPrivate", SocialUserPrivate.ToString());
				objModules.UpdateModuleSetting(moduleId, "SocialGroupSecurity", (System.Convert.ToInt32(SocialGroupSecurity)).ToString());
				objModules.UpdateModuleSetting(moduleId, "EnrolListSortDirection", (System.Convert.ToInt32(EnrolListSortDirection)).ToString());
				objModules.UpdateModuleSetting(moduleId, "EnrolListDaysBefore", (EnrolListDaysBefore).ToString());
				objModules.UpdateModuleSetting(moduleId, "EnrolListDaysAfter", (EnrolListDaysAfter).ToString());
				objModules.UpdateModuleSetting(moduleId, "JournalIntegration", JournalIntegration.ToString());
				string cacheKey = "EventsSettings" + moduleId.ToString();
				DataCache.SetCache(cacheKey, this);
				
			}
			
#endregion
			
#region  Private Methods
			
			private static void ReadValue(Hashtable valueTable, string valueName, ref string variable)
			{
				if (!ReferenceEquals(valueTable[valueName], null))
				{
					try
					{
						variable = System.Convert.ToString(valueTable[valueName]);
					}
					catch (Exception)
					{
					}
				}
			}
			
			private static void ReadValue(Hashtable valueTable, string valueName, ref int variable)
			{
				if (!ReferenceEquals(valueTable[valueName], null))
				{
					try
					{
						variable = System.Convert.ToInt32(valueTable[valueName]);
					}
					catch (Exception)
					{
					}
				}
			}
			
			private static void ReadValue(Hashtable valueTable, string valueName, ref bool variable)
			{
				if (!ReferenceEquals(valueTable[valueName], null))
				{
					try
					{
						variable = System.Convert.ToBoolean(valueTable[valueName]);
					}
					catch (Exception)
					{
					}
				}
			}
			
			private static void ReadValue(Hashtable valueTable, string valueName, ref System.Web.UI.WebControls.FirstDayOfWeek variable)
			{
				if (!ReferenceEquals(valueTable[valueName], null))
				{
					try
					{
						variable = (System.Web.UI.WebControls.FirstDayOfWeek) (System.Convert.ToInt32(valueTable[valueName]));
					}
					catch (Exception)
					{
					}
				}
			}
			
			private static void ReadValue(Hashtable valueTable, string valueName, ref float variable)
			{
				if (!ReferenceEquals(valueTable[valueName], null))
				{
					try
					{
						variable = System.Convert.ToSingle(System.Convert.ToSingle(valueTable[valueName]));
					}
					catch (Exception)
					{
					}
				}
			}
			
			private static void ReadValue(Hashtable valueTable, string valueName, ref TimeZones variable)
			{
				if (!ReferenceEquals(valueTable[valueName], null))
				{
					try
					{
						variable = (TimeZones) (valueTable[valueName]);
					}
					catch (Exception)
					{
					}
				}
			}
			
			private static void ReadValue(Hashtable valueTable, string valueName, ref DisplayCategories variable)
			{
				if (!ReferenceEquals(valueTable[valueName], null))
				{
					try
					{
						variable = (DisplayCategories) (valueTable[valueName]);
					}
					catch (Exception)
					{
					}
				}
			}
			
			private static void ReadValue(Hashtable valueTable, string valueName, ref DisplayLocations variable)
			{
				if (!ReferenceEquals(valueTable[valueName], null))
				{
					try
					{
						variable = (DisplayLocations) (valueTable[valueName]);
					}
					catch (Exception)
					{
					}
				}
			}
			
			private static void ReadValue(Hashtable valueTable, string valueName, ref SocialModule variable)
			{
				if (!ReferenceEquals(valueTable[valueName], null))
				{
					try
					{
						variable = (SocialModule) (valueTable[valueName]);
					}
					catch (Exception)
					{
					}
				}
			}
			
			private static void ReadValue(Hashtable valueTable, string valueName, ref SocialGroupPrivacy variable)
			{
				if (!ReferenceEquals(valueTable[valueName], null))
				{
					try
					{
						variable = (SocialGroupPrivacy) (valueTable[valueName]);
					}
					catch (Exception)
					{
					}
				}
			}
			
			private static void ReadValue(Hashtable valueTable, string valueName, ref SortDirection variable)
			{
				if (!ReferenceEquals(valueTable[valueName], null))
				{
					try
					{
						variable = (SortDirection) (valueTable[valueName]);
					}
					catch (Exception)
					{
					}
				}
			}
			
			private static void ReadValue(Hashtable valueTable, string valueName, ArrayList variable)
			{
				if (!ReferenceEquals(valueTable[valueName], null))
				{
					try
					{
						string[] tmpArray = Strings.Split(System.Convert.ToString(valueTable[valueName]), ",");
						variable.Clear();
						for (int i = 0; i <= tmpArray.Length - 1; i++)
						{
							if (tmpArray[i] != "")
							{
								variable.Add(int.Parse(tmpArray[i]));
							}
						}
					}
					catch (Exception)
					{
					}
				}
			}
			
			private void UpdateDefaults()
			{
				string vers = System.Convert.ToString(_allsettings["version"]);
				if (!ReferenceEquals(_allsettings["onlyview"], null) && System.Convert.ToBoolean(_allsettings["onlyview"]))
				{
					if (System.Convert.ToString(_allsettings["DefaultView"]) == "EventList.ascx")
					{
						_listAllowed = true;
					}
					else
					{
						_listAllowed = false;
					}
					if (System.Convert.ToString(_allsettings["DefaultView"]) == "EventWeek.ascx")
					{
						_weekAllowed = true;
					}
					else
					{
						_weekAllowed = false;
					}
					if (System.Convert.ToString(_allsettings["DefaultView"]) == "EventMonth.ascx")
					{
						_monthAllowed = true;
					}
					else
					{
						_monthAllowed = false;
					}
				}
				if (string.IsNullOrEmpty(vers))
				{
					_enforcesubcalperms = true;
				}
				else
				{
					_enforcesubcalperms = false;
				}
				if (!ReferenceEquals(_allsettings["allowsubscriptions"], null) && System.Convert.ToBoolean(_allsettings["allowsubscriptions"]))
				{
					_neweventemails = "Subscribe";
				}
				if (string.IsNullOrEmpty(vers))
				{
					_enablecontainerskin = true;
				}
				else if (int.Parse(vers.Substring(0, vers.IndexOf(".") + 0)) < 5)
				{
					_enablecontainerskin = false;
				}
				else
				{
					_enablecontainerskin = true;
				}
				if (!ReferenceEquals(_allsettings["eventtooltip"], null))
				{
					_eventtooltipmonth = System.Convert.ToBoolean(_allsettings["eventtooltip"]);
					_eventtooltipweek = System.Convert.ToBoolean(_allsettings["eventtooltip"]);
					_eventtooltipday = System.Convert.ToBoolean(_allsettings["eventtooltip"]);
					_eventtooltiplist = System.Convert.ToBoolean(_allsettings["eventtooltip"]);
				}
				if (!ReferenceEquals(_allsettings["SocialGroupModule"], null))
				{
					if ((SocialModule)this._allsettings["SocialGroupModule"] == SocialModule.UserProfile)
					{
						_socialUserPrivate = false;
					}
					if ((SocialModule)this._allsettings["SocialGroupModule"] == SocialModule.SocialGroup)
					{
						_socialGroupSecurity = SocialGroupPrivacy.OpenToAll;
					}
				}
			}
			
#endregion
			
#region  Properties
			public int MaxThumbWidth
			{
				get
				{
					return _maxThumbWidth;
				}
				set
				{
					_maxThumbWidth = value;
				}
			}
			
			public int MaxThumbHeight
			{
				get
				{
					return _maxThumbHeight;
				}
				set
				{
					_maxThumbHeight = value;
				}
			}
			
			public string FBAppID
			{
				get
				{
					return _fbAppID;
				}
				set
				{
					_fbAppID = value;
				}
			}
			
			public string FBAdmins
			{
				get
				{
					return _fbAdmins;
				}
				set
				{
					_fbAdmins = value;
				}
			}
			
			public string StandardEmail
			{
				get
				{
					if (ReferenceEquals(_standardEmail, null))
					{
						Entities.Portals.PortalSettings portalsettings = Entities.Portals.PortalController.GetCurrentPortalSettings();
						if (!ReferenceEquals(portalsettings, null))
						{
							_standardEmail = portalsettings.Email;
						}
					}
					return _standardEmail;
				}
				set
				{
					_standardEmail = value;
				}
			}
			
			public bool IcalIncludeCalname
			{
				get
				{
					return _iIcalIncludeCalname;
				}
				set
				{
					_iIcalIncludeCalname = value;
				}
			}
			
			public bool IcalEmailEnable
			{
				get
				{
					return _icalEmailEnable;
				}
				set
				{
					_icalEmailEnable = value;
				}
			}
			
			public bool IcalURLInLocation
			{
				get
				{
					return _iIcalURLInLocation;
				}
				set
				{
					_iIcalURLInLocation = value;
				}
			}
			
			public bool IcalOnIconBar
			{
				get
				{
					return _icalOnIconBar;
				}
				set
				{
					_icalOnIconBar = value;
				}
			}
			
			public bool ListViewUseTime
			{
				get
				{
					return _listViewUseTime;
				}
				set
				{
					_listViewUseTime = value;
				}
			}
			
			public System.Web.UI.WebControls.FirstDayOfWeek WeekStart
			{
				get
				{
					return _weekStart;
				}
				set
				{
					_weekStart = value;
				}
			}
			
			public int RptRows
			{
				get
				{
					return _rptRows;
				}
				set
				{
					_rptRows = value;
				}
			}
			
			public int RptColumns
			{
				get
				{
					return _rptColumns;
				}
				set
				{
					_rptColumns = value;
				}
			}
			
			public bool ListViewTable
			{
				get
				{
					return _listViewTable;
				}
				set
				{
					_listViewTable = value;
				}
			}
			
			public bool ListViewGrid
			{
				get
				{
					return _listViewGrid;
				}
				set
				{
					_listViewGrid = value;
				}
			}
			
			public int SiteMapDaysAfter
			{
				get
				{
					return _siteMapDaysAfter;
				}
				set
				{
					_siteMapDaysAfter = value;
				}
			}
			
			public int SiteMapDaysBefore
			{
				get
				{
					return _siteMapDaysBefore;
				}
				set
				{
					_siteMapDaysBefore = value;
				}
			}
			
			public float SiteMapPriority
			{
				get
				{
					return _siteMapPriority;
				}
				set
				{
					_siteMapPriority = value;
				}
			}
			
			public bool EnableSitemap
			{
				get
				{
					return _enableSitemap;
				}
				set
				{
					_enableSitemap = value;
				}
			}
			
			public int SEODescriptionLength
			{
				get
				{
					return _seoDescriptionLength;
				}
				set
				{
					_seoDescriptionLength = value;
				}
			}
			
			public bool EnableSEO
			{
				get
				{
					return _enableSEO;
				}
				set
				{
					_enableSEO = value;
				}
			}
			
			public string IcalDefaultImage
			{
				get
				{
					return _icalDefaultImage;
				}
				set
				{
					_icalDefaultImage = value;
				}
			}
			
			public string IcalURLAppend
			{
				get
				{
					return _icalURLAppend;
				}
				set
				{
					_icalURLAppend = value;
				}
			}
			
			public int IcalDaysAfter
			{
				get
				{
					return _iCalDaysAfter;
				}
				set
				{
					_iCalDaysAfter = value;
				}
			}
			
			public int IcalDaysBefore
			{
				get
				{
					return _iCalDaysBefore;
				}
				set
				{
					_iCalDaysBefore = value;
				}
			}
			
			public string HTMLEmail
			{
				get
				{
					return _htmlEmail;
				}
				set
				{
					_htmlEmail = value;
				}
			}
			
			public string EventsListSortColumn
			{
				get
				{
					return _eventsListSortColumn;
				}
				set
				{
					_eventsListSortColumn = value;
				}
			}
			
			public ArrayList ModuleCategoryIDs
			{
				get
				{
					if (ReferenceEquals(_moduleCategoryIDs, null))
					{
						ArrayList arCat = new ArrayList();
						arCat.Add(ModuleCategoryID);
						_moduleCategoryIDs = arCat;
					}
					return _moduleCategoryIDs;
				}
				set
				{
					_moduleCategoryIDs = value;
				}
			}
			
			public CategoriesSelected ModuleCategoriesSelected
			{
				get
				{
					if (ModuleCategoryIDs.Count == 0)
					{
						_moduleCategoriesSelected = CategoriesSelected.None;
					}
					else if (System.Convert.ToInt32(ModuleCategoryIDs[0]) == -1)
					{
						_moduleCategoriesSelected = CategoriesSelected.All;
					}
					else
					{
						_moduleCategoriesSelected = CategoriesSelected.Some;
					}
					return _moduleCategoriesSelected;
				}
				set
				{
					_moduleCategoriesSelected = value;
				}
			}
			
			public ArrayList ModuleLocationIDs
			{
				get
				{
					if (ReferenceEquals(_moduleLocationIDs, null))
					{
						ArrayList arLoc = new ArrayList();
						arLoc.Add(ModuleLocationID);
						_moduleLocationIDs = arLoc;
					}
					return _moduleLocationIDs;
				}
				set
				{
					_moduleLocationIDs = value;
				}
			}
			
			public LocationsSelected ModuleLocationsSelected
			{
				get
				{
					if (ModuleLocationIDs.Count == 0)
					{
						_moduleLocationsSelected = LocationsSelected.None;
					}
					else if (System.Convert.ToInt32(ModuleLocationIDs[0]) == -1)
					{
						_moduleLocationsSelected = LocationsSelected.All;
					}
					else
					{
						_moduleLocationsSelected = LocationsSelected.Some;
					}
					return _moduleLocationsSelected;
				}
				set
				{
					_moduleLocationsSelected = value;
				}
			}
			
			public string RecurDummy
			{
				get
				{
					return _recurDummy;
				}
				set
				{
					_recurDummy = value;
				}
			}
			
			public int EventsListAfterDays
			{
				get
				{
					return _eventsListAfterDays;
				}
				set
				{
					_eventsListAfterDays = value;
				}
			}
			
			public int EventsListBeforeDays
			{
				get
				{
					return _eventsListBeforeDays;
				}
				set
				{
					_eventsListBeforeDays = value;
				}
			}
			
			public string EventsListShowHeader
			{
				get
				{
					return _eventsListShowHeader;
				}
				set
				{
					_eventsListShowHeader = value;
				}
			}
			
			public string EventsListFields
			{
				get
				{
					return _eventsListFields;
				}
				set
				{
					_eventsListFields = value;
				}
			}
			
			public string EventThemeDefault
			{
				get
				{
					return _eventThemeDefault;
				}
				set
				{
					_eventThemeDefault = value;
				}
			}
			
			public string EventTheme
			{
				get
				{
					if (string.IsNullOrEmpty(_eventTheme))
					{
						_eventTheme = _eventThemeDefault;
					}
					return _eventTheme;
				}
				set
				{
					_eventTheme = value;
				}
			}
			
			public string PrivateMessage
			{
				get
				{
					return _privateMessage;
				}
				set
				{
					_privateMessage = value;
				}
			}
			
			public bool IconListEnroll
			{
				get
				{
					return _iconListEnroll;
				}
				set
				{
					_iconListEnroll = value;
				}
			}
			
			public bool IconListReminder
			{
				get
				{
					return _iconListReminder;
				}
				set
				{
					_iconListReminder = value;
				}
			}
			
			public bool IconListRec
			{
				get
				{
					return _iconListRec;
				}
				set
				{
					_iconListRec = value;
				}
			}
			
			public bool IconListPrio
			{
				get
				{
					return _iconListPrio;
				}
				set
				{
					_iconListPrio = value;
				}
			}
			
			public bool IconWeekEnroll
			{
				get
				{
					return _iconWeekEnroll;
				}
				set
				{
					_iconWeekEnroll = value;
				}
			}
			
			public bool IconWeekReminder
			{
				get
				{
					return _iconWeekReminder;
				}
				set
				{
					_iconWeekReminder = value;
				}
			}
			
			public bool IconWeekRec
			{
				get
				{
					return _iconWeekRec;
				}
				set
				{
					_iconWeekRec = value;
				}
			}
			
			public bool IconWeekPrio
			{
				get
				{
					return _iconWeekPrio;
				}
				set
				{
					_iconWeekPrio = value;
				}
			}
			
			public bool IconMonthEnroll
			{
				get
				{
					return _iconMonthEnroll;
				}
				set
				{
					_iconMonthEnroll = value;
				}
			}
			
			public bool IconMonthReminder
			{
				get
				{
					return _iconMonthReminder;
				}
				set
				{
					_iconMonthReminder = value;
				}
			}
			
			public bool IconMonthRec
			{
				get
				{
					return _iconMonthRec;
				}
				set
				{
					_iconMonthRec = value;
				}
			}
			
			public bool IconMonthPrio
			{
				get
				{
					return _iconMonthPrio;
				}
				set
				{
					_iconMonthPrio = value;
				}
			}
			
			public bool EventsCustomField2
			{
				get
				{
					return _eventsCustomField2;
				}
				set
				{
					_eventsCustomField2 = value;
				}
			}
			
			public bool EventsCustomField1
			{
				get
				{
					return _eventsCustomField1;
				}
				set
				{
					_eventsCustomField1 = value;
				}
			}
			
			public bool DetailPageAllowed
			{
				get
				{
					return _detailPageAllowed;
				}
				set
				{
					_detailPageAllowed = value;
				}
			}
			
			public bool EnrollmentPageAllowed
			{
				get
				{
					return _enrollmentPageAllowed;
				}
				set
				{
					_enrollmentPageAllowed = value;
				}
			}
			
			public string EnrollmentPageDefaultUrl
			{
				get
				{
					return _enrollmentPageDefaultUrl;
				}
				set
				{
					_enrollmentPageDefaultUrl = value;
				}
			}
			
			public int Enrolcanceldays
			{
				get
				{
					return _enrolcanceldays;
				}
				set
				{
					_enrolcanceldays = value;
				}
			}
			
			public int Maxnoenrolees
			{
				get
				{
					return _maxnoenrolees;
				}
				set
				{
					_maxnoenrolees = value;
				}
			}
			
			public bool Eventhidefullenroll
			{
				get
				{
					return _eventhidefullenroll;
				}
				set
				{
					_eventhidefullenroll = value;
				}
			}
			
			public string EnrollAnonFields
			{
				get
				{
					return _enrollAnonFields;
				}
				set
				{
					_enrollAnonFields = value;
				}
			}
			
			public string EnrollViewFields
			{
				get
				{
					return _enrollViewFields;
				}
				set
				{
					_enrollViewFields = value;
				}
			}
			
			public string EnrollEditFields
			{
				get
				{
					return _enrollEditFields;
				}
				set
				{
					_enrollEditFields = value;
				}
			}
			
			public string IconBar
			{
				get
				{
					return _iconBar;
				}
				set
				{
					_iconBar = value;
				}
			}
			
			public bool EventImageWeek
			{
				get
				{
					return _eventImageWeek;
				}
				set
				{
					_eventImageWeek = value;
				}
			}
			
			public bool EventImageMonth
			{
				get
				{
					return _eventImageMonth;
				}
				set
				{
					_eventImageMonth = value;
				}
			}
			
			public bool ListAllowed
			{
				get
				{
					return _listAllowed;
				}
				set
				{
					_listAllowed = value;
				}
			}
			
			public bool WeekAllowed
			{
				get
				{
					return _weekAllowed;
				}
				set
				{
					_weekAllowed = value;
				}
			}
			
			public bool MonthAllowed
			{
				get
				{
					return _monthAllowed;
				}
				set
				{
					_monthAllowed = value;
				}
			}
			
			public bool Exportanonowneremail
			{
				get
				{
					return _exportanonowneremail;
				}
				set
				{
					_exportanonowneremail = value;
				}
			}
			
			public bool Exportowneremail
			{
				get
				{
					return _exportowneremail;
				}
				set
				{
					_exportowneremail = value;
				}
			}
			
			public string Expireevents
			{
				get
				{
					return _expireevents;
				}
				set
				{
					_expireevents = value;
				}
			}
			
			public string RSSDesc
			{
				get
				{
					if (string.IsNullOrEmpty(_rssDesc)&& !ReferenceEquals(_localresourcefile, null))
					{
						_rssDesc = Localization.GetString("RSSFeedDescDefault", _localresourcefile);
					}
					return _rssDesc;
				}
				set
				{
					_rssDesc = value;
				}
			}
			
			public string RSSTitle
			{
				get
				{
					if (string.IsNullOrEmpty(_rssTitle)&& !ReferenceEquals(_localresourcefile, null))
					{
						_rssTitle = Localization.GetString("RSSFeedTitleDefault", _localresourcefile);
					}
					return _rssTitle;
				}
				set
				{
					_rssTitle = value;
				}
			}
			
			public int RSSDays
			{
				get
				{
					return _rssDays;
				}
				set
				{
					_rssDays = value;
				}
			}
			
			public string RSSDateField
			{
				get
				{
					return _rssDateField;
				}
				set
				{
					_rssDateField = value;
				}
			}
			
			public bool RSSEnable
			{
				get
				{
					return _rssEnable;
				}
				set
				{
					_rssEnable = value;
				}
			}
			
			public string EventsListSortDirection
			{
				get
				{
					return _eventsListSortDirection;
				}
				set
				{
					_eventsListSortDirection = value;
				}
			}
			
			public int EventsListPageSize
			{
				get
				{
					return _eventsListPageSize;
				}
				set
				{
					_eventsListPageSize = value;
				}
			}
			
			public int EventsListEventDays
			{
				get
				{
					return _eventsListEventDays;
				}
				set
				{
					_eventsListEventDays = value;
				}
			}
			
			public int EventsListNumEvents
			{
				get
				{
					return _eventsListNumEvents;
				}
				set
				{
					_eventsListNumEvents = value;
				}
			}
			
			public string EventsListSelectType
			{
				get
				{
					return _eventsListSelectType;
				}
				set
				{
					_eventsListSelectType = value;
				}
			}
			
			public string Reminderfrom
			{
				get
				{
					if (ReferenceEquals(_reminderfrom, null))
					{
						Entities.Portals.PortalSettings portalsettings = Entities.Portals.PortalController.GetCurrentPortalSettings();
						if (!ReferenceEquals(portalsettings, null))
						{
							_reminderfrom = portalsettings.Email;
						}
					}
					return _reminderfrom;
				}
				set
				{
					_reminderfrom = value;
				}
			}
			
			public bool Moderateall
			{
				get
				{
					return _moderateall;
				}
				set
				{
					_moderateall = value;
				}
			}
			
			public string Paypalaccount
			{
				get
				{
					if (ReferenceEquals(_paypalaccount, null))
					{
						Entities.Portals.PortalSettings portalsettings = Entities.Portals.PortalController.GetCurrentPortalSettings();
						if (!ReferenceEquals(portalsettings, null))
						{
							_paypalaccount = portalsettings.Email;
						}
					}
					return _paypalaccount;
				}
				set
				{
					_paypalaccount = value;
				}
			}
			
			public bool Eventdefaultenrollview
			{
				get
				{
					return _eventdefaultenrollview;
				}
				set
				{
					_eventdefaultenrollview = value;
				}
			}
			
			public bool Fridayweekend
			{
				get
				{
					return _fridayweekend;
				}
				set
				{
					_fridayweekend = value;
				}
			}
			
			public bool Enforcesubcalperms
			{
				get
				{
					return _enforcesubcalperms;
				}
				set
				{
					_enforcesubcalperms = value;
				}
			}
			
			public bool Addsubmodulename
			{
				get
				{
					return _addsubmodulename;
				}
				set
				{
					_addsubmodulename = value;
				}
			}
			
			public bool MasterEvent
			{
				get
				{
					return _masterEvent;
				}
				set
				{
					_masterEvent = value;
				}
			}
			
			public bool Monthdayselect
			{
				get
				{
					return _monthdayselect;
				}
				set
				{
					_monthdayselect = value;
				}
			}
			
			public bool Timeintitle
			{
				get
				{
					return _timeintitle;
				}
				set
				{
					_timeintitle = value;
				}
			}
			
			public bool ShowEventsAlways
			{
				get
				{
					return _showEventsAlways;
				}
				set
				{
					_showEventsAlways = value;
				}
			}
			
			public bool Locationconflict
			{
				get
				{
					return _locationconflict;
				}
				set
				{
					_locationconflict = value;
				}
			}
			
			public bool Preventconflicts
			{
				get
				{
					return _preventconflicts;
				}
				set
				{
					_preventconflicts = value;
				}
			}
			
			public bool Eventsearch
			{
				get
				{
					return _eventsearch;
				}
				set
				{
					_eventsearch = value;
				}
			}
			
			public bool Allowreoccurring
			{
				get
				{
					return _allowreoccurring;
				}
				set
				{
					_allowreoccurring = value;
				}
			}
			
			public bool Eventimage
			{
				get
				{
					return _eventimage;
				}
				set
				{
					_eventimage = value;
				}
			}
			
			public bool Showvaluemarks
			{
				get
				{
					return _showvaluemarks;
				}
				set
				{
					_showvaluemarks = value;
				}
			}
			
			public bool Includeendvalue
			{
				get
				{
					return _includeendvalue;
				}
				set
				{
					_includeendvalue = value;
				}
			}
			
			public bool Fulltimescale
			{
				get
				{
					return _fulltimescale;
				}
				set
				{
					_fulltimescale = value;
				}
			}
			
			public bool Collapserecurring
			{
				get
				{
					return _collapserecurring;
				}
				set
				{
					_collapserecurring = value;
				}
			}
			
			public bool DisableEventnav
			{
				get
				{
					return _disableEventnav;
				}
				set
				{
					_disableEventnav = value;
				}
			}
			
			public string Paypalurl
			{
				get
				{
					return _paypalurl;
				}
				set
				{
					_paypalurl = value;
				}
			}
			
			public bool Tzdisplay
			{
				get
				{
					return _tzdisplay;
				}
				set
				{
					_tzdisplay = value;
				}
			}
			
			public bool Newpereventemail
			{
				get
				{
					return _newpereventemail;
				}
				set
				{
					_newpereventemail = value;
				}
			}
			
			public int Neweventemailrole
			{
				get
				{
					if (_neweventemailrole < 0)
					{
						Entities.Portals.PortalSettings portalsettings = Entities.Portals.PortalController.GetCurrentPortalSettings();
						if (!ReferenceEquals(portalsettings, null))
						{
							_neweventemailrole = portalsettings.RegisteredRoleId;
						}
					}
					return _neweventemailrole;
				}
				set
				{
					_neweventemailrole = value;
				}
			}
			
			public string Neweventemails
			{
				get
				{
					return _neweventemails;
				}
				set
				{
					_neweventemails = value;
				}
			}
			
			public bool Sendreminderdefault
			{
				get
				{
					return _sendreminderdefault;
				}
				set
				{
					_sendreminderdefault = value;
				}
			}
			
			public bool Notifyanon
			{
				get
				{
					return _notifyanon;
				}
				set
				{
					_notifyanon = value;
				}
			}
			
			public bool Eventnotify
			{
				get
				{
					return _eventnotify;
				}
				set
				{
					_eventnotify = value;
				}
			}
			
			public string DefaultView
			{
				get
				{
					return _defaultView;
				}
				set
				{
					_defaultView = value;
				}
			}
			
			public bool Eventdaynewpage
			{
				get
				{
					return _eventdaynewpage;
				}
				set
				{
					_eventdaynewpage = value;
				}
			}
			
			public bool Enableenrollpopup
			{
				get
				{
					return _enableenrollpopup;
				}
				set
				{
					_enableenrollpopup = value;
				}
			}
			
			public string Maxrecurrences
			{
				get
				{
					return _maxrecurrences;
				}
				set
				{
					_maxrecurrences = value;
				}
			}
			
			public string Version
			{
				get
				{
					return _version;
				}
				set
				{
					_version = value;
				}
			}
			
			public string Timeinterval
			{
				get
				{
					return _timeinterval;
				}
				set
				{
					_timeinterval = value;
				}
			}
			
			private string TimeZone
			{
				get
				{
					return _timeZone;
				}
				set
				{
					_timeZone = value;
				}
			}
			
			public string TimeZoneId
			{
				get
				{
					if (ReferenceEquals(_timeZoneId, null))
					{
						if (ReferenceEquals(_timeZone, null))
						{
							Entities.Portals.PortalSettings portalsettings = Entities.Portals.PortalController.GetCurrentPortalSettings();
							if (!ReferenceEquals(portalsettings, null))
							{
								_timeZoneId = portalsettings.TimeZone.Id;
							}
						}
						else
						{
							_timeZoneId = Localization.ConvertLegacyTimeZoneOffsetToTimeZoneInfo(int.Parse(_timeZone)).Id;
						}
					}
					return _timeZoneId;
				}
				set
				{
					_timeZoneId = value;
				}
			}
			
			public bool EnableEventTimeZones
			{
				get
				{
					return _enableEventTimeZones;
				}
				set
				{
					_enableEventTimeZones = value;
				}
			}
			
			public TimeZones PrimaryTimeZone
			{
				get
				{
					return _primaryTimeZone;
				}
				set
				{
					_primaryTimeZone = value;
				}
			}
			
			public TimeZones SecondaryTimeZone
			{
				get
				{
					return _secondaryTimeZone;
				}
				set
				{
					_secondaryTimeZone = value;
				}
			}
			
			public bool Eventtooltiplist
			{
				get
				{
					return _eventtooltiplist;
				}
				set
				{
					_eventtooltiplist = value;
				}
			}
			
			public bool Eventtooltipday
			{
				get
				{
					return _eventtooltipday;
				}
				set
				{
					_eventtooltipday = value;
				}
			}
			
			public bool Eventtooltipweek
			{
				get
				{
					return _eventtooltipweek;
				}
				set
				{
					_eventtooltipweek = value;
				}
			}
			
			public bool Eventtooltipmonth
			{
				get
				{
					return _eventtooltipmonth;
				}
				set
				{
					_eventtooltipmonth = value;
				}
			}
			
			public int Eventtooltiplength
			{
				get
				{
					return _eventtooltiplength;
				}
				set
				{
					_eventtooltiplength = value;
				}
			}
			
			public bool Monthcellnoevents
			{
				get
				{
					return _monthcellnoevents;
				}
				set
				{
					_monthcellnoevents = value;
				}
			}
			
			public bool Restrictcategories
			{
				get
				{
					return _restrictcategories;
				}
				set
				{
					_restrictcategories = value;
				}
			}
			
			public bool RestrictCategoriesToTimeFrame
			{
				get
				{
					return _restrictcategoriestotimeframe;
				}
				set
				{
					_restrictcategoriestotimeframe = value;
				}
			}
			
			public DisplayCategories Enablecategories
			{
				get
				{
					if ((int) _enablecategories == 0)
					{
						if (_disablecategories)
						{
							return DisplayCategories.DoNotDisplay;
						}
						else
						{
							return DisplayCategories.MultiSelect;
						}
					}
					else
					{
						return _enablecategories;
					}
				}
				set
				{
					_enablecategories = value;
				}
			}
			
			public bool Restrictlocations
			{
				get
				{
					return _restrictlocations;
				}
				set
				{
					_restrictlocations = value;
				}
			}
			
			public bool RestrictLocationsToTimeFrame
			{
				get
				{
					return _restrictlocationstotimeframe;
				}
				set
				{
					_restrictlocationstotimeframe = value;
				}
			}
			
			public DisplayLocations Enablelocations
			{
				get
				{
					if ((int) _enablelocations == 0)
					{
						if (_disablelocations)
						{
							return DisplayLocations.DoNotDisplay;
						}
						else
						{
							return DisplayLocations.MultiSelect;
						}
					}
					else
					{
						return _enablelocations;
					}
				}
				set
				{
					_enablelocations = value;
				}
			}
			
			public bool Enablecontainerskin
			{
				get
				{
					return _enablecontainerskin;
				}
				set
				{
					_enablecontainerskin = value;
				}
			}
			
			public bool Eventdetailnewpage
			{
				get
				{
					return _eventdetailnewpage;
				}
				set
				{
					_eventdetailnewpage = value;
				}
			}
			
			public bool Ownerchangeallowed
			{
				get
				{
					return _ownerchangeallowed;
				}
				set
				{
					_ownerchangeallowed = value;
				}
			}
			
			public bool Eventsignupallowpaid
			{
				get
				{
					return _eventsignupallowpaid;
				}
				set
				{
					_eventsignupallowpaid = value;
				}
			}
			
			public bool Eventsignup
			{
				get
				{
					return _eventsignup;
				}
				set
				{
					_eventsignup = value;
				}
			}
			
			public bool SendEnrollMessageApproved
			{
				get
				{
					return _sendEnrollMessageApproved;
				}
				set
				{
					_sendEnrollMessageApproved = value;
				}
			}
			
			public bool SendEnrollMessageWaiting
			{
				get
				{
					return _sendEnrollMessageWaiting;
				}
				set
				{
					_sendEnrollMessageWaiting = value;
				}
			}
			
			public bool SendEnrollMessageDenied
			{
				get
				{
					return _sendEnrollMessageDenied;
				}
				set
				{
					_sendEnrollMessageDenied = value;
				}
			}
			
			public bool SendEnrollMessageAdded
			{
				get
				{
					return _sendEnrollMessageAdded;
				}
				set
				{
					_sendEnrollMessageAdded = value;
				}
			}
			
			public bool SendEnrollMessageDeleted
			{
				get
				{
					return _sendEnrollMessageDeleted;
				}
				set
				{
					_sendEnrollMessageDeleted = value;
				}
			}
			
			public bool SendEnrollMessagePaying
			{
				get
				{
					return _sendEnrollMessagePaying;
				}
				set
				{
					_sendEnrollMessagePaying = value;
				}
			}
			
			public bool SendEnrollMessagePending
			{
				get
				{
					return _sendEnrollMessagePending;
				}
				set
				{
					_sendEnrollMessagePending = value;
				}
			}
			
			public bool SendEnrollMessagePaid
			{
				get
				{
					return _sendEnrollMessagePaid;
				}
				set
				{
					_sendEnrollMessagePaid = value;
				}
			}
			
			public bool SendEnrollMessageIncorrect
			{
				get
				{
					return _sendEnrollMessageIncorrect;
				}
				set
				{
					_sendEnrollMessageIncorrect = value;
				}
			}
			
			public bool SendEnrollMessageCancelled
			{
				get
				{
					return _sendEnrollMessageCancelled;
				}
				set
				{
					_sendEnrollMessageCancelled = value;
				}
			}
			
			public bool AllowAnonEnroll
			{
				get
				{
					return _allowanonenroll;
				}
				set
				{
					_allowanonenroll = value;
				}
			}
			
			public SocialModule SocialGroupModule
			{
				get
				{
					return _socialGroupModule;
				}
				set
				{
					_socialGroupModule = value;
				}
			}
			
			public bool SocialUserPrivate
			{
				get
				{
					return _socialUserPrivate;
				}
				set
				{
					_socialUserPrivate = value;
				}
			}
			
			public SocialGroupPrivacy SocialGroupSecurity
			{
				get
				{
					return _socialGroupSecurity;
				}
				set
				{
					_socialGroupSecurity = value;
				}
			}
			
			public SortDirection EnrolListSortDirection
			{
				get
				{
					return _enrolListSortDirection;
				}
				set
				{
					_enrolListSortDirection = value;
				}
			}
			
			public int EnrolListDaysBefore
			{
				get
				{
					return _enrolListDaysBefore;
				}
				set
				{
					_enrolListDaysBefore = value;
				}
			}
			
			public int EnrolListDaysAfter
			{
				get
				{
					return _enrolListDaysAfter;
				}
				set
				{
					_enrolListDaysAfter = value;
				}
			}
			
			public bool JournalIntegration
			{
				get
				{
					return _journalIntegration;
				}
				set
				{
					_journalIntegration = value;
				}
			}
			
			public EventTemplates Templates
			{
				get
				{
					return _templates;
				}
				set
				{
					_templates = value;
				}
			}
			
#endregion
			
		}
		
#endregion
		
#region EventTemplates
		
		[Serializable()]
			public class EventTemplates
			{
			
#region  Private Members
			// ReSharper disable InconsistentNaming
			private string _EventDetailsTemplate = null;
			private string _NewEventTemplate = null;
			private string _txtToolTipTemplateTitleNT = null;
			private string _txtToolTipTemplateBodyNT = null;
			private string _txtToolTipTemplateTitle = null;
			private string _txtToolTipTemplateBody = null;
			private string _moderateemailsubject = null;
			private string _moderateemailmessage = null;
			private string _txtEmailSubject = null;
			private string _txtEmailMessage = null;
			private string _txtEnrollMessageSubject = null;
			private string _txtEnrollMessageApproved = null;
			private string _txtEnrollMessageWaiting = null;
			private string _txtEnrollMessageDenied = null;
			private string _txtEnrollMessageAdded = null;
			private string _txtEnrollMessageDeleted = null;
			private string _txtEnrollMessagePaying = null;
			private string _txtEnrollMessagePending = null;
			private string _txtEnrollMessagePaid = null;
			private string _txtEnrollMessageIncorrect = null;
			private string _txtEnrollMessageCancelled = null;
			private string _txtEditViewEmailSubject = null;
			private string _txtEditViewEmailBody = null;
			private string _txtSubject = null;
			private string _txtMessage = null;
			private string _txtNewEventEmailSubject = null;
			private string _txtNewEventEmailMessage = null;
			private string _txtListEventTimeBegin = null;
			private string _txtListEventTimeEnd = null;
			private string _txtListLocation = null;
			private string _txtListEventDescription = null;
			private string _txtListRptHeader = null;
			private string _txtListRptBody = null;
			private string _txtListRptFooter = null;
			private string _txtDayEventTimeBegin = null;
			private string _txtDayEventTimeEnd = null;
			private string _txtDayLocation = null;
			private string _txtDayEventDescription = null;
			private string _txtWeekEventText = null;
			private string _txtWeekTitleDate = null;
			private string _txtMonthEventText = null;
			private string _txtMonthDayEventCount = null;
			private string _txtRSSTitle = null;
			private string _txtRSSDescription = null;
			private string _txtSEOPageTitle = null;
			private string _txtSEOPageDescription = null;
			private string _EventiCalSubject = null;
			private string _EventiCalBody = null;
			// ReSharper restore InconsistentNaming
			
#endregion
			
#region  Constructors
			
			public EventTemplates(int moduleID, Hashtable allsettings, string localResourceFile)
			{
				
				Type t = this.GetType();
				System.Reflection.PropertyInfo p = default(System.Reflection.PropertyInfo);
				foreach (System.Reflection.PropertyInfo tempLoopVar_p in t.GetProperties())
				{
					p = tempLoopVar_p;
					string pn = p.Name;
					string pv = null;
					if (!ReferenceEquals(allsettings[pn], null))
					{
						pv = System.Convert.ToString(allsettings[pn]);
						if (pv.Length > 1900)
						{
							if (!ReferenceEquals(allsettings[pn + "2"], null))
							{
								pv = System.Convert.ToString(pv + System.Convert.ToString(allsettings[pn + "2"]));
							}
						}
					}
					else
					{
						if (!ReferenceEquals(localResourceFile, null))
						{
							pv = Localization.GetString(pn, localResourceFile);
							if (moduleID > 0)
							{
								SaveTemplate(moduleID, pn, pv);
							}
						}
					}
					p.SetValue(this, pv, null);
				}
				
			}
			
#endregion
			
#region  Public Methods
			public string GetTemplate(string templateName)
			{
				Type t = this.GetType();
				System.Reflection.PropertyInfo p = default(System.Reflection.PropertyInfo);
				foreach (System.Reflection.PropertyInfo tempLoopVar_p in t.GetProperties())
				{
					p = tempLoopVar_p;
					if (p.Name == templateName)
					{
						return p.GetValue(this, null).ToString();
					}
				}
				return "";
			}
			
			public void SaveTemplate(int moduleID, string templateName, string templateValue)
			{
				Type t = this.GetType();
				System.Reflection.PropertyInfo p = default(System.Reflection.PropertyInfo);
				foreach (System.Reflection.PropertyInfo tempLoopVar_p in t.GetProperties())
				{
					p = tempLoopVar_p;
					if (p.Name == templateName)
					{
						p.SetValue(this, templateValue, null);
						ModuleController objModules = new ModuleController();
						
						if (templateValue.Length > 2000)
						{
							objModules.UpdateModuleSetting(moduleID, templateName, templateValue.Substring(0, 2000));
							objModules.UpdateModuleSetting(moduleID, templateName + "2", templateValue.Substring(2000));
						}
						else
						{
							objModules.UpdateModuleSetting(moduleID, templateName, templateValue.Trim());
							objModules.DeleteModuleSetting(moduleID, templateName + "2");
						}
					}
				}
				string cacheKey = "EventsSettings" + moduleID.ToString();
				DataCache.ClearCache(cacheKey);
			}
			
			public void ResetTemplate(int moduleID, string templateName, string localResourceFile)
			{
				string templateValue = Localization.GetString(templateName, localResourceFile);
				SaveTemplate(moduleID, templateName, templateValue);
				
			}
			
#endregion
			
#region  Private Methods
			
#endregion
			
#region  Properties
			// ReSharper disable InconsistentNaming
			public string EventDetailsTemplate
			{
				get
				{
					return _EventDetailsTemplate;
				}
				set
				{
					_EventDetailsTemplate = value;
				}
			}
			
			public string NewEventTemplate
			{
				get
				{
					return _NewEventTemplate;
				}
				set
				{
					_NewEventTemplate = value;
				}
			}
			
			public string txtTooltipTemplateTitleNT
			{
				get
				{
					return _txtToolTipTemplateTitleNT;
				}
				set
				{
					_txtToolTipTemplateTitleNT = value;
				}
			}
			
			public string txtTooltipTemplateBodyNT
			{
				get
				{
					return _txtToolTipTemplateBodyNT;
				}
				set
				{
					_txtToolTipTemplateBodyNT = value;
				}
			}
			
			public string txtTooltipTemplateTitle
			{
				get
				{
					return _txtToolTipTemplateTitle;
				}
				set
				{
					_txtToolTipTemplateTitle = value;
				}
			}
			
			public string txtTooltipTemplateBody
			{
				get
				{
					return _txtToolTipTemplateBody;
				}
				set
				{
					_txtToolTipTemplateBody = value;
				}
			}
			
			public string moderateemailsubject
			{
				get
				{
					return _moderateemailsubject;
				}
				set
				{
					_moderateemailsubject = value;
				}
			}
			
			public string moderateemailmessage
			{
				get
				{
					return _moderateemailmessage;
				}
				set
				{
					_moderateemailmessage = value;
				}
			}
			
			public string txtEmailSubject
			{
				get
				{
					return _txtEmailSubject;
				}
				set
				{
					_txtEmailSubject = value;
				}
			}
			
			public string txtEmailMessage
			{
				get
				{
					return _txtEmailMessage;
				}
				set
				{
					_txtEmailMessage = value;
				}
			}
			
			public string txtEnrollMessageSubject
			{
				get
				{
					return _txtEnrollMessageSubject;
				}
				set
				{
					_txtEnrollMessageSubject = value;
				}
			}
			
			public string txtEnrollMessageApproved
			{
				get
				{
					return _txtEnrollMessageApproved;
				}
				set
				{
					_txtEnrollMessageApproved = value;
				}
			}
			
			public string txtEnrollMessageWaiting
			{
				get
				{
					return _txtEnrollMessageWaiting;
				}
				set
				{
					_txtEnrollMessageWaiting = value;
				}
			}
			
			public string txtEnrollMessageDenied
			{
				get
				{
					return _txtEnrollMessageDenied;
				}
				set
				{
					_txtEnrollMessageDenied = value;
				}
			}
			
			public string txtEnrollMessageAdded
			{
				get
				{
					return _txtEnrollMessageAdded;
				}
				set
				{
					_txtEnrollMessageAdded = value;
				}
			}
			
			public string txtEnrollMessageDeleted
			{
				get
				{
					return _txtEnrollMessageDeleted;
				}
				set
				{
					_txtEnrollMessageDeleted = value;
				}
			}
			
			public string txtEnrollMessagePaying
			{
				get
				{
					return _txtEnrollMessagePaying;
				}
				set
				{
					_txtEnrollMessagePaying = value;
				}
			}
			
			public string txtEnrollMessagePending
			{
				get
				{
					return _txtEnrollMessagePending;
				}
				set
				{
					_txtEnrollMessagePending = value;
				}
			}
			
			public string txtEnrollMessagePaid
			{
				get
				{
					return _txtEnrollMessagePaid;
				}
				set
				{
					_txtEnrollMessagePaid = value;
				}
			}
			
			public string txtEnrollMessageIncorrect
			{
				get
				{
					return _txtEnrollMessageIncorrect;
				}
				set
				{
					_txtEnrollMessageIncorrect = value;
				}
			}
			
			public string txtEnrollMessageCancelled
			{
				get
				{
					return _txtEnrollMessageCancelled;
				}
				set
				{
					_txtEnrollMessageCancelled = value;
				}
			}
			
			public string txtEditViewEmailSubject
			{
				get
				{
					return _txtEditViewEmailSubject;
				}
				set
				{
					_txtEditViewEmailSubject = value;
				}
			}
			
			public string txtEditViewEmailBody
			{
				get
				{
					return _txtEditViewEmailBody;
				}
				set
				{
					_txtEditViewEmailBody = value;
				}
			}
			
			public string txtSubject
			{
				get
				{
					return _txtSubject;
				}
				set
				{
					_txtSubject = value;
				}
			}
			
			public string txtMessage
			{
				get
				{
					return _txtMessage;
				}
				set
				{
					_txtMessage = value;
				}
			}
			
			public string txtNewEventEmailSubject
			{
				get
				{
					return _txtNewEventEmailSubject;
				}
				set
				{
					_txtNewEventEmailSubject = value;
				}
			}
			
			public string txtNewEventEmailMessage
			{
				get
				{
					return _txtNewEventEmailMessage;
				}
				set
				{
					_txtNewEventEmailMessage = value;
				}
			}
			
			public string txtListEventTimeBegin
			{
				get
				{
					return _txtListEventTimeBegin;
				}
				set
				{
					_txtListEventTimeBegin = value;
				}
			}
			
			public string txtListEventTimeEnd
			{
				get
				{
					return _txtListEventTimeEnd;
				}
				set
				{
					_txtListEventTimeEnd = value;
				}
			}
			
			public string txtListLocation
			{
				get
				{
					return _txtListLocation;
				}
				set
				{
					_txtListLocation = value;
				}
			}
			
			public string txtListEventDescription
			{
				get
				{
					return _txtListEventDescription;
				}
				set
				{
					_txtListEventDescription = value;
				}
			}
			
			public string txtListRptHeader
			{
				get
				{
					return _txtListRptHeader;
				}
				set
				{
					_txtListRptHeader = value;
				}
			}
			
			public string txtListRptBody
			{
				get
				{
					return _txtListRptBody;
				}
				set
				{
					_txtListRptBody = value;
				}
			}
			
			public string txtListRptFooter
			{
				get
				{
					return _txtListRptFooter;
				}
				set
				{
					_txtListRptFooter = value;
				}
			}
			
			public string txtDayEventTimeBegin
			{
				get
				{
					return _txtDayEventTimeBegin;
				}
				set
				{
					_txtDayEventTimeBegin = value;
				}
			}
			
			public string txtDayEventTimeEnd
			{
				get
				{
					return _txtDayEventTimeEnd;
				}
				set
				{
					_txtDayEventTimeEnd = value;
				}
			}
			
			public string txtDayLocation
			{
				get
				{
					return _txtDayLocation;
				}
				set
				{
					_txtDayLocation = value;
				}
			}
			
			public string txtDayEventDescription
			{
				get
				{
					return _txtDayEventDescription;
				}
				set
				{
					_txtDayEventDescription = value;
				}
			}
			
			public string txtWeekEventText
			{
				get
				{
					return _txtWeekEventText;
				}
				set
				{
					_txtWeekEventText = value;
				}
			}
			
			public string txtWeekTitleDate
			{
				get
				{
					return _txtWeekTitleDate;
				}
				set
				{
					_txtWeekTitleDate = value;
				}
			}
			
			public string txtMonthEventText
			{
				get
				{
					return _txtMonthEventText;
				}
				set
				{
					_txtMonthEventText = value;
				}
			}
			
			public string txtMonthDayEventCount
			{
				get
				{
					return _txtMonthDayEventCount;
				}
				set
				{
					_txtMonthDayEventCount = value;
				}
			}
			
			public string txtRSSTitle
			{
				get
				{
					return _txtRSSTitle;
				}
				set
				{
					_txtRSSTitle = value;
				}
			}
			
			public string txtRSSDescription
			{
				get
				{
					return _txtRSSDescription;
				}
				set
				{
					_txtRSSDescription = value;
				}
			}
			
			public string txtSEOPageTitle
			{
				get
				{
					return _txtSEOPageTitle;
				}
				set
				{
					_txtSEOPageTitle = value;
				}
			}
			
			public string txtSEOPageDescription
			{
				get
				{
					return _txtSEOPageDescription;
				}
				set
				{
					_txtSEOPageDescription = value;
				}
			}
			
			public string EventiCalSubject
			{
				get
				{
					return _EventiCalSubject;
				}
				set
				{
					_EventiCalSubject = value;
				}
			}
			
			public string EventiCalBody
			{
				get
				{
					return _EventiCalBody;
				}
				set
				{
					_EventiCalBody = value;
				}
			}
			// ReSharper restore InconsistentNaming
			
#endregion
			
		}
#endregion
		
	}
	
