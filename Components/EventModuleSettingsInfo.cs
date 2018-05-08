//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using DotNetNuke.Entities.Modules.Settings;

//namespace Components
//{
//    using System.Collections;
//    using System.Web.UI.WebControls;
//    using DotNetNuke.Modules.Events;

//    public class EventModuleSettingsInfo
//    {
//        [ModuleSetting]
//        public int _moduleID { get; set; } = -1;

//        [ModuleSetting]
//        public string _localresourcefile { get; set; } = null;

//        [ModuleSetting]
//        public string _version { get; set; } = "";

//        [ModuleSetting]
//        public bool _ownerchangeallowed { get; set; } = false;

//        [ModuleSetting]
//        public bool _eventsignup { get; set; } = false;

//        [ModuleSetting]
//        public bool _eventsignupallowpaid { get; set; } = true;

//        [ModuleSetting]
//        public string _timeinterval { get; set; } = "30";

//        [ModuleSetting]
//        public string _timeZone { get; set; } = null;

//        [ModuleSetting]
//        public string _timeZoneId { get; set; } = null;

//        [ModuleSetting]
//        public bool _enableEventTimeZones { get; set; } = false;

//        [ModuleSetting]
//        public EventModuleSettings.TimeZones _primaryTimeZone { get; set; } = EventModuleSettings.TimeZones.UserTZ;

//        [ModuleSetting]
//        public EventModuleSettings.TimeZones _secondaryTimeZone { get; set; } = EventModuleSettings.TimeZones.PortalTZ;

//        [ModuleSetting]
//        public bool _eventtooltipmonth { get; set; } = true;

//        [ModuleSetting]
//        public bool _eventtooltipweek { get; set; } = true;

//        [ModuleSetting]
//        public bool _eventtooltipday { get; set; } = true;

//        [ModuleSetting]
//        public bool _eventtooltiplist { get; set; } = true;

//        [ModuleSetting]
//        public int _eventtooltiplength { get; set; } = 10000;

//        [ModuleSetting]
//        public bool _monthcellnoevents { get; set; } = false;

//        [ModuleSetting]
//        public bool _disablecategories { get; set; } = false;

//        [ModuleSetting]
//        public EventModuleSettings.DisplayCategories _enablecategories { get; set; } = (EventModuleSettings.DisplayCategories)0;

//        [ModuleSetting]
//        public bool _restrictcategories { get; set; } = false;

//        [ModuleSetting]
//        public bool _restrictcategoriestotimeframe { get; set; } = false;

//        [ModuleSetting]
//        public bool _disablelocations { get; set; } = false;

//        [ModuleSetting]
//        public EventModuleSettings.DisplayLocations _enablelocations { get; set; } = (EventModuleSettings.DisplayLocations)0;

//        [ModuleSetting]
//        public bool _restrictlocations { get; set; } = false;

//        [ModuleSetting]
//        public bool _restrictlocationstotimeframe { get; set; } = false;

//        [ModuleSetting]
//        public bool _enablecontainerskin { get; set; } = true;

//        [ModuleSetting]
//        public bool _eventdetailnewpage { get; set; } = false;

//        [ModuleSetting]
//        public string _maxrecurrences { get; set; } = "1000";

//        [ModuleSetting]
//        public bool _enableenrollpopup { get; set; } = true;

//        [ModuleSetting]
//        public bool _eventdaynewpage { get; set; } = false;

//        [ModuleSetting]
//        public string _defaultView { get; set; } = "EventMonth.ascx";

//        [ModuleSetting]
//        public bool _eventnotify { get; set; } = true;

//        [ModuleSetting]
//        public bool _notifyanon { get; set; } = false;

//        [ModuleSetting]
//        public bool _sendreminderdefault { get; set; } = false;

//        [ModuleSetting]
//        public string _neweventemails { get; set; } = "Never";

//        [ModuleSetting]
//        public int _neweventemailrole { get; set; } = -1;

//        [ModuleSetting]
//        public bool _newpereventemail { get; set; } = false;

