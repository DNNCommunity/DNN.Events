' 
' Copyright (c) 2004-2009 DNN-Europe, http://www.dnn-europe.net
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this 
' software and associated documentation files (the "Software"), to deal in the Software 
' without restriction, including without limitation the rights to use, copy, modify, merge, 
' publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons 
' to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or 
' substantial portions of the Software.

' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
' INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
' PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
' FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
' ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
' 
Imports DotNetNuke.Common.Utilities.DataCache
Imports DotNetNuke.Entities.Modules

Namespace DotNetNuke.Modules.Events

#Region "EventModuleSettings"
    <Serializable()>
    Public Class EventModuleSettings
        Public Enum TimeZones As Integer
            UserTZ = 1
            ModuleTZ
            PortalTZ
        End Enum

        Public Enum CategoriesSelected As Integer
            All = 1
            Some
            None
        End Enum

        Public Enum DisplayCategories As Integer
            DoNotDisplay = 1
            SingleSelect
            MultiSelect
        End Enum

        Public Enum LocationsSelected As Integer
            All = 1
            Some
            None
        End Enum

        Public Enum DisplayLocations As Integer
            DoNotDisplay = 1
            SingleSelect
            MultiSelect
        End Enum

        Public Enum SocialModule As Integer
            No = 1
            SocialGroup
            UserProfile
        End Enum

        Public Enum SocialGroupPrivacy As Integer
            OpenToAll = 1
            EditByGroup
            PrivateToGroup
        End Enum

#Region " Private Members "
        Private ReadOnly _allsettings As Hashtable
        Private _moduleID As Integer = -1
        Private ReadOnly _localresourcefile As String = Nothing

        Private _version As String = ""
        Private _ownerchangeallowed As Boolean = False
        Private _eventsignup As Boolean = False
        Private _eventsignupallowpaid As Boolean = True
        Private _timeinterval As String = "30"
        Private _timeZone As String = Nothing
        Private _timeZoneId As String = Nothing
        Private _enableEventTimeZones As Boolean = False
        Private _primaryTimeZone As TimeZones = TimeZones.UserTZ
        Private _secondaryTimeZone As TimeZones = TimeZones.PortalTZ
        Private _eventtooltipmonth As Boolean = True
        Private _eventtooltipweek As Boolean = True
        Private _eventtooltipday As Boolean = True
        Private _eventtooltiplist As Boolean = True
        Private _eventtooltiplength As Integer = 10000
        Private _monthcellnoevents As Boolean = False
        ' ReSharper disable FieldCanBeMadeReadOnly.Local
        Private _disablecategories As Boolean = False
        ' ReSharper restore FieldCanBeMadeReadOnly.Local
        Private _enablecategories As DisplayCategories = 0
        Private _restrictcategories As Boolean = False
        Private _restrictcategoriestotimeframe As Boolean = False
        ' ReSharper disable FieldCanBeMadeReadOnly.Local
        Private _disablelocations As Boolean = False
        ' ReSharper restore FieldCanBeMadeReadOnly.Local
        Private _enablelocations As DisplayLocations = 0
        Private _restrictlocations As Boolean = False
        Private _restrictlocationstotimeframe As Boolean = False
        Private _enablecontainerskin As Boolean = True
        Private _eventdetailnewpage As Boolean = False
        Private _maxrecurrences As String = "1000"
        Private _enableenrollpopup As Boolean = True
        Private _eventdaynewpage As Boolean = False
        Private _defaultView As String = "EventMonth.ascx"
        Private _eventnotify As Boolean = True
        Private _notifyanon As Boolean = False
        Private _sendreminderdefault As Boolean = False
        Private _neweventemails As String = "Never"
        Private _neweventemailrole As Integer = -1
        Private _newpereventemail As Boolean = False
        Private _tzdisplay As Boolean = False
        Private _paypalurl As String = "https://www.paypal.com"
        Private _disableEventnav As Boolean = False
        Private _collapserecurring As Boolean = False
        Private _fulltimescale As Boolean = False
        Private _includeendvalue As Boolean = True
        Private _showvaluemarks As Boolean = False
        Private _eventimage As Boolean = True
        Private _allowreoccurring As Boolean = True
        Private _eventsearch As Boolean = True
        Private _preventconflicts As Boolean = False
        Private _locationconflict As Boolean = False
        Private _showEventsAlways As Boolean = False
        Private _timeintitle As Boolean = False
        Private _monthdayselect As Boolean = False
        Private _masterEvent As Boolean = False
        Private _addsubmodulename As Boolean = True
        Private _enforcesubcalperms As Boolean = True
        Private _fridayweekend As Boolean = False
        Private _eventdefaultenrollview As Boolean = False
        Private _paypalaccount As String = Nothing
        Private _moderateall As Boolean = False
        Private _reminderfrom As String = Nothing
        Private _eventsListSelectType As String = "EVENTS"
        Private _eventsListNumEvents As Integer = 10
        Private _eventsListEventDays As Integer = 365
        Private _eventsListPageSize As Integer = 10
        Private _eventsListSortDirection As String = "ASC"
        Private _rssEnable As Boolean = False
        Private _rssDateField As String = "UPDATEDDATE"
        Private _rssDays As Integer = 365
        Private _rssTitle As String = Nothing
        Private _rssDesc As String = Nothing
        Private _expireevents As String = ""
        Private _exportowneremail As Boolean = False
        Private _exportanonowneremail As Boolean = False
        Private _monthAllowed As Boolean = True
        Private _weekAllowed As Boolean = True
        Private _listAllowed As Boolean = True
        Private _eventImageMonth As Boolean = True
        Private _eventImageWeek As Boolean = False
        Private _iconBar As String = "TOP"
        Private _enrollEditFields As String = "03;05;"
        Private _enrollViewFields As String = ""
        Private _enrollAnonFields As String = "02;06;"
        Private _eventhidefullenroll As Boolean = False
        Private _maxnoenrolees As Integer = 1
        Private _enrolcanceldays As Integer = 0
        Private _detailPageAllowed As Boolean = False
        Private _enrollmentPageAllowed As Boolean = False
        Private _enrollmentPageDefaultUrl As String = ""
        Private _eventsCustomField1 As Boolean = False
        Private _eventsCustomField2 As Boolean = False
        Private _iconMonthPrio As Boolean = True
        Private _iconMonthRec As Boolean = True
        Private _iconMonthReminder As Boolean = True
        Private _iconMonthEnroll As Boolean = True
        Private _iconWeekPrio As Boolean = True
        Private _iconWeekRec As Boolean = True
        Private _iconWeekReminder As Boolean = True
        Private _iconWeekEnroll As Boolean = True
        Private _iconListPrio As Boolean = True
        Private _iconListRec As Boolean = True
        Private _iconListReminder As Boolean = True
        Private _iconListEnroll As Boolean = True
        Private _privateMessage As String = ""
        Private _eventTheme As String = ""
        Private _eventThemeDefault As String = "0,MinimalExtropy,"
        Private _eventsListFields As String = "BD;ED;EN"
        Private _eventsListShowHeader As String = "Yes"
        Private _eventsListBeforeDays As Integer = 1
        Private _eventsListAfterDays As Integer = 7
        Private _recurDummy As String = Nothing
        Private _templates As EventTemplates
        Private Const ModuleCategoryID As Integer = -1
        Private _moduleCategoryIDs As ArrayList = Nothing
        Private _moduleCategoriesSelected As CategoriesSelected = CategoriesSelected.All
        Private Const ModuleLocationID As Integer = -1
        Private _moduleLocationIDs As ArrayList = Nothing
        Private _moduleLocationsSelected As LocationsSelected = LocationsSelected.All
        Private _eventsListSortColumn As String = "EventDateBegin"
        Private _htmlEmail As String = "html"
        Private _iCalDaysBefore As Integer = 365
        Private _iCalDaysAfter As Integer = 365
        Private _icalURLAppend As String = ""
        Private _icalDefaultImage As String = ""
        Private _icalOnIconBar As Boolean = False
        Private _icalEmailEnable As Boolean = False
        Private _iIcalURLInLocation As Boolean = True
        Private _iIcalIncludeCalname As Boolean = False
        Private _enableSEO As Boolean = True
        Private _seoDescriptionLength As Integer = 256
        Private _enableSitemap As Boolean = False
        Private _siteMapPriority As Single = 0.5
        Private _siteMapDaysBefore As Integer = 365
        Private _siteMapDaysAfter As Integer = 365
        Private _listViewGrid As Boolean = True
        Private _listViewTable As Boolean = True
        Private _rptColumns As Integer = 1
        Private _rptRows As Integer = 5
        Private _weekStart As System.Web.UI.WebControls.FirstDayOfWeek = System.Web.UI.WebControls.FirstDayOfWeek.Default
        Private _listViewUseTime As Boolean = False
        Private _standardEmail As String = Nothing
        Private _fbAdmins As String = ""
        Private _fbAppID As String = ""
        Private _maxThumbWidth As Integer = 125
        Private _maxThumbHeight As Integer = 125
        Private _sendEnrollMessageApproved As Boolean = True
        Private _sendEnrollMessageWaiting As Boolean = True
        Private _sendEnrollMessageDenied As Boolean = True
        Private _sendEnrollMessageAdded As Boolean = True
        Private _sendEnrollMessageDeleted As Boolean = True
        Private _sendEnrollMessagePaying As Boolean = True
        Private _sendEnrollMessagePending As Boolean = True
        Private _sendEnrollMessagePaid As Boolean = True
        Private _sendEnrollMessageIncorrect As Boolean = True
        Private _sendEnrollMessageCancelled As Boolean = True
        Private _allowanonenroll As Boolean = False
        Private _socialGroupModule As SocialModule = SocialModule.No
        Private _enrolListSortDirection As SortDirection = SortDirection.Descending
        Private _enrolListDaysBefore As Integer = (365 * 4) + 1
        Private _enrolListDaysAfter As Integer = (365 * 4) + 1
        Private _journalIntegration As Boolean = True
        Private _socialUserPrivate As Boolean = True
        Private _socialGroupSecurity As SocialGroupPrivacy = SocialGroupPrivacy.EditByGroup

#End Region

