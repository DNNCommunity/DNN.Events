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
    using System.Drawing;
    using System.Threading;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Localization;
    using global::Components;
    using Microsoft.VisualBasic;

    [DNNtc.ModuleControlProperties("Day", "Events Day", DNNtc.ControlType.View, "https://dnnevents.codeplex.com/documentation", true, true)]
    public partial class EventDay : EventBase
    {
        #region Helper Functions

        private void BindDataGrid()
        {
            var culture = Thread.CurrentThread.CurrentCulture;
            var startDate = default(DateTime); // Start View Date Events Range
            var endDate = default(DateTime); // End View Date Events Range
            var objEvent = default(EventInfo);
            var objEventInfoHelper = new EventInfoHelper(this.ModuleId, this.TabId, this.PortalId, this.Settings);
            var editButtonVisible = false;

            // Set Date Range
            var dDate = this.SelectedDate.Date;
            startDate = dDate.AddDays(-1);
            endDate = dDate.AddDays(1);

            // Get Events/Sub-Calendar Events
            var getSubEvents = this.Settings.MasterEvent;
            this._selectedEvents =
                objEventInfoHelper.GetEvents(startDate, endDate, getSubEvents, this.SelectCategory.SelectedCategory,
                                             this.SelectLocation.SelectedLocation, this.GetUrlGroupId(),
                                             this.GetUrlUserId());

            this._selectedEvents =
                objEventInfoHelper.ConvertEventListToDisplayTimeZone(this._selectedEvents, this.GetDisplayTimeZoneId());

            if (this._selectedEvents.Count == 0)
            {
                this.lstEvents.Visible = false;
                this.divMessage.Visible = true;
                return;
            }
            this.lstEvents.Visible = true;
            this.divMessage.Visible = false;

            // Get Date Events (used for Multiday event)
            var dayEvents = default(ArrayList);
            dayEvents = objEventInfoHelper.GetDateEvents(this._selectedEvents, dDate);

            var fmtEventTimeBegin = this.Settings.Templates.txtDayEventTimeBegin;
            if (string.IsNullOrEmpty(fmtEventTimeBegin))
            {
                fmtEventTimeBegin = "g";
            }

            var fmtEventTimeEnd = this.Settings.Templates.txtDayEventTimeEnd;
            if (string.IsNullOrEmpty(fmtEventTimeEnd))
            {
                fmtEventTimeEnd = "g";
            }

            var tmpDayDescription = this.Settings.Templates.txtDayEventDescription;
            var tmpDayLocation = this.Settings.Templates.txtDayLocation;

            if (this.Settings.Eventtooltipday)
            {
                this.toolTipManager.TargetControls.Clear();
            }

            var colEvents = new ArrayList();
            var lstEvent = default(EventListObject);
            foreach (EventInfo tempLoopVar_objEvent in dayEvents)
            {
                objEvent = tempLoopVar_objEvent;
                // If full enrollments should be hidden, ignore
                if (this.HideFullEvent(objEvent))
                {
                    continue;
                }

                var blAddEvent = true;
                if (this.Settings.Collapserecurring)
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
                    var objCtlEventRecurMaster = new EventRecurMasterController();
                    var tcc = new TokenReplaceControllerClass(this.ModuleId, this.LocalResourceFile);
                    var fmtRowEnd = "";
                    var fmtRowBegin = "";
                    fmtRowEnd = tcc.TokenParameters(fmtEventTimeEnd, objEvent, this.Settings);
                    fmtRowBegin = tcc.TokenParameters(fmtEventTimeBegin, objEvent, this.Settings);

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

                    var isEvtEditor = this.IsEventEditor(objEvent, false);

                    var templatedescr = "";
                    var iconString = "";

                    if (!this.IsPrivateNotModerator || this.UserId == objEvent.OwnerID)
                    {
                        templatedescr = tcc.TokenReplaceEvent(objEvent, tmpDayDescription, null, false, isEvtEditor);
                        lstEvent.CategoryColor = this.GetColor(objEvent.Color);
                        lstEvent.CategoryFontColor = this.GetColor(objEvent.FontColor);

                        iconString = this.CreateIconString(objEvent, this.Settings.IconListPrio,
                                                           this.Settings.IconListRec, this.Settings.IconListReminder,
                                                           this.Settings.IconListEnroll);
                    }

                    lstEvent.EventName = this.CreateEventName(objEvent, "[event:title]");
                    lstEvent.EventDesc = objEvent.EventDesc;
                    // RWJS - not sure why replace ' with \' - lstEvent.DecodedDesc = System.Web.HttpUtility.HtmlDecode(objEvent.EventDesc).Replace(Environment.NewLine, "").Trim.Replace("'", "\'")
                    lstEvent.DecodedDesc =
                        Convert.ToString(HttpUtility.HtmlDecode(templatedescr).Replace(Environment.NewLine, ""));
                    lstEvent.EventID = objEvent.EventID;
                    lstEvent.ModuleID = objEvent.ModuleID;

                    var objEventRRULE = default(EventRRULEInfo);
                    objEventRRULE = objCtlEventRecurMaster.DecomposeRRULE(objEvent.RRULE, objEvent.EventTimeBegin);
                    lstEvent.RecurText =
                        objCtlEventRecurMaster.RecurrenceText(objEventRRULE, this.LocalResourceFile, culture,
                                                              objEvent.EventTimeBegin);
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
                    if (this.Settings.Eventimage && objEvent.ImageURL != null && objEvent.ImageDisplay)
                    {
                        lstEvent.ImageURL =
                            this.ImageInfo(objEvent.ImageURL, objEvent.ImageHeight, objEvent.ImageWidth);
                    }

                    // Get detail page url
                    lstEvent.URL = objEventInfoHelper.DetailPageURL(objEvent);
                    if (objEvent.DetailPage && objEvent.DetailNewWin)
                    {
                        lstEvent.Target = "_blank";
                    }

                    lstEvent.Icons = iconString;
                    lstEvent.DisplayDuration = Convert.ToInt32(Conversion.Int((double) objEvent.Duration / 1440 + 1));
                    lstEvent.CategoryName = objEvent.CategoryName;
                    lstEvent.LocationName = tcc.TokenReplaceEvent(objEvent, tmpDayLocation);
                    lstEvent.CustomField1 = objEvent.CustomField1;
                    lstEvent.CustomField2 = objEvent.CustomField2;
                    lstEvent.RecurMasterID = objEvent.RecurMasterID;

                    if (this.Settings.Eventtooltipday)
                    {
                        lstEvent.Tooltip =
                            this.ToolTipCreate(objEvent, this.Settings.Templates.txtTooltipTemplateTitle,
                                               this.Settings.Templates.txtTooltipTemplateBody, isEvtEditor);
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
            if (!this.IsPrivateNotModerator)
            {
                if (this.Settings.EventsListFields.LastIndexOf("EB", StringComparison.Ordinal) < 0 ||
                    editButtonVisible == false)
                {
                    this.lstEvents.Columns[0].Visible = false;
                }
                else
                {
                    this.lstEvents.Columns[0].Visible = true;
                }
                if (this.Settings.EventsListFields.LastIndexOf("BD", StringComparison.Ordinal) < 0)
                {
                    this.lstEvents.Columns[1].Visible = false;
                }
                if (this.Settings.EventsListFields.LastIndexOf("ED", StringComparison.Ordinal) < 0)
                {
                    this.lstEvents.Columns[2].Visible = false;
                }
                if (this.Settings.EventsListFields.LastIndexOf("EN", StringComparison.Ordinal) < 0)
                {
                    this.lstEvents.Columns[3].Visible = false;
                }
                if (this.Settings.EventsListFields.LastIndexOf("IM", StringComparison.Ordinal) < 0)
                {
                    this.lstEvents.Columns[4].Visible = false;
                }
                if (this.Settings.EventsListFields.LastIndexOf("DU", StringComparison.Ordinal) < 0)
                {
                    this.lstEvents.Columns[5].Visible = false;
                }
                if (this.Settings.EventsListFields.LastIndexOf("CA", StringComparison.Ordinal) < 0)
                {
                    this.lstEvents.Columns[6].Visible = false;
                }
                if (this.Settings.EventsListFields.LastIndexOf("LO", StringComparison.Ordinal) < 0)
                {
                    this.lstEvents.Columns[7].Visible = false;
                }
                if (!this.Settings.EventsCustomField1 ||
                    this.Settings.EventsListFields.LastIndexOf("C1", StringComparison.Ordinal) < 0)
                {
                    this.lstEvents.Columns[8].Visible = false;
                }
                if (!this.Settings.EventsCustomField2 ||
                    this.Settings.EventsListFields.LastIndexOf("C2", StringComparison.Ordinal) < 0)
                {
                    this.lstEvents.Columns[9].Visible = false;
                }
                if (this.Settings.EventsListFields.LastIndexOf("DE", StringComparison.Ordinal) < 0)
                {
                    this.lstEvents.Columns[10].Visible = false;
                }
                if (this.Settings.EventsListFields.LastIndexOf("RT", StringComparison.Ordinal) < 0)
                {
                    this.lstEvents.Columns[11].Visible = false;
                }
                if (this.Settings.EventsListFields.LastIndexOf("RU", StringComparison.Ordinal) < 0)
                {
                    this.lstEvents.Columns[12].Visible = false;
                }
            }
            else
            {
                // Set Defaults
                this.lstEvents.Columns[0].Visible = false; // Edit Buttom
                this.lstEvents.Columns[1].Visible = true; // Begin Date
                this.lstEvents.Columns[2].Visible = true; // End Date
                this.lstEvents.Columns[3].Visible = true; // Title
                this.lstEvents.Columns[4].Visible = false; // Image
                this.lstEvents.Columns[5].Visible = false; // Duration
                this.lstEvents.Columns[6].Visible = false; // Category
                this.lstEvents.Columns[7].Visible = false; // Location
                this.lstEvents.Columns[8].Visible = false; // Custom Field 1
                this.lstEvents.Columns[9].Visible = false; // Custom Field 2
                this.lstEvents.Columns[10].Visible = false; // Description
                this.lstEvents.Columns[11].Visible = false; // Recurrence Pattern
                this.lstEvents.Columns[12].Visible = false; // Recur Until
            }

            this.lstEvents.DataSource = colEvents;
            this.lstEvents.DataBind();
        }

        #endregion

        #region Event Handlers

        private ArrayList _selectedEvents;

        private void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetupViewControls(this.EventIcons, this.EventIcons2, this.SelectCategory, this.SelectLocation);

                if (this.Settings.Eventdaynewpage)
                {
                    this.SetTheme(this.pnlEventsModuleDay);
                    this.AddFacebookMetaTags();
                }

                //Show header - or not
                if (this.Settings.EventsListShowHeader == "Yes")
                {
                    this.lstEvents.ShowHeader = true;
                }
                else
                {
                    this.lstEvents.ShowHeader = false;
                }

                if (this.Page.IsPostBack == false)
                {
                    if (this.Settings.EventsListShowHeader != "No")
                    {
                        this.lstEvents.ShowHeader = true;
                        Localization.LocalizeDataGrid(ref this.lstEvents, this.LocalResourceFile);
                    }
                    this.BindDataGrid();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion

        #region Control Events

        protected void lstEvents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if ((e.Item.ItemType == ListItemType.Item) | (e.Item.ItemType == ListItemType.AlternatingItem))
            {
                var lnkevent = (HyperLink) e.Item.FindControl("lnkEvent");
                if (this.Settings.Eventtooltipday)
                {
                    var tooltip = Convert.ToString(DataBinder.Eval(e.Item.DataItem, "Tooltip"));
                    e.Item.Attributes.Add("title", tooltip);
                    this.toolTipManager.TargetControls.Add(e.Item.ClientID, true);
                }
                var backColor = (Color) DataBinder.Eval(e.Item.DataItem, "CategoryColor");
                if (backColor.Name != "0")
                {
                    for (var i = 0; i <= e.Item.Cells.Count - 1; i++)
                    {
                        if (e.Item.Cells[i].Visible && !(this.lstEvents.Columns[i].SortExpression == "Description"))
                        {
                            e.Item.Cells[i].BackColor = backColor;
                        }
                    }
                }
                if (this.IsPrivateNotModerator &&
                    !(this.UserId == Convert.ToInt32(DataBinder.Eval(e.Item.DataItem, "OwnerID"))))
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
                        var iItemID = Convert.ToInt32(e.CommandArgument);
                        var objEventInfoHelper =
                            new EventInfoHelper(this.ModuleId, this.TabId, this.PortalId, this.Settings);
                        this.Response.Redirect(
                            objEventInfoHelper.GetEditURL(iItemID, this.GetUrlGroupId(), this.GetUrlUserId()));
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
            this.SelectLocation.StoreLocations();
            this.BindDataGrid();
        }

        protected void SelectLocation_LocationSelected(object sender, CommandEventArgs e)
        {
            //Store the other selection(s) too.
            this.SelectCategory.StoreCategories();
            this.BindDataGrid();
        }

        protected void returnButton_Click(object sender, EventArgs e)
        {
            this.Response.Redirect(this.GetSocialNavigateUrl(), true);
        }

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
    }
}