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

namespace Components
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Web.UI.WebControls;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Entities.Modules.Settings;
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Modules.Events;
    using DotNetNuke.Services.Localization;
    
        [Serializable]
    public class EventModuleSettings
    {
        public enum CategoriesSelected
        {
            All = 1,
            Some,
            None
        }

        public enum DisplayCategories
        {
            DoNotDisplay = 1,
            SingleSelect,
            MultiSelect
        }

        public enum DisplayLocations
        {
            DoNotDisplay = 1,
            SingleSelect,
            MultiSelect
        }

        public enum LocationsSelected
        {
            All = 1,
            Some,
            None
        }

        public enum SocialGroupPrivacy
        {
            OpenToAll = 1,
            EditByGroup,
            PrivateToGroup
        }

        public enum SocialModule
        {
            No = 1,
            SocialGroup,
            UserProfile
        }

        public enum TimeZones
        {
            UserTZ = 1,
            ModuleTZ,
            PortalTZ
        }

        #region  Public Methods

        public static EventModuleSettings GetEventModuleSettings(int moduleID, string localResourceFile)
        {
            if (moduleID > 0)
            {
                var cacheKey = "EventsSettings" + moduleID;
                var bs = default(EventModuleSettings);
                bs = (EventModuleSettings) DataCache.GetCache(cacheKey);
                if (ReferenceEquals(bs, null))
                {
                    bs = CreateEventModuleSettings(moduleID, localResourceFile);
                    if (!string.IsNullOrEmpty(localResourceFile))
                    {
                        DataCache.SetCache(cacheKey, bs);
                    }
                }
                return bs;
            }
            return null;
        }

        #endregion

        #region  Private Methods

        private void UpdateDefaults()
        {
            var vers = Version;
            if (OnlyView != null && (bool) OnlyView)
            {
                if (DefaultView == "EventList.ascx")
                {
                    ListAllowed = true;
                }
                else
                {
                    ListAllowed = false;
                }
                if (DefaultView == "EventWeek.ascx")
                {
                    WeekAllowed = true;
                }
                else
                {
                    WeekAllowed = false;
                }
                if (DefaultView == "EventMonth.ascx")
                {
                    MonthAllowed = true;
                }
                else
                {
                    MonthAllowed = false;
                }
            }
            if (string.IsNullOrEmpty(vers))
            {
                Enforcesubcalperms = true;
            }
            else
            {
                Enforcesubcalperms = false;
            }
            if (AllowSubscriptions != null && (bool) AllowSubscriptions)
            {
                Neweventemails = "Subscribe";
            }
            if (string.IsNullOrEmpty(vers))
            {
                Enablecontainerskin = true;
            }
            else if (int.Parse(vers.Substring(0, vers.IndexOf(".") + 0)) < 5)
            {
                Enablecontainerskin = false;
            }
            else
            {
                Enablecontainerskin = true;
            }
            if (Eventtooltip != null)
            {
                Eventtooltipmonth = (bool) Eventtooltip;
                Eventtooltipweek = (bool) Eventtooltip;
                Eventtooltipday = (bool) Eventtooltip;
                Eventtooltiplist = (bool) Eventtooltip;
            }

            if (SocialGroupModule != null)
            {
                if ((SocialModule) Enum.Parse(typeof(SocialModule), SocialGroupModule.ToString(), true) ==
                    SocialModule.UserProfile)
                {
                    _socialUserPrivate = false;
                }
                //if ((SocialModule)Convert.ToInt32(this._allsettings["SocialGroupModule"]) == SocialModule.SocialGroup)
                if ((SocialModule) Enum.Parse(typeof(SocialModule), SocialGroupModule.ToString(), true) ==
                    SocialModule.SocialGroup)
                {
                    _socialGroupSecurity = SocialGroupPrivacy.OpenToAll;
                }
            }
        }

        #endregion

        #region  Private Members

        private readonly Hashtable _allsettings;
        private int _moduleID = -1; // Should be made obsolete
        private string _localresourcefile; // Should be made obsolete

        private string _timeZone;
        private string _timeZoneId = string.Empty;

        // ReSharper disable FieldCanBeMadeReadOnly.Local
        private bool _disablecategories = false;
        // ReSharper restore FieldCanBeMadeReadOnly.Local

        private DisplayCategories _enablecategories = 0;

        // ReSharper disable FieldCanBeMadeReadOnly.Local
        private bool _disablelocations;
        // ReSharper restore FieldCanBeMadeReadOnly.Local

        private DisplayLocations _enablelocations = 0;
        private bool _enablecontainerskin = true;
        private string _neweventemails = "Never";
        private int _neweventemailrole = -1;
        private string _paypalaccount = string.Empty;
        private string _reminderfrom = string.Empty;
        private string _rssTitle = string.Empty;
        private string _rssDesc = string.Empty;
        private string _eventTheme = string.Empty;
        private readonly string _eventThemeDefault = "0,MinimalExtropy,";
        private const int ModuleCategoryID = -1;
        private ArrayList _moduleCategoryIDs;
        private CategoriesSelected _moduleCategoriesSelected = CategoriesSelected.All;
        private const int ModuleLocationID = -1;
        private ArrayList _moduleLocationIDs;
        private LocationsSelected _moduleLocationsSelected = LocationsSelected.All;
        private FirstDayOfWeek _weekStart = FirstDayOfWeek.Default;
        private string _standardEmail = string.Empty;
        private bool _socialUserPrivate = true;
        private SocialGroupPrivacy _socialGroupSecurity = SocialGroupPrivacy.EditByGroup;

        #endregion

        #region  Constructors

        private static EventModuleSettings CreateEventModuleSettings(int moduleId, string localResourceFile)
        {
            var moduleController = new ModuleController();
            var moduleInfo = moduleController.GetModule(moduleId);

            var repository = new EventModuleSettingsRepository();
            var settings = repository.GetSettings(moduleInfo);

            settings._moduleID = moduleId;
            settings._localresourcefile = localResourceFile;

            settings.UpdateDefaults();
            settings.Templates = new EventTemplates(moduleId, moduleInfo.ModuleSettings, localResourceFile);

            return settings;
        }

        #endregion

        #region  Properties

        [ModuleSetting]
        public int MaxThumbWidth { get; set; } = 125;

        [ModuleSetting]
        public int MaxThumbHeight { get; set; } = 125;

        [ModuleSetting]
        public string FBAppID { get; set; } = string.Empty;

        [ModuleSetting]
        public string FBAdmins { get; set; } = string.Empty;

        [ModuleSetting]
        public string StandardEmail
        {
            get
            {
                if (string.IsNullOrEmpty(_standardEmail))
                {
                    var portalsettings = PortalController.GetCurrentPortalSettings();
                    if (!ReferenceEquals(portalsettings, null))
                    {
                        _standardEmail = portalsettings.Email;
                    }
                }
                return _standardEmail;
            }
            set { _standardEmail = value; }
        }

        [ModuleSetting(ParameterName = "iCalIncludeCalname")]
        public bool IcalIncludeCalname { get; set; }

        [ModuleSetting(ParameterName = "iCalEmailEnable")]
        public bool IcalEmailEnable { get; set; }

        [ModuleSetting(ParameterName = "iCalURLinLocation")]
        public bool IcalURLInLocation { get; set; } = true;

        [ModuleSetting(ParameterName = "iCalOnIconBar")]
        public bool IcalOnIconBar { get; set; }

        [ModuleSetting]
        public bool ListViewUseTime { get; set; }

        [ModuleSetting]
        public FirstDayOfWeek WeekStart { get; set; } = FirstDayOfWeek.Default;

        [ModuleSetting(ParameterName = "rptRows")]
        public int RptRows { get; set; } = 5;

        [ModuleSetting(ParameterName = "rptColumns")]
        public int RptColumns { get; set; } = 1;

        [ModuleSetting]
        public bool ListViewTable { get; set; } = true;

        [ModuleSetting]
        public bool ListViewGrid { get; set; } = true;

        [ModuleSetting(ParameterName = "siteMapDaysAfter")]
        public int SiteMapDaysAfter { get; set; } = 365;

        [ModuleSetting(ParameterName = "siteMapDaysBefore")]
        public int SiteMapDaysBefore { get; set; } = 365;

        [ModuleSetting(ParameterName = "siteMapPriority")]
        public float SiteMapPriority { get; set; } = 0.5F;

        [ModuleSetting(ParameterName = "enableSitemap")]
        public bool EnableSitemap { get; set; }

        [ModuleSetting]
        public int SEODescriptionLength { get; set; } = 256;

        [ModuleSetting(ParameterName = "enableSEO")]
        public bool EnableSEO { get; set; } = true;

        [ModuleSetting(ParameterName = "iCalDefaultImage")]
        public string IcalDefaultImage { get; set; } = string.Empty;

        [ModuleSetting(ParameterName = "iCalURLAppend")]
        public string IcalURLAppend { get; set; } = string.Empty;

        [ModuleSetting(ParameterName = "iCalDaysAfter")]
        public int IcalDaysAfter { get; set; } = 365;

        [ModuleSetting(ParameterName = "iCalDaysBefore")]
        public int IcalDaysBefore { get; set; } = 365;

        [ModuleSetting]
        public string HTMLEmail { get; set; } = "html";

        [ModuleSetting(ParameterName = "eventslistsortcolumn")]
        public string EventsListSortColumn { get; set; } = "EventDateBegin";

        public ArrayList ModuleCategoryIDs
        {
            get
            {
                if (ReferenceEquals(_moduleCategoryIDs, null))
                {
                    var arCat = new ArrayList();
                    arCat.Add(ModuleCategoryID);
                    _moduleCategoryIDs = arCat;
                }
                return _moduleCategoryIDs;
            }
            set { _moduleCategoryIDs = value; }
        }

        [ModuleSetting(ParameterName = "ModuleCategoryIds")]
        public string ModuleCategoryIdsList
        {
            get { return string.Join(";", ModuleCategoryIDs ?? new ArrayList()); }

            set
            {
                ModuleCategoryIDs = !string.IsNullOrWhiteSpace(value)
                                             ? new ArrayList(
                                                 value.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries)
                                                      .Select(arg => arg)
                                                      .ToArray())
                                             : new ArrayList();
            }
        }


        public CategoriesSelected ModuleCategoriesSelected
        {
            get
            {
                int moduleCatAll = 0;
                if (ModuleCategoryIDs.Count == 0)
                {
                    _moduleCategoriesSelected = CategoriesSelected.None;
                }
                else
                {
                    moduleCatAll = int.TryParse(ModuleCategoryIDs[0] as string, out moduleCatAll) ? moduleCatAll : -1;
                    if (moduleCatAll == -1)
                    {
                        _moduleCategoriesSelected = CategoriesSelected.All;
                    }
                    else
                    {
                        _moduleCategoriesSelected = CategoriesSelected.Some;
                    }
                }
                return _moduleCategoriesSelected;
            }
            set { _moduleCategoriesSelected = value; }
        }

        [ModuleSetting]
        public ArrayList ModuleLocationIDs
        {
            get
            {
                if (ReferenceEquals(_moduleLocationIDs, null))
                {
                    var arLoc = new ArrayList();
                    arLoc.Add(ModuleLocationID);
                    _moduleLocationIDs = arLoc;
                }
                return _moduleLocationIDs;
            }
            set { _moduleLocationIDs = value; }
        }

        public LocationsSelected ModuleLocationsSelected
        {
            get
            {
                if (ModuleLocationIDs.Count == 0)
                {
                    _moduleLocationsSelected = LocationsSelected.None;
                }
                else if (Convert.ToInt32(ModuleLocationIDs[0]) == -1)
                {
                    _moduleLocationsSelected = LocationsSelected.All;
                }
                else
                {
                    _moduleLocationsSelected = LocationsSelected.Some;
                }
                return _moduleLocationsSelected;
            }
            set { _moduleLocationsSelected = value; }
        }

        [ModuleSetting]
        public string RecurDummy { get; set; } = string.Empty;

        [ModuleSetting]
        public int EventsListAfterDays { get; set; } = 7;

        [ModuleSetting]
        public int EventsListBeforeDays { get; set; } = 1;

        [ModuleSetting]
        public string EventsListShowHeader { get; set; } = "Yes";

        [ModuleSetting]
        public string EventsListFields { get; set; } = "BD;ED;EN";

        [ModuleSetting]
        public string EventThemeDefault { get; set; } = "0,MinimalExtropy,";

        [ModuleSetting]
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
            set { _eventTheme = value; }
        }

        [ModuleSetting]
        public string PrivateMessage { get; set; } = string.Empty;

        [ModuleSetting]
        public bool IconListEnroll { get; set; } = true;

        [ModuleSetting]
        public bool IconListReminder { get; set; } = true;

        [ModuleSetting]
        public bool IconListRec { get; set; } = true;

        [ModuleSetting]
        public bool IconListPrio { get; set; } = true;

        [ModuleSetting]
        public bool IconWeekEnroll { get; set; } = true;

        [ModuleSetting]
        public bool IconWeekReminder { get; set; } = true;

        [ModuleSetting]
        public bool IconWeekRec { get; set; } = true;

        [ModuleSetting]
        public bool IconWeekPrio { get; set; } = true;

        [ModuleSetting]
        public bool IconMonthEnroll { get; set; } = true;

        [ModuleSetting]
        public bool IconMonthReminder { get; set; } = true;

        [ModuleSetting]
        public bool IconMonthRec { get; set; } = true;

        [ModuleSetting]
        public bool IconMonthPrio { get; set; } = true;

        [ModuleSetting]
        public bool EventsCustomField2 { get; set; }

        [ModuleSetting]
        public bool EventsCustomField1 { get; set; }

        [ModuleSetting]
        public bool DetailPageAllowed { get; set; }

        [ModuleSetting]
        public bool EnrollmentPageAllowed { get; set; }

        [ModuleSetting]
        public string EnrollmentPageDefaultUrl { get; set; } = string.Empty;

        [ModuleSetting(ParameterName = "enrolcanceldays")]
        public int Enrolcanceldays { get; set; } = 0;

        [ModuleSetting(ParameterName = "maxnoenrolees")]
        public int Maxnoenrolees { get; set; } = 1;

        [ModuleSetting(ParameterName = "eventhidefullenroll")]
        public bool Eventhidefullenroll { get; set; }

        [ModuleSetting]
        public string EnrollAnonFields { get; set; } = "02;06;";

        [ModuleSetting]
        public string EnrollViewFields { get; set; } = string.Empty;

        [ModuleSetting]
        public string EnrollEditFields { get; set; } = "03;05;";

        [ModuleSetting]
        public string IconBar { get; set; } = "TOP";

        [ModuleSetting]
        public bool EventImageWeek { get; set; }

        [ModuleSetting]
        public bool EventImageMonth { get; set; } = true;

        [ModuleSetting]
        public bool ListAllowed { get; set; } = true;

        [ModuleSetting]
        public bool WeekAllowed { get; set; } = true;

        [ModuleSetting]
        public bool MonthAllowed { get; set; } = true;

        [ModuleSetting(ParameterName = "exportanonowneremail")]
        public bool Exportanonowneremail { get; set; }

        [ModuleSetting(ParameterName = "exportowneremail")]
        public bool Exportowneremail { get; set; }

        [ModuleSetting(ParameterName = "expireevents")]
        public string Expireevents { get; set; } = string.Empty;

        [ModuleSetting]
        public string RSSDesc
        {
            get
            {
                if (string.IsNullOrEmpty(_rssDesc) && !string.IsNullOrEmpty(_localresourcefile))
                {
                    _rssDesc = Localization.GetString("RSSFeedDescDefault", _localresourcefile);
                }
                return _rssDesc;
            }
            set { _rssDesc = value; }
        }

        [ModuleSetting]
        public string RSSTitle
        {
            get
            {
                if (string.IsNullOrEmpty(_rssTitle) && !string.IsNullOrEmpty(_localresourcefile))
                {
                    _rssTitle = Localization.GetString("RSSFeedTitleDefault", _localresourcefile);
                }
                return _rssTitle;
            }
            set { _rssTitle = value; }
        }

        [ModuleSetting]
        public int RSSDays { get; set; } = 365;

        [ModuleSetting]
        public string RSSDateField { get; set; } = "UPDATEDDATE";

        [ModuleSetting]
        public bool RSSEnable { get; set; }

        [ModuleSetting]
        public string EventsListSortDirection { get; set; } = "ASC";

        [ModuleSetting]
        public int EventsListPageSize { get; set; } = 10;

        [ModuleSetting]
        public int EventsListEventDays { get; set; } = 365;

        [ModuleSetting]
        public int EventsListNumEvents { get; set; } = 10;

        [ModuleSetting]
        public string EventsListSelectType { get; set; } = "EVENTS";

        [ModuleSetting(ParameterName = "reminderfrom")]
        public string Reminderfrom
        {
            get
            {
                if (string.IsNullOrEmpty(_reminderfrom))
                {
                    var portalsettings = PortalController.GetCurrentPortalSettings();
                    if (!ReferenceEquals(portalsettings, null))
                    {
                        _reminderfrom = portalsettings.Email;
                    }
                }
                return _reminderfrom;
            }
            set { _reminderfrom = value; }
        }

        [ModuleSetting(ParameterName = "moderateall")]
        public bool Moderateall { get; set; }

        [ModuleSetting(ParameterName = "paypalaccount")]
        public string Paypalaccount
        {
            get
            {
                if (ReferenceEquals(_paypalaccount, null))
                {
                    var portalsettings = PortalController.GetCurrentPortalSettings();
                    if (!ReferenceEquals(portalsettings, null))
                    {
                        _paypalaccount = portalsettings.Email;
                    }
                }
                return _paypalaccount;
            }
            set { _paypalaccount = value; }
        }

        [ModuleSetting(ParameterName = "eventdefaultenrollview")]
        public bool Eventdefaultenrollview { get; set; }

        [ModuleSetting(ParameterName = "fridayweekend")]
        public bool Fridayweekend { get; set; }

        [ModuleSetting(ParameterName = "enforcesubcalperms")]
        public bool Enforcesubcalperms { get; set; } = true;

        [ModuleSetting(ParameterName = "addsubmodulename")]
        public bool Addsubmodulename { get; set; } = true;

        [ModuleSetting(ParameterName = "masterEvent")]
        public bool MasterEvent { get; set; }

        [ModuleSetting(ParameterName = "monthdayselect")]
        public bool Monthdayselect { get; set; }

        [ModuleSetting(ParameterName = "timeintitle")]
        public bool Timeintitle { get; set; }

        [ModuleSetting(ParameterName = "showEventsAlways")]
        public bool ShowEventsAlways { get; set; }

        [ModuleSetting(ParameterName = "locationconflict")]
        public bool Locationconflict { get; set; }

        [ModuleSetting(ParameterName = "preventconflicts")]
        public bool Preventconflicts { get; set; }

        [ModuleSetting]
        public bool Eventsearch { get; set; } = true;

        [ModuleSetting(ParameterName = "allowreoccurring")]
        public bool Allowreoccurring { get; set; } = true;

        [ModuleSetting(ParameterName = "eventimage")]
        public bool Eventimage { get; set; } = true;

        [ModuleSetting(ParameterName = "showvaluemarks")]
        public bool Showvaluemarks { get; set; }

        [ModuleSetting(ParameterName = "includeendvalue")]
        public bool Includeendvalue { get; set; } = true;

        [ModuleSetting(ParameterName = "fulltimescale")]
        public bool Fulltimescale { get; set; }

        [ModuleSetting(ParameterName = "collapserecurring")]
        public bool Collapserecurring { get; set; }

        [ModuleSetting(ParameterName = "disableEventnav")]
        public bool DisableEventnav { get; set; }

        [ModuleSetting(ParameterName = "paypalurl")]
        public string Paypalurl { get; set; } = "https://www.paypal.com";

        [ModuleSetting(ParameterName = "tzdisplay")]
        public bool Tzdisplay { get; set; }

        [ModuleSetting(ParameterName = "newpereventemail")]
        public bool Newpereventemail { get; set; }

        [ModuleSetting(ParameterName = "neweventemailrole")]
        public int Neweventemailrole
        {
            get
            {
                if (_neweventemailrole < 0)
                {
                    var portalsettings = PortalController.GetCurrentPortalSettings();
                    if (!ReferenceEquals(portalsettings, null))
                    {
                        _neweventemailrole = portalsettings.RegisteredRoleId;
                    }
                }
                return _neweventemailrole;
            }
            set { _neweventemailrole = value; }
        }

        [ModuleSetting(ParameterName = "neweventemails")]
        public string Neweventemails { get; set; } = "Never";

        [ModuleSetting(ParameterName = "sendreminderdefault")]
        public bool Sendreminderdefault { get; set; }

        [ModuleSetting(ParameterName = "notifyanon")]
        public bool Notifyanon { get; set; }

        [ModuleSetting]
        public bool Eventnotify { get; set; } = true;

        [ModuleSetting]
        public string DefaultView { get; set; } = "EventMonth.ascx";

        [ModuleSetting(ParameterName = "eventdaynewpage")]
        public bool Eventdaynewpage { get; set; }

        [ModuleSetting(ParameterName = "enableenrollpopup")]
        public bool Enableenrollpopup { get; set; } = true;

        [ModuleSetting(ParameterName = "maxrecurrences")]
        public string Maxrecurrences { get; set; } = "1000";

        [ModuleSetting(ParameterName = "version")]
        public string Version { get; set; } = string.Empty;

        [ModuleSetting(ParameterName = "timeinterval")]
        public string Timeinterval { get; set; } = "30";

        [ModuleSetting]
        public string TimeZoneId
        {
            get
            {
                if (string.IsNullOrEmpty(_timeZoneId))
                {
                    if (string.IsNullOrEmpty(_timeZone))
                    {
                        var portalsettings = PortalController.GetCurrentPortalSettings();
                        if (!ReferenceEquals(portalsettings, null))
                        {
                            _timeZoneId = portalsettings.TimeZone.Id;
                        }
                    }
                    else
                    {
                        _timeZoneId = Localization
                            .ConvertLegacyTimeZoneOffsetToTimeZoneInfo(int.Parse(_timeZone)).Id;
                    }
                }
                return _timeZoneId;
            }
            set { _timeZoneId = value; }
        }

        [ModuleSetting]
        public bool EnableEventTimeZones { get; set; } = false;

        [ModuleSetting]
        public TimeZones PrimaryTimeZone { get; set; } = TimeZones.UserTZ;

        [ModuleSetting]
        public TimeZones SecondaryTimeZone { get; set; } = TimeZones.PortalTZ;

        [ModuleSetting(ParameterName = "eventtooltiplist")]
        public bool Eventtooltiplist { get; set; } = true;

        [ModuleSetting(ParameterName = "eventtooltipday")]
        public bool Eventtooltipday { get; set; } = true;

        [ModuleSetting(ParameterName = "eventtooltipweek")]
        public bool Eventtooltipweek { get; set; } = true;

        [ModuleSetting(ParameterName = "eventtooltipmonth")]
        public bool Eventtooltipmonth { get; set; } = true;

        [ModuleSetting(ParameterName = "eventtooltiplength")]
        public int Eventtooltiplength { get; set; } = 10000;

        [ModuleSetting(ParameterName = "monthcellnoevents")]
        public bool Monthcellnoevents { get; set; } = false;

        [ModuleSetting(ParameterName = "restrictcategories")]
        public bool Restrictcategories { get; set; } = false;

        [ModuleSetting(ParameterName = "restrictcategoriestotimeframe")]
        public bool RestrictCategoriesToTimeFrame { get; set; }

        [ModuleSetting(ParameterName = "enablecategories")]
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
                    return DisplayCategories.MultiSelect;
                }
                return _enablecategories;
            }
            set { _enablecategories = value; }
        }

        [ModuleSetting(ParameterName = "restrictlocations")]
        public bool Restrictlocations { get; set; }

        [ModuleSetting(ParameterName = "restrictlocationstotimeframe")]
        public bool RestrictLocationsToTimeFrame { get; set; }

        [ModuleSetting]
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
                    return DisplayLocations.MultiSelect;
                }
                return _enablelocations;
            }
            set { _enablelocations = value; }
        }

        [ModuleSetting(ParameterName = "enablecontainerskin")]
        public bool Enablecontainerskin { get; set; } = true;

        [ModuleSetting(ParameterName = "eventdetailnewpage")]
        public bool Eventdetailnewpage { get; set; }

        [ModuleSetting(ParameterName = "ownerchangeallowed")]
        public bool Ownerchangeallowed { get; set; } = false;

        [ModuleSetting(ParameterName = "eventsignupallowpaid")]
        public bool Eventsignupallowpaid { get; set; } = true;

        [ModuleSetting(ParameterName = "eventsignup")]
        public bool Eventsignup { get; set; } = false;

        [ModuleSetting]
        public bool SendEnrollMessageApproved { get; set; } = true;

        [ModuleSetting]
        public bool SendEnrollMessageWaiting { get; set; } = true;

        [ModuleSetting]
        public bool SendEnrollMessageDenied { get; set; } = true;

        [ModuleSetting]
        public bool SendEnrollMessageAdded { get; set; } = true;

        [ModuleSetting]
        public bool SendEnrollMessageDeleted { get; set; } = true;

        [ModuleSetting]
        public bool SendEnrollMessagePaying { get; set; } = true;

        [ModuleSetting]
        public bool SendEnrollMessagePending { get; set; } = true;

        [ModuleSetting]
        public bool SendEnrollMessagePaid { get; set; } = true;

        [ModuleSetting]
        public bool SendEnrollMessageIncorrect { get; set; } = true;

        [ModuleSetting]
        public bool SendEnrollMessageCancelled { get; set; } = true;

        [ModuleSetting(ParameterName = "allowanonenroll")]
        public bool AllowAnonEnroll { get; set; }

        [ModuleSetting]
        public SocialModule? SocialGroupModule { get; set; } = SocialModule.No;

        [ModuleSetting]
        public bool SocialUserPrivate { get; set; } = true;

        [ModuleSetting]
        public SocialGroupPrivacy SocialGroupSecurity { get; set; } = SocialGroupPrivacy.EditByGroup;

        [ModuleSetting]
        public SortDirection EnrolListSortDirection { get; set; } = SortDirection.Descending;

        [ModuleSetting]
        public int EnrolListDaysBefore { get; set; } = 365 * 4 + 1;

        [ModuleSetting]
        public int EnrolListDaysAfter { get; set; } = 365 * 4 + 1;

        [ModuleSetting]
        public bool JournalIntegration { get; set; } = true;

        [ModuleSetting(ParameterName = "onlyview")]
        public bool? OnlyView { get; set; }

        [ModuleSetting(ParameterName = "allowsubscriptions")]
        public bool? AllowSubscriptions { get; set; }

        [ModuleSetting(ParameterName = "eventtooltip")]
        public bool? Eventtooltip { get; set; }

        public EventTemplates Templates { get; set; }

        #endregion
    }
}