#Region " Constructors "

        Public Sub New()
        End Sub

        Public Sub New(ByVal moduleId As Integer, ByVal localResourceFile As String)
            _moduleID = moduleId
            Dim mc As New ModuleController
            _allsettings = mc.GetModuleSettings(_moduleID)
            _localresourcefile = localResourceFile

            ' Set default correct for those where the basic default is affected by upgrade
            UpdateDefaults()

            ReadValue(_allsettings, "version", Version)
            ReadValue(_allsettings, "timeinterval", Timeinterval)
            ReadValue(_allsettings, "TimeZone", TimeZone)
            ReadValue(_allsettings, "TimeZoneId", TimeZoneId)
            ReadValue(_allsettings, "EnableEventTimeZones", EnableEventTimeZones)
            ReadValue(_allsettings, "PrimaryTimeZone", PrimaryTimeZone)
            ReadValue(_allsettings, "SecondaryTimeZone", SecondaryTimeZone)
            ReadValue(_allsettings, "eventtooltipmonth", Eventtooltipmonth)
            ReadValue(_allsettings, "eventtooltipweek", Eventtooltipweek)
            ReadValue(_allsettings, "eventtooltipday", Eventtooltipday)
            ReadValue(_allsettings, "eventtooltiplist", Eventtooltiplist)
            ReadValue(_allsettings, "eventtooltiplength", Eventtooltiplength)
            ReadValue(_allsettings, "monthcellnoevents", Monthcellnoevents)
            ReadValue(_allsettings, "disablecategories", _disablecategories)
            ReadValue(_allsettings, "enablecategories", Enablecategories)
            ReadValue(_allsettings, "restrictcategories", Restrictcategories)
            ReadValue(_allsettings, "restrictcategoriestotimeframe", RestrictCategoriesToTimeFrame)
            ReadValue(_allsettings, "disablelocations", _disablelocations)
            ReadValue(_allsettings, "enablelocations", Enablelocations)
            ReadValue(_allsettings, "restrictlocations", Restrictlocations)
            ReadValue(_allsettings, "restrictlocationstotimeframe", RestrictLocationsToTimeFrame)
            ReadValue(_allsettings, "enablecontainerskin", Enablecontainerskin)
            ReadValue(_allsettings, "eventdetailnewpage", Eventdetailnewpage)
            ReadValue(_allsettings, "maxrecurrences", Maxrecurrences)
            ReadValue(_allsettings, "enableenrollpopup", Enableenrollpopup)
            ReadValue(_allsettings, "eventdaynewpage", Eventdaynewpage)
            ReadValue(_allsettings, "DefaultView", DefaultView)
            ReadValue(_allsettings, "Eventnotify", Eventnotify)
            ReadValue(_allsettings, "notifyanon", Notifyanon)
            ReadValue(_allsettings, "sendreminderdefault", Sendreminderdefault)
            ReadValue(_allsettings, "neweventemails", Neweventemails)
            ReadValue(_allsettings, "neweventemailrole", Neweventemailrole)
            ReadValue(_allsettings, "newpereventemail", Newpereventemail)
            ReadValue(_allsettings, "tzdisplay", Tzdisplay)
            ReadValue(_allsettings, "paypalurl", Paypalurl)
            ReadValue(_allsettings, "disableEventnav", DisableEventnav)
            ReadValue(_allsettings, "collapserecurring", Collapserecurring)
            ReadValue(_allsettings, "fulltimescale", Fulltimescale)
            ReadValue(_allsettings, "includeendvalue", Includeendvalue)
            ReadValue(_allsettings, "showvaluemarks", Showvaluemarks)
            ReadValue(_allsettings, "eventimage", Eventimage)
            ReadValue(_allsettings, "allowreoccurring", Allowreoccurring)
            ReadValue(_allsettings, "Eventsearch", Eventsearch)
            ReadValue(_allsettings, "preventconflicts", Preventconflicts)
            ReadValue(_allsettings, "locationconflict", Locationconflict)
            ReadValue(_allsettings, "showEventsAlways", ShowEventsAlways)
            ReadValue(_allsettings, "timeintitle", Timeintitle)
            ReadValue(_allsettings, "monthdayselect", Monthdayselect)
            ReadValue(_allsettings, "masterEvent", MasterEvent)
            ReadValue(_allsettings, "addsubmodulename", Addsubmodulename)
            ReadValue(_allsettings, "enforcesubcalperms", Enforcesubcalperms)
            ReadValue(_allsettings, "eventsignup", Eventsignup)
            ReadValue(_allsettings, "eventsignupallowpaid", Eventsignupallowpaid)
            ReadValue(_allsettings, "fridayweekend", Fridayweekend)
            ReadValue(_allsettings, "eventdefaultenrollview", Eventdefaultenrollview)
            ReadValue(_allsettings, "paypalaccount", Paypalaccount)
            ReadValue(_allsettings, "moderateall", Moderateall)
            ReadValue(_allsettings, "reminderfrom", Reminderfrom)
            ReadValue(_allsettings, "EventsListSelectType", EventsListSelectType)
            ReadValue(_allsettings, "EventsListNumEvents", EventsListNumEvents)
            ReadValue(_allsettings, "EventsListEventDays", EventsListEventDays)
            ReadValue(_allsettings, "EventsListPageSize", EventsListPageSize)
            ReadValue(_allsettings, "EventsListSortDirection", EventsListSortDirection)
            ReadValue(_allsettings, "RSSEnable", RSSEnable)
            ReadValue(_allsettings, "RSSDateField", RSSDateField)
            ReadValue(_allsettings, "RSSDays", RSSDays)
            ReadValue(_allsettings, "RSSTitle", RSSTitle)
            ReadValue(_allsettings, "RSSDesc", RSSDesc)
            ReadValue(_allsettings, "expireevents", Expireevents)
            ReadValue(_allsettings, "exportowneremail", Exportowneremail)
            ReadValue(_allsettings, "exportanonowneremail", Exportanonowneremail)
            ReadValue(_allsettings, "ownerchangeallowed", Ownerchangeallowed)
            ReadValue(_allsettings, "MonthAllowed", MonthAllowed)
            ReadValue(_allsettings, "WeekAllowed", WeekAllowed)
            ReadValue(_allsettings, "ListAllowed", ListAllowed)
            ReadValue(_allsettings, "EventImageMonth", EventImageMonth)
            ReadValue(_allsettings, "EventImageWeek", EventImageWeek)
            ReadValue(_allsettings, "IconBar", IconBar)
            ReadValue(_allsettings, "EnrollEditFields", EnrollEditFields)
            ReadValue(_allsettings, "EnrollViewFields", EnrollViewFields)
            ReadValue(_allsettings, "EnrollAnonFields", EnrollAnonFields)
            ReadValue(_allsettings, "eventhidefullenroll", Eventhidefullenroll())
            ReadValue(_allsettings, "maxnoenrolees", Maxnoenrolees)
            ReadValue(_allsettings, "enrolcanceldays", Enrolcanceldays)
            ReadValue(_allsettings, "DetailPageAllowed", DetailPageAllowed)
            ReadValue(_allsettings, "EnrollmentPageAllowed", EnrollmentPageAllowed)
            ReadValue(_allsettings, "EnrollmentPageDefaultUrl", EnrollmentPageDefaultUrl)
            ReadValue(_allsettings, "EventsCustomField1", EventsCustomField1)
            ReadValue(_allsettings, "EventsCustomField2", EventsCustomField2)
            ReadValue(_allsettings, "IconMonthPrio", IconMonthPrio)
            ReadValue(_allsettings, "IconMonthRec", IconMonthRec)
            ReadValue(_allsettings, "IconMonthReminder", IconMonthReminder)
            ReadValue(_allsettings, "IconMonthEnroll", IconMonthEnroll)
            ReadValue(_allsettings, "IconWeekPrio", IconWeekPrio)
            ReadValue(_allsettings, "IconWeekRec", IconWeekRec)
            ReadValue(_allsettings, "IconWeekReminder", IconWeekReminder)
            ReadValue(_allsettings, "IconWeekEnroll", IconWeekEnroll)
            ReadValue(_allsettings, "IconListPrio", IconListPrio)
            ReadValue(_allsettings, "IconListRec", IconListRec)
            ReadValue(_allsettings, "IconListReminder", IconListReminder)
            ReadValue(_allsettings, "IconListEnroll", IconListEnroll)
            ReadValue(_allsettings, "PrivateMessage", PrivateMessage)
            ReadValue(_allsettings, "EventTheme", EventTheme)
            ReadValue(_allsettings, "EventThemeDefault", EventThemeDefault)
            ReadValue(_allsettings, "EventsListFields", EventsListFields)
            ReadValue(_allsettings, "EventsListShowHeader", EventsListShowHeader)
            ReadValue(_allsettings, "EventsListBeforeDays", EventsListBeforeDays)
            ReadValue(_allsettings, "EventsListAfterDays", EventsListAfterDays)
            ReadValue(_allsettings, "RecurDummy", RecurDummy)
            ReadValue(_allsettings, "modulecategoryid", ModuleCategoryID)
            ReadValue(_allsettings, "modulecategoryids", ModuleCategoryIDs)
            ReadValue(_allsettings, "modulelocationid", ModuleLocationID)
            ReadValue(_allsettings, "modulelocationids", ModuleLocationIDs)
            ReadValue(_allsettings, "eventslistsortcolumn", EventsListSortColumn)
            ReadValue(_allsettings, "HTMLEmail", HTMLEmail)
            ReadValue(_allsettings, "iCalDaysBefore", IcalDaysBefore)
            ReadValue(_allsettings, "iCalDaysAfter", IcalDaysAfter)
            ReadValue(_allsettings, "iCalURLAppend", IcalURLAppend)
            ReadValue(_allsettings, "iCalDefaultImage", IcalDefaultImage)
            ReadValue(_allsettings, "iCalURLinLocation", IcalURLInLocation)
            ReadValue(_allsettings, "iCalIncludeCalname", IcalIncludeCalname)
            ReadValue(_allsettings, "enableSEO", EnableSEO)
            ReadValue(_allsettings, "SEODescriptionLength", SEODescriptionLength)
            ReadValue(_allsettings, "enableSitemap", EnableSitemap)
            ReadValue(_allsettings, "siteMapPriority", SiteMapPriority)
            ReadValue(_allsettings, "siteMapDaysBefore", SiteMapDaysBefore)
            ReadValue(_allsettings, "siteMapDaysAfter", SiteMapDaysAfter)
            ReadValue(_allsettings, "ListViewGrid", ListViewGrid)
            ReadValue(_allsettings, "ListViewTable", ListViewTable)
            ReadValue(_allsettings, "rptColumns", RptColumns)
            ReadValue(_allsettings, "rptRows", RptRows)
            ReadValue(_allsettings, "WeekStart", WeekStart)
            ReadValue(_allsettings, "ListViewUseTime", ListViewUseTime)
            ReadValue(_allsettings, "iCalOnIconBar", IcalOnIconBar)
            ReadValue(_allsettings, "iCalEmailEnable", IcalEmailEnable)
            ReadValue(_allsettings, "StandardEmail", StandardEmail)
            ReadValue(_allsettings, "FBAdmins", FBAdmins)
            ReadValue(_allsettings, "FBAppID", FBAppID)
            ReadValue(_allsettings, "MaxThumbWidth", MaxThumbWidth)
            ReadValue(_allsettings, "MaxThumbHeight", MaxThumbHeight)
            ReadValue(_allsettings, "SendEnrollMessageApproved", SendEnrollMessageApproved)
            ReadValue(_allsettings, "SendEnrollMessageWaiting", SendEnrollMessageWaiting)
            ReadValue(_allsettings, "SendEnrollMessageDenied", SendEnrollMessageDenied)
            ReadValue(_allsettings, "SendEnrollMessageAdded", SendEnrollMessageAdded)
            ReadValue(_allsettings, "SendEnrollMessageDeleted", SendEnrollMessageDeleted)
            ReadValue(_allsettings, "SendEnrollMessagePaying", SendEnrollMessagePaying)
            ReadValue(_allsettings, "SendEnrollMessagePending", SendEnrollMessagePending)
            ReadValue(_allsettings, "SendEnrollMessagePaid", SendEnrollMessagePaid)
            ReadValue(_allsettings, "SendEnrollMessageIncorrect", SendEnrollMessageIncorrect)
            ReadValue(_allsettings, "SendEnrollMessageCancelled", SendEnrollMessageCancelled)
            ReadValue(_allsettings, "allowanonenroll", AllowAnonEnroll)
            ReadValue(_allsettings, "SocialGroupModule", SocialGroupModule)
            ReadValue(_allsettings, "SocialUserPrivate", SocialUserPrivate)
            ReadValue(_allsettings, "SSocialGroupSecurity", SocialGroupSecurity)
            ReadValue(_allsettings, "EnrolListSortDirection", EnrolListSortDirection)
            ReadValue(_allsettings, "EnrolListDaysBefore", EnrolListDaysBefore)
            ReadValue(_allsettings, "EnrolListDaysAfter", EnrolListDaysAfter)
            ReadValue(_allsettings, "JournalIntegration", JournalIntegration)
            Templates = New EventTemplates(_moduleID, _allsettings, _localresourcefile)
        End Sub

#End Region

