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
    using System.IO;
    using System.Reflection;
    using System.Web;
    using System.Web.UI.WebControls;
    using DotNetNuke.Common.Lists;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Entities.Tabs;
    using DotNetNuke.Security;
    using DotNetNuke.Security.Roles;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.FileSystem;
    using DotNetNuke.Services.Localization;
    using DotNetNuke.Web.UI.WebControls.Extensions;
    using global::Components;
    using Microsoft.VisualBasic;
    using Telerik.Web.UI;
    using FirstDayOfWeek = System.Web.UI.WebControls.FirstDayOfWeek;
    using Globals = DotNetNuke.Common.Globals;

    [DNNtc.ModuleControlProperties("EventSettings", "Event Settings", DNNtc.ControlType.View, "https://github.com/DNNCommunity/DNN.Events/wiki", true, true)]
    public partial class EventSettings : EventBase
    {
        #region Private Data

        private readonly EventMasterController _objCtlMasterEvent = new EventMasterController();

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

        #region Help Methods

        // If adding new Setting also see 'SetDefaultModuleSettings' method in EventInfoHelper Class

        /// <summary>
        ///     Load current settings into the controls from the modulesettings
        /// </summary>
        /// <remarks></remarks>
        private void Page_Load(object sender, EventArgs e)
        {
            if (PortalSecurity.IsInRole(this.PortalSettings.AdministratorRoleName) || this.IsSettingsEditor())
            { }
            else
            {
                this.Response.Redirect(this.GetSocialNavigateUrl(), true);
            }

            // Set the selected theme
            this.SetTheme(this.pnlEventsModuleSettings);

            // Do we have to load the settings
            if (!this.Page.IsPostBack)
            {
                this.LoadSettings();
            }

            // Add the javascript to the page
            this.AddJavaScript();
        }

        private void LoadSettings()
        {
            var availableFields = new ArrayList();
            var selectedFields = new ArrayList();

            // Create Lists and Schedule - they should always exist
            var objEventController = new EventController();
            objEventController.CreateListsAndSchedule();

            //Set text and tooltip from resourcefile
            this.chkMonthAllowed.Text = Localization.GetString("Month", this.LocalResourceFile);
            this.chkWeekAllowed.Text = Localization.GetString("Week", this.LocalResourceFile);
            this.chkListAllowed.Text = Localization.GetString("List", this.LocalResourceFile);
            this.cmdAdd.ToolTip = Localization.GetString("Add", this.LocalResourceFile);
            this.cmdAddAll.ToolTip = Localization.GetString("AddAll", this.LocalResourceFile);
            this.cmdRemove.ToolTip = Localization.GetString("Remove", this.LocalResourceFile);
            this.cmdRemoveAll.ToolTip = Localization.GetString("RemoveAll", this.LocalResourceFile);
            this.cmdAddCals.ToolTip = Localization.GetString("AddCals", this.LocalResourceFile);
            this.cmdAddAllCals.ToolTip = Localization.GetString("AddAllCals", this.LocalResourceFile);
            this.cmdRemoveCals.ToolTip = Localization.GetString("RemoveCals", this.LocalResourceFile);
            this.cmdRemoveAllCals.ToolTip = Localization.GetString("RemoveAllCals", this.LocalResourceFile);
            this.chkIconMonthPrio.Text = Localization.GetString("Priority", this.LocalResourceFile);
            this.imgIconMonthPrioHigh.AlternateText = Localization.GetString("HighPrio", this.LocalResourceFile);
            this.imgIconMonthPrioLow.AlternateText = Localization.GetString("LowPrio", this.LocalResourceFile);
            this.chkIconMonthRec.Text = Localization.GetString("Recurring", this.LocalResourceFile);
            this.imgIconMonthRec.AlternateText = Localization.GetString("RecurringEvent", this.LocalResourceFile);
            this.chkIconMonthReminder.Text = Localization.GetString("Reminder", this.LocalResourceFile);
            this.imgIconMonthReminder.AlternateText = Localization.GetString("ReminderEnabled", this.LocalResourceFile);
            this.chkIconMonthEnroll.Text = Localization.GetString("Enroll", this.LocalResourceFile);
            this.imgIconMonthEnroll.AlternateText = Localization.GetString("EnrollEnabled", this.LocalResourceFile);
            this.chkIconWeekPrio.Text = Localization.GetString("Priority", this.LocalResourceFile);
            this.imgIconWEEKPrioHigh.AlternateText = Localization.GetString("HighPrio", this.LocalResourceFile);
            this.imgIconWeekPrioLow.AlternateText = Localization.GetString("LowPrio", this.LocalResourceFile);
            this.chkIconWeekRec.Text = Localization.GetString("Recurring", this.LocalResourceFile);
            this.imgIconWeekRec.AlternateText = Localization.GetString("RecurringEvent", this.LocalResourceFile);
            this.chkIconWeekReminder.Text = Localization.GetString("Reminder", this.LocalResourceFile);
            this.imgIconWeekReminder.AlternateText = Localization.GetString("ReminderEnabled", this.LocalResourceFile);
            this.chkIconWeekEnroll.Text = Localization.GetString("Enroll", this.LocalResourceFile);
            this.imgIconWeekEnroll.AlternateText = Localization.GetString("EnrollEnabled", this.LocalResourceFile);
            this.chkIconListPrio.Text = Localization.GetString("Priority", this.LocalResourceFile);
            this.imgIconListPrioHigh.AlternateText = Localization.GetString("HighPrio", this.LocalResourceFile);
            this.imgIconListPrioLow.AlternateText = Localization.GetString("LowPrio", this.LocalResourceFile);
            this.chkIconListRec.Text = Localization.GetString("Recurring", this.LocalResourceFile);
            this.imgIconListRec.AlternateText = Localization.GetString("RecurringEvent", this.LocalResourceFile);
            this.chkIconListReminder.Text = Localization.GetString("Reminder", this.LocalResourceFile);
            this.imgIconListReminder.AlternateText = Localization.GetString("ReminderEnabled", this.LocalResourceFile);
            this.chkIconListEnroll.Text = Localization.GetString("Enroll", this.LocalResourceFile);
            this.imgIconListEnroll.AlternateText = Localization.GetString("EnrollEnabled", this.LocalResourceFile);
            this.cmdUpdateTemplate.Text = Localization.GetString("cmdUpdateTemplate", this.LocalResourceFile);
            this.rbThemeStandard.Text = Localization.GetString("rbThemeStandard", this.LocalResourceFile);
            this.rbThemeCustom.Text = Localization.GetString("rbThemeCustom", this.LocalResourceFile);
            this.ddlModuleCategories.EmptyMessage = Localization.GetString("NoCategories", this.LocalResourceFile);
            this.ddlModuleCategories.Localization.AllItemsCheckedString =
                Localization.GetString("AllCategories", this.LocalResourceFile);
            this.ddlModuleCategories.Localization.CheckAllString =
                Localization.GetString("SelectAllCategories", this.LocalResourceFile);
            this.ddlModuleLocations.EmptyMessage = Localization.GetString("NoLocations", this.LocalResourceFile);
            this.ddlModuleLocations.Localization.AllItemsCheckedString =
                Localization.GetString("AllLocations", this.LocalResourceFile);
            this.ddlModuleLocations.Localization.CheckAllString =
                Localization.GetString("SelectAllLocations", this.LocalResourceFile);


            //Add templates link
            // lnkTemplatesHelp.HRef = AddSkinContainerControls(EditUrl("", "", "TemplateHelp", "dnnprintmode=true"), "?")
            this.lnkTemplatesHelp.HRef =
                this.AddSkinContainerControls(
                    Globals.NavigateURL(this.TabId, this.PortalSettings, "", "mid=" + Convert.ToString(this.ModuleId),
                                        "ctl=TemplateHelp", "ShowNav=False", "dnnprintmode=true"), "?");
            this.lnkTemplatesHelp.InnerText = Localization.GetString("TemplatesHelp", this.LocalResourceFile);

            //Support for Time Interval Dropdown
            var ctlLists = new ListController();
            var colThreadStatus = ctlLists.GetListEntryInfoItems("Timeinterval");
            this.ddlTimeInterval.Items.Clear();

            foreach (var entry in colThreadStatus)
            {
                this.ddlTimeInterval.Items.Add(entry.Value);
            }
            this.ddlTimeInterval.Items.FindByValue(this.Settings.Timeinterval).Selected = true;

            // Set Dropdown TimeZone
            this.cboTimeZone.DataBind(this.Settings.TimeZoneId);

            this.chkEnableEventTimeZones.Checked = this.Settings.EnableEventTimeZones;

            this.BindToEnum(typeof(EventModuleSettings.TimeZones), this.ddlPrimaryTimeZone);
            this.ddlPrimaryTimeZone.Items.FindByValue(Convert.ToString((int) this.Settings.PrimaryTimeZone)).Selected =
                true;
            this.BindToEnum(typeof(EventModuleSettings.TimeZones), this.ddlSecondaryTimeZone);
            this.ddlSecondaryTimeZone.Items.FindByValue(Convert.ToString((int) this.Settings.SecondaryTimeZone))
                .Selected = true;

            this.chkToolTipMonth.Checked = this.Settings.Eventtooltipmonth;
            this.chkToolTipWeek.Checked = this.Settings.Eventtooltipweek;
            this.chkToolTipDay.Checked = this.Settings.Eventtooltipday;
            this.chkToolTipList.Checked = this.Settings.Eventtooltiplist;
            this.txtTooltipLength.Text = this.Settings.Eventtooltiplength.ToString();
            this.chkImageEnabled.Checked = this.Settings.Eventimage;
            this.txtMaxThumbHeight.Text = this.Settings.MaxThumbHeight.ToString();
            this.txtMaxThumbWidth.Text = this.Settings.MaxThumbWidth.ToString();

            this.chkMonthCellEvents.Checked = true;
            if (this.Settings.Monthcellnoevents)
            {
                this.chkMonthCellEvents.Checked = false;
            }

            this.chkAddSubModuleName.Checked = this.Settings.Addsubmodulename;
            this.chkEnforceSubCalPerms.Checked = this.Settings.Enforcesubcalperms;

            this.BindToEnum(typeof(EventModuleSettings.DisplayCategories), this.ddlEnableCategories);
            this.ddlEnableCategories.Items.FindByValue(Convert.ToString((int) this.Settings.Enablecategories))
                .Selected = true;
            this.chkRestrictCategories.Checked = this.Settings.Restrictcategories;
            this.BindToEnum(typeof(EventModuleSettings.DisplayLocations), this.ddlEnableLocations);
            this.ddlEnableLocations.Items.FindByValue(Convert.ToString((int) this.Settings.Enablelocations)).Selected =
                true;
            this.chkRestrictLocations.Checked = this.Settings.Restrictlocations;

            this.chkEnableContainerSkin.Checked = this.Settings.Enablecontainerskin;
            this.chkEventDetailNewPage.Checked = this.Settings.Eventdetailnewpage;
            this.chkEnableEnrollPopup.Checked = this.Settings.Enableenrollpopup;
            this.chkEventImageMonth.Checked = this.Settings.EventImageMonth;
            this.chkEventImageWeek.Checked = this.Settings.EventImageWeek;
            this.chkEventDayNewPage.Checked = this.Settings.Eventdaynewpage;
            this.chkFullTimeScale.Checked = this.Settings.Fulltimescale;
            this.chkCollapseRecurring.Checked = this.Settings.Collapserecurring;
            this.chkIncludeEndValue.Checked = this.Settings.Includeendvalue;
            this.chkShowValueMarks.Checked = this.Settings.Showvaluemarks;

            this.chkEnableSEO.Checked = this.Settings.EnableSEO;
            this.txtSEODescriptionLength.Text = this.Settings.SEODescriptionLength.ToString();

            this.chkEnableSitemap.Checked = this.Settings.EnableSitemap;
            this.txtSitemapPriority.Text = this.Settings.SiteMapPriority.ToString();
            this.txtSitemapDaysBefore.Text = this.Settings.SiteMapDaysBefore.ToString();
            this.txtSitemapDaysAfter.Text = this.Settings.SiteMapDaysAfter.ToString();

            this.chkiCalOnIconBar.Checked = this.Settings.IcalOnIconBar;
            this.chkiCalEmailEnable.Checked = this.Settings.IcalEmailEnable;
            this.chkiCalURLinLocation.Checked = this.Settings.IcalURLInLocation;
            this.chkiCalIncludeCalname.Checked = this.Settings.IcalIncludeCalname;
            this.txtiCalDaysBefore.Text = this.Settings.IcalDaysBefore.ToString();
            this.txtiCalDaysAfter.Text = this.Settings.IcalDaysAfter.ToString();
            this.txtiCalURLAppend.Text = this.Settings.IcalURLAppend;
            this.ctliCalDefaultImage.FileFilter = Globals.glbImageFileTypes;
            this.ctliCalDefaultImage.Url = "";
            this.chkiCalDisplayImage.Checked = false;
            if (this.Settings.IcalDefaultImage != "")
            {
                this.ctliCalDefaultImage.Url = this.Settings.IcalDefaultImage.Substring(6);
                this.chkiCalDisplayImage.Checked = true;
            }
            if (this.ctliCalDefaultImage.Url.StartsWith("FileID="))
            {
                var fileId = int.Parse(Convert.ToString(this.ctliCalDefaultImage.Url.Substring(7)));
                var objFileInfo = FileManager.Instance.GetFile(fileId);
                if (!ReferenceEquals(objFileInfo, null))
                {
                    this.ctliCalDefaultImage.Url = objFileInfo.Folder + objFileInfo.FileName;
                }
                else
                {
                    this.ctliCalDefaultImage.Url = "";
                }
            }
            var socialGroupId = this.GetUrlGroupId();
            var socialGroupStr = "";
            if (socialGroupId > 0)
            {
                socialGroupStr = "&groupid=" + socialGroupId;
            }
            this.lbliCalURL.Text = Globals.AddHTTP(this.PortalSettings.PortalAlias.HTTPAlias +
                                                   "/DesktopModules/Events/EventVCal.aspx?ItemID=0&Mid=" +
                                                   Convert.ToString(this.ModuleId) + "&tabid=" +
                                                   Convert.ToString(this.TabId) + socialGroupStr);

            // Set Up Themes
            this.LoadThemes();

            this.txtPayPalURL.Text = this.Settings.Paypalurl;

            this.chkEnableEventNav.Checked = true;
            if (this.Settings.DisableEventnav)
            {
                this.chkEnableEventNav.Checked = false;
            }

            this.chkAllowRecurring.Checked = this.Settings.Allowreoccurring;
            this.txtMaxRecurrences.Text = this.Settings.Maxrecurrences;
            this.chkEventNotify.Checked = this.Settings.Eventnotify;
            this.chkDetailPageAllowed.Checked = this.Settings.DetailPageAllowed;
            this.chkEnrollmentPageAllowed.Checked = this.Settings.EnrollmentPageAllowed;
            this.txtEnrollmentPageDefaultURL.Text = this.Settings.EnrollmentPageDefaultUrl;
            this.chkNotifyAnon.Checked = this.Settings.Notifyanon;
            this.chkSendReminderDefault.Checked = this.Settings.Sendreminderdefault;

            this.rblNewEventEmail.Items[0].Selected = true;
            switch (this.Settings.Neweventemails)
            {
                case "Subscribe":
                    this.rblNewEventEmail.Items[1].Selected = true;
                    break;
                case "Role":
                    this.rblNewEventEmail.Items[2].Selected = true;
                    break;
            }

            this.LoadNewEventEmailRoles(this.Settings.Neweventemailrole);
            this.chkNewPerEventEmail.Checked = this.Settings.Newpereventemail;

            this.ddlDefaultView.Items.Clear();
            this.ddlDefaultView.Items.Add(new ListItem(Localization.GetString("Month", this.LocalResourceFile),
                                                       "EventMonth.ascx"));
            this.ddlDefaultView.Items.Add(new ListItem(Localization.GetString("Week", this.LocalResourceFile),
                                                       "EventWeek.ascx"));
            this.ddlDefaultView.Items.Add(new ListItem(Localization.GetString("List", this.LocalResourceFile),
                                                       "EventList.ascx"));

            this.ddlDefaultView.Items.FindByValue(this.Settings.DefaultView).Selected = true;

            this.chkMonthAllowed.Checked = this.Settings.MonthAllowed;
            this.chkWeekAllowed.Checked = this.Settings.WeekAllowed;
            this.chkListAllowed.Checked = this.Settings.ListAllowed;
            this.chkEnableSearch.Checked = this.Settings.Eventsearch;
            this.chkPreventConflicts.Checked = this.Settings.Preventconflicts;
            this.chkLocationConflict.Checked = this.Settings.Locationconflict;
            this.chkShowEventsAlways.Checked = this.Settings.ShowEventsAlways;
            this.chkTimeInTitle.Checked = this.Settings.Timeintitle;
            this.chkMonthDaySelect.Checked = this.Settings.Monthdayselect;
            this.chkEventSignup.Checked = this.Settings.Eventsignup;
            this.chkEventSignupAllowPaid.Checked = this.Settings.Eventsignupallowpaid;
            this.chkDefaultEnrollView.Checked = this.Settings.Eventdefaultenrollview;
            this.chkHideFullEnroll.Checked = this.Settings.Eventhidefullenroll;
            this.txtMaxNoEnrolees.Text = this.Settings.Maxnoenrolees.ToString();
            this.txtCancelDays.Text = this.Settings.Enrolcanceldays.ToString();
            this.chkFridayWeekend.Checked = this.Settings.Fridayweekend;
            this.chkModerateAll.Checked = this.Settings.Moderateall;
            this.chkTZDisplay.Checked = this.Settings.Tzdisplay;
            this.chkListViewUseTime.Checked = this.Settings.ListViewUseTime;

            this.txtPayPalAccount.Text = this.Settings.Paypalaccount;
            if (this.txtPayPalAccount.Text.Length == 0)
            {
                this.txtPayPalAccount.Text = this.PortalSettings.Email;
            }

            this.txtReminderFrom.Text = this.Settings.Reminderfrom;
            if (this.txtReminderFrom.Text.Length == 0)
            {
                this.txtReminderFrom.Text = this.PortalSettings.Email;
            }

            this.txtStandardEmail.Text = this.Settings.StandardEmail;
            if (this.txtStandardEmail.Text.Length == 0)
            {
                this.txtStandardEmail.Text = this.PortalSettings.Email;
            }

            this.BindSubEvents();
            this.BindAvailableEvents();

            this.chkMasterEvent.Checked = this.Settings.MasterEvent;

            this.Enable_Disable_Cals();

            this.chkIconMonthPrio.Checked = this.Settings.IconMonthPrio;
            this.chkIconWeekPrio.Checked = this.Settings.IconWeekPrio;
            this.chkIconListPrio.Checked = this.Settings.IconListPrio;
            this.chkIconMonthRec.Checked = this.Settings.IconMonthRec;
            this.chkIconWeekRec.Checked = this.Settings.IconMonthRec;
            this.chkIconListRec.Checked = this.Settings.IconListRec;
            this.chkIconMonthReminder.Checked = this.Settings.IconMonthReminder;
            this.chkIconWeekReminder.Checked = this.Settings.IconWeekReminder;
            this.chkIconListReminder.Checked = this.Settings.IconListReminder;
            this.chkIconMonthEnroll.Checked = this.Settings.IconMonthEnroll;
            this.chkIconWeekEnroll.Checked = this.Settings.IconWeekEnroll;
            this.chkIconListEnroll.Checked = this.Settings.IconListEnroll;
            this.txtPrivateMessage.Text = this.Settings.PrivateMessage;

            var columnNo = 0;
            for (columnNo = 1; columnNo <= 13; columnNo++)
            {
                var columnAcronym = this.GetListColumnAcronym(columnNo);
                var columnName = this.GetListColumnName(columnAcronym);
                if (this.Settings.EventsListFields.LastIndexOf(columnAcronym, StringComparison.Ordinal) > -1)
                {
                    selectedFields.Add(columnName);
                }
                else
                {
                    availableFields.Add(columnName);
                }
            }

            this.lstAvailable.DataSource = availableFields;
            this.lstAvailable.DataBind();
            this.Sort(this.lstAvailable);

            this.lstAssigned.DataSource = selectedFields;
            this.lstAssigned.DataBind();
            this.Sort(this.lstAssigned);

            if (this.Settings.EventsListSelectType == this.rblSelectionTypeDays.Value)
            {
                this.rblSelectionTypeDays.Checked = true;
                this.rblSelectionTypeEvents.Checked = false;
            }
            else
            {
                this.rblSelectionTypeDays.Checked = false;
                this.rblSelectionTypeEvents.Checked = true;
            }

            if (this.Settings.ListViewGrid)
            {
                this.rblListViewGrid.Items[0].Selected = true;
            }
            else
            {
                this.rblListViewGrid.Items[1].Selected = true;
            }
            this.chkListViewTable.Checked = this.Settings.ListViewTable;
            this.txtRptColumns.Text = this.Settings.RptColumns.ToString();
            this.txtRptRows.Text = this.Settings.RptRows.ToString();

            // Do we have to display the EventsList header
            if (this.Settings.EventsListShowHeader != "No")
            {
                this.rblShowHeader.Items[0].Selected = true;
            }
            else
            {
                this.rblShowHeader.Items[1].Selected = true;
            }

            this.txtDaysBefore.Text = this.Settings.EventsListBeforeDays.ToString();
            this.txtDaysAfter.Text = this.Settings.EventsListAfterDays.ToString();
            this.txtNumEvents.Text = this.Settings.EventsListNumEvents.ToString();
            this.txtEventDays.Text = this.Settings.EventsListEventDays.ToString();
            this.chkRestrictCategoriesToTimeFrame.Checked = this.Settings.RestrictCategoriesToTimeFrame;
            this.chkRestrictLocationsToTimeFrame.Checked = this.Settings.RestrictLocationsToTimeFrame;

            this.chkCustomField1.Checked = this.Settings.EventsCustomField1;
            this.chkCustomField2.Checked = this.Settings.EventsCustomField2;

            this.ddlPageSize.Items.FindByValue(Convert.ToString(this.Settings.EventsListPageSize)).Selected = true;
            this.ddlListSortedFieldDirection.Items.Clear();
            this.ddlListSortedFieldDirection.Items.Add(
                new ListItem(Localization.GetString("Asc", this.LocalResourceFile), "ASC"));
            this.ddlListSortedFieldDirection.Items.Add(
                new ListItem(Localization.GetString("Desc", this.LocalResourceFile), "DESC"));
            this.ddlListSortedFieldDirection.Items.FindByValue(this.Settings.EventsListSortDirection).Selected = true;

            this.ddlListDefaultColumn.Items.Clear();
            this.ddlListDefaultColumn.Items.Add(
                new ListItem(Localization.GetString("SortEventID", this.LocalResourceFile), "EventID"));
            this.ddlListDefaultColumn.Items.Add(
                new ListItem(Localization.GetString("SortEventDateBegin", this.LocalResourceFile),
                             "EventDateBegin"));
            this.ddlListDefaultColumn.Items.Add(
                new ListItem(Localization.GetString("SortEventDateEnd", this.LocalResourceFile), "EventDateEnd"));
            this.ddlListDefaultColumn.Items.Add(
                new ListItem(Localization.GetString("SortEventName", this.LocalResourceFile), "EventName"));
            this.ddlListDefaultColumn.Items.Add(
                new ListItem(Localization.GetString("SortDuration", this.LocalResourceFile), "Duration"));
            this.ddlListDefaultColumn.Items.Add(
                new ListItem(Localization.GetString("SortCategoryName", this.LocalResourceFile), "CategoryName"));
            this.ddlListDefaultColumn.Items.Add(
                new ListItem(Localization.GetString("SortCustomField1", this.LocalResourceFile), "CustomField1"));
            this.ddlListDefaultColumn.Items.Add(
                new ListItem(Localization.GetString("SortCustomField2", this.LocalResourceFile), "CustomField2"));
            this.ddlListDefaultColumn.Items.Add(
                new ListItem(Localization.GetString("SortDescription", this.LocalResourceFile), "Description"));
            this.ddlListDefaultColumn.Items.Add(
                new ListItem(Localization.GetString("SortLocationName", this.LocalResourceFile), "LocationName"));
            this.ddlListDefaultColumn.Items.FindByValue(this.Settings.EventsListSortColumn).Selected = true;

            this.ddlWeekStart.Items.Clear();
            this.ddlWeekStart.Items.Add(new ListItem(FirstDayOfWeek.Default.ToString(),
                                                     Convert.ToInt32(FirstDayOfWeek.Default).ToString()));
            this.ddlWeekStart.Items.Add(new ListItem(FirstDayOfWeek.Monday.ToString(),
                                                     Convert.ToInt32(FirstDayOfWeek.Monday).ToString()));
            this.ddlWeekStart.Items.Add(new ListItem(FirstDayOfWeek.Tuesday.ToString(),
                                                     Convert.ToInt32(FirstDayOfWeek.Tuesday).ToString()));
            this.ddlWeekStart.Items.Add(new ListItem(FirstDayOfWeek.Wednesday.ToString(),
                                                     Convert.ToInt32(FirstDayOfWeek.Wednesday).ToString()));
            this.ddlWeekStart.Items.Add(new ListItem(FirstDayOfWeek.Thursday.ToString(),
                                                     Convert.ToInt32(FirstDayOfWeek.Thursday).ToString()));
            this.ddlWeekStart.Items.Add(new ListItem(FirstDayOfWeek.Friday.ToString(),
                                                     Convert.ToInt32(FirstDayOfWeek.Friday).ToString()));
            this.ddlWeekStart.Items.Add(new ListItem(FirstDayOfWeek.Saturday.ToString(),
                                                     Convert.ToInt32(FirstDayOfWeek.Saturday).ToString()));
            this.ddlWeekStart.Items.Add(new ListItem(FirstDayOfWeek.Sunday.ToString(),
                                                     Convert.ToInt32(FirstDayOfWeek.Sunday).ToString()));
            this.ddlWeekStart.Items.FindByValue(Convert.ToInt32(this.Settings.WeekStart).ToString()).Selected = true;

            if (this.Settings.EnrollEditFields.LastIndexOf("01", StringComparison.Ordinal) > -1)
            {
                this.rblEnUserEdit.Checked = true;
            }
            else if (this.Settings.EnrollViewFields.LastIndexOf("01", StringComparison.Ordinal) > -1)
            {
                this.rblEnUserView.Checked = true;
            }
            else if (this.Settings.EnrollAnonFields.LastIndexOf("01", StringComparison.Ordinal) > -1)
            {
                this.rblEnUserAnon.Checked = true;
            }
            else
            {
                this.rblEnUserNone.Checked = true;
            }

            if (this.Settings.EnrollEditFields.LastIndexOf("02", StringComparison.Ordinal) > -1)
            {
                this.rblEnDispEdit.Checked = true;
            }
            else if (this.Settings.EnrollViewFields.LastIndexOf("02", StringComparison.Ordinal) > -1)
            {
                this.rblEnDispView.Checked = true;
            }
            else if (this.Settings.EnrollAnonFields.LastIndexOf("02", StringComparison.Ordinal) > -1)
            {
                this.rblEnDispAnon.Checked = true;
            }
            else
            {
                this.rblEnDispNone.Checked = true;
            }

            if (this.Settings.EnrollEditFields.LastIndexOf("03", StringComparison.Ordinal) > -1)
            {
                this.rblEnEmailEdit.Checked = true;
            }
            else if (this.Settings.EnrollViewFields.LastIndexOf("03", StringComparison.Ordinal) > -1)
            {
                this.rblEnEmailView.Checked = true;
            }
            else if (this.Settings.EnrollAnonFields.LastIndexOf("03", StringComparison.Ordinal) > -1)
            {
                this.rblEnEmailAnon.Checked = true;
            }
            else
            {
                this.rblEnEmailNone.Checked = true;
            }

            if (this.Settings.EnrollEditFields.LastIndexOf("04", StringComparison.Ordinal) > -1)
            {
                this.rblEnPhoneEdit.Checked = true;
            }
            else if (this.Settings.EnrollViewFields.LastIndexOf("04", StringComparison.Ordinal) > -1)
            {
                this.rblEnPhoneView.Checked = true;
            }
            else if (this.Settings.EnrollAnonFields.LastIndexOf("04", StringComparison.Ordinal) > -1)
            {
                this.rblEnPhoneAnon.Checked = true;
            }
            else
            {
                this.rblEnPhoneNone.Checked = true;
            }

            if (this.Settings.EnrollEditFields.LastIndexOf("05", StringComparison.Ordinal) > -1)
            {
                this.rblEnApproveEdit.Checked = true;
            }
            else if (this.Settings.EnrollViewFields.LastIndexOf("05", StringComparison.Ordinal) > -1)
            {
                this.rblEnApproveView.Checked = true;
            }
            else if (this.Settings.EnrollAnonFields.LastIndexOf("05", StringComparison.Ordinal) > -1)
            {
                this.rblEnApproveAnon.Checked = true;
            }
            else
            {
                this.rblEnApproveNone.Checked = true;
            }

            if (this.Settings.EnrollEditFields.LastIndexOf("06", StringComparison.Ordinal) > -1)
            {
                this.rblEnNoEdit.Checked = true;
            }
            else if (this.Settings.EnrollViewFields.LastIndexOf("06", StringComparison.Ordinal) > -1)
            {
                this.rblEnNoView.Checked = true;
            }
            else if (this.Settings.EnrollAnonFields.LastIndexOf("06", StringComparison.Ordinal) > -1)
            {
                this.rblEnNoAnon.Checked = true;
            }
            else
            {
                this.rblEnNoNone.Checked = true;
            }


            this.chkRSSEnable.Checked = this.Settings.RSSEnable;
            this.ddlRSSDateField.Items.Clear();
            this.ddlRSSDateField.Items.Add(new ListItem(Localization.GetString("UpdatedDate", this.LocalResourceFile),
                                                        "UPDATEDDATE"));
            this.ddlRSSDateField.Items.Add(new ListItem(Localization.GetString("CreationDate", this.LocalResourceFile),
                                                        "CREATIONDATE"));
            this.ddlRSSDateField.Items.Add(new ListItem(Localization.GetString("EventDate", this.LocalResourceFile),
                                                        "EVENTDATE"));
            this.ddlRSSDateField.Items.FindByValue(this.Settings.RSSDateField).Selected = true;
            this.txtRSSDays.Text = this.Settings.RSSDays.ToString();
            this.txtRSSTitle.Text = this.Settings.RSSTitle;
            this.txtRSSDesc.Text = this.Settings.RSSDesc;
            this.txtExpireEvents.Text = this.Settings.Expireevents;
            this.chkExportOwnerEmail.Checked = this.Settings.Exportowneremail;
            this.chkExportAnonOwnerEmail.Checked = this.Settings.Exportanonowneremail;
            this.chkOwnerChangeAllowed.Checked = this.Settings.Ownerchangeallowed;

            this.txtFBAdmins.Text = this.Settings.FBAdmins;
            this.txtFBAppID.Text = this.Settings.FBAppID;

            switch (this.Settings.IconBar)
            {
                case "BOTTOM":
                    this.rblIconBar.Items[1].Selected = true;
                    break;
                case "NONE":
                    this.rblIconBar.Items[2].Selected = true;
                    break;
                default:
                    this.rblIconBar.Items[0].Selected = true;
                    break;
            }

            switch (this.Settings.HTMLEmail)
            {
                case "auto":
                    this.rblHTMLEmail.Items[1].Selected = true;
                    break;
                case "text":
                    this.rblHTMLEmail.Items[2].Selected = true;
                    break;
                default:
                    this.rblHTMLEmail.Items[0].Selected = true;
                    break;
            }


            this.chkEnrollMessageApproved.Checked = this.Settings.SendEnrollMessageApproved;
            this.chkEnrollMessageWaiting.Checked = this.Settings.SendEnrollMessageWaiting;
            this.chkEnrollMessageDenied.Checked = this.Settings.SendEnrollMessageDenied;
            this.chkEnrollMessageAdded.Checked = this.Settings.SendEnrollMessageAdded;
            this.chkEnrollMessageDeleted.Checked = this.Settings.SendEnrollMessageDeleted;
            this.chkEnrollMessagePaying.Checked = this.Settings.SendEnrollMessagePaying;
            this.chkEnrollMessagePending.Checked = this.Settings.SendEnrollMessagePending;
            this.chkEnrollMessagePaid.Checked = this.Settings.SendEnrollMessagePaid;
            this.chkEnrollMessageIncorrect.Checked = this.Settings.SendEnrollMessageIncorrect;
            this.chkEnrollMessageCancelled.Checked = this.Settings.SendEnrollMessageCancelled;

            this.chkAllowAnonEnroll.Checked = this.Settings.AllowAnonEnroll;
            this.BindToEnum(typeof(EventModuleSettings.SocialModule), this.ddlSocialGroupModule);
            this.ddlSocialGroupModule.Items.FindByValue(Convert.ToString((int) this.Settings.SocialGroupModule))
                .Selected = true;
            this.chkSocialUserPrivate.Checked = this.Settings.SocialUserPrivate;
            this.BindToEnum(typeof(EventModuleSettings.SocialGroupPrivacy), this.ddlSocialGroupSecurity);
            this.ddlSocialGroupSecurity.Items.FindByValue(Convert.ToString((int) this.Settings.SocialGroupSecurity))
                .Selected = true;

            this.ddlEnrolListSortDirection.Items.Clear();
            this.ddlEnrolListSortDirection.Items.Add(
                new ListItem(Localization.GetString("Asc", this.LocalResourceFile), "0"));
            this.ddlEnrolListSortDirection.Items.Add(
                new ListItem(Localization.GetString("Desc", this.LocalResourceFile), "1"));
            this.ddlEnrolListSortDirection.Items
                .FindByValue(Convert.ToInt32(this.Settings.EnrolListSortDirection).ToString()).Selected = true;

            this.txtEnrolListDaysBefore.Text = this.Settings.EnrolListDaysBefore.ToString();
            this.txtEnrolListDaysAfter.Text = this.Settings.EnrolListDaysAfter.ToString();

            this.chkJournalIntegration.Checked = this.Settings.JournalIntegration;

            this.LoadCategories();
            this.LoadLocations();

            this.LoadTemplates();
        }

        private void AddJavaScript()
        {
            //Add the external Validation.js to the Page
            const string csname = "ExtValidationScriptFile";
            var cstype = MethodBase.GetCurrentMethod().GetType();
            var cstext = "<script src=\"" + this.ResolveUrl("~/DesktopModules/Events/Scripts/Validation.js") +
                         "\" type=\"text/javascript\"></script>";
            if (!this.Page.ClientScript.IsClientScriptBlockRegistered(csname))
            {
                this.Page.ClientScript.RegisterClientScriptBlock(cstype, csname, cstext, false);
            }


            // Add javascript actions where required and build startup script
            var script = "";
            var cstext2 = "";
            cstext2 += "<script type=\"text/javascript\">";
            cstext2 += "EventSettingsStartupScript = function() {";

            script = "disableactivate('" + this.ddlDefaultView.ClientID + "','" + this.chkMonthAllowed.ClientID +
                     "','" + this.chkWeekAllowed.ClientID + "','" + this.chkListAllowed.ClientID + "');";
            cstext2 += script;
            this.ddlDefaultView.Attributes.Add("onchange", script);

            script = "disableControl('" + this.chkPreventConflicts.ClientID + "',false, '" +
                     this.chkLocationConflict.ClientID + "');";
            cstext2 += script;
            this.chkPreventConflicts.InputAttributes.Add("onclick", script);

            script = "disableControl('" + this.chkMonthCellEvents.ClientID + "',true, '" +
                     this.chkEventDayNewPage.ClientID + "');";
            script += "disableControl('" + this.chkMonthCellEvents.ClientID + "',false, '" +
                      this.chkMonthDaySelect.ClientID + "');";
            script += "disableControl('" + this.chkMonthCellEvents.ClientID + "',false, '" +
                      this.chkTimeInTitle.ClientID + "');";
            script += "disableControl('" + this.chkMonthCellEvents.ClientID + "',false, '" +
                      this.chkEventImageMonth.ClientID + "');";
            script += "disableControl('" + this.chkMonthCellEvents.ClientID + "',false, '" +
                      this.chkIconMonthPrio.ClientID + "');";
            script += "disableControl('" + this.chkMonthCellEvents.ClientID + "',false, '" +
                      this.chkIconMonthRec.ClientID + "');";
            script += "disableControl('" + this.chkMonthCellEvents.ClientID + "',false, '" +
                      this.chkIconMonthReminder.ClientID + "');";
            script += "disableControl('" + this.chkMonthCellEvents.ClientID + "',false, '" +
                      this.chkIconMonthEnroll.ClientID + "');";
            cstext2 += script;
            this.chkMonthCellEvents.InputAttributes.Add("onclick", script);

            script = "disableRbl('" + this.rblListViewGrid.ClientID + "', 'Repeater', '" +
                     this.chkListViewTable.ClientID + "');";
            script += "disableRbl('" + this.rblListViewGrid.ClientID + "', 'Repeater', '" +
                      this.txtRptColumns.ClientID + "');";
            script += "disableRbl('" + this.rblListViewGrid.ClientID + "', 'Repeater', '" + this.txtRptRows.ClientID +
                      "');";
            script += "disableRbl('" + this.rblListViewGrid.ClientID + "', 'Grid', '" + this.rblShowHeader.ClientID +
                      "');";
            script += "disableRbl('" + this.rblListViewGrid.ClientID + "', 'Grid', '" + this.lstAvailable.ClientID +
                      "');";
            script += "disableRbl('" + this.rblListViewGrid.ClientID + "', 'Grid', '" + this.cmdAdd.ClientID + "');";
            script += "disableRbl('" + this.rblListViewGrid.ClientID + "', 'Grid', '" + this.cmdRemove.ClientID + "');";
            script += "disableRbl('" + this.rblListViewGrid.ClientID + "', 'Grid', '" + this.cmdAddAll.ClientID + "');";
            script += "disableRbl('" + this.rblListViewGrid.ClientID + "', 'Grid', '" + this.cmdRemoveAll.ClientID +
                      "');";
            script += "disableRbl('" + this.rblListViewGrid.ClientID + "', 'Grid', '" + this.lstAssigned.ClientID +
                      "');";
            script += "disableRbl('" + this.rblListViewGrid.ClientID + "', 'Grid', '" + this.ddlPageSize.ClientID +
                      "');";
            script += "disableRbl('" + this.rblListViewGrid.ClientID + "', 'Grid', '" +
                      this.chkIconListEnroll.ClientID + "');";
            script += "disableRbl('" + this.rblListViewGrid.ClientID + "', 'Grid', '" + this.chkIconListPrio.ClientID +
                      "');";
            script += "disableRbl('" + this.rblListViewGrid.ClientID + "', 'Grid', '" + this.chkIconListRec.ClientID +
                      "');";
            script += "disableRbl('" + this.rblListViewGrid.ClientID + "', 'Grid', '" +
                      this.chkIconListReminder.ClientID + "');";
            cstext2 += script;
            this.rblListViewGrid.Attributes.Add("onclick", script);

            script = "CheckBoxFalse('" + this.chkIncludeEndValue.ClientID + "', true, '" +
                     this.chkShowValueMarks.ClientID + "');";
            cstext2 += script;
            this.chkIncludeEndValue.InputAttributes.Add("onclick", script);

            script = "disablelistsettings('" + this.rblSelectionTypeDays.ClientID + "',true,'" +
                     this.txtDaysBefore.ClientID + "','" + this.txtDaysAfter.ClientID + "','" +
                     this.txtNumEvents.ClientID + "','" + this.txtEventDays.ClientID + "');";
            cstext2 += script;
            this.rblSelectionTypeDays.Attributes.Add("onclick", script);

            script = "disablelistsettings('" + this.rblSelectionTypeEvents.ClientID + "',false,'" +
                     this.txtDaysBefore.ClientID + "','" + this.txtDaysAfter.ClientID + "','" +
                     this.txtNumEvents.ClientID + "','" + this.txtEventDays.ClientID + "');";
            cstext2 += script;
            this.rblSelectionTypeEvents.Attributes.Add("onclick", script);

            script = "showTbl('" + this.chkEventNotify.ClientID + "','" + this.divEventNotify.ClientID + "');";
            cstext2 += script;
            this.chkEventNotify.InputAttributes.Add("onclick", script);

            script = "showTbl('" + this.chkRSSEnable.ClientID + "','" + this.divRSSEnable.ClientID + "');";
            cstext2 += script;
            this.chkRSSEnable.InputAttributes.Add("onclick", script);

            script = "showTbl('" + this.chkImageEnabled.ClientID + "','" + this.diviCalEventImage.ClientID +
                     "'); showTbl('" + this.chkImageEnabled.ClientID + "','" + this.divImageEnabled.ClientID + "');";
            cstext2 += script;
            this.chkImageEnabled.InputAttributes.Add("onclick", script);

            script = "showTbl('" + this.chkiCalDisplayImage.ClientID + "','" + this.diviCalDisplayImage.ClientID +
                     "');";
            cstext2 += script;
            this.chkiCalDisplayImage.InputAttributes.Add("onclick", script);

            script = "disableControl('" + this.chkExportOwnerEmail.ClientID + "',false, '" +
                     this.chkExportAnonOwnerEmail.ClientID + "');";
            cstext2 += script;
            this.chkExportOwnerEmail.InputAttributes.Add("onclick", script);

            script = "disableDDL('" + this.ddlSocialGroupModule.ClientID + "','" +
                     Convert.ToInt32(EventModuleSettings.SocialModule.UserProfile) + "','" +
                     this.chkSocialUserPrivate.ClientID + "');";
            cstext2 += script;
            this.ddlSocialGroupModule.Attributes.Add("onclick", script);

            if (this.Settings.SocialGroupModule == EventModuleSettings.SocialModule.No)
            {
                script = "disableRbl('" + this.rblNewEventEmail.ClientID + "', 'Role', '" +
                         this.ddNewEventEmailRoles.ClientID + "');";
                cstext2 += script;
                this.rblNewEventEmail.Attributes.Add("onclick", script);
            }

            if (this.Settings.SocialGroupModule != EventModuleSettings.SocialModule.UserProfile)
            {
                script = "showTbl('" + this.chkEventSignup.ClientID + "','" + this.divEventSignup.ClientID + "');";
                cstext2 += script;
                this.chkEventSignup.InputAttributes.Add("onclick", script);

                script = "showTbl('" + this.chkEventSignupAllowPaid.ClientID + "','" +
                         this.divEventSignupAllowPaid.ClientID + "');";
                cstext2 += script;
                this.chkEventSignupAllowPaid.InputAttributes.Add("onclick", script);

                script = "showTbl('" + this.chkEnableSEO.ClientID + "','" + this.divSEOEnable.ClientID + "');";
                cstext2 += script;
                this.chkEnableSEO.InputAttributes.Add("onclick", script);

                script = "showTbl('" + this.chkEnableSitemap.ClientID + "','" + this.divSitemapEnable.ClientID + "');";
                cstext2 += script;
                this.chkEnableSitemap.InputAttributes.Add("onclick", script);
            }

            cstext2 += "};";
            cstext2 += "EventSettingsStartupScript();";
            cstext2 += "Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EventEndRequestHandler);";
            cstext2 += "function EventEndRequestHandler(sender, args) { EventSettingsStartupScript(); }";
            cstext2 += "</script>";

            // Register the startup script
            const string csname2 = "EventSettingsStartupScript";
            var cstype2 = MethodBase.GetCurrentMethod().GetType();
            if (!this.Page.ClientScript.IsStartupScriptRegistered(csname2))
            {
                this.Page.ClientScript.RegisterStartupScript(cstype2, csname2, cstext2, false);
            }
        }

        private void BindToEnum(Type enumType, DropDownList ddl)
        {
            // get the names from the enumeration
            var names = Enum.GetNames(enumType);
            // get the values from the enumeration
            var values = Enum.GetValues(enumType);
            // turn it into a hash table
            ddl.Items.Clear();
            for (var i = 0; i <= names.Length - 1; i++)
            {
                // note the cast to integer here is important
                // otherwise we'll just get the enum string back again
                ddl.Items.Add(new ListItem(Localization.GetString(names[i], this.LocalResourceFile),
                                           Convert.ToString(Convert.ToInt32(values.GetValue(i)))));
            }
            // return the dictionary to be bound to
        }


        private string GetListColumnName(string columnAcronym)
        {
            switch (columnAcronym)
            {
                case "EB":
                    return "01 - " + Localization.GetString("EditButton", this.LocalResourceFile);
                case "BD":
                    return "02 - " + Localization.GetString("BeginDateTime", this.LocalResourceFile);
                case "ED":
                    return "03 - " + Localization.GetString("EndDateTime", this.LocalResourceFile);
                case "EN":
                    return "04 - " + Localization.GetString("EventName", this.LocalResourceFile);
                case "IM":
                    return "05 - " + Localization.GetString("Image", this.LocalResourceFile);
                case "DU":
                    return "06 - " + Localization.GetString("Duration", this.LocalResourceFile);
                case "CA":
                    return "07 - " + Localization.GetString("Category", this.LocalResourceFile);
                case "LO":
                    return "08 - " + Localization.GetString("Location", this.LocalResourceFile);
                case "C1":
                    return "09 - " + Localization.GetString("CustomField1", this.LocalResourceFile);
                case "C2":
                    return "10 - " + Localization.GetString("CustomField2", this.LocalResourceFile);
                case "DE":
                    return "11 - " + Localization.GetString("Description", this.LocalResourceFile);
                case "RT":
                    return "12 - " + Localization.GetString("RecurText", this.LocalResourceFile);
                case "RU":
                    return "13 - " + Localization.GetString("RecurUntil", this.LocalResourceFile);
                default:
                    return "";
            }
        }

        private string GetListColumnAcronym(int columnNo)
        {
            switch (columnNo)
            {
                case 1:
                    return "EB";
                case 2:
                    return "BD";
                case 3:
                    return "ED";
                case 4:
                    return "EN";
                case 5:
                    return "IM";
                case 6:
                    return "DU";
                case 7:
                    return "CA";
                case 8:
                    return "LO";
                case 9:
                    return "C1";
                case 10:
                    return "C2";
                case 11:
                    return "DE";
                case 12:
                    return "RT";
                case 13:
                    return "RU";
                default:
                    return "";
            }
        }

        /// <summary>
        ///     Fill the themelist based on selection for default or custom skins
        /// </summary>
        /// <remarks></remarks>
        private void LoadThemes()
        {
            try
            {
                const string moduleThemesDirectoryPath = "/DesktopModules/Events/Themes";

                //Clear list
                this.ddlThemeStandard.Items.Clear();
                this.ddlThemeCustom.Items.Clear();

                //Add javascript to enable/disable ddl's
                this.rbThemeCustom.Attributes.Add(
                    "onclick",
                    string.Format("{0}.disabled='disabled';{1}.disabled=''", this.ddlThemeStandard.ClientID,
                                  this.ddlThemeCustom.ClientID));
                this.rbThemeStandard.Attributes.Add(
                    "onclick",
                    string.Format("{0}.disabled='disabled';{1}.disabled=''", this.ddlThemeCustom.ClientID,
                                  this.ddlThemeStandard.ClientID));

                //Get the settings
                var themeSettings = new ThemeSetting();
                if (themeSettings.ValidateSetting(this.Settings.EventTheme) == false)
                {
                    themeSettings.ReadSetting(this.Settings.EventThemeDefault, this.PortalId);
                }
                else if (this.Settings.EventTheme != "")
                {
                    themeSettings.ReadSetting(this.Settings.EventTheme, this.PortalId);
                }
                switch (themeSettings.SettingType)
                {
                    case ThemeSetting.ThemeSettingTypeEnum.CustomTheme:
                        this.rbThemeCustom.Checked = true;
                        break;
                    case ThemeSetting.ThemeSettingTypeEnum.DefaultTheme:
                        this.rbThemeStandard.Checked = true;
                        break;
                }

                //Is default or custom selected
                var moduleThemesDirectory = Globals.ApplicationPath + moduleThemesDirectoryPath;
                var serverThemesDirectory = this.Server.MapPath(moduleThemesDirectory);
                var themeDirectories = Directory.GetDirectories(serverThemesDirectory);
                var themeDirectory = "";
                foreach (var tempLoopVar_themeDirectory in themeDirectories)
                {
                    themeDirectory = tempLoopVar_themeDirectory;
                    var dirparts = themeDirectory.Split('\\');
                    this.ddlThemeStandard.Items.Add(
                        new ListItem(dirparts[dirparts.Length - 1], dirparts[dirparts.Length - 1]));
                }
                if (themeSettings.SettingType == ThemeSetting.ThemeSettingTypeEnum.DefaultTheme)
                {
                    if (!ReferenceEquals(this.ddlThemeStandard.Items.FindByText(themeSettings.ThemeName), null))
                    {
                        this.ddlThemeStandard.Items.FindByText(themeSettings.ThemeName).Selected = true;
                    }
                }
                else
                {
                    this.ddlThemeStandard.Attributes.Add("disabled", "disabled");
                }

                //Add custom event theme's
                var pc = new PortalController();
                var with_1 = pc.GetPortal(this.PortalId);
                var eventSkinPath = string.Format("{0}\\DNNEvents\\Themes", with_1.HomeDirectoryMapPath);
                if (!Directory.Exists(eventSkinPath))
                {
                    Directory.CreateDirectory(eventSkinPath);
                }

                foreach (var d in Directory.GetDirectories(eventSkinPath))
                {
                    this.ddlThemeCustom.Items.Add(new ListItem(new DirectoryInfo(d).Name, new DirectoryInfo(d).Name));
                }
                if (this.ddlThemeCustom.Items.Count == 0)
                {
                    this.rbThemeCustom.Enabled = false;
                }

                if (themeSettings.SettingType == ThemeSetting.ThemeSettingTypeEnum.CustomTheme)
                {
                    if (!ReferenceEquals(this.ddlThemeCustom.Items.FindByText(themeSettings.ThemeName), null))
                    {
                        this.ddlThemeCustom.Items.FindByText(themeSettings.ThemeName).Selected = true;
                    }
                }
                else
                {
                    this.ddlThemeCustom.Attributes.Add("disabled", "disabled");
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void LoadTemplates()
        {
            this.ddlTemplates.Items.Clear();

            var t = this.Settings.Templates.GetType();
            var p = default(PropertyInfo);
            foreach (var tempLoopVar_p in t.GetProperties())
            {
                p = tempLoopVar_p;
                this.ddlTemplates.Items.Add(
                    new ListItem(Localization.GetString(p.Name + "Name", this.LocalResourceFile), p.Name));
            }

            this.ddlTemplates.Items.FindByValue("EventDetailsTemplate").Selected = true;
            this.txtEventTemplate.Text = this.Settings.Templates.GetTemplate(this.ddlTemplates.SelectedValue);
            this.lblTemplateUpdated.Visible = false;
        }

        private void LoadNewEventEmailRoles(int roleID)
        {
            var objRoles = new RoleController();
            this.ddNewEventEmailRoles.DataSource = objRoles.GetPortalRoles(this.PortalId);
            this.ddNewEventEmailRoles.DataTextField = "RoleName";
            this.ddNewEventEmailRoles.DataValueField = "RoleID";
            this.ddNewEventEmailRoles.DataBind();
            if (roleID < 0 || ReferenceEquals(this.ddNewEventEmailRoles.Items.FindByValue(Convert.ToString(roleID)),
                                              null))
            {
                try
                {
                    this.ddNewEventEmailRoles.Items.FindByValue(this.PortalSettings.RegisteredRoleId.ToString())
                        .Selected = true;
                }
                catch
                { }
            }
            else
            {
                this.ddNewEventEmailRoles.Items.FindByValue(Convert.ToString(roleID)).Selected = true;
            }
        }

        private void LoadCategories()
        {
            this.ddlModuleCategories.Items.Clear();
            var ctrlEventCategories = new EventCategoryController();
            var lstCategories = ctrlEventCategories.EventsCategoryList(this.PortalId);
            this.ddlModuleCategories.DataSource = lstCategories;
            this.ddlModuleCategories.DataBind();

            if (this.Settings.ModuleCategoriesSelected == EventModuleSettings.CategoriesSelected.Some)
            {
                foreach (string moduleCategory in this.Settings.ModuleCategoryIDs)
                {
                    foreach (RadComboBoxItem item in this.ddlModuleCategories.Items)
                    {
                        if (item.Value == moduleCategory)
                        {
                            item.Checked = true;
                        }
                    }
                }
            }
            else if (this.Settings.ModuleCategoriesSelected == EventModuleSettings.CategoriesSelected.All)
            {
                foreach (RadComboBoxItem item in this.ddlModuleCategories.Items)
                {
                    item.Checked = true;
                }
            }
        }

        private void LoadLocations()
        {
            this.ddlModuleLocations.Items.Clear();
            var ctrlEventLocations = new EventLocationController();
            var lstLocations = ctrlEventLocations.EventsLocationList(this.PortalId);
            this.ddlModuleLocations.DataSource = lstLocations;
            this.ddlModuleLocations.DataBind();

            if (this.Settings.ModuleLocationsSelected == EventModuleSettings.LocationsSelected.Some)
            {
                foreach (string moduleLocation in this.Settings.ModuleLocationIDs)
                {
                    foreach (RadComboBoxItem item in this.ddlModuleLocations.Items)
                    {
                        if (item.Value == moduleLocation)
                        {
                            item.Checked = true;
                        }
                    }
                }
            }
            else if (this.Settings.ModuleLocationsSelected == EventModuleSettings.LocationsSelected.All)
            {
                foreach (RadComboBoxItem item in this.ddlModuleLocations.Items)
                {
                    item.Checked = true;
                }
            }
        }

        /// <summary>
        ///     Take all settings and write them back to the database
        /// </summary>
        /// <remarks></remarks>
        private void UpdateSettings()
        {
            var repository = new EventModuleSettingsRepository();
            var emSettings = repository.GetSettings(this.ModuleConfiguration);

            emSettings.Timeinterval = this.ddlTimeInterval.SelectedValue.Trim();
            emSettings.TimeZoneId = this.cboTimeZone.SelectedValue;
            emSettings.EnableEventTimeZones = this.chkEnableEventTimeZones.Checked;
            emSettings.PrimaryTimeZone =
                (EventModuleSettings.TimeZones) int.Parse(this.ddlPrimaryTimeZone.SelectedValue);

            try
            {
                emSettings.Timeinterval = this.ddlTimeInterval.SelectedValue.Trim();
                emSettings.TimeZoneId = this.cboTimeZone.SelectedValue;
                emSettings.EnableEventTimeZones = this.chkEnableEventTimeZones.Checked;
                emSettings.PrimaryTimeZone =
                    (EventModuleSettings.TimeZones) int.Parse(this.ddlPrimaryTimeZone.SelectedValue);
                emSettings.SecondaryTimeZone =
                    (EventModuleSettings.TimeZones) int.Parse(this.ddlSecondaryTimeZone.SelectedValue);
                emSettings.Eventtooltipmonth = this.chkToolTipMonth.Checked;
                emSettings.Eventtooltipweek = this.chkToolTipWeek.Checked;
                emSettings.Eventtooltipday = this.chkToolTipDay.Checked;
                emSettings.Eventtooltiplist = this.chkToolTipList.Checked;
                emSettings.Eventtooltiplength = int.Parse(this.txtTooltipLength.Text);
                if (this.chkMonthCellEvents.Checked)
                {
                    emSettings.Monthcellnoevents = false;
                }
                else
                {
                    emSettings.Monthcellnoevents = true;
                }
                emSettings.Enablecategories =
                    (EventModuleSettings.DisplayCategories) int.Parse(this.ddlEnableCategories.SelectedValue);
                emSettings.Restrictcategories = this.chkRestrictCategories.Checked;
                emSettings.Enablelocations =
                    (EventModuleSettings.DisplayLocations) int.Parse(this.ddlEnableLocations.SelectedValue);
                emSettings.Restrictlocations = this.chkRestrictLocations.Checked;
                emSettings.Enablecontainerskin = this.chkEnableContainerSkin.Checked;
                emSettings.Eventdetailnewpage = this.chkEventDetailNewPage.Checked;
                emSettings.Enableenrollpopup = this.chkEnableEnrollPopup.Checked;
                emSettings.Eventdaynewpage = this.chkEventDayNewPage.Checked;
                emSettings.EventImageMonth = this.chkEventImageMonth.Checked;
                emSettings.EventImageWeek = this.chkEventImageWeek.Checked;
                emSettings.Eventnotify = this.chkEventNotify.Checked;
                emSettings.DetailPageAllowed = this.chkDetailPageAllowed.Checked;
                emSettings.EnrollmentPageAllowed = this.chkEnrollmentPageAllowed.Checked;
                emSettings.EnrollmentPageDefaultUrl = this.txtEnrollmentPageDefaultURL.Text;
                emSettings.Notifyanon = this.chkNotifyAnon.Checked;
                emSettings.Sendreminderdefault = this.chkSendReminderDefault.Checked;
                emSettings.Neweventemails = this.rblNewEventEmail.SelectedValue;
                emSettings.Neweventemailrole = int.Parse(this.ddNewEventEmailRoles.SelectedValue);
                emSettings.Newpereventemail = this.chkNewPerEventEmail.Checked;
                emSettings.Tzdisplay = this.chkTZDisplay.Checked;
                emSettings.Paypalurl = this.txtPayPalURL.Text;
                if (this.chkEnableEventNav.Checked)
                {
                    emSettings.DisableEventnav = false;
                }
                else
                {
                    emSettings.DisableEventnav = true;
                }
                emSettings.Fulltimescale = this.chkFullTimeScale.Checked;
                emSettings.Collapserecurring = this.chkCollapseRecurring.Checked;
                emSettings.Includeendvalue = this.chkIncludeEndValue.Checked;
                emSettings.Showvaluemarks = this.chkShowValueMarks.Checked;
                emSettings.Eventimage = this.chkImageEnabled.Checked;
                emSettings.MaxThumbHeight = int.Parse(this.txtMaxThumbHeight.Text);
                emSettings.MaxThumbWidth = int.Parse(this.txtMaxThumbWidth.Text);
                emSettings.Allowreoccurring = this.chkAllowRecurring.Checked;
                emSettings.Maxrecurrences = this.txtMaxRecurrences.Text;
                emSettings.Eventsearch = this.chkEnableSearch.Checked;
                emSettings.Addsubmodulename = this.chkAddSubModuleName.Checked;
                emSettings.Enforcesubcalperms = this.chkEnforceSubCalPerms.Checked;
                emSettings.Preventconflicts = this.chkPreventConflicts.Checked;
                emSettings.Locationconflict = this.chkLocationConflict.Checked;
                emSettings.ShowEventsAlways = this.chkShowEventsAlways.Checked;
                emSettings.Timeintitle = this.chkTimeInTitle.Checked;
                emSettings.Monthdayselect = this.chkMonthDaySelect.Checked;
                emSettings.MasterEvent = this.chkMasterEvent.Checked;
                emSettings.Eventsignup = this.chkEventSignup.Checked;
                emSettings.Eventsignupallowpaid = this.chkEventSignupAllowPaid.Checked;
                emSettings.Eventdefaultenrollview = this.chkDefaultEnrollView.Checked;
                emSettings.Eventhidefullenroll = this.chkHideFullEnroll.Checked;
                emSettings.Maxnoenrolees = int.Parse(this.txtMaxNoEnrolees.Text);
                emSettings.Enrolcanceldays = int.Parse(this.txtCancelDays.Text);
                emSettings.Fridayweekend = this.chkFridayWeekend.Checked;
                emSettings.Moderateall = this.chkModerateAll.Checked;
                emSettings.Paypalaccount = this.txtPayPalAccount.Text;
                emSettings.Reminderfrom = this.txtReminderFrom.Text;
                emSettings.StandardEmail = this.txtStandardEmail.Text;
                emSettings.EventsCustomField1 = this.chkCustomField1.Checked;
                emSettings.EventsCustomField2 = this.chkCustomField2.Checked;
                emSettings.DefaultView = this.ddlDefaultView.SelectedItem.Value;
                emSettings.EventsListPageSize = int.Parse(this.ddlPageSize.SelectedItem.Value);
                emSettings.EventsListSortDirection = this.ddlListSortedFieldDirection.SelectedItem.Value;
                emSettings.EventsListSortColumn = this.ddlListDefaultColumn.SelectedItem.Value;
                emSettings.RSSEnable = this.chkRSSEnable.Checked;
                emSettings.RSSDateField = this.ddlRSSDateField.SelectedItem.Value;
                emSettings.RSSDays = int.Parse(this.txtRSSDays.Text);
                emSettings.RSSTitle = this.txtRSSTitle.Text;
                emSettings.RSSDesc = this.txtRSSDesc.Text;
                emSettings.Expireevents = this.txtExpireEvents.Text;
                emSettings.Exportowneremail = this.chkExportOwnerEmail.Checked;
                emSettings.Exportanonowneremail = this.chkExportAnonOwnerEmail.Checked;
                emSettings.Ownerchangeallowed = this.chkOwnerChangeAllowed.Checked;
                emSettings.IconMonthPrio = this.chkIconMonthPrio.Checked;
                emSettings.IconMonthRec = this.chkIconMonthRec.Checked;
                emSettings.IconMonthReminder = this.chkIconMonthReminder.Checked;
                emSettings.IconMonthEnroll = this.chkIconMonthEnroll.Checked;
                emSettings.IconWeekPrio = this.chkIconWeekPrio.Checked;
                emSettings.IconWeekRec = this.chkIconWeekRec.Checked;
                emSettings.IconWeekReminder = this.chkIconWeekReminder.Checked;
                emSettings.IconWeekEnroll = this.chkIconWeekEnroll.Checked;
                emSettings.IconListPrio = this.chkIconListPrio.Checked;
                emSettings.IconListRec = this.chkIconListRec.Checked;
                emSettings.IconListReminder = this.chkIconListReminder.Checked;
                emSettings.IconListEnroll = this.chkIconListEnroll.Checked;
                emSettings.PrivateMessage = this.txtPrivateMessage.Text.Trim();
                emSettings.EnableSEO = this.chkEnableSEO.Checked;
                emSettings.SEODescriptionLength = int.Parse(this.txtSEODescriptionLength.Text);
                emSettings.EnableSitemap = this.chkEnableSitemap.Checked;
                emSettings.SiteMapPriority = Convert.ToSingle(Convert.ToSingle(this.txtSitemapPriority.Text));
                emSettings.SiteMapDaysBefore = int.Parse(this.txtSitemapDaysBefore.Text);
                emSettings.SiteMapDaysAfter = int.Parse(this.txtSitemapDaysAfter.Text);
                emSettings.WeekStart = (FirstDayOfWeek) int.Parse(this.ddlWeekStart.SelectedValue);
                emSettings.ListViewUseTime = this.chkListViewUseTime.Checked;

                emSettings.IcalOnIconBar = this.chkiCalOnIconBar.Checked;
                emSettings.IcalEmailEnable = this.chkiCalEmailEnable.Checked;
                emSettings.IcalURLInLocation = this.chkiCalURLinLocation.Checked;
                emSettings.IcalIncludeCalname = this.chkiCalIncludeCalname.Checked;
                emSettings.IcalDaysBefore = int.Parse(this.txtiCalDaysBefore.Text);
                emSettings.IcalDaysAfter = int.Parse(this.txtiCalDaysAfter.Text);
                emSettings.IcalURLAppend = this.txtiCalURLAppend.Text;
                if (this.chkiCalDisplayImage.Checked)
                {
                    emSettings.IcalDefaultImage = "Image=" + this.ctliCalDefaultImage.Url;
                }
                else
                {
                    emSettings.IcalDefaultImage = "";
                }
                //objModules.UpdateModuleSetting(ModuleId, "EventDetailsTemplate", txtEventDetailsTemplate.Text.Trim)

                var moduleCategories = new ArrayList();
                if (this.ddlModuleCategories.CheckedItems.Count != this.ddlModuleCategories.Items.Count)
                {
                    foreach (var item in this.ddlModuleCategories.CheckedItems)
                    {
                        moduleCategories.Add(item.Value);
                    }
                }
                else
                {
                    moduleCategories.Add("-1");
                }
                emSettings.ModuleCategoryIDs = moduleCategories;

                var moduleLocations = new ArrayList();
                if (this.ddlModuleLocations.CheckedItems.Count != this.ddlModuleLocations.Items.Count)
                {
                    foreach (var item in this.ddlModuleLocations.CheckedItems)
                    {
                        moduleLocations.Add(item.Value);
                    }
                }
                else
                {
                    moduleLocations.Add("-1");
                }
                emSettings.ModuleLocationIDs = moduleLocations;

                // ReSharper disable LocalizableElement
                if (this.chkMonthAllowed.Checked || this.ddlDefaultView.SelectedItem.Value == "EventMonth.ascx")
                {
                    emSettings.MonthAllowed = true;
                }
                else
                {
                    emSettings.MonthAllowed = false;
                }
                if (this.chkWeekAllowed.Checked || this.ddlDefaultView.SelectedItem.Value == "EventWeek.ascx")
                {
                    emSettings.WeekAllowed = true;
                }
                else
                {
                    emSettings.WeekAllowed = false;
                }
                if (this.chkListAllowed.Checked || this.ddlDefaultView.SelectedItem.Value == "EventList.ascx")
                {
                    emSettings.ListAllowed = true;
                }
                else
                {
                    emSettings.ListAllowed = false;
                }
                // ReSharper restore LocalizableElement

                switch (this.rblIconBar.SelectedIndex)
                {
                    case 0:
                        emSettings.IconBar = "TOP";
                        break;
                    case 1:
                        emSettings.IconBar = "BOTTOM";
                        break;
                    case 2:
                        emSettings.IconBar = "NONE";
                        break;
                }

                switch (this.rblHTMLEmail.SelectedIndex)
                {
                    case 0:
                        emSettings.HTMLEmail = "html";
                        break;
                    case 1:
                        emSettings.HTMLEmail = "auto";
                        break;
                    case 2:
                        emSettings.HTMLEmail = "text";
                        break;
                }

                //EPT: Be sure we start next display time in the correct view
                // Update the cookie so the appropriate view is shown when settings page is exited

                var objCookie = new HttpCookie("DNNEvents" + Convert.ToString(this.ModuleId));
                objCookie.Value = this.ddlDefaultView.SelectedItem.Value;
                if (ReferenceEquals(this.Request.Cookies.Get("DNNEvents" + Convert.ToString(this.ModuleId)), null))
                {
                    this.Response.Cookies.Add(objCookie);
                }
                else
                {
                    this.Response.Cookies.Set(objCookie);
                }

                //Set eventtheme data
                var themeSettings = new ThemeSetting();
                if (this.rbThemeStandard.Checked)
                {
                    themeSettings.SettingType = ThemeSetting.ThemeSettingTypeEnum.DefaultTheme;
                    themeSettings.ThemeName = this.ddlThemeStandard.SelectedItem.Text;
                    themeSettings.ThemeFile = "";
                }
                else if (this.rbThemeCustom.Checked)
                {
                    themeSettings.SettingType = ThemeSetting.ThemeSettingTypeEnum.CustomTheme;
                    themeSettings.ThemeName = this.ddlThemeCustom.SelectedItem.Text;
                    themeSettings.ThemeFile = "";
                }
                emSettings.EventTheme = themeSettings.ToString();

                //List Events Mode Stuff
                //Update Fields to Display
                var objListItem = default(ListItem);
                var listFields = "";
                foreach (ListItem tempLoopVar_objListItem in this.lstAssigned.Items)
                {
                    objListItem = tempLoopVar_objListItem;
                    var columnNo = int.Parse(objListItem.Text.Substring(0, 2));
                    var columnAcronym = this.GetListColumnAcronym(columnNo);
                    if (listFields.Length > 0)
                    {
                        listFields = listFields + ";" + columnAcronym;
                    }
                    else
                    {
                        listFields = columnAcronym;
                    }
                }
                emSettings.EventsListFields = listFields;

                listFields = this.EnrollListFields(this.rblEnUserAnon.Checked, this.rblEnDispAnon.Checked,
                                                   this.rblEnEmailAnon.Checked, this.rblEnPhoneAnon.Checked,
                                                   this.rblEnApproveAnon.Checked, this.rblEnNoAnon.Checked);
                emSettings.EnrollAnonFields = listFields;

                listFields = this.EnrollListFields(this.rblEnUserView.Checked, this.rblEnDispView.Checked,
                                                   this.rblEnEmailView.Checked, this.rblEnPhoneView.Checked,
                                                   this.rblEnApproveView.Checked, this.rblEnNoView.Checked);
                emSettings.EnrollViewFields = listFields;

                listFields = this.EnrollListFields(this.rblEnUserEdit.Checked, this.rblEnDispEdit.Checked,
                                                   this.rblEnEmailEdit.Checked, this.rblEnPhoneEdit.Checked,
                                                   this.rblEnApproveEdit.Checked, this.rblEnNoEdit.Checked);
                emSettings.EnrollEditFields = listFields;

                if (this.rblSelectionTypeDays.Checked)
                {
                    emSettings.EventsListSelectType = this.rblSelectionTypeDays.Value;
                }
                else
                {
                    emSettings.EventsListSelectType = this.rblSelectionTypeEvents.Value;
                }
                if (this.rblListViewGrid.Items[0].Selected)
                {
                    emSettings.ListViewGrid = true;
                }
                else
                {
                    emSettings.ListViewGrid = false;
                }
                emSettings.ListViewTable = this.chkListViewTable.Checked;
                emSettings.RptColumns = int.Parse(this.txtRptColumns.Text.Trim());
                emSettings.RptRows = int.Parse(this.txtRptRows.Text.Trim());

                if (this.rblShowHeader.Items[0].Selected)
                {
                    emSettings.EventsListShowHeader = this.rblShowHeader.Items[0].Value;
                }
                else
                {
                    emSettings.EventsListShowHeader = this.rblShowHeader.Items[1].Value;
                }
                emSettings.EventsListBeforeDays = int.Parse(this.txtDaysBefore.Text.Trim());
                emSettings.EventsListAfterDays = int.Parse(this.txtDaysAfter.Text.Trim());
                emSettings.EventsListNumEvents = int.Parse(this.txtNumEvents.Text.Trim());
                emSettings.EventsListEventDays = int.Parse(this.txtEventDays.Text.Trim());
                emSettings.RestrictCategoriesToTimeFrame = this.chkRestrictCategoriesToTimeFrame.Checked;
                emSettings.RestrictLocationsToTimeFrame = this.chkRestrictLocationsToTimeFrame.Checked;

                emSettings.FBAdmins = this.txtFBAdmins.Text;
                emSettings.FBAppID = this.txtFBAppID.Text;

                emSettings.SendEnrollMessageApproved = this.chkEnrollMessageApproved.Checked;
                emSettings.SendEnrollMessageWaiting = this.chkEnrollMessageWaiting.Checked;
                emSettings.SendEnrollMessageDenied = this.chkEnrollMessageDenied.Checked;
                emSettings.SendEnrollMessageAdded = this.chkEnrollMessageAdded.Checked;
                emSettings.SendEnrollMessageDeleted = this.chkEnrollMessageDeleted.Checked;
                emSettings.SendEnrollMessagePaying = this.chkEnrollMessagePaying.Checked;
                emSettings.SendEnrollMessagePending = this.chkEnrollMessagePending.Checked;
                emSettings.SendEnrollMessagePaid = this.chkEnrollMessagePaid.Checked;
                emSettings.SendEnrollMessageIncorrect = this.chkEnrollMessageIncorrect.Checked;
                emSettings.SendEnrollMessageCancelled = this.chkEnrollMessageCancelled.Checked;

                emSettings.AllowAnonEnroll = this.chkAllowAnonEnroll.Checked;
                emSettings.SocialGroupModule =
                    (EventModuleSettings.SocialModule) int.Parse(this.ddlSocialGroupModule.SelectedValue);
                if (emSettings.SocialGroupModule != EventModuleSettings.SocialModule.No)
                {
                    emSettings.Ownerchangeallowed = false;
                    emSettings.Neweventemails = "Never";
                    emSettings.MasterEvent = false;
                }
                if (emSettings.SocialGroupModule == EventModuleSettings.SocialModule.UserProfile)
                {
                    emSettings.EnableSitemap = false;
                    emSettings.Eventsearch = false;
                    emSettings.Eventsignup = false;
                    emSettings.Moderateall = false;
                }
                emSettings.SocialUserPrivate = this.chkSocialUserPrivate.Checked;
                emSettings.SocialGroupSecurity =
                    (EventModuleSettings.SocialGroupPrivacy) int.Parse(this.ddlSocialGroupSecurity.SelectedValue);

                emSettings.EnrolListSortDirection =
                    (SortDirection) int.Parse(this.ddlEnrolListSortDirection.SelectedValue);
                emSettings.EnrolListDaysBefore = int.Parse(this.txtEnrolListDaysBefore.Text);
                emSettings.EnrolListDaysAfter = int.Parse(this.txtEnrolListDaysAfter.Text);

                emSettings.JournalIntegration = this.chkJournalIntegration.Checked;

                var objDesktopModule = DesktopModuleController.GetDesktopModuleByModuleName("DNN_Events", 0);

                emSettings.Version = objDesktopModule.Version;

                repository.SaveSettings(this.ModuleConfiguration, emSettings);

                this.CreateThemeDirectory();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        ///     Get Assigned Sub Events and Bind to Grid
        /// </summary>
        /// <remarks></remarks>
        private void BindSubEvents()
        {
            this.lstAssignedCals.DataTextField = "SubEventTitle";
            this.lstAssignedCals.DataValueField = "MasterID";
            this.lstAssignedCals.DataSource = null;
            this.lstAssignedCals.DataBind();
            this.lstAssignedCals.DataSource = this._objCtlMasterEvent.EventsMasterAssignedModules(this.ModuleId);
            this.lstAssignedCals.DataBind();
        }

        /// <summary>
        ///     Get Avaiable Sub Events for Portal and Bind to DropDown
        /// </summary>
        /// <remarks></remarks>
        private void BindAvailableEvents()
        {
            this.lstAvailableCals.DataTextField = "SubEventTitle";
            this.lstAvailableCals.DataValueField = "SubEventID";
            this.lstAvailableCals.DataSource = null;
            this.lstAvailableCals.DataBind();
            this.lstAvailableCals.DataSource =
                this._objCtlMasterEvent.EventsMasterAvailableModules(this.PortalId, this.ModuleId);
            this.lstAvailableCals.DataBind();
        }

        private void Enable_Disable_Cals()
        {
            this.divMasterEvent.Visible = this.chkMasterEvent.Checked;
        }

        private string EnrollListFields(bool blUser, bool blDisp, bool blEmail, bool blPhone, bool blApprove, bool blNo)
        {
            var listFields = "";
            if (blUser)
            {
                listFields = listFields + "01;";
            }
            if (blDisp)
            {
                listFields = listFields + "02;";
            }
            if (blEmail)
            {
                listFields = listFields + "03;";
            }
            if (blPhone)
            {
                listFields = listFields + "04;";
            }
            if (blApprove)
            {
                listFields = listFields + "05;";
            }
            if (blNo)
            {
                listFields = listFields + "06;";
            }
            return listFields;
        }

        public string AddSkinContainerControls(string url, string addchar)
        {
            var objCtlTab = new TabController();
            var objTabInfo = objCtlTab.GetTab(this.TabId, this.PortalId, false);
            string skinSrc = null;
            if (!(objTabInfo.SkinSrc == ""))
            {
                skinSrc = objTabInfo.SkinSrc;
                if (skinSrc.Substring(skinSrc.Length - 5, 5) == ".ascx")
                {
                    skinSrc = skinSrc.Substring(0, skinSrc.Length - 5);
                }
            }
            var objCtlModule = new ModuleController();
            var objModuleInfo = objCtlModule.GetModule(this.ModuleId, this.TabId, false);
            string containerSrc = null;
            if (objModuleInfo.DisplayTitle)
            {
                if (!(objModuleInfo.ContainerSrc == ""))
                {
                    containerSrc = objModuleInfo.ContainerSrc;
                }
                else if (!(objTabInfo.ContainerSrc == ""))
                {
                    containerSrc = objTabInfo.ContainerSrc;
                }
                if (!ReferenceEquals(containerSrc, null))
                {
                    if (containerSrc.Substring(containerSrc.Length - 5, 5) == ".ascx")
                    {
                        containerSrc = containerSrc.Substring(0, containerSrc.Length - 5);
                    }
                }
            }
            else
            {
                containerSrc = "[G]Containers/_default/No+Container";
            }
            if (!ReferenceEquals(containerSrc, null))
            {
                url += addchar + "ContainerSrc=" + HttpUtility.HtmlEncode(containerSrc);
                addchar = "&";
            }
            if (!ReferenceEquals(skinSrc, null))
            {
                url += addchar + "SkinSrc=" + HttpUtility.HtmlEncode(skinSrc);
            }
            return url;
        }

        #endregion

        #region Links, Buttons and Events

        protected void chkMasterEvent_CheckedChanged(object sender, EventArgs e)
        {
            this.cmdRemoveAllCals_Click(sender, e);
            this.Enable_Disable_Cals();
        }

        protected void cmdAdd_Click(object sender, EventArgs e)
        {
            var objListItem = default(ListItem);

            var objList = new ArrayList();

            foreach (ListItem tempLoopVar_objListItem in this.lstAvailable.Items)
            {
                objListItem = tempLoopVar_objListItem;
                objList.Add(objListItem);
            }

            foreach (ListItem tempLoopVar_objListItem in objList)
            {
                objListItem = tempLoopVar_objListItem;
                if (objListItem.Selected)
                {
                    this.lstAvailable.Items.Remove(objListItem);
                    this.lstAssigned.Items.Add(objListItem);
                }
            }

            this.lstAvailable.ClearSelection();
            this.lstAssigned.ClearSelection();

            this.Sort(this.lstAssigned);
        }

        protected void cmdRemove_Click(object sender, EventArgs e)
        {
            var objListItem = default(ListItem);

            var objList = new ArrayList();

            foreach (ListItem tempLoopVar_objListItem in this.lstAssigned.Items)
            {
                objListItem = tempLoopVar_objListItem;
                objList.Add(objListItem);
            }

            foreach (ListItem tempLoopVar_objListItem in objList)
            {
                objListItem = tempLoopVar_objListItem;
                if (objListItem.Selected)
                {
                    this.lstAssigned.Items.Remove(objListItem);
                    this.lstAvailable.Items.Add(objListItem);
                }
            }

            this.lstAvailable.ClearSelection();
            this.lstAssigned.ClearSelection();

            this.Sort(this.lstAvailable);
        }

        protected void cmdAddAll_Click(object sender, EventArgs e)
        {
            var objListItem = default(ListItem);

            foreach (ListItem tempLoopVar_objListItem in this.lstAvailable.Items)
            {
                objListItem = tempLoopVar_objListItem;
                this.lstAssigned.Items.Add(objListItem);
            }

            this.lstAvailable.Items.Clear();

            this.lstAvailable.ClearSelection();
            this.lstAssigned.ClearSelection();

            this.Sort(this.lstAssigned);
        }

        protected void cmdRemoveAll_Click(object sender, EventArgs e)
        {
            var objListItem = default(ListItem);

            foreach (ListItem tempLoopVar_objListItem in this.lstAssigned.Items)
            {
                objListItem = tempLoopVar_objListItem;
                this.lstAvailable.Items.Add(objListItem);
            }

            this.lstAssigned.Items.Clear();

            this.lstAvailable.ClearSelection();
            this.lstAssigned.ClearSelection();

            this.Sort(this.lstAvailable);
        }

        protected void Sort(ListBox ctlListBox)
        {
            var arrListItems = new ArrayList();
            var objListItem = default(ListItem);

            // store listitems in temp arraylist
            foreach (ListItem tempLoopVar_objListItem in ctlListBox.Items)
            {
                objListItem = tempLoopVar_objListItem;
                arrListItems.Add(objListItem);
            }

            // sort arraylist based on text value
            arrListItems.Sort(new ListItemComparer());

            // clear control
            ctlListBox.Items.Clear();

            // add listitems to control
            foreach (ListItem tempLoopVar_objListItem in arrListItems)
            {
                objListItem = tempLoopVar_objListItem;
                ctlListBox.Items.Add(objListItem);
            }
        }

        protected void cmdAddCals_Click(object sender, EventArgs e)
        {
            var objListItem = default(ListItem);
            var masterEvent = new EventMasterInfo();

            foreach (ListItem tempLoopVar_objListItem in this.lstAvailableCals.Items)
            {
                objListItem = tempLoopVar_objListItem;
                if (objListItem.Selected)
                {
                    masterEvent.MasterID = 0;
                    masterEvent.ModuleID = this.ModuleId;
                    masterEvent.SubEventID = Convert.ToInt32(objListItem.Value);
                    this._objCtlMasterEvent.EventsMasterSave(masterEvent);
                }
            }

            this.BindSubEvents();
            this.BindAvailableEvents();
        }

        protected void cmdAddAllCals_Click(object sender, EventArgs e)
        {
            var objListItem = default(ListItem);
            var masterEvent = new EventMasterInfo();

            foreach (ListItem tempLoopVar_objListItem in this.lstAvailableCals.Items)
            {
                objListItem = tempLoopVar_objListItem;
                masterEvent.MasterID = 0;
                masterEvent.ModuleID = this.ModuleId;
                masterEvent.SubEventID = Convert.ToInt32(objListItem.Value);
                this._objCtlMasterEvent.EventsMasterSave(masterEvent);
            }

            this.BindSubEvents();
            this.BindAvailableEvents();
        }

        protected void cmdRemoveCals_Click(object sender, EventArgs e)
        {
            var objListItem = default(ListItem);

            foreach (ListItem tempLoopVar_objListItem in this.lstAssignedCals.Items)
            {
                objListItem = tempLoopVar_objListItem;
                if (objListItem.Selected)
                {
                    this._objCtlMasterEvent.EventsMasterDelete(int.Parse(objListItem.Value), this.ModuleId);
                }
            }

            this.BindSubEvents();
            this.BindAvailableEvents();
        }

        protected void cmdRemoveAllCals_Click(object sender, EventArgs e)
        {
            var objListItem = default(ListItem);

            foreach (ListItem tempLoopVar_objListItem in this.lstAssignedCals.Items)
            {
                objListItem = tempLoopVar_objListItem;
                this._objCtlMasterEvent.EventsMasterDelete(int.Parse(objListItem.Value), this.ModuleId);
            }

            this.BindSubEvents();
            this.BindAvailableEvents();
        }

        protected void cmdUpdateTemplate_Click(object sender, EventArgs e)
        {
            var strTemplate = this.ddlTemplates.SelectedValue;
            this.Settings.Templates.SaveTemplate(this.ModuleId, strTemplate, this.txtEventTemplate.Text.Trim());
            this.lblTemplateUpdated.Visible = true;
            this.lblTemplateUpdated.Text =
                string.Format(Localization.GetString("TemplateUpdated", this.LocalResourceFile),
                              Localization.GetString(strTemplate + "Name", this.LocalResourceFile));
        }

        protected void cmdResetTemplate_Click(object sender, EventArgs e)
        {
            var strTemplate = this.ddlTemplates.SelectedValue;
            this.Settings.Templates.ResetTemplate(this.ModuleId, strTemplate, this.LocalResourceFile);
            this.txtEventTemplate.Text = this.Settings.Templates.GetTemplate(strTemplate);
            this.lblTemplateUpdated.Visible = true;
            this.lblTemplateUpdated.Text =
                string.Format(Localization.GetString("TemplateReset", this.LocalResourceFile),
                              Localization.GetString(strTemplate + "Name", this.LocalResourceFile));
        }

        protected void ddlTemplates_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.txtEventTemplate.Text = this.Settings.Templates.GetTemplate(this.ddlTemplates.SelectedValue);
            this.lblTemplateUpdated.Visible = false;
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

        protected void updateButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.UpdateSettings();
                this.Response.Redirect(this.GetSocialNavigateUrl(), true);
            }
            catch (Exception) //Module failed to load
            {
                //ProcessModuleLoadException(Me, exc)
            }
        }

        #endregion
    }

    #region Comparer Class

    public class ListItemComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            var a = (ListItem) x;
            var b = (ListItem) y;
            var c = new CaseInsensitiveComparer();
            return c.Compare(a.Text, b.Text);
        }
    }

    #endregion

    #region DataClasses

    public class ThemeSetting
    {
        #region Enumerators

        public enum ThemeSettingTypeEnum
        {
            DefaultTheme = 0,
            CustomTheme = 1
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", Convert.ToInt32(this.SettingType), this.ThemeName, this.ThemeFile);
        }

        #endregion

        #region Member Variables

        public ThemeSettingTypeEnum SettingType;
        public string ThemeName;
        public string ThemeFile;
        public string CssClass;

        #endregion

        #region Methods

        public bool ValidateSetting(string setting)
        {
            var s = setting.Split(',');
            if (!(s.GetUpperBound(0) == 2))
            {
                return false;
            }
            if (!Information.IsNumeric(s[0]))
            {
                return false;
            }
            return true;
        }

        public void ReadSetting(string setting, int portalID)
        {
            if (!this.ValidateSetting(setting))
            {
                throw new Exception("Setting is not right format to convert into ThemeSetting");
            }

            var s = setting.Split(',');
            this.SettingType = (ThemeSettingTypeEnum) int.Parse(s[0]);
            this.ThemeName = s[1];
            this.CssClass = "Theme" + this.ThemeName;
            switch (this.SettingType)
            {
                case ThemeSettingTypeEnum.DefaultTheme:
                    this.ThemeFile = string.Format("{0}/DesktopModules/Events/Themes/{1}/{1}.css",
                                                   Globals.ApplicationPath, this.ThemeName);
                    break;
                case ThemeSettingTypeEnum.CustomTheme:
                    var pc = new PortalController();
                    var with_1 = pc.GetPortal(portalID);
                    this.ThemeFile = string.Format("{0}/{1}/DNNEvents/Themes/{2}/{2}.css", Globals.ApplicationPath,
                                                   with_1.HomeDirectory, this.ThemeName);
                    break;
            }
        }

        #endregion
    }

    #endregion
}