//        [ModuleSetting]
//        public bool _tzdisplay { get; set; } = false;

//        [ModuleSetting]
//        public string _paypalurl { get; set; } = "https://www.paypal.com";

//        [ModuleSetting]
//        public bool _disableEventnav { get; set; } = false;

//        [ModuleSetting]
//        public bool _collapserecurring { get; set; } = false;

//        [ModuleSetting]
//        public bool _fulltimescale { get; set; } = false;

//        [ModuleSetting]
//        public bool _includeendvalue { get; set; } = true;

//        [ModuleSetting]
//        public bool _showvaluemarks { get; set; } = false;

//        [ModuleSetting]
//        public bool _eventimage { get; set; } = true;

//        [ModuleSetting]
//        public bool _allowreoccurring { get; set; } = true;

//        [ModuleSetting]
//        public bool _eventsearch { get; set; } = true;

//        [ModuleSetting]
//        public bool _preventconflicts { get; set; } = false;

//        [ModuleSetting]
//        public bool _locationconflict { get; set; } = false;

//        [ModuleSetting]
//        public bool _showEventsAlways { get; set; } = false;

//        [ModuleSetting]
//        public bool _timeintitle { get; set; } = false;

//        [ModuleSetting]
//        public bool _monthdayselect { get; set; } = false;

//        [ModuleSetting]
//        public bool _masterEvent { get; set; } = false;

//        [ModuleSetting]
//        public bool _addsubmodulename { get; set; } = true;

//        [ModuleSetting]
//        public bool _enforcesubcalperms { get; set; } = true;

//        [ModuleSetting]
//        public bool _fridayweekend { get; set; } = false;

//        [ModuleSetting]
//        public bool _eventdefaultenrollview { get; set; } = false;

//        [ModuleSetting]
//        public string _paypalaccount { get; set; } = null;

//        [ModuleSetting]
//        public bool _moderateall { get; set; } = false;

//        [ModuleSetting]
//        public string _reminderfrom { get; set; } = null;

//        [ModuleSetting]
//        public string _eventsListSelectType { get; set; } = "EVENTS";

//        [ModuleSetting]
//        public int _eventsListNumEvents { get; set; } = 10;

//        [ModuleSetting]
//        public int _eventsListEventDays { get; set; } = 365;

//        [ModuleSetting]
//        public int _eventsListPageSize { get; set; } = 10;

//        [ModuleSetting]
//        public string _eventsListSortDirection { get; set; } = "ASC";

//        [ModuleSetting]
//        public bool _rssEnable { get; set; } = false;

//        [ModuleSetting]
//        public string _rssDateField { get; set; } = "UPDATEDDATE";

//        [ModuleSetting]
//        public int _rssDays { get; set; } = 365;

//        [ModuleSetting]
//        public string _rssTitle { get; set; } = null;

//        [ModuleSetting]
//        public string _rssDesc { get; set; } = null;

//        [ModuleSetting]
//        public string _expireevents { get; set; } = "";

//        [ModuleSetting]
//        public bool _exportowneremail { get; set; } = false;

//        [ModuleSetting]
//        public bool _exportanonowneremail { get; set; } = false;

//        [ModuleSetting]
//        public bool _monthAllowed { get; set; } = true;

//        [ModuleSetting]
//        public bool _weekAllowed { get; set; } = true;

//        [ModuleSetting]
//        public bool _listAllowed { get; set; } = true;

//        [ModuleSetting]
//        public bool _eventImageMonth { get; set; } = true;

//        [ModuleSetting]
//        public bool _eventImageWeek { get; set; } = false;

//        [ModuleSetting]
//        public string _iconBar { get; set; } = "TOP";

//        [ModuleSetting]
//        public string _enrollEditFields { get; set; } = "03;05;";

//        [ModuleSetting]
//        public string _enrollViewFields { get; set; } = "";

//        [ModuleSetting]
//        public string _enrollAnonFields { get; set; } = "02;06;";