#Region " Public Methods "

        Public Function GetEventModuleSettings(ByVal moduleID As Integer, ByVal localResourceFile As String) As EventModuleSettings
            _moduleID = moduleID
            If _moduleID > 0 Then
                Dim cacheKey As String = "EventsSettings" & moduleID.ToString
                Dim bs As EventModuleSettings
                bs = CType(GetCache(cacheKey), EventModuleSettings)
                If bs Is Nothing Then
                    bs = New EventModuleSettings(_moduleID, localResourceFile)
                    If Not localResourceFile Is Nothing Then
                        SetCache(cacheKey, bs)
                    End If
                End If
                Return bs
            End If
            Return Nothing
        End Function


        Public Sub SaveSettings(ByVal moduleId As Integer)

            Dim objModules As New ModuleController
            With objModules
                Dim objDesktopModule As DesktopModuleInfo
                objDesktopModule = DesktopModuleController.GetDesktopModuleByModuleName("DNN_Events", 0)
                .UpdateModuleSetting(moduleId, "version", objDesktopModule.Version)

                .UpdateModuleSetting(moduleId, "timeinterval", Timeinterval.ToString)
                .UpdateModuleSetting(moduleId, "TimeZoneId", TimeZoneId)
                .UpdateModuleSetting(moduleId, "EnableEventTimeZones", EnableEventTimeZones.ToString)
                .UpdateModuleSetting(moduleId, "PrimaryTimeZone", CInt(PrimaryTimeZone).ToString)
                .UpdateModuleSetting(moduleId, "SecondaryTimeZone", CInt(SecondaryTimeZone).ToString)
                .UpdateModuleSetting(moduleId, "eventtooltipmonth", Eventtooltipmonth.ToString)
                .UpdateModuleSetting(moduleId, "eventtooltipweek", Eventtooltipweek.ToString)
                .UpdateModuleSetting(moduleId, "eventtooltipday", Eventtooltipday.ToString)
                .UpdateModuleSetting(moduleId, "eventtooltiplist", Eventtooltiplist.ToString)
                .UpdateModuleSetting(moduleId, "eventtooltiplength", Eventtooltiplength.ToString)
                .UpdateModuleSetting(moduleId, "monthcellnoevents", Monthcellnoevents.ToString)
                .UpdateModuleSetting(moduleId, "enablecategories", CInt(Enablecategories).ToString)
                .UpdateModuleSetting(moduleId, "restrictcategories", Restrictcategories.ToString)
                .UpdateModuleSetting(moduleId, "restrictcategoriestotimeframe", RestrictCategoriesToTimeFrame.ToString)
                .UpdateModuleSetting(moduleId, "enablelocations", CInt(Enablelocations).ToString)
                .UpdateModuleSetting(moduleId, "restrictlocations", Restrictlocations.ToString)
                .UpdateModuleSetting(moduleId, "restrictlocationstotimeframe", RestrictLocationsToTimeFrame.ToString)
                .UpdateModuleSetting(moduleId, "enablecontainerskin", Enablecontainerskin.ToString)
                .UpdateModuleSetting(moduleId, "eventdetailnewpage", Eventdetailnewpage.ToString)
                .UpdateModuleSetting(moduleId, "maxrecurrences", Maxrecurrences.ToString)
                .UpdateModuleSetting(moduleId, "enableenrollpopup", Enableenrollpopup.ToString)
                .UpdateModuleSetting(moduleId, "eventdaynewpage", Eventdaynewpage.ToString)
                .UpdateModuleSetting(moduleId, "DefaultView", DefaultView.ToString)
                .UpdateModuleSetting(moduleId, "Eventnotify", Eventnotify.ToString)
                .UpdateModuleSetting(moduleId, "notifyanon", Notifyanon.ToString)
                .UpdateModuleSetting(moduleId, "sendreminderdefault", Sendreminderdefault.ToString)
                .UpdateModuleSetting(moduleId, "neweventemails", Neweventemails.ToString)
                .UpdateModuleSetting(moduleId, "neweventemailrole", Neweventemailrole.ToString)
                .UpdateModuleSetting(moduleId, "newpereventemail", Newpereventemail.ToString)
                .UpdateModuleSetting(moduleId, "tzdisplay", Tzdisplay.ToString)
                .UpdateModuleSetting(moduleId, "paypalurl", Paypalurl.ToString)
                .UpdateModuleSetting(moduleId, "disableEventnav", DisableEventnav.ToString)
                .UpdateModuleSetting(moduleId, "collapserecurring", Collapserecurring.ToString)
                .UpdateModuleSetting(moduleId, "fulltimescale", Fulltimescale.ToString)
                .UpdateModuleSetting(moduleId, "includeendvalue", Includeendvalue.ToString)
                .UpdateModuleSetting(moduleId, "showvaluemarks", Showvaluemarks.ToString)
                .UpdateModuleSetting(moduleId, "eventimage", Eventimage.ToString)
                .UpdateModuleSetting(moduleId, "allowreoccurring", Allowreoccurring.ToString)
                .UpdateModuleSetting(moduleId, "Eventsearch", Eventsearch.ToString)
                .UpdateModuleSetting(moduleId, "preventconflicts", Preventconflicts.ToString)
                .UpdateModuleSetting(moduleId, "locationconflict", Locationconflict.ToString)
                .UpdateModuleSetting(moduleId, "showEventsAlways", ShowEventsAlways.ToString)
                .UpdateModuleSetting(moduleId, "timeintitle", Timeintitle.ToString)
                .UpdateModuleSetting(moduleId, "monthdayselect", Monthdayselect.ToString)
                .UpdateModuleSetting(moduleId, "masterEvent", MasterEvent.ToString)
                .UpdateModuleSetting(moduleId, "addsubmodulename", Addsubmodulename.ToString)
                .UpdateModuleSetting(moduleId, "enforcesubcalperms", Enforcesubcalperms.ToString)
                .UpdateModuleSetting(moduleId, "eventsignup", Eventsignup.ToString)
                .UpdateModuleSetting(moduleId, "eventsignupallowpaid", Eventsignupallowpaid.ToString)
                .UpdateModuleSetting(moduleId, "fridayweekend", Fridayweekend.ToString)
                .UpdateModuleSetting(moduleId, "eventdefaultenrollview", Eventdefaultenrollview.ToString)
                .UpdateModuleSetting(moduleId, "paypalaccount", Paypalaccount.ToString)
                .UpdateModuleSetting(moduleId, "moderateall", Moderateall.ToString)
                .UpdateModuleSetting(moduleId, "reminderfrom", Reminderfrom.ToString)
                .UpdateModuleSetting(moduleId, "EventsListSelectType", EventsListSelectType.ToString)
                .UpdateModuleSetting(moduleId, "EventsListNumEvents", EventsListNumEvents.ToString)
                .UpdateModuleSetting(moduleId, "EventsListEventDays", EventsListEventDays.ToString)
                .UpdateModuleSetting(moduleId, "EventsListPageSize", EventsListPageSize.ToString)
                .UpdateModuleSetting(moduleId, "EventsListSortDirection", EventsListSortDirection.ToString)
                .UpdateModuleSetting(moduleId, "RSSEnable", RSSEnable.ToString)
                .UpdateModuleSetting(moduleId, "RSSDateField", RSSDateField.ToString)
                .UpdateModuleSetting(moduleId, "RSSDays", RSSDays.ToString)
                .UpdateModuleSetting(moduleId, "RSSTitle", RSSTitle.ToString)
                .UpdateModuleSetting(moduleId, "RSSDesc", RSSDesc.ToString)
                .UpdateModuleSetting(moduleId, "expireevents", Expireevents.ToString)
                .UpdateModuleSetting(moduleId, "exportowneremail", Exportowneremail.ToString)
                .UpdateModuleSetting(moduleId, "exportanonowneremail", Exportanonowneremail.ToString)
                .UpdateModuleSetting(moduleId, "ownerchangeallowed", Ownerchangeallowed.ToString)
                .UpdateModuleSetting(moduleId, "MonthAllowed", MonthAllowed.ToString)
                .UpdateModuleSetting(moduleId, "WeekAllowed", WeekAllowed.ToString)
                .UpdateModuleSetting(moduleId, "ListAllowed", ListAllowed.ToString)
                .UpdateModuleSetting(moduleId, "EventImageMonth", EventImageMonth.ToString)
                .UpdateModuleSetting(moduleId, "EventImageWeek", EventImageWeek.ToString)
                .UpdateModuleSetting(moduleId, "IconBar", IconBar.ToString)
                .UpdateModuleSetting(moduleId, "EnrollEditFields", EnrollEditFields.ToString)
                .UpdateModuleSetting(moduleId, "EnrollViewFields", EnrollViewFields.ToString)
                .UpdateModuleSetting(moduleId, "EnrollAnonFields", EnrollAnonFields.ToString)
                .UpdateModuleSetting(moduleId, "eventhidefullenroll", Eventhidefullenroll().ToString)
                .UpdateModuleSetting(moduleId, "maxnoenrolees", Maxnoenrolees.ToString)
                .UpdateModuleSetting(moduleId, "enrolcanceldays", Enrolcanceldays.ToString)
                .UpdateModuleSetting(moduleId, "DetailPageAllowed", DetailPageAllowed.ToString)
                .UpdateModuleSetting(moduleId, "EnrollmentPageAllowed", EnrollmentPageAllowed.ToString)
                .UpdateModuleSetting(moduleId, "EnrollmentPageDefaultUrl", EnrollmentPageDefaultUrl.ToString)
                .UpdateModuleSetting(moduleId, "EventsCustomField1", EventsCustomField1.ToString)
                .UpdateModuleSetting(moduleId, "EventsCustomField2", EventsCustomField2.ToString)
                .UpdateModuleSetting(moduleId, "IconMonthPrio", IconMonthPrio.ToString)
                .UpdateModuleSetting(moduleId, "IconMonthRec", IconMonthRec.ToString)
                .UpdateModuleSetting(moduleId, "IconMonthReminder", IconMonthReminder.ToString)
                .UpdateModuleSetting(moduleId, "IconMonthEnroll", IconMonthEnroll.ToString)
                .UpdateModuleSetting(moduleId, "IconWeekPrio", IconWeekPrio.ToString)
                .UpdateModuleSetting(moduleId, "IconWeekRec", IconWeekRec.ToString)
                .UpdateModuleSetting(moduleId, "IconWeekReminder", IconWeekReminder.ToString)
                .UpdateModuleSetting(moduleId, "IconWeekEnroll", IconWeekEnroll.ToString)
                .UpdateModuleSetting(moduleId, "IconListPrio", IconListPrio.ToString)
                .UpdateModuleSetting(moduleId, "IconListRec", IconListRec.ToString)
                .UpdateModuleSetting(moduleId, "IconListReminder", IconListReminder.ToString)
                .UpdateModuleSetting(moduleId, "IconListEnroll", IconListEnroll.ToString)
                .UpdateModuleSetting(moduleId, "PrivateMessage", PrivateMessage.ToString)
                .UpdateModuleSetting(moduleId, "EventTheme", EventTheme.ToString)
                ' Hard coded value, not set anywhere
                ' .UpdateModuleSetting(ModuleId, "EventThemeDefault", EventThemeDefault.ToString)
                .UpdateModuleSetting(moduleId, "EventsListFields", EventsListFields.ToString)
                .UpdateModuleSetting(moduleId, "EventsListShowHeader", EventsListShowHeader.ToString)
                .UpdateModuleSetting(moduleId, "EventsListBeforeDays", EventsListBeforeDays.ToString)
                .UpdateModuleSetting(moduleId, "EventsListAfterDays", EventsListAfterDays.ToString)
                If Not RecurDummy Is Nothing Then
                    .UpdateModuleSetting(moduleId, "RecurDummy", RecurDummy.ToString)
                End If
                .UpdateModuleSetting(moduleId, "modulecategoryids", String.Join(",", CType(ModuleCategoryIDs.ToArray(GetType(String)), String())))
                .UpdateModuleSetting(moduleId, "modulelocationids", String.Join(",", CType(ModuleLocationIDs.ToArray(GetType(String)), String())))
                .UpdateModuleSetting(moduleId, "eventslistsortcolumn", EventsListSortColumn.ToString)
                .UpdateModuleSetting(moduleId, "HTMLEmail", HTMLEmail.ToString)
                .UpdateModuleSetting(moduleId, "iCalDaysBefore", IcalDaysBefore.ToString)
                .UpdateModuleSetting(moduleId, "iCalDaysAfter", IcalDaysAfter.ToString)
                .UpdateModuleSetting(moduleId, "iCalURLAppend", IcalURLAppend.ToString)
                .UpdateModuleSetting(moduleId, "iCalDefaultImage", IcalDefaultImage.ToString)
                .UpdateModuleSetting(moduleId, "iCalURLinLocation", IcalURLInLocation.ToString)
                .UpdateModuleSetting(moduleId, "iCalIncludeCalname", IcalIncludeCalname.ToString)
                .UpdateModuleSetting(moduleId, "enableSEO", EnableSEO.ToString)
                .UpdateModuleSetting(moduleId, "SEODescriptionLength", SEODescriptionLength.ToString)
                .UpdateModuleSetting(moduleId, "enableSitemap", EnableSitemap.ToString)
                .UpdateModuleSetting(moduleId, "siteMapPriority", SiteMapPriority.ToString)
                .UpdateModuleSetting(moduleId, "siteMapDaysBefore", SiteMapDaysBefore.ToString)
                .UpdateModuleSetting(moduleId, "siteMapDaysAfter", SiteMapDaysAfter.ToString)
                .UpdateModuleSetting(moduleId, "ListViewGrid", ListViewGrid.ToString)
                .UpdateModuleSetting(moduleId, "ListViewTable", ListViewTable.ToString)
                .UpdateModuleSetting(moduleId, "rptColumns", RptColumns.ToString)
                .UpdateModuleSetting(moduleId, "rptRows", RptRows.ToString)
                .UpdateModuleSetting(moduleId, "WeekStart", CInt(WeekStart).ToString)
                .UpdateModuleSetting(moduleId, "ListViewUseTime", ListViewUseTime.ToString)
                .UpdateModuleSetting(moduleId, "iCalOnIconBar", IcalOnIconBar.ToString)
                .UpdateModuleSetting(moduleId, "iCalEmailEnable", IcalEmailEnable.ToString)
                .UpdateModuleSetting(moduleId, "StandardEmail", StandardEmail.ToString)
                .UpdateModuleSetting(moduleId, "FBAdmins", FBAdmins.ToString)
                .UpdateModuleSetting(moduleId, "FBAppID", FBAppID.ToString)
                .UpdateModuleSetting(moduleId, "MaxThumbHeight", MaxThumbHeight.ToString)
                .UpdateModuleSetting(moduleId, "MaxThumbWidth", MaxThumbWidth.ToString)
                .UpdateModuleSetting(moduleId, "SendEnrollMessageApproved", SendEnrollMessageApproved.ToString)
                .UpdateModuleSetting(moduleId, "SendEnrollMessageWaiting", SendEnrollMessageWaiting.ToString)
                .UpdateModuleSetting(moduleId, "SendEnrollMessageDenied", SendEnrollMessageDenied.ToString)
                .UpdateModuleSetting(moduleId, "SendEnrollMessageAdded", SendEnrollMessageAdded.ToString)
                .UpdateModuleSetting(moduleId, "SendEnrollMessageDeleted", SendEnrollMessageDeleted.ToString)
                .UpdateModuleSetting(moduleId, "SendEnrollMessagePaying", SendEnrollMessagePaying.ToString)
                .UpdateModuleSetting(moduleId, "SendEnrollMessagePending", SendEnrollMessagePending.ToString)
                .UpdateModuleSetting(moduleId, "SendEnrollMessagePaid", SendEnrollMessagePaid.ToString)
                .UpdateModuleSetting(moduleId, "SendEnrollMessageIncorrect", SendEnrollMessageIncorrect.ToString)
                .UpdateModuleSetting(moduleId, "SendEnrollMessageCancelled", SendEnrollMessageCancelled.ToString)
                .UpdateModuleSetting(moduleId, "allowanonenroll", AllowAnonEnroll.ToString)
                .UpdateModuleSetting(moduleId, "SocialGroupModule", CInt(SocialGroupModule).ToString)
                .UpdateModuleSetting(moduleId, "SocialUserPrivate", SocialUserPrivate.ToString)
                .UpdateModuleSetting(moduleId, "SocialGroupSecurity", CInt(SocialGroupSecurity).ToString)
                .UpdateModuleSetting(moduleId, "EnrolListSortDirection", CInt(EnrolListSortDirection).ToString)
                .UpdateModuleSetting(moduleId, "EnrolListDaysBefore", CInt(EnrolListDaysBefore).ToString)
                .UpdateModuleSetting(moduleId, "EnrolListDaysAfter", CInt(EnrolListDaysAfter).ToString)
                .UpdateModuleSetting(moduleId, "JournalIntegration", JournalIntegration.ToString)
            End With
            Dim cacheKey As String = "EventsSettings" & moduleId.ToString
            SetCache(cacheKey, Me)

        End Sub

