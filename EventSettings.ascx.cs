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


using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.UI.WebControls;
using Components;
using DNNtc;
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
using Microsoft.VisualBasic;
using Telerik.Web.UI;
using FirstDayOfWeek = System.Web.UI.WebControls.FirstDayOfWeek;
using Globals = DotNetNuke.Common.Globals;

namespace DotNetNuke.Modules.Events
{
    [DNNtc.ModuleControlProperties("EventSettings", "Event Settings", DNNtc.ControlType.View, "https://github.com/DNNCommunity/DNN.Events/wiki", true, true)]
    public partial class EventSettings : EventBase
    {
        #region Private Data

        private readonly EventMasterController _objCtlMasterEvent = new EventMasterController();

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            // Add the click event
            cmdUpdateTemplate.Click += cmdUpdateTemplate_Click;
            cmdResetTemplate.Click += cmdResetTemplate_Click;
        }

        #region  Web Form Designer Generated Code

        //This call is required by the Web Form Designer.
        [DebuggerStepThrough]
        private void InitializeComponent()
        { }

        private void Page_Init(object sender, EventArgs e)
        {
            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            InitializeComponent();
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
            if (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName) || IsSettingsEditor())
            { }
            else
            {
                Response.Redirect(GetSocialNavigateUrl(), true);
            }

            // Set the selected theme
            SetTheme(pnlEventsModuleSettings);

            // Do we have to load the settings
            if (!Page.IsPostBack)
            {
                LoadSettings();
            }