//        [ModuleSetting]
//        public bool _eventhidefullenroll { get; set; } = false;

//        [ModuleSetting]
//        public int _maxnoenrolees { get; set; } = 1;

//        [ModuleSetting]
//        public int _enrolcanceldays { get; set; } = 0;

//        [ModuleSetting]
//        public bool _detailPageAllowed { get; set; } = false;

//        [ModuleSetting]
//        public bool _enrollmentPageAllowed { get; set; } = false;

//        [ModuleSetting]
//        public string _enrollmentPageDefaultUrl { get; set; } = "";

//        [ModuleSetting]
//        public bool _eventsCustomField1 { get; set; } = false;

//        [ModuleSetting]
//        public bool _eventsCustomField2 { get; set; } = false;

//        [ModuleSetting]
//        public bool _iconMonthPrio { get; set; } = true;

//        [ModuleSetting]
//        public bool _iconMonthRec { get; set; } = true;

//        [ModuleSetting]
//        public bool _iconMonthReminder { get; set; } = true;

//        [ModuleSetting]
//        public bool _iconMonthEnroll { get; set; } = true;

//        [ModuleSetting]
//        public bool _iconWeekPrio { get; set; } = true;

//        [ModuleSetting]
//        public bool _iconWeekRec { get; set; } = true;

//        [ModuleSetting]
//        public bool _iconWeekReminder { get; set; } = true;

//        [ModuleSetting]
//        public bool _iconWeekEnroll { get; set; } = true;

//        [ModuleSetting]
//        public bool _iconListPrio { get; set; } = true;

//        [ModuleSetting]
//        public bool _iconListRec { get; set; } = true;

//        [ModuleSetting]
//        public bool _iconListReminder { get; set; } = true;

//        [ModuleSetting]
//        public bool _iconListEnroll { get; set; } = true;

//        [ModuleSetting]
//        public string _publicMessage { get; set; } = "";

//        [ModuleSetting]
//        public string _eventTheme { get; set; } = "";

//        [ModuleSetting]
//        public string _eventThemeDefault { get; set; } = "0,MinimalExtropy,";

//        [ModuleSetting]
//        public string _eventsListFields { get; set; } = "BD;ED;EN";

//        [ModuleSetting]
//        public string _eventsListShowHeader { get; set; } = "Yes";

//        [ModuleSetting]
//        public int _eventsListBeforeDays { get; set; } = 1;

//        [ModuleSetting]
//        public int _eventsListAfterDays { get; set; } = 7;

//        [ModuleSetting]
//        public string _recurDummy { get; set; } = null;

//        [ModuleSetting]
//        public EventTemplates _templates { get; set; }

//        [ModuleSetting]
//        public int ModuleCategoryID { get; set; } = -1;

//        [ModuleSetting]
//        public ArrayList _moduleCategoryIDs { get; set; } = null;

//        [ModuleSetting]
//        public EventModuleSettings.CategoriesSelected _moduleCategoriesSelected { get; set; } = EventModuleSettings.CategoriesSelected.All;

//        [ModuleSetting]
//        public int ModuleLocationID { get; set; } = -1;

//        [ModuleSetting]
//        public ArrayList _moduleLocationIDs { get; set; } = null;

//        [ModuleSetting]
//        public EventModuleSettings.LocationsSelected _moduleLocationsSelected { get; set; } = EventModuleSettings.LocationsSelected.All;

//        [ModuleSetting]
//        public string _eventsListSortColumn { get; set; } = "EventDateBegin";

//        [ModuleSetting]
//        public string _htmlEmail { get; set; } = "html";

//        [ModuleSetting]
//        public int _iCalDaysBefore { get; set; } = 365;

//        [ModuleSetting]
//        public int _iCalDaysAfter { get; set; } = 365;

//        [ModuleSetting]
//        public string _icalURLAppend { get; set; } = "";

//        [ModuleSetting]
//        public string _icalDefaultImage { get; set; } = "";