#End Region

#Region " Private Methods "

        Private Shared Sub ReadValue(ByRef valueTable As Hashtable, ByVal valueName As String, ByRef variable As String)
            If Not valueTable.Item(valueName) Is Nothing Then
                Try
                    variable = CType(valueTable.Item(valueName), String)
                Catch ex As Exception
                End Try
            End If
        End Sub

        Private Shared Sub ReadValue(ByRef valueTable As Hashtable, ByVal valueName As String, ByRef variable As Integer)
            If Not valueTable.Item(valueName) Is Nothing Then
                Try
                    variable = CType(valueTable.Item(valueName), Integer)
                Catch ex As Exception
                End Try
            End If
        End Sub

        Private Shared Sub ReadValue(ByRef valueTable As Hashtable, ByVal valueName As String, ByRef variable As Boolean)
            If Not valueTable.Item(valueName) Is Nothing Then
                Try
                    variable = CType(valueTable.Item(valueName), Boolean)
                Catch ex As Exception
                End Try
            End If
        End Sub

        Private Shared Sub ReadValue(ByRef valueTable As Hashtable, ByVal valueName As String, ByRef variable As System.Web.UI.WebControls.FirstDayOfWeek)
            If Not valueTable.Item(valueName) Is Nothing Then
                Try
                    variable = CType(CInt(valueTable.Item(valueName)), System.Web.UI.WebControls.FirstDayOfWeek)
                Catch ex As Exception
                End Try
            End If
        End Sub

        Private Shared Sub ReadValue(ByRef valueTable As Hashtable, ByVal valueName As String, ByRef variable As Single)
            If Not valueTable.Item(valueName) Is Nothing Then
                Try
                    variable = CType(valueTable.Item(valueName), Single)
                Catch ex As Exception
                End Try
            End If
        End Sub

        Private Shared Sub ReadValue(ByRef valueTable As Hashtable, ByVal valueName As String, ByRef variable As TimeZones)
            If Not valueTable.Item(valueName) Is Nothing Then
                Try
                    variable = CType(valueTable.Item(valueName), TimeZones)
                Catch ex As Exception
                End Try
            End If
        End Sub

        Private Shared Sub ReadValue(ByRef valueTable As Hashtable, ByVal valueName As String, ByRef variable As DisplayCategories)
            If Not valueTable.Item(valueName) Is Nothing Then
                Try
                    variable = CType(valueTable.Item(valueName), DisplayCategories)
                Catch ex As Exception
                End Try
            End If
        End Sub

        Private Shared Sub ReadValue(ByRef valueTable As Hashtable, ByVal valueName As String, ByRef variable As DisplayLocations)
            If Not valueTable.Item(valueName) Is Nothing Then
                Try
                    variable = CType(valueTable.Item(valueName), DisplayLocations)
                Catch ex As Exception
                End Try
            End If
        End Sub

        Private Shared Sub ReadValue(ByRef valueTable As Hashtable, ByVal valueName As String, ByRef variable As SocialModule)
            If Not valueTable.Item(valueName) Is Nothing Then
                Try
                    variable = CType(valueTable.Item(valueName), SocialModule)
                Catch ex As Exception
                End Try
            End If
        End Sub

        Private Shared Sub ReadValue(ByRef valueTable As Hashtable, ByVal valueName As String, ByRef variable As SocialGroupPrivacy)
            If Not valueTable.Item(valueName) Is Nothing Then
                Try
                    variable = CType(valueTable.Item(valueName), SocialGroupPrivacy)
                Catch ex As Exception
                End Try
            End If
        End Sub

        Private Shared Sub ReadValue(ByRef valueTable As Hashtable, ByVal valueName As String, ByRef variable As SortDirection)
            If Not valueTable.Item(valueName) Is Nothing Then
                Try
                    variable = CType(valueTable.Item(valueName), SortDirection)
                Catch ex As Exception
                End Try
            End If
        End Sub

        Private Shared Sub ReadValue(ByRef valueTable As Hashtable, ByVal valueName As String, ByRef variable As ArrayList)
            If Not valueTable.Item(valueName) Is Nothing Then
                Try
                    Dim tmpArray() As String = Split(CType(valueTable.Item(valueName), String), ",")
                    variable.Clear()
                    For i As Integer = 0 To tmpArray.Length - 1
                        If tmpArray(i) <> "" Then
                            variable.Add(CInt(tmpArray(i)))
                        End If
                    Next
                Catch ex As Exception
                End Try
            End If
        End Sub

        Private Sub UpdateDefaults()
            Dim vers As String = CType(_allsettings("version"), String)
            If Not _allsettings("onlyview") Is Nothing And CType(_allsettings("onlyview"), Boolean) Then
                If CType(_allsettings("DefaultView"), String) = "EventList.ascx" Then
                    _listAllowed = True
                Else
                    _listAllowed = False
                End If
                If CType(_allsettings("DefaultView"), String) = "EventWeek.ascx" Then
                    _weekAllowed = True
                Else
                    _weekAllowed = False
                End If
                If CType(_allsettings("DefaultView"), String) = "EventMonth.ascx" Then
                    _monthAllowed = True
                Else
                    _monthAllowed = False
                End If
            End If
            If vers = "" Then
                _enforcesubcalperms = True
            Else
                _enforcesubcalperms = False
            End If
            If Not _allsettings("allowsubscriptions") Is Nothing And CType(_allsettings("allowsubscriptions"), Boolean) Then
                _neweventemails = "Subscribe"
            End If
            If vers = "" Then
                _enablecontainerskin = True
            ElseIf CInt(Left(vers, InStr(vers, ".") - 1)) < 5 Then
                _enablecontainerskin = False
            Else
                _enablecontainerskin = True
            End If
            If Not _allsettings("eventtooltip") Is Nothing Then
                _eventtooltipmonth = CType(_allsettings("eventtooltip"), Boolean)
                _eventtooltipweek = CType(_allsettings("eventtooltip"), Boolean)
                _eventtooltipday = CType(_allsettings("eventtooltip"), Boolean)
                _eventtooltiplist = CType(_allsettings("eventtooltip"), Boolean)
            End If
            If Not _allsettings("SocialGroupModule") Is Nothing Then
                If CType(_allsettings("SocialGroupModule"), SocialModule) = SocialModule.UserProfile Then
                    _socialUserPrivate = False
                End If
                If CType(_allsettings("SocialGroupModule"), SocialModule) = SocialModule.SocialGroup Then
                    _socialGroupSecurity = SocialGroupPrivacy.OpenToAll
                End If
            End If
        End Sub

#End Region

