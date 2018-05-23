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
                    if (!ReferenceEquals(localResourceFile, null))
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
            var vers = this.Version;
            if (this.OnlyView != null && (bool) this.OnlyView)
            {
                if (this.DefaultView == "EventList.ascx")
                {
                    this.ListAllowed = true;
                }
                else
                {
                    this.ListAllowed = false;
                }
                if (this.DefaultView == "EventWeek.ascx")
                {
                    this.WeekAllowed = true;
                }
                else
                {
                    this.WeekAllowed = false;
                }
                if (this.DefaultView == "EventMonth.ascx")
                {
                    this.MonthAllowed = true;
                }
                else
                {
                    this.MonthAllowed = false;
                }
            }
            if (string.IsNullOrEmpty(vers))
            {
                this.Enforcesubcalperms = true;
            }
            else
            {
                this.Enforcesubcalperms = false;
            }
            if (this.AllowSubscriptions != null && (bool) this.AllowSubscriptions)
            {
                this.Neweventemails = "Subscribe";
            }
            if (string.IsNullOrEmpty(vers))
            {
                this.Enablecontainerskin = true;
            }
            else if (int.Parse(vers.Substring(0, vers.IndexOf(".") + 0)) < 5)
            {
                this.Enablecontainerskin = false;
            }
            else
            {
                this.Enablecontainerskin = true;
            }
            if (this.Eventtooltip != null)
            {
                this.Eventtooltipmonth = (bool) this.Eventtooltip;
                this.Eventtooltipweek = (bool) this.Eventtooltip;
                this.Eventtooltipday = (bool) this.Eventtooltip;
                this.Eventtooltiplist = (bool) this.Eventtooltip;
            }

            if (this.SocialGroupModule != null)
            {
                if ((SocialModule) Enum.Parse(typeof(SocialModule), this.SocialGroupModule.ToString(), true) ==
                    SocialModule.UserProfile)
                {
                    this._socialUserPrivate = false;
                }
                //if ((SocialModule)Convert.ToInt32(this._allsettings["SocialGroupModule"]) == SocialModule.SocialGroup)
                if ((SocialModule) Enum.Parse(typeof(SocialModule), this.SocialGroupModule.ToString(), true) ==
                    SocialModule.SocialGroup)
                {
                    this._socialGroupSecurity = SocialGroupPrivacy.OpenToAll;
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
                    if (ReferenceEquals(this._standardEmail, null))
                    {
                        var portalsettings = PortalController.GetCurrentPortalSettings();
                        if (!ReferenceEquals(portalsettings, null))
                        {
                            this._standardEmail = portalsettings.Email;
                        }
                    }
                    return this._standardEmail;
                }
            set { this._standardEmail = value; }
        }

        public bool IcalIncludeCalname { get; set; }

        [ModuleSetting]
        public bool IcalEmailEnable { get; set; }

        public bool IcalURLInLocation { get; set; } = true;

        [ModuleSetting]
        public bool IcalOnIconBar { get; set; }

        [ModuleSetting]
        public bool ListViewUseTime { get; set; }

        [ModuleSetting]
        public FirstDayOfWeek WeekStart { get; set; } = FirstDayOfWeek.Default;

        [ModuleSetting]
        public int RptRows { get; set; } = 5;

        [ModuleSetting]
        public int RptColumns { get; set; } = 1;

        [ModuleSetting]
        public bool ListViewTable { get; set; } = true;

        [ModuleSetting]
        public bool ListViewGrid { get; set; } = true;

        [ModuleSetting]
        public int SiteMapDaysAfter { get; set; } = 365;

        [ModuleSetting]
        public int SiteMapDaysBefore { get; set; } = 365;

        [ModuleSetting]
        public float SiteMapPriority { get; set; } = 0.5F;

        [ModuleSetting]
        public bool EnableSitemap { get; set; }

        [ModuleSetting]
        public int SEODescriptionLength { get; set; } = 256;

        [ModuleSetting]
        public bool EnableSEO { get; set; } = true;

        [ModuleSetting]
        public string IcalDefaultImage { get; set; } = string.Empty;

        [ModuleSetting]
        public string IcalURLAppend { get; set; } = string.Empty;

        [ModuleSetting]
        public int IcalDaysAfter { get; set; } = 365;

        [ModuleSetting]
        public int IcalDaysBefore { get; set; } = 365;

        [ModuleSetting]
        public string HTMLEmail { get; set; } = "html";

        [ModuleSetting]
        public string EventsListSortColumn { get; set; } = "EventDateBegin";

        public ArrayList ModuleCategoryIDs
        {
            get
                {
                    if (ReferenceEquals(this._moduleCategoryIDs, null))
                    {
                        var arCat = new ArrayList();
                        arCat.Add(ModuleCategoryID);
                        this._moduleCategoryIDs = arCat;
                    }
                    return this._moduleCategoryIDs;
                }
            set { this._moduleCategoryIDs = value; }
        }

        [ModuleSetting(ParameterName = "ModuleCategoryIds")]
        public string ModuleCategoryIdsList
        {
            get { return string.Join(";", this.ModuleCategoryIDs ?? new ArrayList()); }

            set
                {
                    this.ModuleCategoryIDs = !string.IsNullOrWhiteSpace(value)
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
                    if (this.ModuleCategoryIDs.Count == 0)
                    {
                        this._moduleCategoriesSelected = CategoriesSelected.None;
                    }
                    else if (Convert.ToInt32(this.ModuleCategoryIDs[0]) == -1)
                    {
                        this._moduleCategoriesSelected = CategoriesSelected.All;
                    }
                    else
                    {
                        this._moduleCategoriesSelected = CategoriesSelected.Some;
                    }
                    return this._moduleCategoriesSelected;
                }
            set { this._moduleCategoriesSelected = value; }
        }

        [ModuleSetting]
        public ArrayList ModuleLocationIDs
        {
            get
                {
                    if (ReferenceEquals(this._moduleLocationIDs, null))
                    {
                        var arLoc = new ArrayList();
                        arLoc.Add(ModuleLocationID);
                        this._moduleLocationIDs = arLoc;
                    }
                    return this._moduleLocationIDs;
                }
            set { this._moduleLocationIDs = value; }
        }

        public LocationsSelected ModuleLocationsSelected
        {
            get
                {
                    if (this.ModuleLocationIDs.Count == 0)
                    {
                        this._moduleLocationsSelected = LocationsSelected.None;
                    }
                    else if (Convert.ToInt32(this.ModuleLocationIDs[0]) == -1)
                    {
                        this._moduleLocationsSelected = LocationsSelected.All;
                    }
                    else
                    {
                        this._moduleLocationsSelected = LocationsSelected.Some;
                    }
                    return this._moduleLocationsSelected;
                }
            set { this._moduleLocationsSelected = value; }
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
                    if (string.IsNullOrEmpty(this._eventTheme))
                    {
                        this._eventTheme = this._eventThemeDefault;
                    }
                    return this._eventTheme;
                }
            set { this._eventTheme = value; }
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

        [ModuleSetting]
        public int Enrolcanceldays { get; set; } = 0;

        [ModuleSetting]
        public int Maxnoenrolees { get; set; } = 1;

        [ModuleSetting]
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

        [ModuleSetting]
        public bool Exportanonowneremail { get; set; }

        [ModuleSetting]
        public bool Exportowneremail { get; set; }

        [ModuleSetting]
        public string Expireevents { get; set; } = string.Empty;

        [ModuleSetting]
        public string RSSDesc
        {
            get
                {
                    if (string.IsNullOrEmpty(this._rssDesc) && !ReferenceEquals(this._localresourcefile, null))
                    {
                        this._rssDesc = Localization.GetString("RSSFeedDescDefault", this._localresourcefile);
                    }
                    return this._rssDesc;
                }
            set { this._rssDesc = value; }
        }

        [ModuleSetting]
        public string RSSTitle
        {
            get
                {
                    if (string.IsNullOrEmpty(this._rssTitle) && !ReferenceEquals(this._localresourcefile, null))
                    {
                        this._rssTitle = Localization.GetString("RSSFeedTitleDefault", this._localresourcefile);
                    }
                    return this._rssTitle;
                }
            set { this._rssTitle = value; }
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

        [ModuleSetting]
        public string Reminderfrom
        {
            get
                {
                    if (ReferenceEquals(this._reminderfrom, null))
                    {
                        var portalsettings = PortalController.GetCurrentPortalSettings();
                        if (!ReferenceEquals(portalsettings, null))
                        {
                            this._reminderfrom = portalsettings.Email;
                        }
                    }
                    return this._reminderfrom;
                }
            set { this._reminderfrom = value; }
        }

        [ModuleSetting]
        public bool Moderateall { get; set; }

        [ModuleSetting]
        public string Paypalaccount
        {
            get
                {
                    if (ReferenceEquals(this._paypalaccount, null))
                    {
                        var portalsettings = PortalController.GetCurrentPortalSettings();
                        if (!ReferenceEquals(portalsettings, null))
                        {
                            this._paypalaccount = portalsettings.Email;
                        }
                    }
                    return this._paypalaccount;
                }
            set { this._paypalaccount = value; }
        }

        [ModuleSetting]
        public bool Eventdefaultenrollview { get; set; }

        [ModuleSetting]
        public bool Fridayweekend { get; set; }

        [ModuleSetting]
        public bool Enforcesubcalperms { get; set; } = true;

        [ModuleSetting]
        public bool Addsubmodulename { get; set; } = true;

        [ModuleSetting]
        public bool MasterEvent { get; set; }

        [ModuleSetting]
        public bool Monthdayselect { get; set; }

        [ModuleSetting]
        public bool Timeintitle { get; set; }

        [ModuleSetting]
        public bool ShowEventsAlways { get; set; }

        [ModuleSetting]
        public bool Locationconflict { get; set; }

        [ModuleSetting]
        public bool Preventconflicts { get; set; }

        [ModuleSetting]
        public bool Eventsearch { get; set; } = true;

        [ModuleSetting]
        public bool Allowreoccurring { get; set; } = true;

        [ModuleSetting]
        public bool Eventimage { get; set; } = true;

        [ModuleSetting]
        public bool Showvaluemarks { get; set; }

        [ModuleSetting]
        public bool Includeendvalue { get; set; } = true;

        [ModuleSetting]
        public bool Fulltimescale { get; set; }

        [ModuleSetting]
        public bool Collapserecurring { get; set; }

        [ModuleSetting]
        public bool DisableEventnav { get; set; }

        [ModuleSetting]
        public string Paypalurl { get; set; } = "https://www.paypal.com";

        [ModuleSetting]
        public bool Tzdisplay { get; set; }

        [ModuleSetting]
        public bool Newpereventemail { get; set; }

        [ModuleSetting]
        public int Neweventemailrole
        {
            get
                {
                    if (this._neweventemailrole < 0)
                    {
                        var portalsettings = PortalController.GetCurrentPortalSettings();
                        if (!ReferenceEquals(portalsettings, null))
                        {
                            this._neweventemailrole = portalsettings.RegisteredRoleId;
                        }
                    }
                    return this._neweventemailrole;
                }
            set { this._neweventemailrole = value; }
        }

        [ModuleSetting]
        public string Neweventemails { get; set; } = "Never";

        [ModuleSetting]
        public bool Sendreminderdefault { get; set; }

        [ModuleSetting]
        public bool Notifyanon { get; set; }

        [ModuleSetting]
        public bool Eventnotify { get; set; } = true;

        [ModuleSetting]
        public string DefaultView { get; set; } = "EventMonth.ascx";

        [ModuleSetting]
        public bool Eventdaynewpage { get; set; }

        [ModuleSetting]
        public bool Enableenrollpopup { get; set; } = true;

        [ModuleSetting]
        public string Maxrecurrences { get; set; } = "1000";

        [ModuleSetting]
        public string Version { get; set; } = string.Empty;

        [ModuleSetting]
        public string Timeinterval { get; set; } = "30";

        private string TimeZone { get; set; } = string.Empty;

        [ModuleSetting]
        public string TimeZoneId
        {
            get
                {
                    if (ReferenceEquals(this._timeZoneId, null))
                    {
                        if (ReferenceEquals(this._timeZone, null))
                        {
                            var portalsettings = PortalController.GetCurrentPortalSettings();
                            if (!ReferenceEquals(portalsettings, null))
                            {
                                this._timeZoneId = portalsettings.TimeZone.Id;
                            }
                        }
                        else
                        {
                            this._timeZoneId = Localization
                                .ConvertLegacyTimeZoneOffsetToTimeZoneInfo(int.Parse(this._timeZone)).Id;
                        }
                    }
                    return this._timeZoneId;
                }
            set { this._timeZoneId = value; }
        }

        [ModuleSetting]
        public bool EnableEventTimeZones { get; set; } = false;

        [ModuleSetting]
        public TimeZones PrimaryTimeZone { get; set; } = TimeZones.UserTZ;

        [ModuleSetting]
        public TimeZones SecondaryTimeZone { get; set; } = TimeZones.PortalTZ;

        [ModuleSetting]
        public bool Eventtooltiplist { get; set; } = true;

        [ModuleSetting]
        public bool Eventtooltipday { get; set; } = true;

        [ModuleSetting]
        public bool Eventtooltipweek { get; set; } = true;

        [ModuleSetting]
        public bool Eventtooltipmonth { get; set; } = true;

        [ModuleSetting]
        public int Eventtooltiplength { get; set; } = 10000;

        [ModuleSetting]
        public bool Monthcellnoevents { get; set; } = false;

        [ModuleSetting]
        public bool Restrictcategories { get; set; } = false;

        [ModuleSetting]
        public bool RestrictCategoriesToTimeFrame { get; set; }

        [ModuleSetting]
        public DisplayCategories Enablecategories
        {
            get
                {
                    if ((int) this._enablecategories == 0)
                    {
                        if (this._disablecategories)
                        {
                            return DisplayCategories.DoNotDisplay;
                        }
                        return DisplayCategories.MultiSelect;
                    }
                    return this._enablecategories;
                }
            set { this._enablecategories = value; }
        }

        [ModuleSetting]
        public bool Restrictlocations { get; set; }

        [ModuleSetting]
        public bool RestrictLocationsToTimeFrame { get; set; }

        [ModuleSetting]
        public DisplayLocations Enablelocations
        {
            get
                {
                    if ((int) this._enablelocations == 0)
                    {
                        if (this._disablelocations)
                        {
                            return DisplayLocations.DoNotDisplay;
                        }
                        return DisplayLocations.MultiSelect;
                    }
                    return this._enablelocations;
                }
            set { this._enablelocations = value; }
        }

        [ModuleSetting]
        public bool Enablecontainerskin { get; set; } = true;

        [ModuleSetting]
        public bool Eventdetailnewpage { get; set; }

        public bool Ownerchangeallowed { get; set; } = false;

        [ModuleSetting]
        public bool Eventsignupallowpaid { get; set; } = true;

        [ModuleSetting]
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

        [ModuleSetting]
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