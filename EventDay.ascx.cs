using DotNetNuke.Services.Exceptions;
using System.Diagnostics;
using System.Web.UI;
using System.Drawing;
using Microsoft.VisualBasic;
using System.Web.UI.WebControls;
using System.Collections;
using System.Web;
using DotNetNuke.Services.Localization;
using System;
using System.Globalization;


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

    [DNNtc.ModuleControlProperties("Day", "Events Day", DNNtc.ControlType.View, "https://dnnevents.codeplex.com/documentation", true, true)]
    public partial class EventDay : EventBase
    {

        #region Event Handlers
        private ArrayList _selectedEvents;

        private void Page_Load(System.Object sender, EventArgs e)
        {
            try
            {
                SetupViewControls(EventIcons, EventIcons2, SelectCategory, SelectLocation);

                if (Settings.Eventdaynewpage)
                {
                    SetTheme(pnlEventsModuleDay);
                    AddFacebookMetaTags();
                }

                //Show header - or not
                if (Settings.EventsListShowHeader == "Yes")
                {
                    lstEvents.ShowHeader = true;
                }
                else
                {
                    lstEvents.ShowHeader = false;
                }

                if (Page.IsPostBack == false)
                {
                    if (Settings.EventsListShowHeader != "No")
                    {
                        lstEvents.ShowHeader = true;
                        Localization.LocalizeDataGrid(ref lstEvents, LocalResourceFile);
                    }
                    BindDataGrid();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        #endregion

        #region Helper Functions
        private void BindDataGrid()
        {
            CultureInfo culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            DateTime startDate = default(DateTime); // Start View Date Events Range
            DateTime endDate = default(DateTime); // End View Date Events Range
            EventInfo objEvent = default(EventInfo);
            EventInfoHelper objEventInfoHelper = new EventInfoHelper(ModuleId, TabId, PortalId, Settings);
            bool editButtonVisible = false;

            // Set Date Range
            DateTime dDate = SelectedDate.Date;
            startDate = dDate.AddDays(-1);
            endDate = dDate.AddDays(1);

            // Get Events/Sub-Calendar Events
            bool getSubEvents = Settings.MasterEvent;
            _selectedEvents = objEventInfoHelper.GetEvents(startDate, endDate, getSubEvents, SelectCategory.SelectedCategory, SelectLocation.SelectedLocation, GetUrlGroupId(), GetUrlUserId());

            _selectedEvents = objEventInfoHelper.ConvertEventListToDisplayTimeZone(_selectedEvents, GetDisplayTimeZoneId());

            if (_selectedEvents.Count == 0)
            {
                lstEvents.Visible = false;
                divMessage.Visible = true;
                return;
            }
            else
            {
                lstEvents.Visible = true;
                divMessage.Visible = false;
            }

            // Get Date Events (used for Multiday event)
            ArrayList dayEvents = default(ArrayList);
            dayEvents = objEventInfoHelper.GetDateEvents(_selectedEvents, dDate);

            string fmtEventTimeBegin = Settings.Templates.txtDayEventTimeBegin;
            if (string.IsNullOrEmpty(fmtEventTimeBegin))
            {
                fmtEventTimeBegin = "g";
            }

            string fmtEventTimeEnd = Settings.Templates.txtDayEventTimeEnd;
            if (string.IsNullOrEmpty(fmtEventTimeEnd))
            {
                fmtEventTimeEnd = "g";
            }

            string tmpDayDescription = Settings.Templates.txtDayEventDescription;
            string tmpDayLocation = Settings.Templates.txtDayLocation;

            if (Settings.Eventtooltipday)
            {
                toolTipManager.TargetControls.Clear();
            }

            ArrayList colEvents = new ArrayList();
            EventListObject lstEvent = default(EventListObject);
            foreach (EventInfo tempLoopVar_objEvent in dayEvents)
            {
                objEvent = tempLoopVar_objEvent;
                // If full enrollments should be hidden, ignore
                if (HideFullEvent(objEvent))
                {
                    continue;
                }

                bool blAddEvent = true;
                if (Settings.Collapserecurring)
                {
                    foreach (EventListObject tempLoopVar_lstEvent in colEvents)
                    {
                        lstEvent = tempLoopVar_lstEvent;
                        if (lstEvent.RecurMasterID == objEvent.RecurMasterID)
                        {
                            blAddEvent = false;
                        }
                    }
                }
                if (blAddEvent)
                {
                    EventRecurMasterController objCtlEventRecurMaster = new EventRecurMasterController();
                    TokenReplaceControllerClass tcc = new TokenReplaceControllerClass(ModuleId, LocalResourceFile);
                    string fmtRowEnd = "";
                    string fmtRowBegin = "";
                    fmtRowEnd = tcc.TokenParameters(fmtEventTimeEnd, objEvent, Settings);
                    fmtRowBegin = tcc.TokenParameters(fmtEventTimeBegin, objEvent, Settings);

                    lstEvent = new EventListObject();
                    lstEvent.EventID = objEvent.EventID;
                    lstEvent.CreatedByID = objEvent.CreatedByID;
                    lstEvent.OwnerID = objEvent.OwnerID;
                    lstEvent.EventDateBegin = objEvent.EventTimeBegin;
                    lstEvent.EventDateEnd = objEvent.EventTimeEnd;
                    if (objEvent.DisplayEndDate)
                    {
                        lstEvent.TxtEventDateEnd = string.Format("{0:" + fmtRowEnd + "}", lstEvent.EventDateEnd);
                    }
                    else
                    {
                        lstEvent.TxtEventDateEnd = "";
                    }
                    lstEvent.EventTimeBegin = objEvent.EventTimeBegin;
                    lstEvent.TxtEventTimeBegin = string.Format("{0:" + fmtRowBegin + "}", lstEvent.EventTimeBegin);
                    lstEvent.Duration = objEvent.Duration;

                    bool isEvtEditor = IsEventEditor(objEvent, false);

                    string templatedescr = "";
                    string iconString = "";

                    if (!IsPrivateNotModerator || UserId == objEvent.OwnerID)
                    {
                        templatedescr = tcc.TokenReplaceEvent(objEvent, tmpDayDescription, null, false, isEvtEditor);
                        lstEvent.CategoryColor = GetColor(objEvent.Color);
                        lstEvent.CategoryFontColor = GetColor(objEvent.FontColor);

                        iconString = CreateIconString(objEvent, Settings.IconListPrio, Settings.IconListRec, Settings.IconListReminder, Settings.IconListEnroll);
                    }

                    lstEvent.EventName = CreateEventName(objEvent, "[event:title]");
                    lstEvent.EventDesc = objEvent.EventDesc;
                    // RWJS - not sure why replace ' with \' - lstEvent.DecodedDesc = System.Web.HttpUtility.HtmlDecode(objEvent.EventDesc).Replace(Environment.NewLine, "").Trim.Replace("'", "\'")
                    lstEvent.DecodedDesc = System.Convert.ToString(HttpUtility.HtmlDecode(templatedescr).Replace(Environment.NewLine, ""));
                    lstEvent.EventID = objEvent.EventID;
                    lstEvent.ModuleID = objEvent.ModuleID;

                    EventRRULEInfo objEventRRULE = default(EventRRULEInfo);
                    objEventRRULE = objCtlEventRecurMaster.DecomposeRRULE(objEvent.RRULE, objEvent.EventTimeBegin);
                    lstEvent.RecurText = objCtlEventRecurMaster.RecurrenceText(objEventRRULE, LocalResourceFile, culture, objEvent.EventTimeBegin);
                    if (objEvent.RRULE != "")
                    {
                        lstEvent.RecurUntil = objEvent.LastRecurrence.ToShortDateString();
                    }
                    else
                    {
                        lstEvent.RecurUntil = "";
                    }
                    lstEvent.EventID = objEvent.EventID;
                    lstEvent.ModuleID = objEvent.ModuleID;

                    lstEvent.ImageURL = "";
                    if (Settings.Eventimage && objEvent.ImageURL != null && objEvent.ImageDisplay == true)
                    {
                        lstEvent.ImageURL = ImageInfo(objEvent.ImageURL, objEvent.ImageHeight, objEvent.ImageWidth);
                    }

                    // Get detail page url
                    lstEvent.URL = objEventInfoHelper.DetailPageURL(objEvent);
                    if (objEvent.DetailPage && objEvent.DetailNewWin)
                    {
                        lstEvent.Target = "_blank";
                    }

                    lstEvent.Icons = iconString;
                    lstEvent.DisplayDuration = System.Convert.ToInt32(Conversion.Int((double)objEvent.Duration / 1440 + 1));
                    lstEvent.CategoryName = objEvent.CategoryName;
                    lstEvent.LocationName = tcc.TokenReplaceEvent(objEvent, tmpDayLocation);
                    lstEvent.CustomField1 = objEvent.CustomField1;
                    lstEvent.CustomField2 = objEvent.CustomField2;
                    lstEvent.RecurMasterID = objEvent.RecurMasterID;

                    if (Settings.Eventtooltipday)
                    {
                        lstEvent.Tooltip = ToolTipCreate(objEvent, Settings.Templates.txtTooltipTemplateTitle, Settings.Templates.txtTooltipTemplateBody, isEvtEditor);
                    }

                    lstEvent.EditVisibility = false;
                    if (isEvtEditor)
                    {
                        lstEvent.EditVisibility = true;
                        editButtonVisible = true;
                    }

                    colEvents.Add(lstEvent);
                }
            }

            //Determine which fields get displayed
            if (!IsPrivateNotModerator)
            {
                if (Settings.EventsListFields.LastIndexOf("EB", StringComparison.Ordinal) < 0 || editButtonVisible == false)
                {
                    lstEvents.Columns[0].Visible = false;
                }
                else
                {
                    lstEvents.Columns[0].Visible = true;
                }
                if (Settings.EventsListFields.LastIndexOf("BD", StringComparison.Ordinal) < 0)
                {
                    lstEvents.Columns[1].Visible = false;
                }
                if (Settings.EventsListFields.LastIndexOf("ED", StringComparison.Ordinal) < 0)
                {
                    lstEvents.Columns[2].Visible = false;
                }
                if (Settings.EventsListFields.LastIndexOf("EN", StringComparison.Ordinal) < 0)
                {
                    lstEvents.Columns[3].Visible = false;
                }
                if (Settings.EventsListFields.LastIndexOf("IM", StringComparison.Ordinal) < 0)
                {
                    lstEvents.Columns[4].Visible = false;
                }
                if (Settings.EventsListFields.LastIndexOf("DU", StringComparison.Ordinal) < 0)
                {
                    lstEvents.Columns[5].Visible = false;
                }
                if (Settings.EventsListFields.LastIndexOf("CA", StringComparison.Ordinal) < 0)
                {
                    lstEvents.Columns[6].Visible = false;
                }
                if (Settings.EventsListFields.LastIndexOf("LO", StringComparison.Ordinal) < 0)
                {
                    lstEvents.Columns[7].Visible = false;
                }
                if (!Settings.EventsCustomField1 || Settings.EventsListFields.LastIndexOf("C1", StringComparison.Ordinal) < 0)
                {
                    lstEvents.Columns[8].Visible = false;
                }
                if (!Settings.EventsCustomField2 || Settings.EventsListFields.LastIndexOf("C2", StringComparison.Ordinal) < 0)
                {
                    lstEvents.Columns[9].Visible = false;
                }
                if (Settings.EventsListFields.LastIndexOf("DE", StringComparison.Ordinal) < 0)
                {
                    lstEvents.Columns[10].Visible = false;
                }
                if (Settings.EventsListFields.LastIndexOf("RT", StringComparison.Ordinal) < 0)
                {
                    lstEvents.Columns[11].Visible = false;
                }
                if (Settings.EventsListFields.LastIndexOf("RU", StringComparison.Ordinal) < 0)
                {
                    lstEvents.Columns[12].Visible = false;
                }
            }
            else
            {
                // Set Defaults
                lstEvents.Columns[0].Visible = false; // Edit Buttom
                lstEvents.Columns[1].Visible = true; // Begin Date
                lstEvents.Columns[2].Visible = true; // End Date
                lstEvents.Columns[3].Visible = true; // Title
                lstEvents.Columns[4].Visible = false; // Image
                lstEvents.Columns[5].Visible = false; // Duration
                lstEvents.Columns[6].Visible = false; // Category
                lstEvents.Columns[7].Visible = false; // Location
                lstEvents.Columns[8].Visible = false; // Custom Field 1
                lstEvents.Columns[9].Visible = false; // Custom Field 2
                lstEvents.Columns[10].Visible = false; // Description
                lstEvents.Columns[11].Visible = false; // Recurrence Pattern
                lstEvents.Columns[12].Visible = false; // Recur Until
            }

            lstEvents.DataSource = colEvents;
            lstEvents.DataBind();
        }
        #endregion

        #region Control Events
        protected void lstEvents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item | e.Item.ItemType == ListItemType.AlternatingItem)
            {
                HyperLink lnkevent = (HyperLink)(e.Item.FindControl("lnkEvent"));
                if (Settings.Eventtooltipday)
                {
                    string tooltip = System.Convert.ToString(DataBinder.Eval(e.Item.DataItem, "Tooltip"));
                    e.Item.Attributes.Add("title", tooltip);
                    toolTipManager.TargetControls.Add(e.Item.ClientID, true);
                }
                Color backColor = (Color)(DataBinder.Eval(e.Item.DataItem, "CategoryColor"));
                if (backColor.Name != "0")
                {
                    for (int i = 0; i <= e.Item.Cells.Count - 1; i++)
                    {
                        if (e.Item.Cells[i].Visible && !(lstEvents.Columns[i].SortExpression == "Description"))
                        {
                            e.Item.Cells[i].BackColor = backColor;
                        }
                    }
                }
                if (IsPrivateNotModerator && !(UserId == System.Convert.ToInt32(DataBinder.Eval(e.Item.DataItem, "OwnerID"))))
                {
                    lnkevent.Style.Add("cursor", "text");
                    lnkevent.Style.Add("text-decoration", "none");
                    lnkevent.Attributes.Add("onclick", "javascript:return false;");
                }
            }
        }

        protected void lstEvents_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            try
            {
                switch (e.CommandName)
                {
                    case "Edit":
                        //set selected row editable
                        int iItemID = System.Convert.ToInt32(e.CommandArgument);
                        EventInfoHelper objEventInfoHelper = new EventInfoHelper(ModuleId, TabId, PortalId, Settings);
                        Response.Redirect(objEventInfoHelper.GetEditURL(iItemID, GetUrlGroupId(), GetUrlUserId()));
                        break;
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void SelectCategory_CategorySelected(object sender, CommandEventArgs e)
        {
            //Store the other selection(s) too.
            SelectLocation.StoreLocations();
            BindDataGrid();
        }
        protected void SelectLocation_LocationSelected(object sender, CommandEventArgs e)
        {
            //Store the other selection(s) too.
            SelectCategory.StoreCategories();
            BindDataGrid();
        }

        protected void returnButton_Click(System.Object sender, EventArgs e)
        {
            Response.Redirect(GetSocialNavigateUrl(), true);
        }
        #endregion

        #region  Web Form Designer Generated Code

        //This call is required by the Web Form Designer.
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {

        }

        private void Page_Init(System.Object sender, EventArgs e)
        {
            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            InitializeComponent();
        }

        #endregion

    }

}