#Region " Properties "
        Public Property MaxThumbWidth() As Integer
            Get
                Return _maxThumbWidth
            End Get
            Set(ByVal value As Integer)
                _maxThumbWidth = value
            End Set
        End Property

        Public Property MaxThumbHeight() As Integer
            Get
                Return _maxThumbHeight
            End Get
            Set(ByVal value As Integer)
                _maxThumbHeight = value
            End Set
        End Property

        Public Property FBAppID() As String
            Get
                Return _fbAppID
            End Get
            Set(ByVal value As String)
                _fbAppID = value
            End Set
        End Property

        Public Property FBAdmins() As String
            Get
                Return _fbAdmins
            End Get
            Set(ByVal value As String)
                _fbAdmins = value
            End Set
        End Property

        Public Property StandardEmail() As String
            Get
                If _standardEmail Is Nothing Then
                    Dim portalsettings As Entities.Portals.PortalSettings = Entities.Portals.PortalController.GetCurrentPortalSettings
                    If Not portalsettings Is Nothing Then
                        _standardEmail = portalsettings.Email
                    End If
                End If
                Return _standardEmail
            End Get
            Set(ByVal value As String)
                _standardEmail = value
            End Set
        End Property

        Public Property IcalIncludeCalname() As Boolean
            Get
                Return _iIcalIncludeCalname
            End Get
            Set(ByVal value As Boolean)
                _iIcalIncludeCalname = value
            End Set
        End Property

        Public Property IcalEmailEnable() As Boolean
            Get
                Return _icalEmailEnable
            End Get
            Set(ByVal value As Boolean)
                _icalEmailEnable = value
            End Set
        End Property

        Public Property IcalURLInLocation() As Boolean
            Get
                Return _iIcalURLInLocation
            End Get
            Set(ByVal value As Boolean)
                _iIcalURLInLocation = value
            End Set
        End Property

        Public Property IcalOnIconBar() As Boolean
            Get
                Return _icalOnIconBar
            End Get
            Set(ByVal value As Boolean)
                _icalOnIconBar = value
            End Set
        End Property

        Public Property ListViewUseTime() As Boolean
            Get
                Return _listViewUseTime
            End Get
            Set(ByVal value As Boolean)
                _listViewUseTime = value
            End Set
        End Property

        Public Property WeekStart() As System.Web.UI.WebControls.FirstDayOfWeek
            Get
                Return _weekStart
            End Get
            Set(ByVal value As System.Web.UI.WebControls.FirstDayOfWeek)
                _weekStart = value
            End Set
        End Property

        Public Property RptRows() As Integer
            Get
                Return _rptRows
            End Get
            Set(ByVal value As Integer)
                _rptRows = value
            End Set
        End Property

        Public Property RptColumns() As Integer
            Get
                Return _rptColumns
            End Get
            Set(ByVal value As Integer)
                _rptColumns = value
            End Set
        End Property

        Public Property ListViewTable() As Boolean
            Get
                Return _listViewTable
            End Get
            Set(ByVal value As Boolean)
                _listViewTable = value
            End Set
        End Property

        Public Property ListViewGrid() As Boolean
            Get
                Return _listViewGrid
            End Get
            Set(ByVal value As Boolean)
                _listViewGrid = value
            End Set
        End Property

        Public Property SiteMapDaysAfter() As Integer
            Get
                Return _siteMapDaysAfter
            End Get
            Set(ByVal value As Integer)
                _siteMapDaysAfter = value
            End Set
        End Property

        Public Property SiteMapDaysBefore() As Integer
            Get
                Return _siteMapDaysBefore
            End Get
            Set(ByVal value As Integer)
                _siteMapDaysBefore = value
            End Set
        End Property

        Public Property SiteMapPriority() As Single
            Get
                Return _siteMapPriority
            End Get
            Set(ByVal value As Single)
                _siteMapPriority = value
            End Set
        End Property

        Public Property EnableSitemap() As Boolean
            Get
                Return _enableSitemap
            End Get
            Set(ByVal value As Boolean)
                _enableSitemap = value
            End Set
        End Property

        Public Property SEODescriptionLength() As Integer
            Get
                Return _seoDescriptionLength
            End Get
            Set(ByVal value As Integer)
                _seoDescriptionLength = value
            End Set
        End Property

        Public Property EnableSEO() As Boolean
            Get
                Return _enableSEO
            End Get
            Set(ByVal value As Boolean)
                _enableSEO = value
            End Set
        End Property

        Public Property IcalDefaultImage() As String
            Get
                Return _icalDefaultImage
            End Get
            Set(ByVal value As String)
                _icalDefaultImage = value
            End Set
        End Property

        Public Property IcalURLAppend() As String
            Get
                Return _icalURLAppend
            End Get
            Set(ByVal value As String)
                _icalURLAppend = value
            End Set
        End Property

        Public Property IcalDaysAfter() As Integer
            Get
                Return _iCalDaysAfter
            End Get
            Set(ByVal value As Integer)
                _iCalDaysAfter = value
            End Set
        End Property

        Public Property IcalDaysBefore() As Integer
            Get
                Return _iCalDaysBefore
            End Get
            Set(ByVal value As Integer)
                _iCalDaysBefore = value
            End Set
        End Property

        Public Property HTMLEmail() As String
            Get
                Return _htmlEmail
            End Get
            Set(ByVal value As String)
                _htmlEmail = value
            End Set
        End Property

        Public Property EventsListSortColumn() As String
            Get
                Return _eventsListSortColumn
            End Get
            Set(ByVal value As String)
                _eventsListSortColumn = value
            End Set
        End Property

        Public Property ModuleCategoryIDs() As ArrayList
            Get
                If _moduleCategoryIDs Is Nothing Then
                    Dim arCat As New ArrayList
                    arCat.Add(ModuleCategoryID)
                    _moduleCategoryIDs = arCat
                End If
                Return _moduleCategoryIDs
            End Get
            Set(ByVal value As ArrayList)
                _moduleCategoryIDs = value
            End Set
        End Property

        Public Property ModuleCategoriesSelected() As CategoriesSelected
            Get
                If ModuleCategoryIDs.Count = 0 Then
                    _moduleCategoriesSelected = CategoriesSelected.None
                ElseIf CInt(ModuleCategoryIDs.Item(0)) = -1 Then
                    _moduleCategoriesSelected = CategoriesSelected.All
                Else
                    _moduleCategoriesSelected = CategoriesSelected.Some
                End If
                Return _moduleCategoriesSelected
            End Get
            Set(ByVal value As CategoriesSelected)
                _moduleCategoriesSelected = value
            End Set
        End Property

        Public Property ModuleLocationIDs() As ArrayList
            Get
                If _moduleLocationIDs Is Nothing Then
                    Dim arLoc As New ArrayList
                    arLoc.Add(ModuleLocationID)
                    _moduleLocationIDs = arLoc
                End If
                Return _moduleLocationIDs
            End Get
            Set(ByVal value As ArrayList)
                _moduleLocationIDs = value
            End Set
        End Property

        Public Property ModuleLocationsSelected() As LocationsSelected
            Get
                If ModuleLocationIDs.Count = 0 Then
                    _moduleLocationsSelected = LocationsSelected.None
                ElseIf CInt(ModuleLocationIDs.Item(0)) = -1 Then
                    _moduleLocationsSelected = LocationsSelected.All
                Else
                    _moduleLocationsSelected = LocationsSelected.Some
                End If
                Return _moduleLocationsSelected
            End Get
            Set(ByVal value As LocationsSelected)
                _moduleLocationsSelected = value
            End Set
        End Property

        Public Property RecurDummy() As String
            Get
                Return _recurDummy
            End Get
            Set(ByVal value As String)
                _recurDummy = value
            End Set
        End Property

        Public Property EventsListAfterDays() As Integer
            Get
                Return _eventsListAfterDays
            End Get
            Set(ByVal value As Integer)
                _eventsListAfterDays = value
            End Set
        End Property

        Public Property EventsListBeforeDays() As Integer
            Get
                Return _eventsListBeforeDays
            End Get
            Set(ByVal value As Integer)
                _eventsListBeforeDays = value
            End Set
        End Property

        Public Property EventsListShowHeader() As String
            Get
                Return _eventsListShowHeader
            End Get
            Set(ByVal value As String)
                _eventsListShowHeader = value
            End Set
        End Property

        Public Property EventsListFields() As String
            Get
                Return _eventsListFields
            End Get
            Set(ByVal value As String)
                _eventsListFields = value
            End Set
        End Property

        Public Property EventThemeDefault() As String
            Get
                Return _eventThemeDefault
            End Get
            Set(ByVal value As String)
                _eventThemeDefault = value
            End Set
        End Property

        Public Property EventTheme() As String
            Get
                If _eventTheme = Nothing Then
                    _eventTheme = _eventThemeDefault
                End If
                Return _eventTheme
            End Get
            Set(ByVal value As String)
                _eventTheme = value
            End Set
        End Property

        Public Property PrivateMessage() As String
            Get
                Return _privateMessage
            End Get
            Set(ByVal value As String)
                _privateMessage = value
            End Set
        End Property

        Public Property IconListEnroll() As Boolean
            Get
                Return _iconListEnroll
            End Get
            Set(ByVal value As Boolean)
                _iconListEnroll = value
            End Set
        End Property

        Public Property IconListReminder() As Boolean
            Get
                Return _iconListReminder
            End Get
            Set(ByVal value As Boolean)
                _iconListReminder = value
            End Set
        End Property

        Public Property IconListRec() As Boolean
            Get
                Return _iconListRec
            End Get
            Set(ByVal value As Boolean)
                _iconListRec = value
            End Set
        End Property

        Public Property IconListPrio() As Boolean
            Get
                Return _iconListPrio
            End Get
            Set(ByVal value As Boolean)
                _iconListPrio = value
            End Set
        End Property

        Public Property IconWeekEnroll() As Boolean
            Get
                Return _iconWeekEnroll
            End Get
            Set(ByVal value As Boolean)
                _iconWeekEnroll = value
            End Set
        End Property

        Public Property IconWeekReminder() As Boolean
            Get
                Return _iconWeekReminder
            End Get
            Set(ByVal value As Boolean)
                _iconWeekReminder = value
            End Set
        End Property

        Public Property IconWeekRec() As Boolean
            Get
                Return _iconWeekRec
            End Get
            Set(ByVal value As Boolean)
                _iconWeekRec = value
            End Set
        End Property

        Public Property IconWeekPrio() As Boolean
            Get
                Return _iconWeekPrio
            End Get
            Set(ByVal value As Boolean)
                _iconWeekPrio = value
            End Set
        End Property

        Public Property IconMonthEnroll() As Boolean
            Get
                Return _iconMonthEnroll
            End Get
            Set(ByVal value As Boolean)
                _iconMonthEnroll = value
            End Set
        End Property

        Public Property IconMonthReminder() As Boolean
            Get
                Return _iconMonthReminder
            End Get
            Set(ByVal value As Boolean)
                _iconMonthReminder = value
            End Set
        End Property

        Public Property IconMonthRec() As Boolean
            Get
                Return _iconMonthRec
            End Get
            Set(ByVal value As Boolean)
                _iconMonthRec = value
            End Set
        End Property

        Public Property IconMonthPrio() As Boolean
            Get
                Return _iconMonthPrio
            End Get
            Set(ByVal value As Boolean)
                _iconMonthPrio = value
            End Set
        End Property

        Public Property EventsCustomField2() As Boolean
            Get
                Return _eventsCustomField2
            End Get
            Set(ByVal value As Boolean)
                _eventsCustomField2 = value
            End Set
        End Property

        Public Property EventsCustomField1() As Boolean
            Get
                Return _eventsCustomField1
            End Get
            Set(ByVal value As Boolean)
                _eventsCustomField1 = value
            End Set
        End Property

        Public Property DetailPageAllowed() As Boolean
            Get
                Return _detailPageAllowed
            End Get
            Set(ByVal value As Boolean)
                _detailPageAllowed = value
            End Set
        End Property

        Public Property EnrollmentPageAllowed() As Boolean
            Get
                Return _enrollmentPageAllowed
            End Get
            Set(ByVal value As Boolean)
                _enrollmentPageAllowed = value
            End Set
        End Property

        Public Property EnrollmentPageDefaultUrl() As String
            Get
                Return _enrollmentPageDefaultUrl
            End Get
            Set(ByVal value As String)
                _enrollmentPageDefaultUrl = value
            End Set
        End Property

        Public Property Enrolcanceldays() As Integer
            Get
                Return _enrolcanceldays
            End Get
            Set(ByVal value As Integer)
                _enrolcanceldays = value
            End Set
        End Property

        Public Property Maxnoenrolees() As Integer
            Get
                Return _maxnoenrolees
            End Get
            Set(ByVal value As Integer)
                _maxnoenrolees = value
            End Set
        End Property

        Public Property Eventhidefullenroll() As Boolean
            Get
                Return _eventhidefullenroll
            End Get
            Set(ByVal value As Boolean)
                _eventhidefullenroll = value
            End Set
        End Property

        Public Property EnrollAnonFields() As String
            Get
                Return _enrollAnonFields
            End Get
            Set(ByVal value As String)
                _enrollAnonFields = value
            End Set
        End Property

        Public Property EnrollViewFields() As String
            Get
                Return _enrollViewFields
            End Get
            Set(ByVal value As String)
                _enrollViewFields = value
            End Set
        End Property

        Public Property EnrollEditFields() As String
            Get
                Return _enrollEditFields
            End Get
            Set(ByVal value As String)
                _enrollEditFields = value
            End Set
        End Property

        Public Property IconBar() As String
            Get
                Return _iconBar
            End Get
            Set(ByVal value As String)
                _iconBar = value
            End Set
        End Property

        Public Property EventImageWeek() As Boolean
            Get
                Return _eventImageWeek
            End Get
            Set(ByVal value As Boolean)
                _eventImageWeek = value
            End Set
        End Property

        Public Property EventImageMonth() As Boolean
            Get
                Return _eventImageMonth
            End Get
            Set(ByVal value As Boolean)
                _eventImageMonth = value
            End Set
        End Property

        Public Property ListAllowed() As Boolean
            Get
                Return _listAllowed
            End Get
            Set(ByVal value As Boolean)
                _listAllowed = value
            End Set
        End Property

        Public Property WeekAllowed() As Boolean
            Get
                Return _weekAllowed
            End Get
            Set(ByVal value As Boolean)
                _weekAllowed = value
            End Set
        End Property

        Public Property MonthAllowed() As Boolean
            Get
                Return _monthAllowed
            End Get
            Set(ByVal value As Boolean)
                _monthAllowed = value
            End Set
        End Property

        Public Property Exportanonowneremail() As Boolean
            Get
                Return _exportanonowneremail
            End Get
            Set(ByVal value As Boolean)
                _exportanonowneremail = value
            End Set
        End Property

        Public Property Exportowneremail() As Boolean
            Get
                Return _exportowneremail
            End Get
            Set(ByVal value As Boolean)
                _exportowneremail = value
            End Set
        End Property

        Public Property Expireevents() As String
            Get
                Return _expireevents
            End Get
            Set(ByVal value As String)
                _expireevents = value
            End Set
        End Property

        Public Property RSSDesc() As String
            Get
                If _rssDesc = Nothing And Not _localresourcefile Is Nothing Then
                    _rssDesc = Localization.GetString("RSSFeedDescDefault", _localresourcefile)
                End If
                Return _rssDesc
            End Get
            Set(ByVal value As String)
                _rssDesc = value
            End Set
        End Property

        Public Property RSSTitle() As String
            Get
                If _rssTitle = Nothing And Not _localresourcefile Is Nothing Then
                    _rssTitle = Localization.GetString("RSSFeedTitleDefault", _localresourcefile)
                End If
                Return _rssTitle
            End Get
            Set(ByVal value As String)
                _rssTitle = value
            End Set
        End Property

        Public Property RSSDays() As Integer
            Get
                Return _rssDays
            End Get
            Set(ByVal value As Integer)
                _rssDays = value
            End Set
        End Property

        Public Property RSSDateField() As String
            Get
                Return _rssDateField
            End Get
            Set(ByVal value As String)
                _rssDateField = value
            End Set
        End Property

        Public Property RSSEnable() As Boolean
            Get
                Return _rssEnable
            End Get
            Set(ByVal value As Boolean)
                _rssEnable = value
            End Set
        End Property

        Public Property EventsListSortDirection() As String
            Get
                Return _eventsListSortDirection
            End Get
            Set(ByVal value As String)
                _eventsListSortDirection = value
            End Set
        End Property

        Public Property EventsListPageSize() As Integer
            Get
                Return _eventsListPageSize
            End Get
            Set(ByVal value As Integer)
                _eventsListPageSize = value
            End Set
        End Property

        Public Property EventsListEventDays() As Integer
            Get
                Return _eventsListEventDays
            End Get
            Set(ByVal value As Integer)
                _eventsListEventDays = value
            End Set
        End Property

        Public Property EventsListNumEvents() As Integer
            Get
                Return _eventsListNumEvents
            End Get
            Set(ByVal value As Integer)
                _eventsListNumEvents = value
            End Set
        End Property

        Public Property EventsListSelectType() As String
            Get
                Return _eventsListSelectType
            End Get
            Set(ByVal value As String)
                _eventsListSelectType = value
            End Set
        End Property

        Public Property Reminderfrom() As String
            Get
                If _reminderfrom Is Nothing Then
                    Dim portalsettings As Entities.Portals.PortalSettings = Entities.Portals.PortalController.GetCurrentPortalSettings
                    If Not portalsettings Is Nothing Then
                        _reminderfrom = portalsettings.Email
                    End If
                End If
                Return _reminderfrom
            End Get
            Set(ByVal value As String)
                _reminderfrom = value
            End Set
        End Property

        Public Property Moderateall() As Boolean
            Get
                Return _moderateall
            End Get
            Set(ByVal value As Boolean)
                _moderateall = value
            End Set
        End Property

        Public Property Paypalaccount() As String
            Get
                If _paypalaccount Is Nothing Then
                    Dim portalsettings As Entities.Portals.PortalSettings = Entities.Portals.PortalController.GetCurrentPortalSettings
                    If Not portalsettings Is Nothing Then
                        _paypalaccount = portalsettings.Email
                    End If
                End If
                Return _paypalaccount
            End Get
            Set(ByVal value As String)
                _paypalaccount = value
            End Set
        End Property

        Public Property Eventdefaultenrollview() As Boolean
            Get
                Return _eventdefaultenrollview
            End Get
            Set(ByVal value As Boolean)
                _eventdefaultenrollview = value
            End Set
        End Property

        Public Property Fridayweekend() As Boolean
            Get
                Return _fridayweekend
            End Get
            Set(ByVal value As Boolean)
                _fridayweekend = value
            End Set
        End Property

        Public Property Enforcesubcalperms() As Boolean
            Get
                Return _enforcesubcalperms
            End Get
            Set(ByVal value As Boolean)
                _enforcesubcalperms = value
            End Set
        End Property

        Public Property Addsubmodulename() As Boolean
            Get
                Return _addsubmodulename
            End Get
            Set(ByVal value As Boolean)
                _addsubmodulename = value
            End Set
        End Property

        Public Property MasterEvent() As Boolean
            Get
                Return _masterEvent
            End Get
            Set(ByVal value As Boolean)
                _masterEvent = value
            End Set
        End Property

        Public Property Monthdayselect() As Boolean
            Get
                Return _monthdayselect
            End Get
            Set(ByVal value As Boolean)
                _monthdayselect = value
            End Set
        End Property

        Public Property Timeintitle() As Boolean
            Get
                Return _timeintitle
            End Get
            Set(ByVal value As Boolean)
                _timeintitle = value
            End Set
        End Property

        Public Property ShowEventsAlways() As Boolean
            Get
                Return _showEventsAlways
            End Get
            Set(ByVal value As Boolean)
                _showEventsAlways = value
            End Set
        End Property

        Public Property Locationconflict() As Boolean
            Get
                Return _locationconflict
            End Get
            Set(ByVal value As Boolean)
                _locationconflict = value
            End Set
        End Property

        Public Property Preventconflicts() As Boolean
            Get
                Return _preventconflicts
            End Get
            Set(ByVal value As Boolean)
                _preventconflicts = value
            End Set
        End Property

        Public Property Eventsearch() As Boolean
            Get
                Return _eventsearch
            End Get
            Set(ByVal value As Boolean)
                _eventsearch = value
            End Set
        End Property

        Public Property Allowreoccurring() As Boolean
            Get
                Return _allowreoccurring
            End Get
            Set(ByVal value As Boolean)
                _allowreoccurring = value
            End Set
        End Property

        Public Property Eventimage() As Boolean
            Get
                Return _eventimage
            End Get
            Set(ByVal value As Boolean)
                _eventimage = value
            End Set
        End Property

        Public Property Showvaluemarks() As Boolean
            Get
                Return _showvaluemarks
            End Get
            Set(ByVal value As Boolean)
                _showvaluemarks = value
            End Set
        End Property

        Public Property Includeendvalue() As Boolean
            Get
                Return _includeendvalue
            End Get
            Set(ByVal value As Boolean)
                _includeendvalue = value
            End Set
        End Property

        Public Property Fulltimescale() As Boolean
            Get
                Return _fulltimescale
            End Get
            Set(ByVal value As Boolean)
                _fulltimescale = value
            End Set
        End Property

        Public Property Collapserecurring() As Boolean
            Get
                Return _collapserecurring
            End Get
            Set(ByVal value As Boolean)
                _collapserecurring = value
            End Set
        End Property

        Public Property DisableEventnav() As Boolean
            Get
                Return _disableEventnav
            End Get
            Set(ByVal value As Boolean)
                _disableEventnav = value
            End Set
        End Property

        Public Property Paypalurl() As String
            Get
                Return _paypalurl
            End Get
            Set(ByVal value As String)
                _paypalurl = value
            End Set
        End Property

        Public Property Tzdisplay() As Boolean
            Get
                Return _tzdisplay
            End Get
            Set(ByVal value As Boolean)
                _tzdisplay = value
            End Set
        End Property

        Public Property Newpereventemail() As Boolean
            Get
                Return _newpereventemail
            End Get
            Set(ByVal value As Boolean)
                _newpereventemail = value
            End Set
        End Property

        Public Property Neweventemailrole() As Integer
            Get
                If _neweventemailrole < 0 Then
                    Dim portalsettings As Entities.Portals.PortalSettings = Entities.Portals.PortalController.GetCurrentPortalSettings
                    If Not portalsettings Is Nothing Then
                        _neweventemailrole = portalsettings.RegisteredRoleId
                    End If
                End If
                Return _neweventemailrole
            End Get
            Set(ByVal value As Integer)
                _neweventemailrole = value
            End Set
        End Property

        Public Property Neweventemails() As String
            Get
                Return _neweventemails
            End Get
            Set(ByVal value As String)
                _neweventemails = value
            End Set
        End Property

        Public Property Sendreminderdefault() As Boolean
            Get
                Return _sendreminderdefault
            End Get
            Set(ByVal value As Boolean)
                _sendreminderdefault = value
            End Set
        End Property

        Public Property Notifyanon() As Boolean
            Get
                Return _notifyanon
            End Get
            Set(ByVal value As Boolean)
                _notifyanon = value
            End Set
        End Property

        Public Property Eventnotify() As Boolean
            Get
                Return _eventnotify
            End Get
            Set(ByVal value As Boolean)
                _eventnotify = value
            End Set
        End Property

        Public Property DefaultView() As String
            Get
                Return _defaultView
            End Get
            Set(ByVal value As String)
                _defaultView = value
            End Set
        End Property

        Public Property Eventdaynewpage() As Boolean
            Get
                Return _eventdaynewpage
            End Get
            Set(ByVal value As Boolean)
                _eventdaynewpage = value
            End Set
        End Property

        Public Property Enableenrollpopup() As Boolean
            Get
                Return _enableenrollpopup
            End Get
            Set(ByVal value As Boolean)
                _enableenrollpopup = value
            End Set
        End Property

        Public Property Maxrecurrences() As String
            Get
                Return _maxrecurrences
            End Get
            Set(ByVal value As String)
                _maxrecurrences = value
            End Set
        End Property

        Public Property Version() As String
            Get
                Return _version
            End Get
            Set(ByVal value As String)
                _version = value
            End Set
        End Property

        Public Property Timeinterval() As String
            Get
                Return _timeinterval
            End Get
            Set(ByVal value As String)
                _timeinterval = value
            End Set
        End Property

        Private Property TimeZone() As String
            Get
                Return _timeZone
            End Get
            Set(ByVal value As String)
                _timeZone = value
            End Set
        End Property

        Public Property TimeZoneId() As String
            Get
                If _timeZoneId Is Nothing Then
                    If _timeZone Is Nothing Then
                        Dim portalsettings As Entities.Portals.PortalSettings = Entities.Portals.PortalController.GetCurrentPortalSettings
                        If Not portalsettings Is Nothing Then
                            _timeZoneId = portalsettings.TimeZone.Id
                        End If
                    Else
                        _timeZoneId = Localization.ConvertLegacyTimeZoneOffsetToTimeZoneInfo(CInt(_timeZone)).Id
                    End If
                End If
                Return _timeZoneId
            End Get
            Set(ByVal value As String)
                _timeZoneId = value
            End Set
        End Property

        Public Property EnableEventTimeZones() As Boolean
            Get
                Return _enableEventTimeZones
            End Get
            Set(ByVal value As Boolean)
                _enableEventTimeZones = value
            End Set
        End Property

        Public Property PrimaryTimeZone() As TimeZones
            Get
                Return _primaryTimeZone
            End Get
            Set(ByVal value As TimeZones)
                _primaryTimeZone = value
            End Set
        End Property

        Public Property SecondaryTimeZone() As TimeZones
            Get
                Return _secondaryTimeZone
            End Get
            Set(ByVal value As TimeZones)
                _secondaryTimeZone = value
            End Set
        End Property

        Public Property Eventtooltiplist() As Boolean
            Get
                Return _eventtooltiplist
            End Get
            Set(ByVal value As Boolean)
                _eventtooltiplist = value
            End Set
        End Property

        Public Property Eventtooltipday() As Boolean
            Get
                Return _eventtooltipday
            End Get
            Set(ByVal value As Boolean)
                _eventtooltipday = value
            End Set
        End Property

        Public Property Eventtooltipweek() As Boolean
            Get
                Return _eventtooltipweek
            End Get
            Set(ByVal value As Boolean)
                _eventtooltipweek = value
            End Set
        End Property

        Public Property Eventtooltipmonth() As Boolean
            Get
                Return _eventtooltipmonth
            End Get
            Set(ByVal value As Boolean)
                _eventtooltipmonth = value
            End Set
        End Property

        Public Property Eventtooltiplength() As Integer
            Get
                Return _eventtooltiplength
            End Get
            Set(ByVal value As Integer)
                _eventtooltiplength = value
            End Set
        End Property

        Public Property Monthcellnoevents() As Boolean
            Get
                Return _monthcellnoevents
            End Get
            Set(ByVal value As Boolean)
                _monthcellnoevents = value
            End Set
        End Property

        Public Property Restrictcategories() As Boolean
            Get
                Return _restrictcategories
            End Get
            Set(ByVal value As Boolean)
                _restrictcategories = value
            End Set
        End Property

        Public Property RestrictCategoriesToTimeFrame() As Boolean
            Get
                Return _restrictcategoriestotimeframe
            End Get
            Set(ByVal value As Boolean)
                _restrictcategoriestotimeframe = value
            End Set
        End Property

        Public Property Enablecategories() As DisplayCategories
            Get
                If _enablecategories = 0 Then
                    If _disablecategories Then
                        Return DisplayCategories.DoNotDisplay
                    Else
                        Return DisplayCategories.MultiSelect
                    End If
                Else
                    Return _enablecategories
                End If
            End Get
            Set(ByVal value As DisplayCategories)
                _enablecategories = value
            End Set
        End Property

        Public Property Restrictlocations() As Boolean
            Get
                Return _restrictlocations
            End Get
            Set(ByVal value As Boolean)
                _restrictlocations = value
            End Set
        End Property

        Public Property RestrictLocationsToTimeFrame() As Boolean
            Get
                Return _restrictlocationstotimeframe
            End Get
            Set(ByVal value As Boolean)
                _restrictlocationstotimeframe = value
            End Set
        End Property

        Public Property Enablelocations() As DisplayLocations
            Get
                If _enablelocations = 0 Then
                    If _disablelocations Then
                        Return DisplayLocations.DoNotDisplay
                    Else
                        Return DisplayLocations.MultiSelect
                    End If
                Else
                    Return _enablelocations
                End If
            End Get
            Set(ByVal value As DisplayLocations)
                _enablelocations = value
            End Set
        End Property

        Public Property Enablecontainerskin() As Boolean
            Get
                Return _enablecontainerskin
            End Get
            Set(ByVal value As Boolean)
                _enablecontainerskin = value
            End Set
        End Property

        Public Property Eventdetailnewpage() As Boolean
            Get
                Return _eventdetailnewpage
            End Get
            Set(ByVal value As Boolean)
                _eventdetailnewpage = value
            End Set
        End Property

        Public Property Ownerchangeallowed() As Boolean
            Get
                Return _ownerchangeallowed
            End Get
            Set(ByVal value As Boolean)
                _ownerchangeallowed = value
            End Set
        End Property

        Public Property Eventsignupallowpaid() As Boolean
            Get
                Return _eventsignupallowpaid
            End Get
            Set(ByVal value As Boolean)
                _eventsignupallowpaid = value
            End Set
        End Property

        Public Property Eventsignup() As Boolean
            Get
                Return _eventsignup
            End Get
            Set(ByVal value As Boolean)
                _eventsignup = value
            End Set
        End Property

        Public Property SendEnrollMessageApproved() As Boolean
            Get
                Return _sendEnrollMessageApproved
            End Get
            Set(ByVal value As Boolean)
                _sendEnrollMessageApproved = value
            End Set
        End Property

        Public Property SendEnrollMessageWaiting() As Boolean
            Get
                Return _sendEnrollMessageWaiting
            End Get
            Set(ByVal value As Boolean)
                _sendEnrollMessageWaiting = value
            End Set
        End Property

        Public Property SendEnrollMessageDenied() As Boolean
            Get
                Return _sendEnrollMessageDenied
            End Get
            Set(ByVal value As Boolean)
                _sendEnrollMessageDenied = value
            End Set
        End Property

        Public Property SendEnrollMessageAdded() As Boolean
            Get
                Return _sendEnrollMessageAdded
            End Get
            Set(ByVal value As Boolean)
                _sendEnrollMessageAdded = value
            End Set
        End Property

        Public Property SendEnrollMessageDeleted() As Boolean
            Get
                Return _sendEnrollMessageDeleted
            End Get
            Set(ByVal value As Boolean)
                _sendEnrollMessageDeleted = value
            End Set
        End Property

        Public Property SendEnrollMessagePaying() As Boolean
            Get
                Return _sendEnrollMessagePaying
            End Get
            Set(ByVal value As Boolean)
                _sendEnrollMessagePaying = value
            End Set
        End Property

        Public Property SendEnrollMessagePending() As Boolean
            Get
                Return _sendEnrollMessagePending
            End Get
            Set(ByVal value As Boolean)
                _sendEnrollMessagePending = value
            End Set
        End Property

        Public Property SendEnrollMessagePaid() As Boolean
            Get
                Return _sendEnrollMessagePaid
            End Get
            Set(ByVal value As Boolean)
                _sendEnrollMessagePaid = value
            End Set
        End Property

        Public Property SendEnrollMessageIncorrect() As Boolean
            Get
                Return _sendEnrollMessageIncorrect
            End Get
            Set(ByVal value As Boolean)
                _sendEnrollMessageIncorrect = value
            End Set
        End Property

        Public Property SendEnrollMessageCancelled() As Boolean
            Get
                Return _sendEnrollMessageCancelled
            End Get
            Set(ByVal value As Boolean)
                _sendEnrollMessageCancelled = value
            End Set
        End Property

        Public Property AllowAnonEnroll() As Boolean
            Get
                Return _allowanonenroll
            End Get
            Set(ByVal value As Boolean)
                _allowanonenroll = value
            End Set
        End Property

        Public Property SocialGroupModule() As SocialModule
            Get
                Return _socialGroupModule
            End Get
            Set(ByVal value As SocialModule)
                _socialGroupModule = value
            End Set
        End Property

        Public Property SocialUserPrivate() As Boolean
            Get
                Return _socialUserPrivate
            End Get
            Set(ByVal value As Boolean)
                _socialUserPrivate = value
            End Set
        End Property

        Public Property SocialGroupSecurity() As SocialGroupPrivacy
            Get
                Return _socialGroupSecurity
            End Get
            Set(ByVal value As SocialGroupPrivacy)
                _socialGroupSecurity = value
            End Set
        End Property

        Public Property EnrolListSortDirection() As SortDirection
            Get
                Return _enrolListSortDirection
            End Get
            Set(ByVal value As SortDirection)
                _enrolListSortDirection = value
            End Set
        End Property

        Public Property EnrolListDaysBefore() As Integer
            Get
                Return _enrolListDaysBefore
            End Get
            Set(ByVal value As Integer)
                _enrolListDaysBefore = value
            End Set
        End Property

        Public Property EnrolListDaysAfter() As Integer
            Get
                Return _enrolListDaysAfter
            End Get
            Set(ByVal value As Integer)
                _enrolListDaysAfter = value
            End Set
        End Property

        Public Property JournalIntegration() As Boolean
            Get
                Return _journalIntegration
            End Get
            Set(ByVal value As Boolean)
                _journalIntegration = value
            End Set
        End Property

        Public Property Templates() As EventTemplates
            Get
                Return _templates
            End Get
            Set(ByVal value As EventTemplates)
                _templates = value
            End Set
        End Property