//        [ModuleSetting]
//        public bool _icalOnIconBar { get; set; } = false;

//        [ModuleSetting]
//        public bool _icalEmailEnable { get; set; } = false;

//        [ModuleSetting]
//        public bool _iIcalURLInLocation { get; set; } = true;

//        [ModuleSetting]
//        public bool _iIcalIncludeCalname { get; set; } = false;

//        [ModuleSetting]
//        public bool _enableSEO { get; set; } = true;

//        [ModuleSetting]
//        public int _seoDescriptionLength { get; set; } = 256;

//        [ModuleSetting]
//        public bool _enableSitemap { get; set; } = false;

//        [ModuleSetting]
//        public float _siteMapPriority { get; set; } = (float)(0.5F);

//        [ModuleSetting]
//        public int _siteMapDaysBefore { get; set; } = 365;

//        [ModuleSetting]
//        public int _siteMapDaysAfter { get; set; } = 365;

//        [ModuleSetting]
//        public bool _listViewGrid { get; set; } = true;

//        [ModuleSetting]
//        public bool _listViewTable { get; set; } = true;

//        [ModuleSetting]
//        public int _rptColumns { get; set; } = 1;

//        [ModuleSetting]
//        public int _rptRows { get; set; } = 5;

//        [ModuleSetting]
//        public System.Web.UI.WebControls.FirstDayOfWeek _weekStart { get; set; } = System.Web.UI.WebControls.FirstDayOfWeek.Default;

//        [ModuleSetting]
//        public bool _listViewUseTime { get; set; } = false;

//        [ModuleSetting]
//        public string _standardEmail { get; set; } = null;

//        [ModuleSetting]
//        public string _fbAdmins { get; set; } = "";

//        [ModuleSetting]
//        public string _fbAppID { get; set; } = "";

//        [ModuleSetting]
//        public int _maxThumbWidth { get; set; } = 125;

//        [ModuleSetting]
//        public int _maxThumbHeight { get; set; } = 125;

//        [ModuleSetting]
//        public bool _sendEnrollMessageApproved { get; set; } = true;

//        [ModuleSetting]
//        public bool _sendEnrollMessageWaiting { get; set; } = true;

//        [ModuleSetting]
//        public bool _sendEnrollMessageDenied { get; set; } = true;

//        [ModuleSetting]
//        public bool _sendEnrollMessageAdded { get; set; } = true;

//        [ModuleSetting]
//        public bool _sendEnrollMessageDeleted { get; set; } = true;

//        [ModuleSetting]
//        public bool _sendEnrollMessagePaying { get; set; } = true;

//        [ModuleSetting]
//        public bool _sendEnrollMessagePending { get; set; } = true;

//        [ModuleSetting]
//        public bool _sendEnrollMessagePaid { get; set; } = true;

//        [ModuleSetting]
//        public bool _sendEnrollMessageIncorrect { get; set; } = true;

//        [ModuleSetting]
//        public bool _sendEnrollMessageCancelled { get; set; } = true;

//        [ModuleSetting]
//        public bool _allowanonenroll { get; set; } = false;

//        [ModuleSetting]
//        public EventModuleSettings.SocialModule _socialGroupModule { get; set; } = EventModuleSettings.SocialModule.No;

//        [ModuleSetting]
//        public SortDirection _enrolListSortDirection { get; set; } = SortDirection.Descending;

//        [ModuleSetting]
//        public int _enrolListDaysBefore { get; set; } = (365 * 4) + 1;

//        [ModuleSetting]
//        public int _enrolListDaysAfter { get; set; } = (365 * 4) + 1;

//        [ModuleSetting]
//        public bool _journalIntegration { get; set; } = true;

//        [ModuleSetting]
//        public bool _socialUserpublic { get; set; } = true;

//        [ModuleSetting]
//        public EventModuleSettings.SocialGroupPrivacy _socialGroupSecurity { get; set; } = EventModuleSettings.SocialGroupPrivacy.EditByGroup;
//    }
//}