            // Add the javascript to the page
            AddJavaScript();
        }

        private void LoadSettings()
        {
            var availableFields = new ArrayList();
            var selectedFields = new ArrayList();

            // Create Lists and Schedule - they should always exist
            var objEventController = new EventController();
            objEventController.CreateListsAndSchedule();

            //Set text and tooltip from resourcefile
            chkMonthAllowed.Text = Localization.GetString("Month", LocalResourceFile);
            chkWeekAllowed.Text = Localization.GetString("Week", LocalResourceFile);
            chkListAllowed.Text = Localization.GetString("List", LocalResourceFile);
            cmdAdd.ToolTip = Localization.GetString("Add", LocalResourceFile);
            cmdAddAll.ToolTip = Localization.GetString("AddAll", LocalResourceFile);
            cmdRemove.ToolTip = Localization.GetString("Remove", LocalResourceFile);
            cmdRemoveAll.ToolTip = Localization.GetString("RemoveAll", LocalResourceFile);
            cmdAddCals.ToolTip = Localization.GetString("AddCals", LocalResourceFile);
            cmdAddAllCals.ToolTip = Localization.GetString("AddAllCals", LocalResourceFile);
            cmdRemoveCals.ToolTip = Localization.GetString("RemoveCals", LocalResourceFile);
            cmdRemoveAllCals.ToolTip = Localization.GetString("RemoveAllCals", LocalResourceFile);
            chkIconMonthPrio.Text = Localization.GetString("Priority", LocalResourceFile);
            imgIconMonthPrioHigh.AlternateText = Localization.GetString("HighPrio", LocalResourceFile);
            imgIconMonthPrioLow.AlternateText = Localization.GetString("LowPrio", LocalResourceFile);
            chkIconMonthRec.Text = Localization.GetString("Recurring", LocalResourceFile);
            imgIconMonthRec.AlternateText = Localization.GetString("RecurringEvent", LocalResourceFile);
            chkIconMonthReminder.Text = Localization.GetString("Reminder", LocalResourceFile);
            imgIconMonthReminder.AlternateText = Localization.GetString("ReminderEnabled", LocalResourceFile);
            chkIconMonthEnroll.Text = Localization.GetString("Enroll", LocalResourceFile);
            imgIconMonthEnroll.AlternateText = Localization.GetString("EnrollEnabled", LocalResourceFile);
            chkIconWeekPrio.Text = Localization.GetString("Priority", LocalResourceFile);
            imgIconWEEKPrioHigh.AlternateText = Localization.GetString("HighPrio", LocalResourceFile);
            imgIconWeekPrioLow.AlternateText = Localization.GetString("LowPrio", LocalResourceFile);
            chkIconWeekRec.Text = Localization.GetString("Recurring", LocalResourceFile);
            imgIconWeekRec.AlternateText = Localization.GetString("RecurringEvent", LocalResourceFile);
            chkIconWeekReminder.Text = Localization.GetString("Reminder", LocalResourceFile);
            imgIconWeekReminder.AlternateText = Localization.GetString("ReminderEnabled", LocalResourceFile);
            chkIconWeekEnroll.Text = Localization.GetString("Enroll", LocalResourceFile);
            imgIconWeekEnroll.AlternateText = Localization.GetString("EnrollEnabled", LocalResourceFile);
            chkIconListPrio.Text = Localization.GetString("Priority", LocalResourceFile);
            imgIconListPrioHigh.AlternateText = Localization.GetString("HighPrio", LocalResourceFile);
            imgIconListPrioLow.AlternateText = Localization.GetString("LowPrio", LocalResourceFile);
            chkIconListRec.Text = Localization.GetString("Recurring", LocalResourceFile);
            imgIconListRec.AlternateText = Localization.GetString("RecurringEvent", LocalResourceFile);
            chkIconListReminder.Text = Localization.GetString("Reminder", LocalResourceFile);
            imgIconListReminder.AlternateText = Localization.GetString("ReminderEnabled", LocalResourceFile);
            chkIconListEnroll.Text = Localization.GetString("Enroll", LocalResourceFile);
            imgIconListEnroll.AlternateText = Localization.GetString("EnrollEnabled", LocalResourceFile);
            cmdUpdateTemplate.Text = Localization.GetString("cmdUpdateTemplate", LocalResourceFile);
            rbThemeStandard.Text = Localization.GetString("rbThemeStandard", LocalResourceFile);
            rbThemeCustom.Text = Localization.GetString("rbThemeCustom", LocalResourceFile);
            ddlModuleCategories.EmptyMessage = Localization.GetString("NoCategories", LocalResourceFile);
            ddlModuleCategories.Localization.AllItemsCheckedString =
                Localization.GetString("AllCategories", LocalResourceFile);
            ddlModuleCategories.Localization.CheckAllString =
                Localization.GetString("SelectAllCategories", LocalResourceFile);
            ddlModuleLocations.EmptyMessage = Localization.GetString("NoLocations", LocalResourceFile);
            ddlModuleLocations.Localization.AllItemsCheckedString =
                Localization.GetString("AllLocations", LocalResourceFile);
            ddlModuleLocations.Localization.CheckAllString =
                Localization.GetString("SelectAllLocations", LocalResourceFile);


            //Add templates link
            // lnkTemplatesHelp.HRef = AddSkinContainerControls(EditUrl("", "", "TemplateHelp", "dnnprintmode=true"), "?")
            lnkTemplatesHelp.HRef =
                AddSkinContainerControls(
                    Globals.NavigateURL(TabId, PortalSettings, "", "mid=" + Convert.ToString(ModuleId),
                                        "ctl=TemplateHelp", "ShowNav=False", "dnnprintmode=true"), "?");
            lnkTemplatesHelp.InnerText = Localization.GetString("TemplatesHelp", LocalResourceFile);

            //Support for Time Interval Dropdown
            var ctlLists = new ListController();
            var colThreadStatus = ctlLists.GetListEntryInfoItems("Timeinterval");
            ddlTimeInterval.Items.Clear();

            foreach (var entry in colThreadStatus)
            {
                ddlTimeInterval.Items.Add(entry.Value);
            }
            ddlTimeInterval.Items.FindByValue(Settings.Timeinterval).Selected = true;

            // Set Dropdown TimeZone
            cboTimeZone.DataBind(Settings.TimeZoneId);

            chkEnableEventTimeZones.Checked = Settings.EnableEventTimeZones;

            BindToEnum(typeof(EventModuleSettings.TimeZones), ddlPrimaryTimeZone);
            ddlPrimaryTimeZone.Items.FindByValue(Convert.ToString((int) Settings.PrimaryTimeZone)).Selected =
                true;
            BindToEnum(typeof(EventModuleSettings.TimeZones), ddlSecondaryTimeZone);
            ddlSecondaryTimeZone.Items.FindByValue(Convert.ToString((int) Settings.SecondaryTimeZone))
                .Selected = true;

            chkToolTipMonth.Checked = Settings.Eventtooltipmonth;
            chkToolTipWeek.Checked = Settings.Eventtooltipweek;
            chkToolTipDay.Checked = Settings.Eventtooltipday;
            chkToolTipList.Checked = Settings.Eventtooltiplist;
            txtTooltipLength.Text = Settings.Eventtooltiplength.ToString();
            chkImageEnabled.Checked = Settings.Eventimage;
            txtMaxThumbHeight.Text = Settings.MaxThumbHeight.ToString();
            txtMaxThumbWidth.Text = Settings.MaxThumbWidth.ToString();

            chkMonthCellEvents.Checked = true;
            if (Settings.Monthcellnoevents)
            {
                chkMonthCellEvents.Checked = false;
            }

            chkAddSubModuleName.Checked = Settings.Addsubmodulename;
            chkEnforceSubCalPerms.Checked = Settings.Enforcesubcalperms;

            BindToEnum(typeof(EventModuleSettings.DisplayCategories), ddlEnableCategories);
            ddlEnableCategories.Items.FindByValue(Convert.ToString((int) Settings.Enablecategories))
                .Selected = true;
            chkRestrictCategories.Checked = Settings.Restrictcategories;
            BindToEnum(typeof(EventModuleSettings.DisplayLocations), ddlEnableLocations);
            ddlEnableLocations.Items.FindByValue(Convert.ToString((int) Settings.Enablelocations)).Selected =
                true;
            chkRestrictLocations.Checked = Settings.Restrictlocations;

            chkEnableContainerSkin.Checked = Settings.Enablecontainerskin;
            chkEventDetailNewPage.Checked = Settings.Eventdetailnewpage;
            chkEnableEnrollPopup.Checked = Settings.Enableenrollpopup;
            chkEventImageMonth.Checked = Settings.EventImageMonth;
            chkEventImageWeek.Checked = Settings.EventImageWeek;
            chkEventDayNewPage.Checked = Settings.Eventdaynewpage;
            chkFullTimeScale.Checked = Settings.Fulltimescale;
            chkCollapseRecurring.Checked = Settings.Collapserecurring;
            chkIncludeEndValue.Checked = Settings.Includeendvalue;
            chkShowValueMarks.Checked = Settings.Showvaluemarks;

            chkEnableSEO.Checked = Settings.EnableSEO;
            txtSEODescriptionLength.Text = Settings.SEODescriptionLength.ToString();

            chkEnableSitemap.Checked = Settings.EnableSitemap;
            txtSitemapPriority.Text = Settings.SiteMapPriority.ToString();
            txtSitemapDaysBefore.Text = Settings.SiteMapDaysBefore.ToString();
            txtSitemapDaysAfter.Text = Settings.SiteMapDaysAfter.ToString();

            chkiCalOnIconBar.Checked = Settings.IcalOnIconBar;
            chkiCalEmailEnable.Checked = Settings.IcalEmailEnable;
            chkiCalURLinLocation.Checked = Settings.IcalURLInLocation;
            chkiCalIncludeCalname.Checked = Settings.IcalIncludeCalname;
            txtiCalDaysBefore.Text = Settings.IcalDaysBefore.ToString();
            txtiCalDaysAfter.Text = Settings.IcalDaysAfter.ToString();
            txtiCalURLAppend.Text = Settings.IcalURLAppend;
            ctliCalDefaultImage.FileFilter = Globals.glbImageFileTypes;
            ctliCalDefaultImage.Url = "";
            chkiCalDisplayImage.Checked = false;
            if (Settings.IcalDefaultImage != "")
            {
                ctliCalDefaultImage.Url = Settings.IcalDefaultImage.Substring(6);
                chkiCalDisplayImage.Checked = true;
            }
            if (ctliCalDefaultImage.Url.StartsWith("FileID="))
            {
                var fileId = int.Parse(Convert.ToString(ctliCalDefaultImage.Url.Substring(7)));
                var objFileInfo = FileManager.Instance.GetFile(fileId);
                if (!ReferenceEquals(objFileInfo, null))
                {
                    ctliCalDefaultImage.Url = objFileInfo.Folder + objFileInfo.FileName;
                }
                else
                {
                    ctliCalDefaultImage.Url = "";
                }
            }
            var socialGroupId = GetUrlGroupId();
            var socialGroupStr = "";
            if (socialGroupId > 0)
            {
                socialGroupStr = "&groupid=" + socialGroupId;
            }
            lbliCalURL.Text = Globals.AddHTTP(PortalSettings.PortalAlias.HTTPAlias +
                                                   "/DesktopModules/Events/EventVCal.aspx?ItemID=0&Mid=" +
                                                   Convert.ToString(ModuleId) + "&tabid=" +
                                                   Convert.ToString(TabId) + socialGroupStr);

            // Set Up Themes
            LoadThemes();

            txtPayPalURL.Text = Settings.Paypalurl;

            chkEnableEventNav.Checked = true;
            if (Settings.DisableEventnav)
            {
                chkEnableEventNav.Checked = false;
            }

            chkAllowRecurring.Checked = Settings.Allowreoccurring;
            txtMaxRecurrences.Text = Settings.Maxrecurrences;
            chkEventNotify.Checked = Settings.Eventnotify;
            chkDetailPageAllowed.Checked = Settings.DetailPageAllowed;
            chkEnrollmentPageAllowed.Checked = Settings.EnrollmentPageAllowed;
            txtEnrollmentPageDefaultURL.Text = Settings.EnrollmentPageDefaultUrl;
            chkNotifyAnon.Checked = Settings.Notifyanon;
            chkSendReminderDefault.Checked = Settings.Sendreminderdefault;

            rblNewEventEmail.Items[0].Selected = true;
            switch (Settings.Neweventemails)
            {
                case "Subscribe":
                    rblNewEventEmail.Items[1].Selected = true;
                    break;
                case "Role":
                    rblNewEventEmail.Items[2].Selected = true;
                    break;
            }

            LoadNewEventEmailRoles(Settings.Neweventemailrole);
            chkNewPerEventEmail.Checked = Settings.Newpereventemail;

            ddlDefaultView.Items.Clear();
            ddlDefaultView.Items.Add(new ListItem(Localization.GetString("Month", LocalResourceFile),
                                                       "EventMonth.ascx"));
            ddlDefaultView.Items.Add(new ListItem(Localization.GetString("Week", LocalResourceFile),
                                                       "EventWeek.ascx"));
            ddlDefaultView.Items.Add(new ListItem(Localization.GetString("List", LocalResourceFile),
                                                       "EventList.ascx"));

            ddlDefaultView.Items.FindByValue(Settings.DefaultView).Selected = true;

            chkMonthAllowed.Checked = Settings.MonthAllowed;
            chkWeekAllowed.Checked = Settings.WeekAllowed;
            chkListAllowed.Checked = Settings.ListAllowed;
            chkEnableSearch.Checked = Settings.Eventsearch;
            chkPreventConflicts.Checked = Settings.Preventconflicts;
            chkLocationConflict.Checked = Settings.Locationconflict;
            chkShowEventsAlways.Checked = Settings.ShowEventsAlways;
            chkTimeInTitle.Checked = Settings.Timeintitle;
            chkMonthDaySelect.Checked = Settings.Monthdayselect;
            chkEventSignup.Checked = Settings.Eventsignup;
            chkEventSignupAllowPaid.Checked = Settings.Eventsignupallowpaid;
            chkDefaultEnrollView.Checked = Settings.Eventdefaultenrollview;
            chkHideFullEnroll.Checked = Settings.Eventhidefullenroll;
            txtMaxNoEnrolees.Text = Settings.Maxnoenrolees.ToString();
            txtCancelDays.Text = Settings.Enrolcanceldays.ToString();
            chkFridayWeekend.Checked = Settings.Fridayweekend;
            chkModerateAll.Checked = Settings.Moderateall;
            chkTZDisplay.Checked = Settings.Tzdisplay;
            chkListViewUseTime.Checked = Settings.ListViewUseTime;

            txtPayPalAccount.Text = Settings.Paypalaccount;
            if (txtPayPalAccount.Text.Length == 0)
            {
                txtPayPalAccount.Text = PortalSettings.Email;
            }

            txtReminderFrom.Text = Settings.Reminderfrom;
            if (txtReminderFrom.Text.Length == 0)
            {
                txtReminderFrom.Text = PortalSettings.Email;
            }

            txtStandardEmail.Text = Settings.StandardEmail;
            if (txtStandardEmail.Text.Length == 0)
            {
                txtStandardEmail.Text = PortalSettings.Email;
            }

            BindSubEvents();
            BindAvailableEvents();

            chkMasterEvent.Checked = Settings.MasterEvent;

            Enable_Disable_Cals();

            chkIconMonthPrio.Checked = Settings.IconMonthPrio;
            chkIconWeekPrio.Checked = Settings.IconWeekPrio;
            chkIconListPrio.Checked = Settings.IconListPrio;
            chkIconMonthRec.Checked = Settings.IconMonthRec;
            chkIconWeekRec.Checked = Settings.IconMonthRec;
            chkIconListRec.Checked = Settings.IconListRec;
            chkIconMonthReminder.Checked = Settings.IconMonthReminder;
            chkIconWeekReminder.Checked = Settings.IconWeekReminder;
            chkIconListReminder.Checked = Settings.IconListReminder;
            chkIconMonthEnroll.Checked = Settings.IconMonthEnroll;
            chkIconWeekEnroll.Checked = Settings.IconWeekEnroll;
            chkIconListEnroll.Checked = Settings.IconListEnroll;
            txtPrivateMessage.Text = Settings.PrivateMessage;

            var columnNo = 0;
            for (columnNo = 1; columnNo <= 13; columnNo++)
            {
                var columnAcronym = GetListColumnAcronym(columnNo);
                var columnName = GetListColumnName(columnAcronym);
                if (Settings.EventsListFields.LastIndexOf(columnAcronym, StringComparison.Ordinal) > -1)
                {
                    selectedFields.Add(columnName);
                }
                else
                {
                    availableFields.Add(columnName);
                }
            }

            lstAvailable.DataSource = availableFields;
            lstAvailable.DataBind();
            Sort(lstAvailable);

            lstAssigned.DataSource = selectedFields;
            lstAssigned.DataBind();
            Sort(lstAssigned);

            if (Settings.EventsListSelectType == rblSelectionTypeDays.Value)
            {
                rblSelectionTypeDays.Checked = true;
                rblSelectionTypeEvents.Checked = false;
            }
            else
            {
                rblSelectionTypeDays.Checked = false;
                rblSelectionTypeEvents.Checked = true;
            }

            if (Settings.ListViewGrid)
            {
                rblListViewGrid.Items[0].Selected = true;
            }
            else
            {
                rblListViewGrid.Items[1].Selected = true;
            }
            chkListViewTable.Checked = Settings.ListViewTable;
            txtRptColumns.Text = Settings.RptColumns.ToString();
            txtRptRows.Text = Settings.RptRows.ToString();

            // Do we have to display the EventsList header
            if (Settings.EventsListShowHeader != "No")
            {
                rblShowHeader.Items[0].Selected = true;
            }
            else
            {
                rblShowHeader.Items[1].Selected = true;
            }

            txtDaysBefore.Text = Settings.EventsListBeforeDays.ToString();
            txtDaysAfter.Text = Settings.EventsListAfterDays.ToString();
            txtNumEvents.Text = Settings.EventsListNumEvents.ToString();
            txtEventDays.Text = Settings.EventsListEventDays.ToString();
            chkRestrictCategoriesToTimeFrame.Checked = Settings.RestrictCategoriesToTimeFrame;
            chkRestrictLocationsToTimeFrame.Checked = Settings.RestrictLocationsToTimeFrame;

            chkCustomField1.Checked = Settings.EventsCustomField1;
            chkCustomField2.Checked = Settings.EventsCustomField2;

            ddlPageSize.Items.FindByValue(Convert.ToString(Settings.EventsListPageSize)).Selected = true;
            ddlListSortedFieldDirection.Items.Clear();
            ddlListSortedFieldDirection.Items.Add(
                new ListItem(Localization.GetString("Asc", LocalResourceFile), "ASC"));
            ddlListSortedFieldDirection.Items.Add(
                new ListItem(Localization.GetString("Desc", LocalResourceFile), "DESC"));
            ddlListSortedFieldDirection.Items.FindByValue(Settings.EventsListSortDirection).Selected = true;

            ddlListDefaultColumn.Items.Clear();
            ddlListDefaultColumn.Items.Add(
                new ListItem(Localization.GetString("SortEventID", LocalResourceFile), "EventID"));
            ddlListDefaultColumn.Items.Add(
                new ListItem(Localization.GetString("SortEventDateBegin", LocalResourceFile),
                             "EventDateBegin"));
            ddlListDefaultColumn.Items.Add(
                new ListItem(Localization.GetString("SortEventDateEnd", LocalResourceFile), "EventDateEnd"));
            ddlListDefaultColumn.Items.Add(
                new ListItem(Localization.GetString("SortEventName", LocalResourceFile), "EventName"));
            ddlListDefaultColumn.Items.Add(
                new ListItem(Localization.GetString("SortDuration", LocalResourceFile), "Duration"));
            ddlListDefaultColumn.Items.Add(
                new ListItem(Localization.GetString("SortCategoryName", LocalResourceFile), "CategoryName"));
            ddlListDefaultColumn.Items.Add(
                new ListItem(Localization.GetString("SortCustomField1", LocalResourceFile), "CustomField1"));
            ddlListDefaultColumn.Items.Add(
                new ListItem(Localization.GetString("SortCustomField2", LocalResourceFile), "CustomField2"));
            ddlListDefaultColumn.Items.Add(
                new ListItem(Localization.GetString("SortDescription", LocalResourceFile), "Description"));
            ddlListDefaultColumn.Items.Add(
                new ListItem(Localization.GetString("SortLocationName", LocalResourceFile), "LocationName"));
            ddlListDefaultColumn.Items.FindByValue(Settings.EventsListSortColumn).Selected = true;

            ddlWeekStart.Items.Clear();
            ddlWeekStart.Items.Add(new ListItem(FirstDayOfWeek.Default.ToString(),
                                                     Convert.ToInt32(FirstDayOfWeek.Default).ToString()));
            ddlWeekStart.Items.Add(new ListItem(FirstDayOfWeek.Monday.ToString(),
                                                     Convert.ToInt32(FirstDayOfWeek.Monday).ToString()));
            ddlWeekStart.Items.Add(new ListItem(FirstDayOfWeek.Tuesday.ToString(),
                                                     Convert.ToInt32(FirstDayOfWeek.Tuesday).ToString()));
            ddlWeekStart.Items.Add(new ListItem(FirstDayOfWeek.Wednesday.ToString(),
                                                     Convert.ToInt32(FirstDayOfWeek.Wednesday).ToString()));
            ddlWeekStart.Items.Add(new ListItem(FirstDayOfWeek.Thursday.ToString(),
                                                     Convert.ToInt32(FirstDayOfWeek.Thursday).ToString()));
            ddlWeekStart.Items.Add(new ListItem(FirstDayOfWeek.Friday.ToString(),
                                                     Convert.ToInt32(FirstDayOfWeek.Friday).ToString()));
            ddlWeekStart.Items.Add(new ListItem(FirstDayOfWeek.Saturday.ToString(),
                                                     Convert.ToInt32(FirstDayOfWeek.Saturday).ToString()));
            ddlWeekStart.Items.Add(new ListItem(FirstDayOfWeek.Sunday.ToString(),
                                                     Convert.ToInt32(FirstDayOfWeek.Sunday).ToString()));
            ddlWeekStart.Items.FindByValue(Convert.ToInt32(Settings.WeekStart).ToString()).Selected = true;

            if (Settings.EnrollEditFields.LastIndexOf("01", StringComparison.Ordinal) > -1)
            {
                rblEnUserEdit.Checked = true;
            }
            else if (Settings.EnrollViewFields.LastIndexOf("01", StringComparison.Ordinal) > -1)
            {
                rblEnUserView.Checked = true;
            }
            else if (Settings.EnrollAnonFields.LastIndexOf("01", StringComparison.Ordinal) > -1)
            {
                rblEnUserAnon.Checked = true;
            }
            else
            {
                rblEnUserNone.Checked = true;
            }

            if (Settings.EnrollEditFields.LastIndexOf("02", StringComparison.Ordinal) > -1)
            {
                rblEnDispEdit.Checked = true;
            }
            else if (Settings.EnrollViewFields.LastIndexOf("02", StringComparison.Ordinal) > -1)
            {
                rblEnDispView.Checked = true;
            }
            else if (Settings.EnrollAnonFields.LastIndexOf("02", StringComparison.Ordinal) > -1)
            {
                rblEnDispAnon.Checked = true;
            }
            else
            {
                rblEnDispNone.Checked = true;
            }

            if (Settings.EnrollEditFields.LastIndexOf("03", StringComparison.Ordinal) > -1)
            {
                rblEnEmailEdit.Checked = true;
            }
            else if (Settings.EnrollViewFields.LastIndexOf("03", StringComparison.Ordinal) > -1)
            {
                rblEnEmailView.Checked = true;
            }
            else if (Settings.EnrollAnonFields.LastIndexOf("03", StringComparison.Ordinal) > -1)
            {
                rblEnEmailAnon.Checked = true;
            }
            else
            {
                rblEnEmailNone.Checked = true;
            }

            if (Settings.EnrollEditFields.LastIndexOf("04", StringComparison.Ordinal) > -1)
            {
                rblEnPhoneEdit.Checked = true;
            }
            else if (Settings.EnrollViewFields.LastIndexOf("04", StringComparison.Ordinal) > -1)
            {
                rblEnPhoneView.Checked = true;
            }
            else if (Settings.EnrollAnonFields.LastIndexOf("04", StringComparison.Ordinal) > -1)
            {
                rblEnPhoneAnon.Checked = true;
            }
            else
            {
                rblEnPhoneNone.Checked = true;
            }

            if (Settings.EnrollEditFields.LastIndexOf("05", StringComparison.Ordinal) > -1)
            {
                rblEnApproveEdit.Checked = true;
            }
            else if (Settings.EnrollViewFields.LastIndexOf("05", StringComparison.Ordinal) > -1)
            {
                rblEnApproveView.Checked = true;
            }
            else if (Settings.EnrollAnonFields.LastIndexOf("05", StringComparison.Ordinal) > -1)
            {
                rblEnApproveAnon.Checked = true;
            }
            else
            {
                rblEnApproveNone.Checked = true;
            }

            if (Settings.EnrollEditFields.LastIndexOf("06", StringComparison.Ordinal) > -1)
            {
                rblEnNoEdit.Checked = true;
            }
            else if (Settings.EnrollViewFields.LastIndexOf("06", StringComparison.Ordinal) > -1)
            {
                rblEnNoView.Checked = true;
            }
            else if (Settings.EnrollAnonFields.LastIndexOf("06", StringComparison.Ordinal) > -1)
            {
                rblEnNoAnon.Checked = true;
            }
            else
            {
                rblEnNoNone.Checked = true;
            }


            chkRSSEnable.Checked = Settings.RSSEnable;
            ddlRSSDateField.Items.Clear();
            ddlRSSDateField.Items.Add(new ListItem(Localization.GetString("UpdatedDate", LocalResourceFile),
                                                        "UPDATEDDATE"));
            ddlRSSDateField.Items.Add(new ListItem(Localization.GetString("CreationDate", LocalResourceFile),
                                                        "CREATIONDATE"));
            ddlRSSDateField.Items.Add(new ListItem(Localization.GetString("EventDate", LocalResourceFile),
                                                        "EVENTDATE"));
            ddlRSSDateField.Items.FindByValue(Settings.RSSDateField).Selected = true;
            txtRSSDays.Text = Settings.RSSDays.ToString();
            txtRSSTitle.Text = Settings.RSSTitle;
            txtRSSDesc.Text = Settings.RSSDesc;
            txtExpireEvents.Text = Settings.Expireevents;
            chkExportOwnerEmail.Checked = Settings.Exportowneremail;
            chkExportAnonOwnerEmail.Checked = Settings.Exportanonowneremail;
            chkOwnerChangeAllowed.Checked = Settings.Ownerchangeallowed;

            txtFBAdmins.Text = Settings.FBAdmins;
            txtFBAppID.Text = Settings.FBAppID;

            switch (Settings.IconBar)
            {
                case "BOTTOM":
                    rblIconBar.Items[1].Selected = true;
                    break;
                case "NONE":
                    rblIconBar.Items[2].Selected = true;
                    break;
                default:
                    rblIconBar.Items[0].Selected = true;
                    break;
            }

            switch (Settings.HTMLEmail)
            {
                case "auto":
                    rblHTMLEmail.Items[1].Selected = true;
                    break;
                case "text":
                    rblHTMLEmail.Items[2].Selected = true;
                    break;
                default:
                    rblHTMLEmail.Items[0].Selected = true;
                    break;
            }


            chkEnrollMessageApproved.Checked = Settings.SendEnrollMessageApproved;
            chkEnrollMessageWaiting.Checked = Settings.SendEnrollMessageWaiting;
            chkEnrollMessageDenied.Checked = Settings.SendEnrollMessageDenied;
            chkEnrollMessageAdded.Checked = Settings.SendEnrollMessageAdded;
            chkEnrollMessageDeleted.Checked = Settings.SendEnrollMessageDeleted;
            chkEnrollMessagePaying.Checked = Settings.SendEnrollMessagePaying;
            chkEnrollMessagePending.Checked = Settings.SendEnrollMessagePending;
            chkEnrollMessagePaid.Checked = Settings.SendEnrollMessagePaid;
            chkEnrollMessageIncorrect.Checked = Settings.SendEnrollMessageIncorrect;
            chkEnrollMessageCancelled.Checked = Settings.SendEnrollMessageCancelled;

            chkAllowAnonEnroll.Checked = Settings.AllowAnonEnroll;
            BindToEnum(typeof(EventModuleSettings.SocialModule), ddlSocialGroupModule);
            ddlSocialGroupModule.Items.FindByValue(Convert.ToString((int) Settings.SocialGroupModule))
                .Selected = true;
            chkSocialUserPrivate.Checked = Settings.SocialUserPrivate;
            BindToEnum(typeof(EventModuleSettings.SocialGroupPrivacy), ddlSocialGroupSecurity);
            ddlSocialGroupSecurity.Items.FindByValue(Convert.ToString((int) Settings.SocialGroupSecurity))
                .Selected = true;

            ddlEnrolListSortDirection.Items.Clear();
            ddlEnrolListSortDirection.Items.Add(
                new ListItem(Localization.GetString("Asc", LocalResourceFile), "0"));
            ddlEnrolListSortDirection.Items.Add(
                new ListItem(Localization.GetString("Desc", LocalResourceFile), "1"));
            ddlEnrolListSortDirection.Items
                .FindByValue(Convert.ToInt32(Settings.EnrolListSortDirection).ToString()).Selected = true;

            txtEnrolListDaysBefore.Text = Settings.EnrolListDaysBefore.ToString();
            txtEnrolListDaysAfter.Text = Settings.EnrolListDaysAfter.ToString();

            chkJournalIntegration.Checked = Settings.JournalIntegration;

            LoadCategories();
            LoadLocations();

            LoadTemplates();
        }

        private void AddJavaScript()
        {
            //Add the external Validation.js to the Page
            const string csname = "ExtValidationScriptFile";
            var cstype = MethodBase.GetCurrentMethod().GetType();
            var cstext = "<script src=\"" + ResolveUrl("~/DesktopModules/Events/Scripts/Validation.js") +
                         "\" type=\"text/javascript\"></script>";
            if (!Page.ClientScript.IsClientScriptBlockRegistered(csname))
            {
                Page.ClientScript.RegisterClientScriptBlock(cstype, csname, cstext, false);
            }


            // Add javascript actions where required and build startup script
            var script = "";
            var cstext2 = "";
            cstext2 += "<script type=\"text/javascript\">";
            cstext2 += "EventSettingsStartupScript = function() {";

            script = "disableactivate('" + ddlDefaultView.ClientID + "','" + chkMonthAllowed.ClientID +
                     "','" + chkWeekAllowed.ClientID + "','" + chkListAllowed.ClientID + "');";
            cstext2 += script;
            ddlDefaultView.Attributes.Add("onchange", script);

            script = "disableControl('" + chkPreventConflicts.ClientID + "',false, '" +
                     chkLocationConflict.ClientID + "');";
            cstext2 += script;
            chkPreventConflicts.InputAttributes.Add("onclick", script);

            script = "disableControl('" + chkMonthCellEvents.ClientID + "',true, '" +
                     chkEventDayNewPage.ClientID + "');";
            script += "disableControl('" + chkMonthCellEvents.ClientID + "',false, '" +
                      chkMonthDaySelect.ClientID + "');";
            script += "disableControl('" + chkMonthCellEvents.ClientID + "',false, '" +
                      chkTimeInTitle.ClientID + "');";
            script += "disableControl('" + chkMonthCellEvents.ClientID + "',false, '" +
                      chkEventImageMonth.ClientID + "');";
            script += "disableControl('" + chkMonthCellEvents.ClientID + "',false, '" +
                      chkIconMonthPrio.ClientID + "');";
            script += "disableControl('" + chkMonthCellEvents.ClientID + "',false, '" +
                      chkIconMonthRec.ClientID + "');";
            script += "disableControl('" + chkMonthCellEvents.ClientID + "',false, '" +
                      chkIconMonthReminder.ClientID + "');";
            script += "disableControl('" + chkMonthCellEvents.ClientID + "',false, '" +
                      chkIconMonthEnroll.ClientID + "');";
            cstext2 += script;
            chkMonthCellEvents.InputAttributes.Add("onclick", script);

            script = "disableRbl('" + rblListViewGrid.ClientID + "', 'Repeater', '" +
                     chkListViewTable.ClientID + "');";
            script += "disableRbl('" + rblListViewGrid.ClientID + "', 'Repeater', '" +
                      txtRptColumns.ClientID + "');";
            script += "disableRbl('" + rblListViewGrid.ClientID + "', 'Repeater', '" + txtRptRows.ClientID +
                      "');";
            script += "disableRbl('" + rblListViewGrid.ClientID + "', 'Grid', '" + rblShowHeader.ClientID +
                      "');";
            script += "disableRbl('" + rblListViewGrid.ClientID + "', 'Grid', '" + lstAvailable.ClientID +
                      "');";
            script += "disableRbl('" + rblListViewGrid.ClientID + "', 'Grid', '" + cmdAdd.ClientID + "');";
            script += "disableRbl('" + rblListViewGrid.ClientID + "', 'Grid', '" + cmdRemove.ClientID + "');";
            script += "disableRbl('" + rblListViewGrid.ClientID + "', 'Grid', '" + cmdAddAll.ClientID + "');";
            script += "disableRbl('" + rblListViewGrid.ClientID + "', 'Grid', '" + cmdRemoveAll.ClientID +
                      "');";
            script += "disableRbl('" + rblListViewGrid.ClientID + "', 'Grid', '" + lstAssigned.ClientID +
                      "');";
            script += "disableRbl('" + rblListViewGrid.ClientID + "', 'Grid', '" + ddlPageSize.ClientID +
                      "');";
            script += "disableRbl('" + rblListViewGrid.ClientID + "', 'Grid', '" +
                      chkIconListEnroll.ClientID + "');";
            script += "disableRbl('" + rblListViewGrid.ClientID + "', 'Grid', '" + chkIconListPrio.ClientID +
                      "');";
            script += "disableRbl('" + rblListViewGrid.ClientID + "', 'Grid', '" + chkIconListRec.ClientID +
                      "');";
            script += "disableRbl('" + rblListViewGrid.ClientID + "', 'Grid', '" +
                      chkIconListReminder.ClientID + "');";
            cstext2 += script;
            rblListViewGrid.Attributes.Add("onclick", script);

            script = "CheckBoxFalse('" + chkIncludeEndValue.ClientID + "', true, '" +
                     chkShowValueMarks.ClientID + "');";
            cstext2 += script;
            chkIncludeEndValue.InputAttributes.Add("onclick", script);

            script = "disablelistsettings('" + rblSelectionTypeDays.ClientID + "',true,'" +
                     txtDaysBefore.ClientID + "','" + txtDaysAfter.ClientID + "','" +
                     txtNumEvents.ClientID + "','" + txtEventDays.ClientID + "');";
            cstext2 += script;
            rblSelectionTypeDays.Attributes.Add("onclick", script);

            script = "disablelistsettings('" + rblSelectionTypeEvents.ClientID + "',false,'" +
                     txtDaysBefore.ClientID + "','" + txtDaysAfter.ClientID + "','" +
                     txtNumEvents.ClientID + "','" + txtEventDays.ClientID + "');";
            cstext2 += script;
            rblSelectionTypeEvents.Attributes.Add("onclick", script);

            script = "showTbl('" + chkEventNotify.ClientID + "','" + divEventNotify.ClientID + "');";
            cstext2 += script;
            chkEventNotify.InputAttributes.Add("onclick", script);

            script = "showTbl('" + chkRSSEnable.ClientID + "','" + divRSSEnable.ClientID + "');";
            cstext2 += script;
            chkRSSEnable.InputAttributes.Add("onclick", script);

            script = "showTbl('" + chkImageEnabled.ClientID + "','" + diviCalEventImage.ClientID +
                     "'); showTbl('" + chkImageEnabled.ClientID + "','" + divImageEnabled.ClientID + "');";
            cstext2 += script;
            chkImageEnabled.InputAttributes.Add("onclick", script);

            script = "showTbl('" + chkiCalDisplayImage.ClientID + "','" + diviCalDisplayImage.ClientID +
                     "');";
            cstext2 += script;
            chkiCalDisplayImage.InputAttributes.Add("onclick", script);

            script = "disableControl('" + chkExportOwnerEmail.ClientID + "',false, '" +
                     chkExportAnonOwnerEmail.ClientID + "');";
            cstext2 += script;
            chkExportOwnerEmail.InputAttributes.Add("onclick", script);

            script = "disableDDL('" + ddlSocialGroupModule.ClientID + "','" +
                     Convert.ToInt32(EventModuleSettings.SocialModule.UserProfile) + "','" +
                     chkSocialUserPrivate.ClientID + "');";
            cstext2 += script;
            ddlSocialGroupModule.Attributes.Add("onclick", script);

            if (Settings.SocialGroupModule == EventModuleSettings.SocialModule.No)
            {
                script = "disableRbl('" + rblNewEventEmail.ClientID + "', 'Role', '" +
                         ddNewEventEmailRoles.ClientID + "');";
                cstext2 += script;
                rblNewEventEmail.Attributes.Add("onclick", script);
            }

            if (Settings.SocialGroupModule != EventModuleSettings.SocialModule.UserProfile)
            {
                script = "showTbl('" + chkEventSignup.ClientID + "','" + divEventSignup.ClientID + "');";
                cstext2 += script;
                chkEventSignup.InputAttributes.Add("onclick", script);

                script = "showTbl('" + chkEventSignupAllowPaid.ClientID + "','" +
                         divEventSignupAllowPaid.ClientID + "');";
                cstext2 += script;
                chkEventSignupAllowPaid.InputAttributes.Add("onclick", script);

                script = "showTbl('" + chkEnableSEO.ClientID + "','" + divSEOEnable.ClientID + "');";
                cstext2 += script;
                chkEnableSEO.InputAttributes.Add("onclick", script);

                script = "showTbl('" + chkEnableSitemap.ClientID + "','" + divSitemapEnable.ClientID + "');";
                cstext2 += script;
                chkEnableSitemap.InputAttributes.Add("onclick", script);
            }

            cstext2 += "};";
            cstext2 += "EventSettingsStartupScript();";
            cstext2 += "Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EventEndRequestHandler);";
            cstext2 += "function EventEndRequestHandler(sender, args) { EventSettingsStartupScript(); }";
            cstext2 += "</script>";

            // Register the startup script
            const string csname2 = "EventSettingsStartupScript";
            var cstype2 = MethodBase.GetCurrentMethod().GetType();
            if (!Page.ClientScript.IsStartupScriptRegistered(csname2))
            {
                Page.ClientScript.RegisterStartupScript(cstype2, csname2, cstext2, false);
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
                ddl.Items.Add(new ListItem(Localization.GetString(names[i], LocalResourceFile),
                                           Convert.ToString(Convert.ToInt32(values.GetValue(i)))));
            }
            // return the dictionary to be bound to
        }


        private string GetListColumnName(string columnAcronym)
        {
            switch (columnAcronym)
            {
                case "EB":
                    return "01 - " + Localization.GetString("EditButton", LocalResourceFile);
                case "BD":
                    return "02 - " + Localization.GetString("BeginDateTime", LocalResourceFile);
                case "ED":
                    return "03 - " + Localization.GetString("EndDateTime", LocalResourceFile);
                case "EN":
                    return "04 - " + Localization.GetString("EventName", LocalResourceFile);
                case "IM":
                    return "05 - " + Localization.GetString("Image", LocalResourceFile);
                case "DU":
                    return "06 - " + Localization.GetString("Duration", LocalResourceFile);
                case "CA":
                    return "07 - " + Localization.GetString("Category", LocalResourceFile);
                case "LO":
                    return "08 - " + Localization.GetString("Location", LocalResourceFile);
                case "C1":
                    return "09 - " + Localization.GetString("CustomField1", LocalResourceFile);
                case "C2":
                    return "10 - " + Localization.GetString("CustomField2", LocalResourceFile);
                case "DE":
                    return "11 - " + Localization.GetString("Description", LocalResourceFile);
                case "RT":
                    return "12 - " + Localization.GetString("RecurText", LocalResourceFile);
                case "RU":
                    return "13 - " + Localization.GetString("RecurUntil", LocalResourceFile);
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
                ddlThemeStandard.Items.Clear();
                ddlThemeCustom.Items.Clear();

                //Add javascript to enable/disable ddl's
                rbThemeCustom.Attributes.Add(
                    "onclick",
                    string.Format("{0}.disabled='disabled';{1}.disabled=''", ddlThemeStandard.ClientID,
                                  ddlThemeCustom.ClientID));
                rbThemeStandard.Attributes.Add(
                    "onclick",
                    string.Format("{0}.disabled='disabled';{1}.disabled=''", ddlThemeCustom.ClientID,
                                  ddlThemeStandard.ClientID));

                //Get the settings
                var themeSettings = new ThemeSetting();
                if (themeSettings.ValidateSetting(Settings.EventTheme) == false)
                {
                    themeSettings.ReadSetting(Settings.EventThemeDefault, PortalId);
                }
                else if (Settings.EventTheme != "")
                {
                    themeSettings.ReadSetting(Settings.EventTheme, PortalId);
                }
                switch (themeSettings.SettingType)
                {
                    case ThemeSetting.ThemeSettingTypeEnum.CustomTheme:
                        rbThemeCustom.Checked = true;
                        break;
                    case ThemeSetting.ThemeSettingTypeEnum.DefaultTheme:
                        rbThemeStandard.Checked = true;
                        break;
                }

                //Is default or custom selected
                var moduleThemesDirectory = Globals.ApplicationPath + moduleThemesDirectoryPath;
                var serverThemesDirectory = Server.MapPath(moduleThemesDirectory);
                var themeDirectories = Directory.GetDirectories(serverThemesDirectory);
                var themeDirectory = "";
                foreach (var tempLoopVar_themeDirectory in themeDirectories)
                {
                    themeDirectory = tempLoopVar_themeDirectory;
                    var dirparts = themeDirectory.Split('\\');
                    ddlThemeStandard.Items.Add(
                        new ListItem(dirparts[dirparts.Length - 1], dirparts[dirparts.Length - 1]));
                }
                if (themeSettings.SettingType == ThemeSetting.ThemeSettingTypeEnum.DefaultTheme)
                {
                    if (!ReferenceEquals(ddlThemeStandard.Items.FindByText(themeSettings.ThemeName), null))
                    {
                        ddlThemeStandard.Items.FindByText(themeSettings.ThemeName).Selected = true;
                    }
                }
                else
                {
                    ddlThemeStandard.Attributes.Add("disabled", "disabled");
                }

                //Add custom event theme's
                var pc = new PortalController();
                var with_1 = pc.GetPortal(PortalId);
                var eventSkinPath = string.Format("{0}\\DNNEvents\\Themes", with_1.HomeDirectoryMapPath);
                if (!Directory.Exists(eventSkinPath))
                {
                    Directory.CreateDirectory(eventSkinPath);
                }

                foreach (var d in Directory.GetDirectories(eventSkinPath))
                {
                    ddlThemeCustom.Items.Add(new ListItem(new DirectoryInfo(d).Name, new DirectoryInfo(d).Name));
                }
                if (ddlThemeCustom.Items.Count == 0)
                {
                    rbThemeCustom.Enabled = false;
                }

                if (themeSettings.SettingType == ThemeSetting.ThemeSettingTypeEnum.CustomTheme)
                {
                    if (!ReferenceEquals(ddlThemeCustom.Items.FindByText(themeSettings.ThemeName), null))
                    {
                        ddlThemeCustom.Items.FindByText(themeSettings.ThemeName).Selected = true;
                    }
                }
                else
                {
                    ddlThemeCustom.Attributes.Add("disabled", "disabled");
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void LoadTemplates()
        {
            ddlTemplates.Items.Clear();

            var t = Settings.Templates.GetType();
            var p = default(PropertyInfo);
            foreach (var tempLoopVar_p in t.GetProperties())
            {
                p = tempLoopVar_p;
                ddlTemplates.Items.Add(
                    new ListItem(Localization.GetString(p.Name + "Name", LocalResourceFile), p.Name));
            }

            ddlTemplates.Items.FindByValue("EventDetailsTemplate").Selected = true;
            txtEventTemplate.Text = Settings.Templates.GetTemplate(ddlTemplates.SelectedValue);
            lblTemplateUpdated.Visible = false;
        }

        private void LoadNewEventEmailRoles(int roleID)
        {
            var objRoles = new RoleController();
            ddNewEventEmailRoles.DataSource = objRoles.GetPortalRoles(PortalId);
            ddNewEventEmailRoles.DataTextField = "RoleName";
            ddNewEventEmailRoles.DataValueField = "RoleID";
            ddNewEventEmailRoles.DataBind();
            if (roleID < 0 || ReferenceEquals(ddNewEventEmailRoles.Items.FindByValue(Convert.ToString(roleID)),
                                              null))
            {
                try
                {
                    ddNewEventEmailRoles.Items.FindByValue(PortalSettings.RegisteredRoleId.ToString())
                        .Selected = true;
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                { }
            }
            else
            {
                ddNewEventEmailRoles.Items.FindByValue(Convert.ToString(roleID)).Selected = true;
            }
        }

        private void LoadCategories()
        {
            ddlModuleCategories.Items.Clear();
            var ctrlEventCategories = new EventCategoryController();
            var lstCategories = ctrlEventCategories.EventsCategoryList(PortalId);
            ddlModuleCategories.DataSource = lstCategories;
            ddlModuleCategories.DataBind();

            if (Settings.ModuleCategoriesSelected == EventModuleSettings.CategoriesSelected.Some)
            {
                foreach (string moduleCategory in Settings.ModuleCategoryIDs)
                {
                    foreach (RadComboBoxItem item in ddlModuleCategories.Items)
                    {
                        if (item.Value == moduleCategory)
                        {
                            item.Checked = true;
                        }
                    }
                }
            }
            else if (Settings.ModuleCategoriesSelected == EventModuleSettings.CategoriesSelected.All)
            {
                foreach (RadComboBoxItem item in ddlModuleCategories.Items)
                {
                    item.Checked = true;
                }
            }
        }

        private void LoadLocations()
        {
            ddlModuleLocations.Items.Clear();
            var ctrlEventLocations = new EventLocationController();
            var lstLocations = ctrlEventLocations.EventsLocationList(PortalId);
            ddlModuleLocations.DataSource = lstLocations;
            ddlModuleLocations.DataBind();

            if (Settings.ModuleLocationsSelected == EventModuleSettings.LocationsSelected.Some)
            {
                foreach (string moduleLocation in Settings.ModuleLocationIDs)
                {
                    foreach (RadComboBoxItem item in ddlModuleLocations.Items)
                    {
                        if (item.Value == moduleLocation)
                        {
                            item.Checked = true;
                        }
                    }
                }
            }
            else if (Settings.ModuleLocationsSelected == EventModuleSettings.LocationsSelected.All)
            {
                foreach (RadComboBoxItem item in ddlModuleLocations.Items)
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
            Settings.Timeinterval = ddlTimeInterval.SelectedValue.Trim();
            Settings.TimeZoneId = cboTimeZone.SelectedValue;
            Settings.EnableEventTimeZones = chkEnableEventTimeZones.Checked;
            Settings.PrimaryTimeZone =
                (EventModuleSettings.TimeZones) int.Parse(ddlPrimaryTimeZone.SelectedValue);

            try
            {
                Settings.Timeinterval = ddlTimeInterval.SelectedValue.Trim();
                Settings.TimeZoneId = cboTimeZone.SelectedValue;
                Settings.EnableEventTimeZones = chkEnableEventTimeZones.Checked;
                Settings.PrimaryTimeZone =
                    (EventModuleSettings.TimeZones) int.Parse(ddlPrimaryTimeZone.SelectedValue);
                Settings.SecondaryTimeZone =
                    (EventModuleSettings.TimeZones) int.Parse(ddlSecondaryTimeZone.SelectedValue);
                Settings.Eventtooltipmonth = chkToolTipMonth.Checked;
                Settings.Eventtooltipweek = chkToolTipWeek.Checked;
                Settings.Eventtooltipday = chkToolTipDay.Checked;
                Settings.Eventtooltiplist = chkToolTipList.Checked;
                Settings.Eventtooltiplength = int.Parse(txtTooltipLength.Text);
                if (chkMonthCellEvents.Checked)
                {
                    Settings.Monthcellnoevents = false;
                }
                else
                {
                    Settings.Monthcellnoevents = true;
                }
                Settings.Enablecategories =
                    (EventModuleSettings.DisplayCategories) int.Parse(ddlEnableCategories.SelectedValue);
                Settings.Restrictcategories = chkRestrictCategories.Checked;
                Settings.Enablelocations =
                    (EventModuleSettings.DisplayLocations) int.Parse(ddlEnableLocations.SelectedValue);
                Settings.Restrictlocations = chkRestrictLocations.Checked;
                Settings.Enablecontainerskin = chkEnableContainerSkin.Checked;
                Settings.Eventdetailnewpage = chkEventDetailNewPage.Checked;
                Settings.Enableenrollpopup = chkEnableEnrollPopup.Checked;
                Settings.Eventdaynewpage = chkEventDayNewPage.Checked;
                Settings.EventImageMonth = chkEventImageMonth.Checked;
                Settings.EventImageWeek = chkEventImageWeek.Checked;
                Settings.Eventnotify = chkEventNotify.Checked;
                Settings.DetailPageAllowed = chkDetailPageAllowed.Checked;
                Settings.EnrollmentPageAllowed = chkEnrollmentPageAllowed.Checked;
                Settings.EnrollmentPageDefaultUrl = txtEnrollmentPageDefaultURL.Text;
                Settings.Notifyanon = chkNotifyAnon.Checked;
                Settings.Sendreminderdefault = chkSendReminderDefault.Checked;
                Settings.Neweventemails = rblNewEventEmail.SelectedValue;
                Settings.Neweventemailrole = int.Parse(ddNewEventEmailRoles.SelectedValue);
                Settings.Newpereventemail = chkNewPerEventEmail.Checked;
                Settings.Tzdisplay = chkTZDisplay.Checked;
                Settings.Paypalurl = txtPayPalURL.Text;
                if (chkEnableEventNav.Checked)
                {
                    Settings.DisableEventnav = false;
                }
                else
                {
                    Settings.DisableEventnav = true;
                }
                Settings.Fulltimescale = chkFullTimeScale.Checked;
                Settings.Collapserecurring = chkCollapseRecurring.Checked;
                Settings.Includeendvalue = chkIncludeEndValue.Checked;
                Settings.Showvaluemarks = chkShowValueMarks.Checked;
                Settings.Eventimage = chkImageEnabled.Checked;
                Settings.MaxThumbHeight = int.Parse(txtMaxThumbHeight.Text);
                Settings.MaxThumbWidth = int.Parse(txtMaxThumbWidth.Text);
                Settings.Allowreoccurring = chkAllowRecurring.Checked;
                Settings.Maxrecurrences = txtMaxRecurrences.Text;
                Settings.Eventsearch = chkEnableSearch.Checked;
                Settings.Addsubmodulename = chkAddSubModuleName.Checked;
                Settings.Enforcesubcalperms = chkEnforceSubCalPerms.Checked;
                Settings.Preventconflicts = chkPreventConflicts.Checked;
                Settings.Locationconflict = chkLocationConflict.Checked;
                Settings.ShowEventsAlways = chkShowEventsAlways.Checked;
                Settings.Timeintitle = chkTimeInTitle.Checked;
                Settings.Monthdayselect = chkMonthDaySelect.Checked;
                Settings.MasterEvent = chkMasterEvent.Checked;
                Settings.Eventsignup = chkEventSignup.Checked;
                Settings.Eventsignupallowpaid = chkEventSignupAllowPaid.Checked;
                Settings.Eventdefaultenrollview = chkDefaultEnrollView.Checked;
                Settings.Eventhidefullenroll = chkHideFullEnroll.Checked;
                Settings.Maxnoenrolees = int.Parse(txtMaxNoEnrolees.Text);
                Settings.Enrolcanceldays = int.Parse(txtCancelDays.Text);
                Settings.Fridayweekend = chkFridayWeekend.Checked;
                Settings.Moderateall = chkModerateAll.Checked;
                Settings.Paypalaccount = txtPayPalAccount.Text;
                Settings.Reminderfrom = txtReminderFrom.Text;
                Settings.StandardEmail = txtStandardEmail.Text;
                Settings.EventsCustomField1 = chkCustomField1.Checked;
                Settings.EventsCustomField2 = chkCustomField2.Checked;
                Settings.DefaultView = ddlDefaultView.SelectedItem.Value;
                Settings.EventsListPageSize = int.Parse(ddlPageSize.SelectedItem.Value);
                Settings.EventsListSortDirection = ddlListSortedFieldDirection.SelectedItem.Value;
                Settings.EventsListSortColumn = ddlListDefaultColumn.SelectedItem.Value;
                Settings.RSSEnable = chkRSSEnable.Checked;
                Settings.RSSDateField = ddlRSSDateField.SelectedItem.Value;
                Settings.RSSDays = int.Parse(txtRSSDays.Text);
                Settings.RSSTitle = txtRSSTitle.Text;
                Settings.RSSDesc = txtRSSDesc.Text;
                Settings.Expireevents = txtExpireEvents.Text;
                Settings.Exportowneremail = chkExportOwnerEmail.Checked;
                Settings.Exportanonowneremail = chkExportAnonOwnerEmail.Checked;
                Settings.Ownerchangeallowed = chkOwnerChangeAllowed.Checked;
                Settings.IconMonthPrio = chkIconMonthPrio.Checked;
                Settings.IconMonthRec = chkIconMonthRec.Checked;
                Settings.IconMonthReminder = chkIconMonthReminder.Checked;
                Settings.IconMonthEnroll = chkIconMonthEnroll.Checked;
                Settings.IconWeekPrio = chkIconWeekPrio.Checked;
                Settings.IconWeekRec = chkIconWeekRec.Checked;
                Settings.IconWeekReminder = chkIconWeekReminder.Checked;
                Settings.IconWeekEnroll = chkIconWeekEnroll.Checked;
                Settings.IconListPrio = chkIconListPrio.Checked;
                Settings.IconListRec = chkIconListRec.Checked;
                Settings.IconListReminder = chkIconListReminder.Checked;
                Settings.IconListEnroll = chkIconListEnroll.Checked;
                Settings.PrivateMessage = txtPrivateMessage.Text.Trim();
                Settings.EnableSEO = chkEnableSEO.Checked;
                Settings.SEODescriptionLength = int.Parse(txtSEODescriptionLength.Text);
                Settings.EnableSitemap = chkEnableSitemap.Checked;
                Settings.SiteMapPriority = Convert.ToSingle(Convert.ToSingle(txtSitemapPriority.Text));
                Settings.SiteMapDaysBefore = int.Parse(txtSitemapDaysBefore.Text);
                Settings.SiteMapDaysAfter = int.Parse(txtSitemapDaysAfter.Text);
                Settings.WeekStart = (FirstDayOfWeek) int.Parse(ddlWeekStart.SelectedValue);
                Settings.ListViewUseTime = chkListViewUseTime.Checked;

                Settings.IcalOnIconBar = chkiCalOnIconBar.Checked;
                Settings.IcalEmailEnable = chkiCalEmailEnable.Checked;
                Settings.IcalURLInLocation = chkiCalURLinLocation.Checked;
                Settings.IcalIncludeCalname = chkiCalIncludeCalname.Checked;
                Settings.IcalDaysBefore = int.Parse(txtiCalDaysBefore.Text);
                Settings.IcalDaysAfter = int.Parse(txtiCalDaysAfter.Text);
                Settings.IcalURLAppend = txtiCalURLAppend.Text;
                if (chkiCalDisplayImage.Checked)
                {
                    Settings.IcalDefaultImage = "Image=" + ctliCalDefaultImage.Url;
                }
                else
                {
                    Settings.IcalDefaultImage = "";
                }
                //objModules.UpdateModuleSetting(ModuleId, "EventDetailsTemplate", txtEventDetailsTemplate.Text.Trim)

                var moduleCategories = new ArrayList();
                if (ddlModuleCategories.CheckedItems.Count != ddlModuleCategories.Items.Count)
                {
                    foreach (var item in ddlModuleCategories.CheckedItems)
                    {
                        moduleCategories.Add(item.Value);
                    }
                }
                else
                {
                    moduleCategories.Add("-1");
                }
                Settings.ModuleCategoryIDs = moduleCategories;

                var moduleLocations = new ArrayList();
                if (ddlModuleLocations.CheckedItems.Count != ddlModuleLocations.Items.Count)
                {
                    foreach (var item in ddlModuleLocations.CheckedItems)
                    {
                        moduleLocations.Add(item.Value);
                    }
                }
                else
                {
                    moduleLocations.Add("-1");
                }
                Settings.ModuleLocationIDs = moduleLocations;

                // ReSharper disable LocalizableElement
                if (chkMonthAllowed.Checked || ddlDefaultView.SelectedItem.Value == "EventMonth.ascx")
                {
                    Settings.MonthAllowed = true;
                }
                else
                {
                    Settings.MonthAllowed = false;
                }
                if (chkWeekAllowed.Checked || ddlDefaultView.SelectedItem.Value == "EventWeek.ascx")
                {
                    Settings.WeekAllowed = true;
                }
                else
                {
                    Settings.WeekAllowed = false;
                }
                if (chkListAllowed.Checked || ddlDefaultView.SelectedItem.Value == "EventList.ascx")
                {
                    Settings.ListAllowed = true;
                }
                else
                {
                    Settings.ListAllowed = false;
                }
                // ReSharper restore LocalizableElement

                switch (rblIconBar.SelectedIndex)
                {
                    case 0:
                        Settings.IconBar = "TOP";
                        break;
                    case 1:
                        Settings.IconBar = "BOTTOM";
                        break;
                    case 2:
                        Settings.IconBar = "NONE";
                        break;
                }

                switch (rblHTMLEmail.SelectedIndex)
                {
                    case 0:
                        Settings.HTMLEmail = "html";
                        break;
                    case 1:
                        Settings.HTMLEmail = "auto";
                        break;
                    case 2:
                        Settings.HTMLEmail = "text";
                        break;
                }

                //EPT: Be sure we start next display time in the correct view
                // Update the cookie so the appropriate view is shown when settings page is exited

                var objCookie = new HttpCookie("DNNEvents" + Convert.ToString(ModuleId));
                objCookie.Value = ddlDefaultView.SelectedItem.Value;
                if (ReferenceEquals(Request.Cookies.Get("DNNEvents" + Convert.ToString(ModuleId)), null))
                {
                    Response.Cookies.Add(objCookie);
                }
                else
                {
                    Response.Cookies.Set(objCookie);
                }

                //Set eventtheme data
                var themeSettings = new ThemeSetting();
                if (rbThemeStandard.Checked)
                {
                    themeSettings.SettingType = ThemeSetting.ThemeSettingTypeEnum.DefaultTheme;
                    themeSettings.ThemeName = ddlThemeStandard.SelectedItem.Text;
                    themeSettings.ThemeFile = "";
                }
                else if (rbThemeCustom.Checked)
                {
                    themeSettings.SettingType = ThemeSetting.ThemeSettingTypeEnum.CustomTheme;
                    themeSettings.ThemeName = ddlThemeCustom.SelectedItem.Text;
                    themeSettings.ThemeFile = "";
                }
                Settings.EventTheme = themeSettings.ToString();

                //List Events Mode Stuff
                //Update Fields to Display
                var objListItem = default(ListItem);
                var listFields = "";
                foreach (ListItem tempLoopVar_objListItem in lstAssigned.Items)
                {
                    objListItem = tempLoopVar_objListItem;
                    var columnNo = int.Parse(objListItem.Text.Substring(0, 2));
                    var columnAcronym = GetListColumnAcronym(columnNo);
                    if (listFields.Length > 0)
                    {
                        listFields = listFields + ";" + columnAcronym;
                    }
                    else
                    {
                        listFields = columnAcronym;
                    }
                }
                Settings.EventsListFields = listFields;

                listFields = EnrollListFields(rblEnUserAnon.Checked, rblEnDispAnon.Checked,
                                                   rblEnEmailAnon.Checked, rblEnPhoneAnon.Checked,
                                                   rblEnApproveAnon.Checked, rblEnNoAnon.Checked);
                Settings.EnrollAnonFields = listFields;

                listFields = EnrollListFields(rblEnUserView.Checked, rblEnDispView.Checked,
                                                   rblEnEmailView.Checked, rblEnPhoneView.Checked,
                                                   rblEnApproveView.Checked, rblEnNoView.Checked);
                Settings.EnrollViewFields = listFields;

                listFields = EnrollListFields(rblEnUserEdit.Checked, rblEnDispEdit.Checked,
                                                   rblEnEmailEdit.Checked, rblEnPhoneEdit.Checked,
                                                   rblEnApproveEdit.Checked, rblEnNoEdit.Checked);
                Settings.EnrollEditFields = listFields;

                if (rblSelectionTypeDays.Checked)
                {
                    Settings.EventsListSelectType = rblSelectionTypeDays.Value;
                }
                else
                {
                    Settings.EventsListSelectType = rblSelectionTypeEvents.Value;
                }
                if (rblListViewGrid.Items[0].Selected)
                {
                    Settings.ListViewGrid = true;
                }
                else
                {
                    Settings.ListViewGrid = false;
                }
                Settings.ListViewTable = chkListViewTable.Checked;
                Settings.RptColumns = int.Parse(txtRptColumns.Text.Trim());
                Settings.RptRows = int.Parse(txtRptRows.Text.Trim());

                if (rblShowHeader.Items[0].Selected)
                {
                    Settings.EventsListShowHeader = rblShowHeader.Items[0].Value;
                }
                else
                {
                    Settings.EventsListShowHeader = rblShowHeader.Items[1].Value;
                }
                Settings.EventsListBeforeDays = int.Parse(txtDaysBefore.Text.Trim());
                Settings.EventsListAfterDays = int.Parse(txtDaysAfter.Text.Trim());
                Settings.EventsListNumEvents = int.Parse(txtNumEvents.Text.Trim());
                Settings.EventsListEventDays = int.Parse(txtEventDays.Text.Trim());
                Settings.RestrictCategoriesToTimeFrame = chkRestrictCategoriesToTimeFrame.Checked;
                Settings.RestrictLocationsToTimeFrame = chkRestrictLocationsToTimeFrame.Checked;

                Settings.FBAdmins = txtFBAdmins.Text;
                Settings.FBAppID = txtFBAppID.Text;

                Settings.SendEnrollMessageApproved = chkEnrollMessageApproved.Checked;
                Settings.SendEnrollMessageWaiting = chkEnrollMessageWaiting.Checked;
                Settings.SendEnrollMessageDenied = chkEnrollMessageDenied.Checked;
                Settings.SendEnrollMessageAdded = chkEnrollMessageAdded.Checked;
                Settings.SendEnrollMessageDeleted = chkEnrollMessageDeleted.Checked;
                Settings.SendEnrollMessagePaying = chkEnrollMessagePaying.Checked;
                Settings.SendEnrollMessagePending = chkEnrollMessagePending.Checked;
                Settings.SendEnrollMessagePaid = chkEnrollMessagePaid.Checked;
                Settings.SendEnrollMessageIncorrect = chkEnrollMessageIncorrect.Checked;
                Settings.SendEnrollMessageCancelled = chkEnrollMessageCancelled.Checked;

                Settings.AllowAnonEnroll = chkAllowAnonEnroll.Checked;
                Settings.SocialGroupModule =
                    (EventModuleSettings.SocialModule) int.Parse(ddlSocialGroupModule.SelectedValue);
                if (Settings.SocialGroupModule != EventModuleSettings.SocialModule.No)
                {
                    Settings.Ownerchangeallowed = false;
                    Settings.Neweventemails = "Never";
                    Settings.MasterEvent = false;
                }
                if (Settings.SocialGroupModule == EventModuleSettings.SocialModule.UserProfile)
                {
                    Settings.EnableSitemap = false;
                    Settings.Eventsearch = false;
                    Settings.Eventsignup = false;
                    Settings.Moderateall = false;
                }
                Settings.SocialUserPrivate = chkSocialUserPrivate.Checked;
                Settings.SocialGroupSecurity =
                    (EventModuleSettings.SocialGroupPrivacy) int.Parse(ddlSocialGroupSecurity.SelectedValue);

                Settings.EnrolListSortDirection =
                    (SortDirection) int.Parse(ddlEnrolListSortDirection.SelectedValue);
                Settings.EnrolListDaysBefore = int.Parse(txtEnrolListDaysBefore.Text);
                Settings.EnrolListDaysAfter = int.Parse(txtEnrolListDaysAfter.Text);

                Settings.JournalIntegration = chkJournalIntegration.Checked;

                var objDesktopModule = DesktopModuleController.GetDesktopModuleByModuleName("DNN_Events", 0);
                Settings.Version = objDesktopModule.Version;

                var repository = new EventModuleSettingsRepository();
                repository.SaveSettings(ModuleConfiguration, Settings);

                CreateThemeDirectory();
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
            lstAssignedCals.DataTextField = "SubEventTitle";
            lstAssignedCals.DataValueField = "MasterID";
            lstAssignedCals.DataSource = null;
            lstAssignedCals.DataBind();
            lstAssignedCals.DataSource = _objCtlMasterEvent.EventsMasterAssignedModules(ModuleId);
            lstAssignedCals.DataBind();
        }

        /// <summary>
        ///     Get Avaiable Sub Events for Portal and Bind to DropDown
        /// </summary>
        /// <remarks></remarks>
        private void BindAvailableEvents()
        {
            lstAvailableCals.DataTextField = "SubEventTitle";
            lstAvailableCals.DataValueField = "SubEventID";
            lstAvailableCals.DataSource = null;
            lstAvailableCals.DataBind();
            lstAvailableCals.DataSource =
                _objCtlMasterEvent.EventsMasterAvailableModules(PortalId, ModuleId);
            lstAvailableCals.DataBind();
        }

        private void Enable_Disable_Cals()
        {
            divMasterEvent.Visible = chkMasterEvent.Checked;
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
            var objTabInfo = objCtlTab.GetTab(TabId, PortalId, false);
            string skinSrc = null;
            if (!(objTabInfo.SkinSrc == string.Empty))
            {
                skinSrc = objTabInfo.SkinSrc;
                if (skinSrc.Substring(skinSrc.Length - 5, 5) == ".ascx")
                {
                    skinSrc = skinSrc.Substring(0, skinSrc.Length - 5);
                }
            }
            var objCtlModule = new ModuleController();
            var objModuleInfo = objCtlModule.GetModule(ModuleId, TabId, false);
            string containerSrc = null;
            if (objModuleInfo.DisplayTitle)
            {
                if (!(objModuleInfo.ContainerSrc == string.Empty))
                {
                    containerSrc = objModuleInfo.ContainerSrc;
                }
                else if (!(objTabInfo.ContainerSrc == string.Empty))
                {
                    containerSrc = objTabInfo.ContainerSrc;
                }
                if (!string.IsNullOrEmpty(containerSrc))
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
            if (!string.IsNullOrEmpty(containerSrc))
            {
                url += addchar + "ContainerSrc=" + HttpUtility.HtmlEncode(containerSrc);
                addchar = "&";
            }
            if (!string.IsNullOrEmpty(skinSrc))
            {
                url += addchar + "SkinSrc=" + HttpUtility.HtmlEncode(skinSrc);
            }
            return url;
        }

        #endregion

        #region Links, Buttons and Events

        protected void chkMasterEvent_CheckedChanged(object sender, EventArgs e)
        {
            cmdRemoveAllCals_Click(sender, e);
            Enable_Disable_Cals();
        }

        protected void cmdAdd_Click(object sender, EventArgs e)
        {
            var objListItem = default(ListItem);

            var objList = new ArrayList();

            foreach (ListItem tempLoopVar_objListItem in lstAvailable.Items)
            {
                objListItem = tempLoopVar_objListItem;
                objList.Add(objListItem);
            }

            foreach (ListItem tempLoopVar_objListItem in objList)
            {
                objListItem = tempLoopVar_objListItem;
                if (objListItem.Selected)
                {
                    lstAvailable.Items.Remove(objListItem);
                    lstAssigned.Items.Add(objListItem);
                }
            }

            lstAvailable.ClearSelection();
            lstAssigned.ClearSelection();

            Sort(lstAssigned);
        }

        protected void cmdRemove_Click(object sender, EventArgs e)
        {
            var objListItem = default(ListItem);

            var objList = new ArrayList();

            foreach (ListItem tempLoopVar_objListItem in lstAssigned.Items)
            {
                objListItem = tempLoopVar_objListItem;
                objList.Add(objListItem);
            }

            foreach (ListItem tempLoopVar_objListItem in objList)
            {
                objListItem = tempLoopVar_objListItem;
                if (objListItem.Selected)
                {
                    lstAssigned.Items.Remove(objListItem);
                    lstAvailable.Items.Add(objListItem);
                }
            }

            lstAvailable.ClearSelection();
            lstAssigned.ClearSelection();

            Sort(lstAvailable);
        }

        protected void cmdAddAll_Click(object sender, EventArgs e)
        {
            var objListItem = default(ListItem);

            foreach (ListItem tempLoopVar_objListItem in lstAvailable.Items)
            {
                objListItem = tempLoopVar_objListItem;
                lstAssigned.Items.Add(objListItem);
            }

            lstAvailable.Items.Clear();

            lstAvailable.ClearSelection();
            lstAssigned.ClearSelection();

            Sort(lstAssigned);
        }

        protected void cmdRemoveAll_Click(object sender, EventArgs e)
        {
            var objListItem = default(ListItem);

            foreach (ListItem tempLoopVar_objListItem in lstAssigned.Items)
            {
                objListItem = tempLoopVar_objListItem;
                lstAvailable.Items.Add(objListItem);
            }

            lstAssigned.Items.Clear();

            lstAvailable.ClearSelection();
            lstAssigned.ClearSelection();

            Sort(lstAvailable);
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

            foreach (ListItem tempLoopVar_objListItem in lstAvailableCals.Items)
            {
                objListItem = tempLoopVar_objListItem;
                if (objListItem.Selected)
                {
                    masterEvent.MasterID = 0;
                    masterEvent.ModuleID = ModuleId;
                    masterEvent.SubEventID = Convert.ToInt32(objListItem.Value);
                    _objCtlMasterEvent.EventsMasterSave(masterEvent);
                }
            }

            BindSubEvents();
            BindAvailableEvents();
        }

        protected void cmdAddAllCals_Click(object sender, EventArgs e)
        {
            var objListItem = default(ListItem);
            var masterEvent = new EventMasterInfo();

            foreach (ListItem tempLoopVar_objListItem in lstAvailableCals.Items)
            {
                objListItem = tempLoopVar_objListItem;
                masterEvent.MasterID = 0;
                masterEvent.ModuleID = ModuleId;
                masterEvent.SubEventID = Convert.ToInt32(objListItem.Value);
                _objCtlMasterEvent.EventsMasterSave(masterEvent);
            }

            BindSubEvents();
            BindAvailableEvents();
        }

        protected void cmdRemoveCals_Click(object sender, EventArgs e)
        {
            var objListItem = default(ListItem);

            foreach (ListItem tempLoopVar_objListItem in lstAssignedCals.Items)
            {
                objListItem = tempLoopVar_objListItem;
                if (objListItem.Selected)
                {
                    _objCtlMasterEvent.EventsMasterDelete(int.Parse(objListItem.Value), ModuleId);
                }
            }

            BindSubEvents();
            BindAvailableEvents();
        }

        protected void cmdRemoveAllCals_Click(object sender, EventArgs e)
        {
            var objListItem = default(ListItem);

            foreach (ListItem tempLoopVar_objListItem in lstAssignedCals.Items)
            {
                objListItem = tempLoopVar_objListItem;
                _objCtlMasterEvent.EventsMasterDelete(int.Parse(objListItem.Value), ModuleId);
            }

            BindSubEvents();
            BindAvailableEvents();
        }

        protected void cmdUpdateTemplate_Click(object sender, EventArgs e)
        {
            var strTemplate = ddlTemplates.SelectedValue;
            Settings.Templates.SaveTemplate(ModuleId, strTemplate, txtEventTemplate.Text.Trim());
            lblTemplateUpdated.Visible = true;
            lblTemplateUpdated.Text =
                string.Format(Localization.GetString("TemplateUpdated", LocalResourceFile),
                              Localization.GetString(strTemplate + "Name", LocalResourceFile));
        }

        protected void cmdResetTemplate_Click(object sender, EventArgs e)
        {
            var strTemplate = ddlTemplates.SelectedValue;
            Settings.Templates.ResetTemplate(ModuleId, strTemplate, LocalResourceFile);
            txtEventTemplate.Text = Settings.Templates.GetTemplate(strTemplate);
            lblTemplateUpdated.Visible = true;
            lblTemplateUpdated.Text =
                string.Format(Localization.GetString("TemplateReset", LocalResourceFile),
                              Localization.GetString(strTemplate + "Name", LocalResourceFile));
        }

        protected void ddlTemplates_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtEventTemplate.Text = Settings.Templates.GetTemplate(ddlTemplates.SelectedValue);
            lblTemplateUpdated.Visible = false;
        }

        protected void cancelButton_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect(GetSocialNavigateUrl(), true);
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
                UpdateSettings();
                Response.Redirect(GetSocialNavigateUrl(), false);
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
            return string.Format("{0},{1},{2}", Convert.ToInt32(SettingType), ThemeName, ThemeFile);
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
            if (!ValidateSetting(setting))
            {
                throw new Exception("Setting is not right format to convert into ThemeSetting");
            }

            var s = setting.Split(',');
            SettingType = (ThemeSettingTypeEnum) int.Parse(s[0]);
            ThemeName = s[1];
            CssClass = "Theme" + ThemeName;
            switch (SettingType)
            {
                case ThemeSettingTypeEnum.DefaultTheme:
                    ThemeFile = string.Format("{0}/DesktopModules/Events/Themes/{1}/{1}.css",
                                                   Globals.ApplicationPath, ThemeName);
                    break;
                case ThemeSettingTypeEnum.CustomTheme:
                    var pc = new PortalController();
                    var with_1 = pc.GetPortal(portalID);
                    ThemeFile = string.Format("{0}/{1}/DNNEvents/Themes/{2}/{2}.css", Globals.ApplicationPath,
                                                   with_1.HomeDirectory, ThemeName);
                    break;
            }
        }

        #endregion
    }

    #endregion
}