#End Region

    End Class

#End Region

#Region "EventTemplates"

    <Serializable()>
    Public Class EventTemplates

#Region " Private Members "
        ' ReSharper disable InconsistentNaming
        Private _EventDetailsTemplate As String = Nothing
        Private _NewEventTemplate As String = Nothing
        Private _txtToolTipTemplateTitleNT As String = Nothing
        Private _txtToolTipTemplateBodyNT As String = Nothing
        Private _txtToolTipTemplateTitle As String = Nothing
        Private _txtToolTipTemplateBody As String = Nothing
        Private _moderateemailsubject As String = Nothing
        Private _moderateemailmessage As String = Nothing
        Private _txtEmailSubject As String = Nothing
        Private _txtEmailMessage As String = Nothing
        Private _txtEnrollMessageSubject As String = Nothing
        Private _txtEnrollMessageApproved As String = Nothing
        Private _txtEnrollMessageWaiting As String = Nothing
        Private _txtEnrollMessageDenied As String = Nothing
        Private _txtEnrollMessageAdded As String = Nothing
        Private _txtEnrollMessageDeleted As String = Nothing
        Private _txtEnrollMessagePaying As String = Nothing
        Private _txtEnrollMessagePending As String = Nothing
        Private _txtEnrollMessagePaid As String = Nothing
        Private _txtEnrollMessageIncorrect As String = Nothing
        Private _txtEnrollMessageCancelled As String = Nothing
        Private _txtEditViewEmailSubject As String = Nothing
        Private _txtEditViewEmailBody As String = Nothing
        Private _txtSubject As String = Nothing
        Private _txtMessage As String = Nothing
        Private _txtNewEventEmailSubject As String = Nothing
        Private _txtNewEventEmailMessage As String = Nothing
        Private _txtListEventTimeBegin As String = Nothing
        Private _txtListEventTimeEnd As String = Nothing
        Private _txtListLocation As String = Nothing
        Private _txtListEventDescription As String = Nothing
        Private _txtListRptHeader As String = Nothing
        Private _txtListRptBody As String = Nothing
        Private _txtListRptFooter As String = Nothing
        Private _txtDayEventTimeBegin As String = Nothing
        Private _txtDayEventTimeEnd As String = Nothing
        Private _txtDayLocation As String = Nothing
        Private _txtDayEventDescription As String = Nothing
        Private _txtWeekEventText As String = Nothing
        Private _txtWeekTitleDate As String = Nothing
        Private _txtMonthEventText As String = Nothing
        Private _txtMonthDayEventCount As String = Nothing
        Private _txtRSSTitle As String = Nothing
        Private _txtRSSDescription As String = Nothing
        Private _txtSEOPageTitle As String = Nothing
        Private _txtSEOPageDescription As String = Nothing
        Private _EventiCalSubject As String = Nothing
        Private _EventiCalBody As String = Nothing
        ' ReSharper restore InconsistentNaming

