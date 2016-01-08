'
' DotNetNuke® - http://www.dnnsoftware.com
' Copyright (c) 2002-2013
' by DNNCorp
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'
Imports System
Imports System.Web
Imports System.Web.UI.WebControls
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Web.UI.WebControls.Extensions

Namespace DotNetNuke.Modules.Events

    <DNNtc.ModuleControlProperties("EventSettings", "Event Settings", DNNtc.ControlType.View, "https://dnnevents.codeplex.com/documentation", True, True)> _
    Partial Class EventSettings
        Inherits EventBase

#Region " Web Form Designer Generated Code "
        'This call is required by the Web Form Designer.
        <DebuggerStepThrough()> Private Sub InitializeComponent()
        End Sub

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()
        End Sub
#End Region

#Region "Private Data"
        Private ReadOnly _objCtlMasterEvent As New EventMasterController
#End Region

#Region "Help Methods"
        ' If adding new Setting also see 'SetDefaultModuleSettings' method in EventInfoHelper Class

        ''' <summary>
        ''' Load current settings into the controls from the modulesettings
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load, Me.Load
            If Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName.ToString) Or _
               IsSettingsEditor() Then
            Else
                Response.Redirect(GetSocialNavigateUrl(), True)
            End If

            ' Set the selected theme 
            SetTheme(pnlEventsModuleSettings)

            ' Do we have to load the settings?
            If Not Page.IsPostBack Then
                LoadSettings()
            End If

            ' Add the javascript to the page
            AddJavaScript()

        End Sub

        Private Sub LoadSettings()

            Dim availableFields As New ArrayList
            Dim selectedFields As New ArrayList

            ' Create Lists and Schedule - they should always exist
            Dim objEventController As New EventController
            objEventController.CreateListsAndSchedule()

            'Set text and tooltip from resourcefile
            chkMonthAllowed.Text = Localization.GetString("Month", LocalResourceFile)
            chkWeekAllowed.Text = Localization.GetString("Week", LocalResourceFile)
            chkListAllowed.Text = Localization.GetString("List", LocalResourceFile)
            cmdAdd.ToolTip = Localization.GetString("Add", LocalResourceFile)
            cmdAddAll.ToolTip = Localization.GetString("AddAll", LocalResourceFile)
            cmdRemove.ToolTip = Localization.GetString("Remove", LocalResourceFile)
            cmdRemoveAll.ToolTip = Localization.GetString("RemoveAll", LocalResourceFile)
            cmdAddCals.ToolTip = Localization.GetString("AddCals", LocalResourceFile)
            cmdAddAllCals.ToolTip = Localization.GetString("AddAllCals", LocalResourceFile)
            cmdRemoveCals.ToolTip = Localization.GetString("RemoveCals", LocalResourceFile)
            cmdRemoveAllCals.ToolTip = Localization.GetString("RemoveAllCals", LocalResourceFile)
            chkIconMonthPrio.Text = Localization.GetString("Priority", LocalResourceFile)
            imgIconMonthPrioHigh.AlternateText = Localization.GetString("HighPrio", LocalResourceFile)
            imgIconMonthPrioLow.AlternateText = Localization.GetString("LowPrio", LocalResourceFile)
            chkIconMonthRec.Text = Localization.GetString("Recurring", LocalResourceFile)
            imgIconMonthRec.AlternateText = Localization.GetString("RecurringEvent", LocalResourceFile)
            chkIconMonthReminder.Text = Localization.GetString("Reminder", LocalResourceFile)
            imgIconMonthReminder.AlternateText = Localization.GetString("ReminderEnabled", LocalResourceFile)
            chkIconMonthEnroll.Text = Localization.GetString("Enroll", LocalResourceFile)
            imgIconMonthEnroll.AlternateText = Localization.GetString("EnrollEnabled", LocalResourceFile)
            chkIconWeekPrio.Text = Localization.GetString("Priority", LocalResourceFile)
            imgIconWEEKPrioHigh.AlternateText = Localization.GetString("HighPrio", LocalResourceFile)
            imgIconWeekPrioLow.AlternateText = Localization.GetString("LowPrio", LocalResourceFile)
            chkIconWeekRec.Text = Localization.GetString("Recurring", LocalResourceFile)
            imgIconWeekRec.AlternateText = Localization.GetString("RecurringEvent", LocalResourceFile)
            chkIconWeekReminder.Text = Localization.GetString("Reminder", LocalResourceFile)
            imgIconWeekReminder.AlternateText = Localization.GetString("ReminderEnabled", LocalResourceFile)
            chkIconWeekEnroll.Text = Localization.GetString("Enroll", LocalResourceFile)
            imgIconWeekEnroll.AlternateText = Localization.GetString("EnrollEnabled", LocalResourceFile)
            chkIconListPrio.Text = Localization.GetString("Priority", LocalResourceFile)
            imgIconListPrioHigh.AlternateText = Localization.GetString("HighPrio", LocalResourceFile)
            imgIconListPrioLow.AlternateText = Localization.GetString("LowPrio", LocalResourceFile)
            chkIconListRec.Text = Localization.GetString("Recurring", LocalResourceFile)
            imgIconListRec.AlternateText = Localization.GetString("RecurringEvent", LocalResourceFile)
            chkIconListReminder.Text = Localization.GetString("Reminder", LocalResourceFile)
            imgIconListReminder.AlternateText = Localization.GetString("ReminderEnabled", LocalResourceFile)
            chkIconListEnroll.Text = Localization.GetString("Enroll", LocalResourceFile)
            imgIconListEnroll.AlternateText = Localization.GetString("EnrollEnabled", LocalResourceFile)
            cmdUpdateTemplate.Text = Localization.GetString("cmdUpdateTemplate", LocalResourceFile)
            rbThemeStandard.Text = Localization.GetString("rbThemeStandard", LocalResourceFile)
            rbThemeCustom.Text = Localization.GetString("rbThemeCustom", LocalResourceFile)
            ddlModuleCategories.EmptyMessage = Localization.GetString("NoCategories", LocalResourceFile)
            ddlModuleCategories.Localization.AllItemsCheckedString = Localization.GetString("AllCategories", LocalResourceFile)
            ddlModuleCategories.Localization.CheckAllString = Localization.GetString("SelectAllCategories", LocalResourceFile)
            ddlModuleLocations.EmptyMessage = Localization.GetString("NoLocations", LocalResourceFile)
            ddlModuleLocations.Localization.AllItemsCheckedString = Localization.GetString("AllLocations", LocalResourceFile)
            ddlModuleLocations.Localization.CheckAllString = Localization.GetString("SelectAllLocations", LocalResourceFile)


            'Add templates link 
            ' lnkTemplatesHelp.HRef = AddSkinContainerControls(EditUrl("", "", "TemplateHelp", "dnnprintmode=true"), "?")
            lnkTemplatesHelp.HRef = AddSkinContainerControls(NavigateURL(TabId, PortalSettings, "", "mid=" & ModuleId, "ctl=TemplateHelp", "ShowNav=False", "dnnprintmode=true"), "?")
            lnkTemplatesHelp.InnerText = Localization.GetString("TemplatesHelp", LocalResourceFile)

            'Support for Time Interval Dropdown
            Dim ctlLists As New Lists.ListController
            Dim colThreadStatus As Generic.IEnumerable(Of Lists.ListEntryInfo) = ctlLists.GetListEntryInfoItems("TimeInterval")
            ddlTimeInterval.Items.Clear()

            For Each entry As Lists.ListEntryInfo In colThreadStatus
                ddlTimeInterval.Items.Add(entry.Value)
            Next
            ddlTimeInterval.Items.FindByValue(Settings.Timeinterval).Selected = True

            ' Set Dropdown TimeZone
            cboTimeZone.DataBind(Settings.TimeZoneId)

            chkEnableEventTimeZones.Checked = Settings.EnableEventTimeZones

            BindToEnum(GetType(EventModuleSettings.TimeZones), ddlPrimaryTimeZone)
            ddlPrimaryTimeZone.Items.FindByValue(CType(Settings.PrimaryTimeZone, String)).Selected = True
            BindToEnum(GetType(EventModuleSettings.TimeZones), ddlSecondaryTimeZone)
            ddlSecondaryTimeZone.Items.FindByValue(CType(Settings.SecondaryTimeZone, String)).Selected = True

            chkToolTipMonth.Checked = Settings.Eventtooltipmonth
            chkToolTipWeek.Checked = Settings.Eventtooltipweek
            chkToolTipDay.Checked = Settings.Eventtooltipday
            chkToolTipList.Checked = Settings.Eventtooltiplist
            txtTooltipLength.Text = Settings.Eventtooltiplength.ToString
            chkImageEnabled.Checked = Settings.Eventimage
            txtMaxThumbHeight.Text = Settings.MaxThumbHeight.ToString
            txtMaxThumbWidth.Text = Settings.MaxThumbWidth.ToString

            chkMonthCellEvents.Checked = True
            If Settings.Monthcellnoevents Then
                chkMonthCellEvents.Checked = False
            End If

            chkAddSubModuleName.Checked = Settings.Addsubmodulename
            chkEnforceSubCalPerms.Checked = Settings.Enforcesubcalperms

            BindToEnum(GetType(EventModuleSettings.DisplayCategories), ddlEnableCategories)
            ddlEnableCategories.Items.FindByValue(CType(Settings.Enablecategories, String)).Selected = True
            chkRestrictCategories.Checked = Settings.Restrictcategories
            BindToEnum(GetType(EventModuleSettings.DisplayLocations), ddlEnableLocations)
            ddlEnableLocations.Items.FindByValue(CType(Settings.Enablelocations, String)).Selected = True
            chkRestrictLocations.Checked = Settings.Restrictlocations

            chkEnableContainerSkin.Checked = Settings.Enablecontainerskin
            chkEventDetailNewPage.Checked = Settings.Eventdetailnewpage
            chkEnableEnrollPopup.Checked = Settings.Enableenrollpopup
            chkEventImageMonth.Checked = Settings.EventImageMonth
            chkEventImageWeek.Checked = Settings.EventImageWeek
            chkEventDayNewPage.Checked = Settings.Eventdaynewpage
            chkFullTimeScale.Checked = Settings.Fulltimescale
            chkCollapseRecurring.Checked = Settings.Collapserecurring
            chkIncludeEndValue.Checked = Settings.Includeendvalue
            chkShowValueMarks.Checked = Settings.Showvaluemarks

            chkEnableSEO.Checked = Settings.EnableSEO
            txtSEODescriptionLength.Text = Settings.SEODescriptionLength.ToString

            chkEnableSitemap.Checked = Settings.EnableSitemap
            txtSitemapPriority.Text = Settings.SiteMapPriority.ToString
            txtSitemapDaysBefore.Text = Settings.SiteMapDaysBefore.ToString
            txtSitemapDaysAfter.Text = Settings.SiteMapDaysAfter.ToString

            chkiCalOnIconBar.Checked = Settings.IcalOnIconBar
            chkiCalEmailEnable.Checked = Settings.IcalEmailEnable
            chkiCalURLinLocation.Checked = Settings.IcalURLInLocation
            chkiCalIncludeCalname.Checked = Settings.IcalIncludeCalname
            txtiCalDaysBefore.Text = Settings.IcalDaysBefore.ToString
            txtiCalDaysAfter.Text = Settings.IcalDaysAfter.ToString
            txtiCalURLAppend.Text = Settings.IcalURLAppend
            ctliCalDefaultImage.FileFilter = glbImageFileTypes
            ctliCalDefaultImage.Url = ""
            chkiCalDisplayImage.Checked = False
            If Settings.IcalDefaultImage <> "" Then
                ctliCalDefaultImage.Url = Mid(Settings.IcalDefaultImage, 7)
                chkiCalDisplayImage.Checked = True
            End If
            If ctliCalDefaultImage.Url.StartsWith("FileID=") Then
                Dim fileId As Integer = Integer.Parse(ctliCalDefaultImage.Url.Substring(7))
                Dim objFileInfo As Services.FileSystem.IFileInfo = Services.FileSystem.FileManager.Instance.GetFile(fileId)
                If Not objFileInfo Is Nothing Then
                    ctliCalDefaultImage.Url = objFileInfo.Folder & objFileInfo.FileName
                Else
                    ctliCalDefaultImage.Url = ""
                End If
            End If
            Dim socialGroupId As Integer = GetUrlGroupId()
            Dim socialGroupStr As String = ""
            If socialGroupId > 0 Then
                socialGroupStr = "&groupid=" & socialGroupId.ToString
            End If
            lbliCalURL.Text = AddHTTP(PortalSettings.PortalAlias.HTTPAlias & "/DesktopModules/Events/EventVCal.aspx?ItemID=0&Mid=" & ModuleId & "&tabid=" & TabId & socialGroupStr)

            ' Set Up Themes
            LoadThemes()

            txtPayPalURL.Text = Settings.Paypalurl

            chkEnableEventNav.Checked = True
            If Settings.DisableEventnav Then
                chkEnableEventNav.Checked = False
            End If

            chkAllowRecurring.Checked = Settings.Allowreoccurring
            txtMaxRecurrences.Text = Settings.Maxrecurrences.ToString
            chkEventNotify.Checked = Settings.Eventnotify
            chkDetailPageAllowed.Checked = Settings.DetailPageAllowed
            chkEnrollmentPageAllowed.Checked = Settings.EnrollmentPageAllowed
            txtEnrollmentPageDefaultURL.Text = Settings.EnrollmentPageDefaultUrl
            chkNotifyAnon.Checked = Settings.Notifyanon
            chkSendReminderDefault.Checked = Settings.Sendreminderdefault

            rblNewEventEmail.Items(0).Selected = True
            Select Case Settings.Neweventemails
                Case "Subscribe"
                    rblNewEventEmail.Items(1).Selected = True
                Case "Role"
                    rblNewEventEmail.Items(2).Selected = True
            End Select

            LoadNewEventEmailRoles(Settings.Neweventemailrole)
            chkNewPerEventEmail.Checked = Settings.Newpereventemail

            ddlDefaultView.Items.Clear()
            ddlDefaultView.Items.Add(New ListItem(Localization.GetString("Month", LocalResourceFile), "EventMonth.ascx"))
            ddlDefaultView.Items.Add(New ListItem(Localization.GetString("Week", LocalResourceFile), "EventWeek.ascx"))
            ddlDefaultView.Items.Add(New ListItem(Localization.GetString("List", LocalResourceFile), "EventList.ascx"))

            ddlDefaultView.Items.FindByValue(Settings.DefaultView).Selected = True

            chkMonthAllowed.Checked = Settings.MonthAllowed
            chkWeekAllowed.Checked = Settings.WeekAllowed
            chkListAllowed.Checked = Settings.ListAllowed
            chkEnableSearch.Checked = Settings.Eventsearch
            chkPreventConflicts.Checked = Settings.Preventconflicts
            chkLocationConflict.Checked = Settings.Locationconflict
            chkShowEventsAlways.Checked = Settings.ShowEventsAlways
            chkTimeInTitle.Checked = Settings.Timeintitle
            chkMonthDaySelect.Checked = Settings.Monthdayselect
            chkEventSignup.Checked = Settings.Eventsignup
            chkEventSignupAllowPaid.Checked = Settings.Eventsignupallowpaid
            chkDefaultEnrollView.Checked = Settings.Eventdefaultenrollview
            chkHideFullEnroll.Checked = Settings.Eventhidefullenroll()
            txtMaxNoEnrolees.Text = Settings.Maxnoenrolees.ToString
            txtCancelDays.Text = Settings.Enrolcanceldays.ToString
            chkFridayWeekend.Checked = Settings.Fridayweekend
            chkModerateAll.Checked = Settings.Moderateall
            chkTZDisplay.Checked = Settings.Tzdisplay
            chkListViewUseTime.Checked = Settings.ListViewUseTime

            txtPayPalAccount.Text = Settings.Paypalaccount
            If txtPayPalAccount.Text.Length = 0 Then
                txtPayPalAccount.Text = PortalSettings.Email
            End If

            txtReminderFrom.Text = Settings.Reminderfrom
            If txtReminderFrom.Text.Length = 0 Then
                txtReminderFrom.Text = PortalSettings.Email
            End If

            txtStandardEmail.Text = Settings.StandardEmail
            If txtStandardEmail.Text.Length = 0 Then
                txtStandardEmail.Text = PortalSettings.Email
            End If

            BindSubEvents()
            BindAvailableEvents()

            chkMasterEvent.Checked = Settings.MasterEvent

            Enable_Disable_Cals()

            chkIconMonthPrio.Checked = Settings.IconMonthPrio
            chkIconWeekPrio.Checked = Settings.IconWeekPrio
            chkIconListPrio.Checked = Settings.IconListPrio
            chkIconMonthRec.Checked = Settings.IconMonthRec
            chkIconWeekRec.Checked = Settings.IconMonthRec
            chkIconListRec.Checked = Settings.IconListRec
            chkIconMonthReminder.Checked = Settings.IconMonthReminder
            chkIconWeekReminder.Checked = Settings.IconWeekReminder
            chkIconListReminder.Checked = Settings.IconListReminder
            chkIconMonthEnroll.Checked = Settings.IconMonthEnroll
            chkIconWeekEnroll.Checked = Settings.IconWeekEnroll
            chkIconListEnroll.Checked = Settings.IconListEnroll
            txtPrivateMessage.Text = Settings.PrivateMessage

            Dim columnNo As Integer
            For columnNo = 1 To 13
                Dim columnAcronym As String = GetListColumnAcronym(columnNo)
                Dim columnName As String = GetListColumnName(columnAcronym)
                If Settings.EventsListFields.LastIndexOf(columnAcronym, StringComparison.Ordinal) > -1 Then
                    selectedFields.Add(columnName)
                Else
                    availableFields.Add(columnName)
                End If
            Next

            lstAvailable.DataSource = availableFields
            lstAvailable.DataBind()
            Sort(lstAvailable)

            lstAssigned.DataSource = selectedFields
            lstAssigned.DataBind()
            Sort(lstAssigned)

            If Settings.EventsListSelectType = rblSelectionTypeDays.Value Then
                rblSelectionTypeDays.Checked = True
                rblSelectionTypeEvents.Checked = False
            Else
                rblSelectionTypeDays.Checked = False
                rblSelectionTypeEvents.Checked = True
            End If

            If Settings.ListViewGrid Then
                rblListViewGrid.Items(0).Selected = True
            Else
                rblListViewGrid.Items(1).Selected = True
            End If
            chkListViewTable.Checked = Settings.ListViewTable
            txtRptColumns.Text = Settings.RptColumns.ToString
            txtRptRows.Text = Settings.RptRows.ToString

            ' Do we have to display the EventsList header?
            If Settings.EventsListShowHeader <> "No" Then
                rblShowHeader.Items(0).Selected = True
            Else
                rblShowHeader.Items(1).Selected = True
            End If

            txtDaysBefore.Text = Settings.EventsListBeforeDays.ToString
            txtDaysAfter.Text = Settings.EventsListAfterDays.ToString
            txtNumEvents.Text = Settings.EventsListNumEvents.ToString
            txtEventDays.Text = Settings.EventsListEventDays.ToString
            chkRestrictCategoriesToTimeFrame.Checked = Settings.RestrictCategoriesToTimeFrame
            chkRestrictLocationsToTimeFrame.Checked = Settings.RestrictLocationsToTimeFrame

            chkCustomField1.Checked = Settings.EventsCustomField1
            chkCustomField2.Checked = Settings.EventsCustomField2

            ddlPageSize.Items.FindByValue(CStr(Settings.EventsListPageSize)).Selected = True
            ddlListSortedFieldDirection.Items.Clear()
            ddlListSortedFieldDirection.Items.Add(New ListItem(Localization.GetString("Asc", LocalResourceFile), "ASC"))
            ddlListSortedFieldDirection.Items.Add(New ListItem(Localization.GetString("Desc", LocalResourceFile), "DESC"))
            ddlListSortedFieldDirection.Items.FindByValue(Settings.EventsListSortDirection).Selected = True

            ddlListDefaultColumn.Items.Clear()
            ddlListDefaultColumn.Items.Add(New ListItem(Localization.GetString("SortEventID", LocalResourceFile), "EventID"))
            ddlListDefaultColumn.Items.Add(New ListItem(Localization.GetString("SortEventDateBegin", LocalResourceFile), "EventDateBegin"))
            ddlListDefaultColumn.Items.Add(New ListItem(Localization.GetString("SortEventDateEnd", LocalResourceFile), "EventDateEnd"))
            ddlListDefaultColumn.Items.Add(New ListItem(Localization.GetString("SortEventName", LocalResourceFile), "EventName"))
            ddlListDefaultColumn.Items.Add(New ListItem(Localization.GetString("SortDuration", LocalResourceFile), "Duration"))
            ddlListDefaultColumn.Items.Add(New ListItem(Localization.GetString("SortCategoryName", LocalResourceFile), "CategoryName"))
            ddlListDefaultColumn.Items.Add(New ListItem(Localization.GetString("SortCustomField1", LocalResourceFile), "CustomField1"))
            ddlListDefaultColumn.Items.Add(New ListItem(Localization.GetString("SortCustomField2", LocalResourceFile), "CustomField2"))
            ddlListDefaultColumn.Items.Add(New ListItem(Localization.GetString("SortDescription", LocalResourceFile), "Description"))
            ddlListDefaultColumn.Items.Add(New ListItem(Localization.GetString("SortLocationName", LocalResourceFile), "LocationName"))
            ddlListDefaultColumn.Items.FindByValue(Settings.EventsListSortColumn).Selected = True

            ddlWeekStart.Items.Clear()
            ddlWeekStart.Items.Add(New ListItem(System.Web.UI.WebControls.FirstDayOfWeek.Default.ToString, CInt(System.Web.UI.WebControls.FirstDayOfWeek.Default).ToString))
            ddlWeekStart.Items.Add(New ListItem(System.Web.UI.WebControls.FirstDayOfWeek.Monday.ToString, CInt(System.Web.UI.WebControls.FirstDayOfWeek.Monday).ToString))
            ddlWeekStart.Items.Add(New ListItem(System.Web.UI.WebControls.FirstDayOfWeek.Tuesday.ToString, CInt(System.Web.UI.WebControls.FirstDayOfWeek.Tuesday).ToString))
            ddlWeekStart.Items.Add(New ListItem(System.Web.UI.WebControls.FirstDayOfWeek.Wednesday.ToString, CInt(System.Web.UI.WebControls.FirstDayOfWeek.Wednesday).ToString))
            ddlWeekStart.Items.Add(New ListItem(System.Web.UI.WebControls.FirstDayOfWeek.Thursday.ToString, CInt(System.Web.UI.WebControls.FirstDayOfWeek.Thursday).ToString))
            ddlWeekStart.Items.Add(New ListItem(System.Web.UI.WebControls.FirstDayOfWeek.Friday.ToString, CInt(System.Web.UI.WebControls.FirstDayOfWeek.Friday).ToString))
            ddlWeekStart.Items.Add(New ListItem(System.Web.UI.WebControls.FirstDayOfWeek.Saturday.ToString, CInt(System.Web.UI.WebControls.FirstDayOfWeek.Saturday).ToString))
            ddlWeekStart.Items.Add(New ListItem(System.Web.UI.WebControls.FirstDayOfWeek.Sunday.ToString, CInt(System.Web.UI.WebControls.FirstDayOfWeek.Sunday).ToString))
            ddlWeekStart.Items.FindByValue(CInt(Settings.WeekStart).ToString).Selected = True

            If Settings.EnrollEditFields.LastIndexOf("01", StringComparison.Ordinal) > -1 Then
                rblEnUserEdit.Checked = True
            ElseIf Settings.EnrollViewFields.LastIndexOf("01", StringComparison.Ordinal) > -1 Then
                rblEnUserView.Checked = True
            ElseIf Settings.EnrollAnonFields.LastIndexOf("01", StringComparison.Ordinal) > -1 Then
                rblEnUserAnon.Checked = True
            Else
                rblEnUserNone.Checked = True
            End If

            If Settings.EnrollEditFields.LastIndexOf("02", StringComparison.Ordinal) > -1 Then
                rblEnDispEdit.Checked = True
            ElseIf Settings.EnrollViewFields.LastIndexOf("02", StringComparison.Ordinal) > -1 Then
                rblEnDispView.Checked = True
            ElseIf Settings.EnrollAnonFields.LastIndexOf("02", StringComparison.Ordinal) > -1 Then
                rblEnDispAnon.Checked = True
            Else
                rblEnDispNone.Checked = True
            End If

            If Settings.EnrollEditFields.LastIndexOf("03", StringComparison.Ordinal) > -1 Then
                rblEnEmailEdit.Checked = True
            ElseIf Settings.EnrollViewFields.LastIndexOf("03", StringComparison.Ordinal) > -1 Then
                rblEnEmailView.Checked = True
            ElseIf Settings.EnrollAnonFields.LastIndexOf("03", StringComparison.Ordinal) > -1 Then
                rblEnEmailAnon.Checked = True
            Else
                rblEnEmailNone.Checked = True
            End If

            If Settings.EnrollEditFields.LastIndexOf("04", StringComparison.Ordinal) > -1 Then
                rblEnPhoneEdit.Checked = True
            ElseIf Settings.EnrollViewFields.LastIndexOf("04", StringComparison.Ordinal) > -1 Then
                rblEnPhoneView.Checked = True
            ElseIf Settings.EnrollAnonFields.LastIndexOf("04", StringComparison.Ordinal) > -1 Then
                rblEnPhoneAnon.Checked = True
            Else
                rblEnPhoneNone.Checked = True
            End If

            If Settings.EnrollEditFields.LastIndexOf("05", StringComparison.Ordinal) > -1 Then
                rblEnApproveEdit.Checked = True
            ElseIf Settings.EnrollViewFields.LastIndexOf("05", StringComparison.Ordinal) > -1 Then
                rblEnApproveView.Checked = True
            ElseIf Settings.EnrollAnonFields.LastIndexOf("05", StringComparison.Ordinal) > -1 Then
                rblEnApproveAnon.Checked = True
            Else
                rblEnApproveNone.Checked = True
            End If

            If Settings.EnrollEditFields.LastIndexOf("06", StringComparison.Ordinal) > -1 Then
                rblEnNoEdit.Checked = True
            ElseIf Settings.EnrollViewFields.LastIndexOf("06", StringComparison.Ordinal) > -1 Then
                rblEnNoView.Checked = True
            ElseIf Settings.EnrollAnonFields.LastIndexOf("06", StringComparison.Ordinal) > -1 Then
                rblEnNoAnon.Checked = True
            Else
                rblEnNoNone.Checked = True
            End If


            chkRSSEnable.Checked = Settings.RSSEnable
            ddlRSSDateField.Items.Clear()
            ddlRSSDateField.Items.Add(New ListItem(Localization.GetString("UpdatedDate", LocalResourceFile), "UPDATEDDATE"))
            ddlRSSDateField.Items.Add(New ListItem(Localization.GetString("CreationDate", LocalResourceFile), "CREATIONDATE"))
            ddlRSSDateField.Items.Add(New ListItem(Localization.GetString("EventDate", LocalResourceFile), "EVENTDATE"))
            ddlRSSDateField.Items.FindByValue(Settings.RSSDateField).Selected = True
            txtRSSDays.Text = Settings.RSSDays.ToString
            txtRSSTitle.Text = Settings.RSSTitle
            txtRSSDesc.Text = Settings.RSSDesc
            txtExpireEvents.Text = Settings.Expireevents
            chkExportOwnerEmail.Checked = Settings.Exportowneremail
            chkExportAnonOwnerEmail.Checked = Settings.Exportanonowneremail
            chkOwnerChangeAllowed.Checked = Settings.Ownerchangeallowed

            txtFBAdmins.Text = Settings.FBAdmins
            txtFBAppID.Text = Settings.FBAppID

            Select Case Settings.IconBar
                Case "BOTTOM"
                    rblIconBar.Items(1).Selected = True
                Case "NONE"
                    rblIconBar.Items(2).Selected = True
                Case Else
                    rblIconBar.Items(0).Selected = True
            End Select

            Select Case Settings.HTMLEmail
                Case "auto"
                    rblHTMLEmail.Items(1).Selected = True
                Case "text"
                    rblHTMLEmail.Items(2).Selected = True
                Case Else
                    rblHTMLEmail.Items(0).Selected = True
            End Select


            chkEnrollMessageApproved.Checked = Settings.SendEnrollMessageApproved
            chkEnrollMessageWaiting.Checked = Settings.SendEnrollMessageWaiting
            chkEnrollMessageDenied.Checked = Settings.SendEnrollMessageDenied
            chkEnrollMessageAdded.Checked = Settings.SendEnrollMessageAdded
            chkEnrollMessageDeleted.Checked = Settings.SendEnrollMessageDeleted
            chkEnrollMessagePaying.Checked = Settings.SendEnrollMessagePaying
            chkEnrollMessagePending.Checked = Settings.SendEnrollMessagePending
            chkEnrollMessagePaid.Checked = Settings.SendEnrollMessagePaid
            chkEnrollMessageIncorrect.Checked = Settings.SendEnrollMessageIncorrect
            chkEnrollMessageCancelled.Checked = Settings.SendEnrollMessageCancelled

            chkAllowAnonEnroll.Checked = Settings.AllowAnonEnroll
            BindToEnum(GetType(EventModuleSettings.SocialModule), ddlSocialGroupModule)
            ddlSocialGroupModule.Items.FindByValue(CType(Settings.SocialGroupModule, String)).Selected = True
            chkSocialUserPrivate.Checked = Settings.SocialUserPrivate
            BindToEnum(GetType(EventModuleSettings.SocialGroupPrivacy), ddlSocialGroupSecurity)
            ddlSocialGroupSecurity.Items.FindByValue(CType(Settings.SocialGroupSecurity, String)).Selected = True

            ddlEnrolListSortDirection.Items.Clear()
            ddlEnrolListSortDirection.Items.Add(New ListItem(Localization.GetString("Asc", LocalResourceFile), "0"))
            ddlEnrolListSortDirection.Items.Add(New ListItem(Localization.GetString("Desc", LocalResourceFile), "1"))
            ddlEnrolListSortDirection.Items.FindByValue(CInt(Settings.EnrolListSortDirection).ToString).Selected = True

            txtEnrolListDaysBefore.Text = Settings.EnrolListDaysBefore.ToString
            txtEnrolListDaysAfter.Text = Settings.EnrolListDaysAfter.ToString

            chkJournalIntegration.Checked = Settings.JournalIntegration

            LoadCategories()
            LoadLocations()

            LoadTemplates()

        End Sub

        Private Sub AddJavaScript()
            'Add the external Validation.js to the Page
            Const csname As String = "ExtValidationScriptFile"
            Dim cstype As Type = Reflection.MethodBase.GetCurrentMethod().GetType()
            Dim cstext As String = "<script src=""" & ResolveUrl("~/DesktopModules/Events/Scripts/Validation.js") & """ type=""text/javascript""></script>"
            If Not Page.ClientScript.IsClientScriptBlockRegistered(csname) Then
                Page.ClientScript.RegisterClientScriptBlock(cstype, csname, cstext, False)
            End If


            ' Add javascript actions where required and build startup script
            Dim script As String
            Dim cstext2 As String = ""
            cstext2 += "<script type=""text/javascript"">"
            cstext2 += "EventSettingsStartupScript = function() {"

            script = "disableactivate('" & ddlDefaultView.ClientID & "','" & chkMonthAllowed.ClientID & "','" & chkWeekAllowed.ClientID & "','" & chkListAllowed.ClientID & "');"
            cstext2 += script
            ddlDefaultView.Attributes.Add("onchange", script)

            script = "disableControl('" & chkPreventConflicts.ClientID & "',false, '" + chkLocationConflict.ClientID + "');"
            cstext2 += script
            chkPreventConflicts.InputAttributes.Add("onclick", script)

            script = "disableControl('" & chkMonthCellEvents.ClientID & "',true, '" + chkEventDayNewPage.ClientID + "');"
            script += "disableControl('" & chkMonthCellEvents.ClientID & "',false, '" + chkMonthDaySelect.ClientID + "');"
            script += "disableControl('" & chkMonthCellEvents.ClientID & "',false, '" + chkTimeInTitle.ClientID + "');"
            script += "disableControl('" & chkMonthCellEvents.ClientID & "',false, '" + chkEventImageMonth.ClientID + "');"
            script += "disableControl('" & chkMonthCellEvents.ClientID & "',false, '" + chkIconMonthPrio.ClientID + "');"
            script += "disableControl('" & chkMonthCellEvents.ClientID & "',false, '" + chkIconMonthRec.ClientID + "');"
            script += "disableControl('" & chkMonthCellEvents.ClientID & "',false, '" + chkIconMonthReminder.ClientID + "');"
            script += "disableControl('" & chkMonthCellEvents.ClientID & "',false, '" + chkIconMonthEnroll.ClientID + "');"
            cstext2 += script
            chkMonthCellEvents.InputAttributes.Add("onclick", script)

            script = "disableRbl('" & rblListViewGrid.ClientID & "', 'Repeater', '" + chkListViewTable.ClientID + "');"
            script += "disableRbl('" & rblListViewGrid.ClientID & "', 'Repeater', '" + txtRptColumns.ClientID + "');"
            script += "disableRbl('" & rblListViewGrid.ClientID & "', 'Repeater', '" + txtRptRows.ClientID + "');"
            script += "disableRbl('" & rblListViewGrid.ClientID & "', 'Grid', '" + rblShowHeader.ClientID + "');"
            script += "disableRbl('" & rblListViewGrid.ClientID & "', 'Grid', '" + lstAvailable.ClientID + "');"
            script += "disableRbl('" & rblListViewGrid.ClientID & "', 'Grid', '" + cmdAdd.ClientID + "');"
            script += "disableRbl('" & rblListViewGrid.ClientID & "', 'Grid', '" + cmdRemove.ClientID + "');"
            script += "disableRbl('" & rblListViewGrid.ClientID & "', 'Grid', '" + cmdAddAll.ClientID + "');"
            script += "disableRbl('" & rblListViewGrid.ClientID & "', 'Grid', '" + cmdRemoveAll.ClientID + "');"
            script += "disableRbl('" & rblListViewGrid.ClientID & "', 'Grid', '" + lstAssigned.ClientID + "');"
            script += "disableRbl('" & rblListViewGrid.ClientID & "', 'Grid', '" + ddlPageSize.ClientID + "');"
            script += "disableRbl('" & rblListViewGrid.ClientID & "', 'Grid', '" + chkIconListEnroll.ClientID + "');"
            script += "disableRbl('" & rblListViewGrid.ClientID & "', 'Grid', '" + chkIconListPrio.ClientID + "');"
            script += "disableRbl('" & rblListViewGrid.ClientID & "', 'Grid', '" + chkIconListRec.ClientID + "');"
            script += "disableRbl('" & rblListViewGrid.ClientID & "', 'Grid', '" + chkIconListReminder.ClientID + "');"
            cstext2 += script
            rblListViewGrid.Attributes.Add("onclick", script)

            script = "CheckBoxFalse('" & chkIncludeEndValue.ClientID & "', true, '" + chkShowValueMarks.ClientID + "');"
            cstext2 += script
            chkIncludeEndValue.InputAttributes.Add("onclick", script)

            script = "disablelistsettings('" & rblSelectionTypeDays.ClientID & "',true,'" + txtDaysBefore.ClientID + "','" + txtDaysAfter.ClientID + "','" + txtNumEvents.ClientID + "','" + txtEventDays.ClientID + "');"
            cstext2 += script
            rblSelectionTypeDays.Attributes.Add("onclick", script)

            script = "disablelistsettings('" & rblSelectionTypeEvents.ClientID & "',false,'" + txtDaysBefore.ClientID + "','" + txtDaysAfter.ClientID + "','" + txtNumEvents.ClientID + "','" + txtEventDays.ClientID + "');"
            cstext2 += script
            rblSelectionTypeEvents.Attributes.Add("onclick", script)

            script = "showTbl('" + chkEventNotify.ClientID + "','" + divEventNotify.ClientID + "');"
            cstext2 += script
            chkEventNotify.InputAttributes.Add("onclick", script)

            script = "showTbl('" + chkRSSEnable.ClientID + "','" + divRSSEnable.ClientID + "');"
            cstext2 += script
            chkRSSEnable.InputAttributes.Add("onclick", script)

            script = "showTbl('" + chkImageEnabled.ClientID + "','" + diviCalEventImage.ClientID + "'); showTbl('" + chkImageEnabled.ClientID + "','" + divImageEnabled.ClientID + "');"
            cstext2 += script
            chkImageEnabled.InputAttributes.Add("onclick", script)

            script = "showTbl('" + chkiCalDisplayImage.ClientID + "','" + diviCalDisplayImage.ClientID + "');"
            cstext2 += script
            chkiCalDisplayImage.InputAttributes.Add("onclick", script)

            script = "disableControl('" & chkExportOwnerEmail.ClientID & "',false, '" + chkExportAnonOwnerEmail.ClientID + "');"
            cstext2 += script
            chkExportOwnerEmail.InputAttributes.Add("onclick", script)

            script = "disableDDL('" & ddlSocialGroupModule.ClientID & "','" & CInt(EventModuleSettings.SocialModule.UserProfile).ToString & "','" + chkSocialUserPrivate.ClientID + "');"
            cstext2 += script
            ddlSocialGroupModule.Attributes.Add("onclick", script)

            If Settings.SocialGroupModule = EventModuleSettings.SocialModule.No Then
                script = "disableRbl('" & rblNewEventEmail.ClientID & "', 'Role', '" + ddNewEventEmailRoles.ClientID + "');"
                cstext2 += script
                rblNewEventEmail.Attributes.Add("onclick", script)
            End If

            If Settings.SocialGroupModule <> EventModuleSettings.SocialModule.UserProfile Then
                script = "showTbl('" + chkEventSignup.ClientID + "','" + divEventSignup.ClientID + "');"
                cstext2 += script
                chkEventSignup.InputAttributes.Add("onclick", script)

                script = "showTbl('" + chkEventSignupAllowPaid.ClientID + "','" + divEventSignupAllowPaid.ClientID + "');"
                cstext2 += script
                chkEventSignupAllowPaid.InputAttributes.Add("onclick", script)

                script = "showTbl('" + chkEnableSEO.ClientID + "','" + divSEOEnable.ClientID + "');"
                cstext2 += script
                chkEnableSEO.InputAttributes.Add("onclick", script)

                script = "showTbl('" + chkEnableSitemap.ClientID + "','" + divSitemapEnable.ClientID + "');"
                cstext2 += script
                chkEnableSitemap.InputAttributes.Add("onclick", script)
            End If

            cstext2 += "};"
            cstext2 += "EventSettingsStartupScript();"
            cstext2 += "Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EventEndRequestHandler);"
            cstext2 += "function EventEndRequestHandler(sender, args) { EventSettingsStartupScript(); }"
            cstext2 += "</script>"

            ' Register the startup script
            Const csname2 As String = "EventSettingsStartupScript"
            Dim cstype2 As Type = Reflection.MethodBase.GetCurrentMethod().GetType()
            If Not Page.ClientScript.IsStartupScriptRegistered(csname2) Then
                Page.ClientScript.RegisterStartupScript(cstype2, csname2, cstext2, False)
            End If

        End Sub

        Private Sub BindToEnum(enumType As Type, ByVal ddl As System.Web.UI.WebControls.DropDownList)
            ' get the names from the enumeration
            Dim names As String() = [Enum].GetNames(enumType)
            ' get the values from the enumeration
            Dim values As Array = [Enum].GetValues(enumType)
            ' turn it into a hash table
            ddl.Items.Clear()
            For i As Integer = 0 To names.Length - 1
                ' note the cast to integer here is important
                ' otherwise we'll just get the enum string back again
                ddl.Items.Add(New ListItem(Localization.GetString(names(i), LocalResourceFile), CStr(CInt(values.GetValue(i)))))
            Next
            ' return the dictionary to be bound to
        End Sub


        Private Function GetListColumnName(ByVal columnAcronym As String) As String
            Select Case columnAcronym
                Case "EB"
                    Return "01 - " & Localization.GetString("EditButton", LocalResourceFile)
                Case "BD"
                    Return "02 - " & Localization.GetString("BeginDateTime", LocalResourceFile)
                Case "ED"
                    Return "03 - " & Localization.GetString("EndDateTime", LocalResourceFile)
                Case "EN"
                    Return "04 - " & Localization.GetString("EventName", LocalResourceFile)
                Case "IM"
                    Return "05 - " & Localization.GetString("Image", LocalResourceFile)
                Case "DU"
                    Return "06 - " & Localization.GetString("Duration", LocalResourceFile)
                Case "CA"
                    Return "07 - " & Localization.GetString("Category", LocalResourceFile)
                Case "LO"
                    Return "08 - " & Localization.GetString("Location", LocalResourceFile)
                Case "C1"
                    Return "09 - " & Localization.GetString("CustomField1", LocalResourceFile)
                Case "C2"
                    Return "10 - " & Localization.GetString("CustomField2", LocalResourceFile)
                Case "DE"
                    Return "11 - " & Localization.GetString("Description", LocalResourceFile)
                Case "RT"
                    Return "12 - " & Localization.GetString("RecurText", LocalResourceFile)
                Case "RU"
                    Return "13 - " & Localization.GetString("RecurUntil", LocalResourceFile)
                Case Else
                    Return ""
            End Select
        End Function

        Private Function GetListColumnAcronym(ByVal columnNo As Integer) As String
            Select Case columnNo
                Case 1
                    Return "EB"
                Case 2
                    Return "BD"
                Case 3
                    Return "ED"
                Case 4
                    Return "EN"
                Case 5
                    Return "IM"
                Case 6
                    Return "DU"
                Case 7
                    Return "CA"
                Case 8
                    Return "LO"
                Case 9
                    Return "C1"
                Case 10
                    Return "C2"
                Case 11
                    Return "DE"
                Case 12
                    Return "RT"
                Case 13
                    Return "RU"
                Case Else
                    Return ""
            End Select
        End Function

        ''' <summary>
        ''' Fill the themelist based on selection for default or custom skins
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub LoadThemes()
            Try
                Const moduleThemesDirectoryPath As String = "/DesktopModules/Events/Themes"

                'Clear list
                ddlThemeStandard.Items.Clear()
                ddlThemeCustom.Items.Clear()

                'Add javascript to enable/disable ddl's
                rbThemeCustom.Attributes.Add("onclick", String.Format("{0}.disabled='disabled';{1}.disabled=''", ddlThemeStandard.ClientID, ddlThemeCustom.ClientID))
                rbThemeStandard.Attributes.Add("onclick", String.Format("{0}.disabled='disabled';{1}.disabled=''", ddlThemeCustom.ClientID, ddlThemeStandard.ClientID))

                'Get the settings
                Dim themeSettings As New ThemeSetting
                If themeSettings.ValidateSetting(Settings.EventTheme) = False Then
                    themeSettings.ReadSetting(Settings.EventThemeDefault, PortalId)
                ElseIf Settings.EventTheme <> "" Then
                    themeSettings.ReadSetting(Settings.EventTheme, PortalId)
                End If
                Select Case themeSettings.SettingType
                    Case ThemeSetting.ThemeSettingTypeEnum.CustomTheme
                        rbThemeCustom.Checked = True
                    Case ThemeSetting.ThemeSettingTypeEnum.DefaultTheme
                        rbThemeStandard.Checked = True
                End Select

                'Is default or custom selected
                Dim moduleThemesDirectory As String = ApplicationPath & moduleThemesDirectoryPath
                Dim serverThemesDirectory As String = Server.MapPath(moduleThemesDirectory)
                Dim themeDirectories() As String = IO.Directory.GetDirectories(serverThemesDirectory)
                Dim themeDirectory As String
                For Each themeDirectory In themeDirectories
                    Dim dirparts() As String = themeDirectory.Split("\"c)
                    ddlThemeStandard.Items.Add(New ListItem(dirparts(dirparts.Length - 1), dirparts(dirparts.Length - 1)))
                Next
                If themeSettings.SettingType = ThemeSetting.ThemeSettingTypeEnum.DefaultTheme Then
                    If Not ddlThemeStandard.Items.FindByText(themeSettings.ThemeName) Is Nothing Then
                        ddlThemeStandard.Items.FindByText(themeSettings.ThemeName).Selected = True
                    End If
                Else
                    ddlThemeStandard.Attributes.Add("disabled", "disabled")
                End If

                'Add custom event theme's
                Dim pc As New Entities.Portals.PortalController
                With pc.GetPortal(PortalId)
                    Dim eventSkinPath As String = String.Format("{0}\DNNEvents\Themes", .HomeDirectoryMapPath)
                    If Not IO.Directory.Exists(eventSkinPath) Then
                        IO.Directory.CreateDirectory(eventSkinPath)
                    End If

                    For Each d As String In IO.Directory.GetDirectories(eventSkinPath)
                        ddlThemeCustom.Items.Add(New ListItem(New IO.DirectoryInfo(d).Name, New IO.DirectoryInfo(d).Name))
                    Next
                    If ddlThemeCustom.Items.Count = 0 Then
                        rbThemeCustom.Enabled = False
                    End If
                End With

                If themeSettings.SettingType = ThemeSetting.ThemeSettingTypeEnum.CustomTheme Then
                    If Not ddlThemeCustom.Items.FindByText(themeSettings.ThemeName) Is Nothing Then
                        ddlThemeCustom.Items.FindByText(themeSettings.ThemeName).Selected = True
                    End If
                Else
                    ddlThemeCustom.Attributes.Add("disabled", "disabled")
                End If

            Catch ex As Exception
                Debug.Write(ex.ToString)
            End Try
        End Sub

        Private Sub LoadTemplates()
            ddlTemplates.Items.Clear()

            Dim t As Type = Settings.Templates.GetType
            Dim p As Reflection.PropertyInfo
            For Each p In t.GetProperties
                ddlTemplates.Items.Add(New ListItem(Localization.GetString(p.Name + "Name", LocalResourceFile), p.Name))
            Next

            ddlTemplates.Items.FindByValue("EventDetailsTemplate").Selected = True
            txtEventTemplate.Text = Settings.Templates.GetTemplate(ddlTemplates.SelectedValue)
            lblTemplateUpdated.Visible = False

        End Sub

        Private Sub LoadNewEventEmailRoles(ByVal roleID As Integer)
            Dim objRoles As New Security.Roles.RoleController
            ddNewEventEmailRoles.DataSource = objRoles.GetRoles(PortalId)
            ddNewEventEmailRoles.DataTextField = "RoleName"
            ddNewEventEmailRoles.DataValueField = "RoleID"
            ddNewEventEmailRoles.DataBind()
            If roleID < 0 Or ddNewEventEmailRoles.Items.FindByValue(CType(roleID, String)) Is Nothing Then
                Try
                    ddNewEventEmailRoles.Items.FindByValue(PortalSettings.RegisteredRoleId.ToString).Selected() = True
                Catch
                End Try
            Else
                ddNewEventEmailRoles.Items.FindByValue(CType(roleID, String)).Selected = True
            End If
        End Sub

        Private Sub LoadCategories()
            ddlModuleCategories.Items.Clear()
            Dim ctrlEventCategories As New EventCategoryController
            Dim lstCategories As ArrayList = ctrlEventCategories.EventsCategoryList(PortalId)
            ddlModuleCategories.DataSource = lstCategories
            ddlModuleCategories.DataBind()

            If Settings.ModuleCategoriesSelected = EventModuleSettings.CategoriesSelected.Some Then
                For Each moduleCategory As String In Settings.ModuleCategoryIDs
                    For Each item As Telerik.Web.UI.RadComboBoxItem In ddlModuleCategories.Items
                        If item.Value = moduleCategory Then
                            item.Checked = True
                        End If
                    Next
                Next
            ElseIf Settings.ModuleCategoriesSelected = EventModuleSettings.CategoriesSelected.All Then
                For Each item As Telerik.Web.UI.RadComboBoxItem In ddlModuleCategories.Items
                    item.Checked = True
                Next
            End If
        End Sub

        Private Sub LoadLocations()
            ddlModuleLocations.Items.Clear()
            Dim ctrlEventLocations As New EventLocationController
            Dim lstLocations As ArrayList = ctrlEventLocations.EventsLocationList(PortalId)
            ddlModuleLocations.DataSource = lstLocations
            ddlModuleLocations.DataBind()

            If Settings.ModuleLocationsSelected = EventModuleSettings.LocationsSelected.Some Then
                For Each moduleLocation As String In Settings.ModuleLocationIDs
                    For Each item As Telerik.Web.UI.RadComboBoxItem In ddlModuleLocations.Items
                        If item.Value = moduleLocation Then
                            item.Checked = True
                        End If
                    Next
                Next
            ElseIf Settings.ModuleLocationsSelected = EventModuleSettings.LocationsSelected.All Then
                For Each item As Telerik.Web.UI.RadComboBoxItem In ddlModuleLocations.Items
                    item.Checked = True
                Next
            End If
        End Sub

        ''' <summary>
        ''' Take all settings and write them back to the database
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub UpdateSettings()

            Dim emSettings As New EventModuleSettings(ModuleId, LocalResourceFile)

            Try
                emSettings.Timeinterval = ddlTimeInterval.SelectedValue.Trim.ToString()
                emSettings.TimeZoneId = cboTimeZone.SelectedValue
                emSettings.EnableEventTimeZones = chkEnableEventTimeZones.Checked
                emSettings.PrimaryTimeZone = CType(ddlPrimaryTimeZone.SelectedValue, EventModuleSettings.TimeZones)
                emSettings.SecondaryTimeZone = CType(ddlSecondaryTimeZone.SelectedValue, EventModuleSettings.TimeZones)
                emSettings.Eventtooltipmonth = chkToolTipMonth.Checked
                emSettings.Eventtooltipweek = chkToolTipWeek.Checked
                emSettings.Eventtooltipday = chkToolTipDay.Checked
                emSettings.Eventtooltiplist = chkToolTipList.Checked
                emSettings.Eventtooltiplength = CInt(txtTooltipLength.Text)
                If chkMonthCellEvents.Checked Then
                    emSettings.Monthcellnoevents = False
                Else
                    emSettings.Monthcellnoevents = True
                End If
                emSettings.Enablecategories = CType(ddlEnableCategories.SelectedValue, EventModuleSettings.DisplayCategories)
                emSettings.Restrictcategories = chkRestrictCategories.Checked
                emSettings.Enablelocations = CType(ddlEnableLocations.SelectedValue, EventModuleSettings.DisplayLocations)
                emSettings.Restrictlocations = chkRestrictLocations.Checked
                emSettings.Enablecontainerskin = chkEnableContainerSkin.Checked
                emSettings.Eventdetailnewpage = chkEventDetailNewPage.Checked
                emSettings.Enableenrollpopup = chkEnableEnrollPopup.Checked
                emSettings.Eventdaynewpage = chkEventDayNewPage.Checked
                emSettings.EventImageMonth = chkEventImageMonth.Checked
                emSettings.EventImageWeek = chkEventImageWeek.Checked
                emSettings.Eventnotify = chkEventNotify.Checked
                emSettings.DetailPageAllowed = chkDetailPageAllowed.Checked
                emSettings.EnrollmentPageAllowed = chkEnrollmentPageAllowed.Checked
                emSettings.EnrollmentPageDefaultUrl = txtEnrollmentPageDefaultURL.Text
                emSettings.Notifyanon = chkNotifyAnon.Checked
                emSettings.Sendreminderdefault = chkSendReminderDefault.Checked
                emSettings.Neweventemails = rblNewEventEmail.SelectedValue
                emSettings.Neweventemailrole = CInt(ddNewEventEmailRoles.SelectedValue)
                emSettings.Newpereventemail = chkNewPerEventEmail.Checked
                emSettings.Tzdisplay = chkTZDisplay.Checked
                emSettings.Paypalurl = txtPayPalURL.Text
                If chkEnableEventNav.Checked Then
                    emSettings.DisableEventnav = False
                Else
                    emSettings.DisableEventnav = True
                End If
                emSettings.Fulltimescale = chkFullTimeScale.Checked
                emSettings.Collapserecurring = chkCollapseRecurring.Checked
                emSettings.Includeendvalue = chkIncludeEndValue.Checked
                emSettings.Showvaluemarks = chkShowValueMarks.Checked
                emSettings.Eventimage = chkImageEnabled.Checked
                emSettings.MaxThumbHeight = CInt(txtMaxThumbHeight.Text)
                emSettings.MaxThumbWidth = CInt(txtMaxThumbWidth.Text)
                emSettings.Allowreoccurring = chkAllowRecurring.Checked
                emSettings.Maxrecurrences = txtMaxRecurrences.Text
                emSettings.Eventsearch = chkEnableSearch.Checked
                emSettings.Addsubmodulename = chkAddSubModuleName.Checked
                emSettings.Enforcesubcalperms = chkEnforceSubCalPerms.Checked
                emSettings.Preventconflicts = chkPreventConflicts.Checked
                emSettings.Locationconflict = chkLocationConflict.Checked
                emSettings.ShowEventsAlways = chkShowEventsAlways.Checked
                emSettings.Timeintitle = chkTimeInTitle.Checked
                emSettings.Monthdayselect = chkMonthDaySelect.Checked
                emSettings.MasterEvent = chkMasterEvent.Checked
                emSettings.Eventsignup = chkEventSignup.Checked
                emSettings.Eventsignupallowpaid = chkEventSignupAllowPaid.Checked
                emSettings.Eventdefaultenrollview = chkDefaultEnrollView.Checked
                emSettings.Eventhidefullenroll() = chkHideFullEnroll.Checked
                emSettings.Maxnoenrolees = CInt(txtMaxNoEnrolees.Text)
                emSettings.Enrolcanceldays = CInt(txtCancelDays.Text)
                emSettings.Fridayweekend = chkFridayWeekend.Checked
                emSettings.Moderateall = chkModerateAll.Checked
                emSettings.Paypalaccount = txtPayPalAccount.Text
                emSettings.Reminderfrom = txtReminderFrom.Text
                emSettings.StandardEmail = txtStandardEmail.Text
                emSettings.EventsCustomField1 = chkCustomField1.Checked
                emSettings.EventsCustomField2 = chkCustomField2.Checked
                emSettings.DefaultView = ddlDefaultView.SelectedItem.Value
                emSettings.EventsListPageSize = CInt(ddlPageSize.SelectedItem.Value)
                emSettings.EventsListSortDirection = ddlListSortedFieldDirection.SelectedItem.Value
                emSettings.EventsListSortColumn = ddlListDefaultColumn.SelectedItem.Value
                emSettings.RSSEnable = chkRSSEnable.Checked
                emSettings.RSSDateField = ddlRSSDateField.SelectedItem.Value
                emSettings.RSSDays = CInt(txtRSSDays.Text)
                emSettings.RSSTitle = txtRSSTitle.Text
                emSettings.RSSDesc = txtRSSDesc.Text
                emSettings.Expireevents = txtExpireEvents.Text
                emSettings.Exportowneremail = chkExportOwnerEmail.Checked
                emSettings.Exportanonowneremail = chkExportAnonOwnerEmail.Checked
                emSettings.Ownerchangeallowed = chkOwnerChangeAllowed.Checked
                emSettings.IconMonthPrio = chkIconMonthPrio.Checked
                emSettings.IconMonthRec = chkIconMonthRec.Checked
                emSettings.IconMonthReminder = chkIconMonthReminder.Checked
                emSettings.IconMonthEnroll = chkIconMonthEnroll.Checked
                emSettings.IconWeekPrio = chkIconWeekPrio.Checked
                emSettings.IconWeekRec = chkIconWeekRec.Checked
                emSettings.IconWeekReminder = chkIconWeekReminder.Checked
                emSettings.IconWeekEnroll = chkIconWeekEnroll.Checked
                emSettings.IconListPrio = chkIconListPrio.Checked
                emSettings.IconListRec = chkIconListRec.Checked
                emSettings.IconListReminder = chkIconListReminder.Checked
                emSettings.IconListEnroll = chkIconListEnroll.Checked
                emSettings.PrivateMessage = txtPrivateMessage.Text.Trim
                emSettings.EnableSEO = chkEnableSEO.Checked
                emSettings.SEODescriptionLength = CInt(txtSEODescriptionLength.Text)
                emSettings.EnableSitemap = chkEnableSitemap.Checked
                emSettings.SiteMapPriority = CType(txtSitemapPriority.Text, Single)
                emSettings.SiteMapDaysBefore = CInt(txtSitemapDaysBefore.Text)
                emSettings.SiteMapDaysAfter = CInt(txtSitemapDaysAfter.Text)
                emSettings.WeekStart = CType(CInt(ddlWeekStart.SelectedValue), System.Web.UI.WebControls.FirstDayOfWeek)
                emSettings.ListViewUseTime = chkListViewUseTime.Checked

                emSettings.IcalOnIconBar = chkiCalOnIconBar.Checked
                emSettings.IcalEmailEnable = chkiCalEmailEnable.Checked
                emSettings.IcalURLInLocation = chkiCalURLinLocation.Checked
                emSettings.IcalIncludeCalname = chkiCalIncludeCalname.Checked
                emSettings.IcalDaysBefore = CInt(txtiCalDaysBefore.Text)
                emSettings.IcalDaysAfter = CInt(txtiCalDaysAfter.Text)
                emSettings.IcalURLAppend = txtiCalURLAppend.Text
                If chkiCalDisplayImage.Checked Then
                    emSettings.IcalDefaultImage = "Image=" & ctliCalDefaultImage.Url
                Else
                    emSettings.IcalDefaultImage = ""
                End If
                'objModules.UpdateModuleSetting(ModuleId, "EventDetailsTemplate", txtEventDetailsTemplate.Text.Trim)

                Dim moduleCategories As New ArrayList
                If ddlModuleCategories.CheckedItems.Count <> ddlModuleCategories.Items.Count Then
                    For Each item As Telerik.Web.UI.RadComboBoxItem In ddlModuleCategories.CheckedItems
                        moduleCategories.Add(item.Value)
                    Next
                Else
                    moduleCategories.Add("-1")
                End If
                emSettings.ModuleCategoryIDs = moduleCategories

                Dim moduleLocations As New ArrayList
                If ddlModuleLocations.CheckedItems.Count <> ddlModuleLocations.Items.Count Then
                    For Each item As Telerik.Web.UI.RadComboBoxItem In ddlModuleLocations.CheckedItems
                        moduleLocations.Add(item.Value)
                    Next
                Else
                    moduleLocations.Add("-1")
                End If
                emSettings.ModuleLocationIDs = moduleLocations

                ' ReSharper disable LocalizableElement
                If chkMonthAllowed.Checked Or ddlDefaultView.SelectedItem.Value = "EventMonth.ascx" Then
                    emSettings.MonthAllowed = True
                Else
                    emSettings.MonthAllowed = False
                End If
                If chkWeekAllowed.Checked Or ddlDefaultView.SelectedItem.Value = "EventWeek.ascx" Then
                    emSettings.WeekAllowed = True
                Else
                    emSettings.WeekAllowed = False
                End If
                If chkListAllowed.Checked Or ddlDefaultView.SelectedItem.Value = "EventList.ascx" Then
                    emSettings.ListAllowed = True
                Else
                    emSettings.ListAllowed = False
                End If
                ' ReSharper restore LocalizableElement

                Select Case rblIconBar.SelectedIndex
                    Case 0
                        emSettings.IconBar = "TOP"
                    Case 1
                        emSettings.IconBar = "BOTTOM"
                    Case 2
                        emSettings.IconBar = "NONE"
                End Select

                Select Case rblHTMLEmail.SelectedIndex
                    Case 0
                        emSettings.HTMLEmail = "html"
                    Case 1
                        emSettings.HTMLEmail = "auto"
                    Case 2
                        emSettings.HTMLEmail = "text"
                End Select

                'EPT: Be sure we start next display time in the correct view
                ' Update the cookie so the appropriate view is shown when settings page is exited

                Dim objCookie As HttpCookie = New HttpCookie("DNNEvents" & ModuleId)
                objCookie.Value = ddlDefaultView.SelectedItem.Value
                If Request.Cookies.Get("DNNEvents" & ModuleId) Is Nothing Then
                    Response.Cookies.Add(objCookie)
                Else
                    Response.Cookies.Set(objCookie)
                End If

                'Set eventtheme data
                Dim themeSettings As New ThemeSetting
                With themeSettings
                    If rbThemeStandard.Checked Then
                        .SettingType = ThemeSetting.ThemeSettingTypeEnum.DefaultTheme
                        .ThemeName = ddlThemeStandard.SelectedItem.Text
                        .ThemeFile = ""
                    ElseIf rbThemeCustom.Checked Then
                        .SettingType = ThemeSetting.ThemeSettingTypeEnum.CustomTheme
                        .ThemeName = ddlThemeCustom.SelectedItem.Text
                        .ThemeFile = ""
                    End If
                End With
                emSettings.EventTheme = themeSettings.ToString

                'List Events Mode Stuff
                'Update Fields to Display
                Dim objListItem As ListItem
                Dim listFields As String = ""
                For Each objListItem In lstAssigned.Items
                    Dim columnNo As Integer = CInt(objListItem.Text.Substring(0, 2))
                    Dim columnAcronym As String = GetListColumnAcronym(columnNo)
                    If listFields.Length > 0 Then
                        listFields = listFields + ";" + columnAcronym
                    Else
                        listFields = columnAcronym
                    End If
                Next
                emSettings.EventsListFields = listFields

                listFields = EnrollListFields(rblEnUserAnon.Checked, rblEnDispAnon.Checked, rblEnEmailAnon.Checked, rblEnPhoneAnon.Checked, rblEnApproveAnon.Checked, rblEnNoAnon.Checked)
                emSettings.EnrollAnonFields = listFields

                listFields = EnrollListFields(rblEnUserView.Checked, rblEnDispView.Checked, rblEnEmailView.Checked, rblEnPhoneView.Checked, rblEnApproveView.Checked, rblEnNoView.Checked)
                emSettings.EnrollViewFields = listFields

                listFields = EnrollListFields(rblEnUserEdit.Checked, rblEnDispEdit.Checked, rblEnEmailEdit.Checked, rblEnPhoneEdit.Checked, rblEnApproveEdit.Checked, rblEnNoEdit.Checked)
                emSettings.EnrollEditFields = listFields

                If rblSelectionTypeDays.Checked Then
                    emSettings.EventsListSelectType = rblSelectionTypeDays.Value
                Else
                    emSettings.EventsListSelectType = rblSelectionTypeEvents.Value
                End If
                If rblListViewGrid.Items(0).Selected = True Then
                    emSettings.ListViewGrid = True
                Else
                    emSettings.ListViewGrid = False
                End If
                emSettings.ListViewTable = chkListViewTable.Checked
                emSettings.RptColumns = CInt(txtRptColumns.Text.Trim.ToString())
                emSettings.RptRows = CInt(txtRptRows.Text.Trim.ToString())

                If rblShowHeader.Items(0).Selected = True Then
                    emSettings.EventsListShowHeader = rblShowHeader.Items(0).Value
                Else
                    emSettings.EventsListShowHeader = rblShowHeader.Items(1).Value
                End If
                emSettings.EventsListBeforeDays = CInt(txtDaysBefore.Text.Trim.ToString())
                emSettings.EventsListAfterDays = CInt(txtDaysAfter.Text.Trim.ToString())
                emSettings.EventsListNumEvents = CInt(txtNumEvents.Text.Trim.ToString())
                emSettings.EventsListEventDays = CInt(txtEventDays.Text.Trim.ToString())
                emSettings.RestrictCategoriesToTimeFrame = chkRestrictCategoriesToTimeFrame.Checked
                emSettings.RestrictLocationsToTimeFrame = chkRestrictLocationsToTimeFrame.Checked

                emSettings.FBAdmins = txtFBAdmins.Text
                emSettings.FBAppID = txtFBAppID.Text

                emSettings.SendEnrollMessageApproved = chkEnrollMessageApproved.Checked
                emSettings.SendEnrollMessageWaiting = chkEnrollMessageWaiting.Checked
                emSettings.SendEnrollMessageDenied = chkEnrollMessageDenied.Checked
                emSettings.SendEnrollMessageAdded = chkEnrollMessageAdded.Checked
                emSettings.SendEnrollMessageDeleted = chkEnrollMessageDeleted.Checked
                emSettings.SendEnrollMessagePaying = chkEnrollMessagePaying.Checked
                emSettings.SendEnrollMessagePending = chkEnrollMessagePending.Checked
                emSettings.SendEnrollMessagePaid = chkEnrollMessagePaid.Checked
                emSettings.SendEnrollMessageIncorrect = chkEnrollMessageIncorrect.Checked
                emSettings.SendEnrollMessageCancelled = chkEnrollMessageCancelled.Checked

                emSettings.AllowAnonEnroll = chkAllowAnonEnroll.Checked
                emSettings.SocialGroupModule = CType(ddlSocialGroupModule.SelectedValue, EventModuleSettings.SocialModule)
                If emSettings.SocialGroupModule <> EventModuleSettings.SocialModule.No Then
                    emSettings.Ownerchangeallowed = False
                    emSettings.Neweventemails = "Never"
                    emSettings.MasterEvent = False
                End If
                If emSettings.SocialGroupModule = EventModuleSettings.SocialModule.UserProfile Then
                    emSettings.EnableSitemap = False
                    emSettings.Eventsearch = False
                    emSettings.Eventsignup = False
                    emSettings.Moderateall = False
                End If
                emSettings.SocialUserPrivate = chkSocialUserPrivate.Checked
                emSettings.SocialGroupSecurity = CType(ddlSocialGroupSecurity.SelectedValue, EventModuleSettings.SocialGroupPrivacy)

                emSettings.EnrolListSortDirection = CType(ddlEnrolListSortDirection.SelectedValue, SortDirection)
                emSettings.EnrolListDaysBefore = CInt(txtEnrolListDaysBefore.Text)
                emSettings.EnrolListDaysAfter = CInt(txtEnrolListDaysAfter.Text)

                emSettings.JournalIntegration = chkJournalIntegration.Checked

                emSettings.SaveSettings(ModuleId)
                CreateThemeDirectory()


            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        ''' <summary>
        ''' Get Assigned Sub Events and Bind to Grid
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub BindSubEvents()
            With lstAssignedCals
                .DataTextField = "SubEventTitle"
                .DataValueField = "MasterID"
                .DataSource = Nothing
                .DataBind()
                .DataSource = _objCtlMasterEvent.EventsMasterAssignedModules(ModuleId)
                .DataBind()
            End With
        End Sub

        ''' <summary>
        ''' Get Avaiable Sub Events for Portal and Bind to DropDown
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub BindAvailableEvents()
            With lstAvailableCals
                .DataTextField = "SubEventTitle"
                .DataValueField = "SubEventID"
                .DataSource = Nothing
                .DataBind()
                .DataSource = _objCtlMasterEvent.EventsMasterAvailableModules(PortalId, ModuleId)
                .DataBind()
            End With
        End Sub

        Private Sub Enable_Disable_Cals()
            divMasterEvent.Visible = chkMasterEvent.Checked
        End Sub

        Private Function EnrollListFields(ByVal blUser As Boolean, ByVal blDisp As Boolean, ByVal blEmail As Boolean, ByVal blPhone As Boolean, ByVal blApprove As Boolean, ByVal blNo As Boolean) As String
            Dim listFields As String = ""
            If blUser Then
                listFields = listFields + "01;"
            End If
            If blDisp Then
                listFields = listFields + "02;"
            End If
            If blEmail Then
                listFields = listFields + "03;"
            End If
            If blPhone Then
                listFields = listFields + "04;"
            End If
            If blApprove Then
                listFields = listFields + "05;"
            End If
            If blNo Then
                listFields = listFields + "06;"
            End If
            Return listFields
        End Function

        Public Function AddSkinContainerControls(ByVal url As String, ByVal addchar As String) As String
            Dim objCtlTab As New TabController
            Dim objTabInfo As TabInfo = objCtlTab.GetTab(TabId, PortalId, False)
            Dim skinSrc As String = Nothing
            If Not objTabInfo.SkinSrc = "" Then
                skinSrc = objTabInfo.SkinSrc
                If Right(skinSrc, 5) = ".ascx" Then
                    skinSrc = Left(skinSrc, Len(skinSrc) - 5)
                End If
            End If
            Dim objCtlModule As New ModuleController
            Dim objModuleInfo As ModuleInfo = objCtlModule.GetModule(ModuleId, TabId, False)
            Dim containerSrc As String = Nothing
            If objModuleInfo.DisplayTitle Then
                If Not objModuleInfo.ContainerSrc = "" Then
                    containerSrc = objModuleInfo.ContainerSrc
                ElseIf Not objTabInfo.ContainerSrc = "" Then
                    containerSrc = objTabInfo.ContainerSrc
                End If
                If Not containerSrc Is Nothing Then
                    If Right(containerSrc, 5) = ".ascx" Then
                        containerSrc = Left(containerSrc, Len(containerSrc) - 5)
                    End If
                End If
            Else
                containerSrc = "[G]Containers/_default/No+Container"
            End If
            If Not containerSrc Is Nothing Then
                url += addchar + "ContainerSrc=" & HttpUtility.HtmlEncode(containerSrc)
                addchar = "&"
            End If
            If Not skinSrc Is Nothing Then
                url += addchar + "SkinSrc=" & HttpUtility.HtmlEncode(skinSrc)
            End If
            Return url
        End Function

#End Region

#Region "Links, Buttons and Events"
        Private Sub chkMasterEvent_CheckedChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles chkMasterEvent.CheckedChanged
            cmdRemoveAllCals_Click(sender, e)
            Enable_Disable_Cals()
        End Sub

        Private Sub cmdAdd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdAdd.Click

            Dim objListItem As ListItem

            Dim objList As ArrayList = New ArrayList

            For Each objListItem In lstAvailable.Items
                objList.Add(objListItem)
            Next

            For Each objListItem In objList
                If objListItem.Selected Then
                    lstAvailable.Items.Remove(objListItem)
                    lstAssigned.Items.Add(objListItem)
                End If
            Next

            lstAvailable.ClearSelection()
            lstAssigned.ClearSelection()

            Sort(lstAssigned)

        End Sub

        Private Sub cmdRemove_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdRemove.Click

            Dim objListItem As ListItem

            Dim objList As ArrayList = New ArrayList

            For Each objListItem In lstAssigned.Items
                objList.Add(objListItem)
            Next

            For Each objListItem In objList
                If objListItem.Selected Then
                    lstAssigned.Items.Remove(objListItem)
                    lstAvailable.Items.Add(objListItem)
                End If
            Next

            lstAvailable.ClearSelection()
            lstAssigned.ClearSelection()

            Sort(lstAvailable)

        End Sub

        Private Sub cmdAddAll_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdAddAll.Click

            Dim objListItem As ListItem

            For Each objListItem In lstAvailable.Items
                lstAssigned.Items.Add(objListItem)
            Next

            lstAvailable.Items.Clear()

            lstAvailable.ClearSelection()
            lstAssigned.ClearSelection()

            Sort(lstAssigned)

        End Sub

        Private Sub cmdRemoveAll_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdRemoveAll.Click

            Dim objListItem As ListItem

            For Each objListItem In lstAssigned.Items
                lstAvailable.Items.Add(objListItem)
            Next

            lstAssigned.Items.Clear()

            lstAvailable.ClearSelection()
            lstAssigned.ClearSelection()

            Sort(lstAvailable)
        End Sub

        Private Sub Sort(ByVal ctlListBox As ListBox)

            Dim arrListItems As New ArrayList
            Dim objListItem As ListItem

            ' store listitems in temp arraylist
            For Each objListItem In ctlListBox.Items
                arrListItems.Add(objListItem)
            Next

            ' sort arraylist based on text value
            arrListItems.Sort(New ListItemComparer)

            ' clear control
            ctlListBox.Items.Clear()

            ' add listitems to control
            For Each objListItem In arrListItems
                ctlListBox.Items.Add(objListItem)
            Next
        End Sub

        Private Sub cmdAddCals_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdAddCals.Click
            Dim objListItem As ListItem
            Dim masterEvent As New EventMasterInfo

            For Each objListItem In lstAvailableCals.Items
                If objListItem.Selected Then
                    masterEvent.MasterID = Nothing
                    masterEvent.ModuleID = ModuleId
                    masterEvent.SubEventID = CType(objListItem.Value(), Integer)
                    _objCtlMasterEvent.EventsMasterSave(masterEvent)
                End If
            Next

            BindSubEvents()
            BindAvailableEvents()
        End Sub

        Private Sub cmdAddAllCals_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdAddAllCals.Click
            Dim objListItem As ListItem
            Dim masterEvent As New EventMasterInfo

            For Each objListItem In lstAvailableCals.Items
                masterEvent.MasterID = Nothing
                masterEvent.ModuleID = ModuleId
                masterEvent.SubEventID = CType(objListItem.Value(), Integer)
                _objCtlMasterEvent.EventsMasterSave(masterEvent)
            Next

            BindSubEvents()
            BindAvailableEvents()
        End Sub

        Private Sub cmdRemoveCals_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdRemoveCals.Click
            Dim objListItem As ListItem

            For Each objListItem In lstAssignedCals.Items
                If objListItem.Selected Then
                    _objCtlMasterEvent.EventsMasterDelete(CInt(objListItem.Value()), ModuleId)
                End If
            Next

            BindSubEvents()
            BindAvailableEvents()
        End Sub

        Private Sub cmdRemoveAllCals_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdRemoveAllCals.Click
            Dim objListItem As ListItem

            For Each objListItem In lstAssignedCals.Items
                _objCtlMasterEvent.EventsMasterDelete(CInt(objListItem.Value()), ModuleId)
            Next

            BindSubEvents()
            BindAvailableEvents()
        End Sub

        Private Sub cmdUpdateTemplate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdUpdateTemplate.Click
            Dim strTemplate As String = ddlTemplates.SelectedValue
            Settings.Templates.SaveTemplate(ModuleId, strTemplate, txtEventTemplate.Text.Trim)
            lblTemplateUpdated.Visible = True
            lblTemplateUpdated.Text = String.Format(Localization.GetString("TemplateUpdated", LocalResourceFile), Localization.GetString(strTemplate + "Name", LocalResourceFile))
        End Sub

        Private Sub cmdResetTemplate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdResetTemplate.Click
            Dim strTemplate As String = ddlTemplates.SelectedValue
            Settings.Templates.ResetTemplate(ModuleId, strTemplate, LocalResourceFile)
            txtEventTemplate.Text = Settings.Templates.GetTemplate(strTemplate)
            lblTemplateUpdated.Visible = True
            lblTemplateUpdated.Text = String.Format(Localization.GetString("TemplateReset", LocalResourceFile), Localization.GetString(strTemplate + "Name", LocalResourceFile))
        End Sub

        Private Sub ddlTemplates_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlTemplates.SelectedIndexChanged
            txtEventTemplate.Text = Settings.Templates.GetTemplate(ddlTemplates.SelectedValue)
            lblTemplateUpdated.Visible = False
        End Sub

        Private Sub cancelButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cancelButton.Click
            Try
                Response.Redirect(GetSocialNavigateUrl(), True)
            Catch exc As Exception 'Module failed to load
                'ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub updateButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles updateButton.Click
            Try
                UpdateSettings()
                Response.Redirect(GetSocialNavigateUrl(), True)
            Catch exc As Exception 'Module failed to load
                'ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

    End Class

#Region "Comparer Class"
    Public Class ListItemComparer
        Implements IComparer

        Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
            Dim a As ListItem = CType(x, ListItem)
            Dim b As ListItem = CType(y, ListItem)
            Dim c As New CaseInsensitiveComparer
            Return c.Compare(a.Text, b.Text)
        End Function
    End Class
#End Region

#Region "DataClasses"
    Public Class ThemeSetting
#Region "Member Variables"
        Public SettingType As ThemeSettingTypeEnum
        Public ThemeName As String
        Public ThemeFile As String
        Public CssClass As String
#End Region
#Region "Enumerators"
        Public Enum ThemeSettingTypeEnum
            DefaultTheme = 0
            CustomTheme = 1
        End Enum
#End Region
#Region "Methods"
        Public Function ValidateSetting(ByVal setting As String) As Boolean
            Dim s() As String = setting.Split(CChar(","))
            If Not s.GetUpperBound(0) = 2 Then Return False
            If Not IsNumeric(s(0)) Then Return False
            Return True
        End Function

        Public Sub ReadSetting(ByVal setting As String, ByVal portalID As Integer)
            If Not ValidateSetting(setting) Then Throw New Exception("Setting is not right format to convert into ThemeSetting")

            Dim s() As String = setting.Split(CChar(","))
            SettingType = CType(s(0), ThemeSettingTypeEnum)
            ThemeName = s(1)
            CssClass = "Theme" & ThemeName
            Select Case SettingType
                Case ThemeSettingTypeEnum.DefaultTheme
                    ThemeFile = String.Format("{0}/DesktopModules/Events/Themes/{1}/{1}.css", ApplicationPath, ThemeName)
                Case ThemeSettingTypeEnum.CustomTheme
                    Dim pc As New Entities.Portals.PortalController
                    With pc.GetPortal(PortalID)
                        ThemeFile = String.Format("{0}/{1}/DNNEvents/Themes/{2}/{2}.css", ApplicationPath, .HomeDirectory, ThemeName)
                    End With
            End Select

        End Sub
#End Region
#Region "Overrides"
        Public Overrides Function ToString() As String
            Return String.Format("{0},{1},{2}", CInt(SettingType), ThemeName, ThemeFile)
        End Function
#End Region
    End Class
#End Region
End Namespace