#End Region

#Region " Constructors "

        Public Sub New(ByVal moduleID As Integer, ByVal allsettings As Hashtable, ByVal localResourceFile As String)

            Dim t As Type = Me.GetType
            Dim p As System.Reflection.PropertyInfo
            For Each p In t.GetProperties
                Dim pn As String = p.Name
                Dim pv As String = Nothing
                If Not allsettings.Item(pn) Is Nothing Then
                    pv = CType(allsettings.Item(pn), String)
                    If Len(pv) > 1900 Then
                        If Not allsettings.Item(pn + "2") Is Nothing Then
                            pv = pv + CType(allsettings.Item(pn + "2"), String)
                        End If
                    End If
                Else
                    If Not localResourceFile Is Nothing Then
                        pv = Localization.GetString(pn, localResourceFile)
                        If moduleID > 0 Then
                            SaveTemplate(moduleID, pn, pv)
                        End If
                    End If
                End If
                p.SetValue(Me, pv, Nothing)
            Next

        End Sub

#End Region

#Region " Public Methods "
        Public Function GetTemplate(ByVal templateName As String) As String
            Dim t As Type = Me.GetType
            Dim p As System.Reflection.PropertyInfo
            For Each p In t.GetProperties
                If p.Name = templateName Then
                    Return p.GetValue(Me, Nothing).ToString
                End If
            Next
            Return ""
        End Function

        Public Sub SaveTemplate(ByVal moduleID As Integer, ByVal templateName As String, ByVal templateValue As String)
            Dim t As Type = Me.GetType
            Dim p As System.Reflection.PropertyInfo
            For Each p In t.GetProperties
                If p.Name = templateName Then
                    p.SetValue(Me, templateValue, Nothing)
                    Dim objModules As New ModuleController

                    If Len(templateValue) > 2000 Then
                        objModules.UpdateModuleSetting(moduleID, templateName, Left(templateValue, 2000))
                        objModules.UpdateModuleSetting(moduleID, templateName + "2", Mid(templateValue, 2001))
                    Else
                        objModules.UpdateModuleSetting(moduleID, templateName, templateValue.Trim)
                        objModules.DeleteModuleSetting(moduleID, templateName + "2")
                    End If
                End If
            Next
            Dim cacheKey As String = "EventsSettings" & moduleID.ToString
            ClearCache(cacheKey)
        End Sub

        Public Sub ResetTemplate(ByVal moduleID As Integer, ByVal templateName As String, ByVal localResourceFile As String)
            Dim templateValue As String = Localization.GetString(templateName, localResourceFile)
            SaveTemplate(moduleID, templateName, templateValue)

        End Sub

#End Region

#Region " Private Methods "

#End Region

#Region " Properties "
        ' ReSharper disable InconsistentNaming
        Public Property EventDetailsTemplate() As String
            Get
                Return _EventDetailsTemplate
            End Get
            Set(ByVal value As String)
                _EventDetailsTemplate = value
            End Set
        End Property

        Public Property NewEventTemplate() As String
            Get
                Return _NewEventTemplate
            End Get
            Set(ByVal value As String)
                _NewEventTemplate = value
            End Set
        End Property

        Public Property txtTooltipTemplateTitleNT() As String
            Get
                Return _txtToolTipTemplateTitleNT
            End Get
            Set(ByVal value As String)
                _txtToolTipTemplateTitleNT = value
            End Set
        End Property

        Public Property txtTooltipTemplateBodyNT() As String
            Get
                Return _txtToolTipTemplateBodyNT
            End Get
            Set(ByVal value As String)
                _txtToolTipTemplateBodyNT = value
            End Set
        End Property

        Public Property txtTooltipTemplateTitle() As String
            Get
                Return _txtToolTipTemplateTitle
            End Get
            Set(ByVal value As String)
                _txtToolTipTemplateTitle = value
            End Set
        End Property

        Public Property txtTooltipTemplateBody() As String
            Get
                Return _txtToolTipTemplateBody
            End Get
            Set(ByVal value As String)
                _txtToolTipTemplateBody = value
            End Set
        End Property

        Public Property moderateemailsubject() As String
            Get
                Return _moderateemailsubject
            End Get
            Set(ByVal value As String)
                _moderateemailsubject = value
            End Set
        End Property

        Public Property moderateemailmessage() As String
            Get
                Return _moderateemailmessage
            End Get
            Set(ByVal value As String)
                _moderateemailmessage = value
            End Set
        End Property

        Public Property txtEmailSubject() As String
            Get
                Return _txtEmailSubject
            End Get
            Set(ByVal value As String)
                _txtEmailSubject = value
            End Set
        End Property

        Public Property txtEmailMessage() As String
            Get
                Return _txtEmailMessage
            End Get
            Set(ByVal value As String)
                _txtEmailMessage = value
            End Set
        End Property

        Public Property txtEnrollMessageSubject() As String
            Get
                Return _txtEnrollMessageSubject
            End Get
            Set(ByVal value As String)
                _txtEnrollMessageSubject = value
            End Set
        End Property

        Public Property txtEnrollMessageApproved() As String
            Get
                Return _txtEnrollMessageApproved
            End Get
            Set(ByVal value As String)
                _txtEnrollMessageApproved = value
            End Set
        End Property

        Public Property txtEnrollMessageWaiting() As String
            Get
                Return _txtEnrollMessageWaiting
            End Get
            Set(ByVal value As String)
                _txtEnrollMessageWaiting = value
            End Set
        End Property

        Public Property txtEnrollMessageDenied() As String
            Get
                Return _txtEnrollMessageDenied
            End Get
            Set(ByVal value As String)
                _txtEnrollMessageDenied = value
            End Set
        End Property

        Public Property txtEnrollMessageAdded() As String
            Get
                Return _txtEnrollMessageAdded
            End Get
            Set(ByVal value As String)
                _txtEnrollMessageAdded = value
            End Set
        End Property

        Public Property txtEnrollMessageDeleted() As String
            Get
                Return _txtEnrollMessageDeleted
            End Get
            Set(ByVal value As String)
                _txtEnrollMessageDeleted = value
            End Set
        End Property

        Public Property txtEnrollMessagePaying() As String
            Get
                Return _txtEnrollMessagePaying
            End Get
            Set(ByVal value As String)
                _txtEnrollMessagePaying = value
            End Set
        End Property

        Public Property txtEnrollMessagePending() As String
            Get
                Return _txtEnrollMessagePending
            End Get
            Set(ByVal value As String)
                _txtEnrollMessagePending = value
            End Set
        End Property

        Public Property txtEnrollMessagePaid() As String
            Get
                Return _txtEnrollMessagePaid
            End Get
            Set(ByVal value As String)
                _txtEnrollMessagePaid = value
            End Set
        End Property

        Public Property txtEnrollMessageIncorrect() As String
            Get
                Return _txtEnrollMessageIncorrect
            End Get
            Set(ByVal value As String)
                _txtEnrollMessageIncorrect = value
            End Set
        End Property

        Public Property txtEnrollMessageCancelled() As String
            Get
                Return _txtEnrollMessageCancelled
            End Get
            Set(ByVal value As String)
                _txtEnrollMessageCancelled = value
            End Set
        End Property

        Public Property txtEditViewEmailSubject() As String
            Get
                Return _txtEditViewEmailSubject
            End Get
            Set(ByVal value As String)
                _txtEditViewEmailSubject = value
            End Set
        End Property

        Public Property txtEditViewEmailBody() As String
            Get
                Return _txtEditViewEmailBody
            End Get
            Set(ByVal value As String)
                _txtEditViewEmailBody = value
            End Set
        End Property

        Public Property txtSubject() As String
            Get
                Return _txtSubject
            End Get
            Set(ByVal value As String)
                _txtSubject = value
            End Set
        End Property

        Public Property txtMessage() As String
            Get
                Return _txtMessage
            End Get
            Set(ByVal value As String)
                _txtMessage = value
            End Set
        End Property

        Public Property txtNewEventEmailSubject() As String
            Get
                Return _txtNewEventEmailSubject
            End Get
            Set(ByVal value As String)
                _txtNewEventEmailSubject = value
            End Set
        End Property

        Public Property txtNewEventEmailMessage() As String
            Get
                Return _txtNewEventEmailMessage
            End Get
            Set(ByVal value As String)
                _txtNewEventEmailMessage = value
            End Set
        End Property

        Public Property txtListEventTimeBegin() As String
            Get
                Return _txtListEventTimeBegin
            End Get
            Set(ByVal value As String)
                _txtListEventTimeBegin = value
            End Set
        End Property

        Public Property txtListEventTimeEnd() As String
            Get
                Return _txtListEventTimeEnd
            End Get
            Set(ByVal value As String)
                _txtListEventTimeEnd = value
            End Set
        End Property

        Public Property txtListLocation() As String
            Get
                Return _txtListLocation
            End Get
            Set(ByVal value As String)
                _txtListLocation = value
            End Set
        End Property

        Public Property txtListEventDescription() As String
            Get
                Return _txtListEventDescription
            End Get
            Set(ByVal value As String)
                _txtListEventDescription = value
            End Set
        End Property

        Public Property txtListRptHeader() As String
            Get
                Return _txtListRptHeader
            End Get
            Set(ByVal value As String)
                _txtListRptHeader = value
            End Set
        End Property

        Public Property txtListRptBody() As String
            Get
                Return _txtListRptBody
            End Get
            Set(ByVal value As String)
                _txtListRptBody = value
            End Set
        End Property

        Public Property txtListRptFooter() As String
            Get
                Return _txtListRptFooter
            End Get
            Set(ByVal value As String)
                _txtListRptFooter = value
            End Set
        End Property

        Public Property txtDayEventTimeBegin() As String
            Get
                Return _txtDayEventTimeBegin
            End Get
            Set(ByVal value As String)
                _txtDayEventTimeBegin = value
            End Set
        End Property

        Public Property txtDayEventTimeEnd() As String
            Get
                Return _txtDayEventTimeEnd
            End Get
            Set(ByVal value As String)
                _txtDayEventTimeEnd = value
            End Set
        End Property

        Public Property txtDayLocation() As String
            Get
                Return _txtDayLocation
            End Get
            Set(ByVal value As String)
                _txtDayLocation = value
            End Set
        End Property

        Public Property txtDayEventDescription() As String
            Get
                Return _txtDayEventDescription
            End Get
            Set(ByVal value As String)
                _txtDayEventDescription = value
            End Set
        End Property

        Public Property txtWeekEventText() As String
            Get
                Return _txtWeekEventText
            End Get
            Set(ByVal value As String)
                _txtWeekEventText = value
            End Set
        End Property

        Public Property txtWeekTitleDate() As String
            Get
                Return _txtWeekTitleDate
            End Get
            Set(ByVal value As String)
                _txtWeekTitleDate = value
            End Set
        End Property

        Public Property txtMonthEventText() As String
            Get
                Return _txtMonthEventText
            End Get
            Set(ByVal value As String)
                _txtMonthEventText = value
            End Set
        End Property

        Public Property txtMonthDayEventCount() As String
            Get
                Return _txtMonthDayEventCount
            End Get
            Set(ByVal value As String)
                _txtMonthDayEventCount = value
            End Set
        End Property

        Public Property txtRSSTitle() As String
            Get
                Return _txtRSSTitle
            End Get
            Set(ByVal value As String)
                _txtRSSTitle = value
            End Set
        End Property

        Public Property txtRSSDescription() As String
            Get
                Return _txtRSSDescription
            End Get
            Set(ByVal value As String)
                _txtRSSDescription = value
            End Set
        End Property

        Public Property txtSEOPageTitle() As String
            Get
                Return _txtSEOPageTitle
            End Get
            Set(ByVal value As String)
                _txtSEOPageTitle = value
            End Set
        End Property

        Public Property txtSEOPageDescription() As String
            Get
                Return _txtSEOPageDescription
            End Get
            Set(ByVal value As String)
                _txtSEOPageDescription = value
            End Set
        End Property

        Public Property EventiCalSubject() As String
            Get
                Return _EventiCalSubject
            End Get
            Set(ByVal value As String)
                _EventiCalSubject = value
            End Set
        End Property

        Public Property EventiCalBody() As String
            Get
                Return _EventiCalBody
            End Get
            Set(ByVal value As String)
                _EventiCalBody = value
            End Set
        End Property
        ' ReSharper restore InconsistentNaming

#End Region

    End Class
#End Region

End Namespace
