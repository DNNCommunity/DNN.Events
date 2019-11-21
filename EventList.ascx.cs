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
using System.Threading;
using System.Web;
using System.Web.UI.WebControls;
using Components;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using Microsoft.VisualBasic;

namespace DotNetNuke.Modules.Events
{
    public partial class EventList : EventBase
    {
        #region Event Handlers

        private ArrayList _selectedEvents;


        private void Page_Load(object sender, EventArgs e)
        {
            try
            {
                SetupViewControls((EventIcons)EventIcons, (EventIcons)EventIcons2, (SelectCategory)SelectCategory, (SelectLocation)SelectLocation);

                gvEvents.PageSize = Settings.EventsListPageSize;

                if (Page.IsPostBack == false)
                {
                    if (Settings.EventsListShowHeader != "No")
                    {
                        gvEvents.ShowHeader = true;
                        Localization.LocalizeGridView(ref gvEvents, LocalResourceFile);
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
            //Default sort from settings
            var sortDirection = default(SortDirection);
            if (Settings.EventsListSortDirection == "ASC")
            {
                sortDirection = SortDirection.Ascending;
            }
            else
            {
                sortDirection = SortDirection.Descending;
            }

            var sortExpression = GetListSortExpression(Settings.EventsListSortColumn);

            //Show header - or not
            if (Settings.EventsListShowHeader == "Yes")
            {
                gvEvents.ShowHeader = true;
            }
            else
            {
                gvEvents.ShowHeader = false;
            }

            //Get cached sort settings
            if (!ReferenceEquals(ViewState["SortExpression"], null) &&
                ViewState["SortExpression"] is EventListObject.SortFilter)
            {
                sortExpression = (EventListObject.SortFilter) ViewState["SortExpression"];
            }
            if (!ReferenceEquals(ViewState["SortDirection"], null) &&
                ViewState["SortDirection"] is SortDirection)
            {
                sortDirection = (SortDirection) ViewState["SortDirection"];
            }

            BindDataGrid(sortExpression, sortDirection);
        }

        private void BindDataGrid(EventListObject.SortFilter sortExpression, SortDirection sortDirection)
        {
            var culture = Thread.CurrentThread.CurrentCulture;
            var objEvent = default(EventInfo);
            var objEventInfoHelper = new EventInfoHelper(ModuleId, TabId, PortalId, Settings);
            var editColumnVisible = false;

            // Get Events/Sub-Calendar Events
            _selectedEvents = Get_ListView_Events(((SelectCategory)SelectCategory).SelectedCategory,
                                                            ((SelectLocation)SelectLocation).SelectedLocation);

            var fmtEventTimeBegin = Settings.Templates.txtListEventTimeBegin;
            if (string.IsNullOrEmpty(fmtEventTimeBegin))
            {
                fmtEventTimeBegin = "g";
            }

            var fmtEventTimeEnd = Settings.Templates.txtListEventTimeEnd;
            if (string.IsNullOrEmpty(fmtEventTimeEnd))
            {
                fmtEventTimeEnd = "g";
            }

            var tmpListDescription = Settings.Templates.txtListEventDescription;
            var tmpListLocation = Settings.Templates.txtListLocation;

            if (_selectedEvents.Count == 0)
            {
                gvEvents.Visible = false;
                divNoEvents.Visible = true;
                return;
            }
            gvEvents.Visible = true;
            divNoEvents.Visible = false;

            if (Settings.Eventtooltiplist)
            {
                toolTipManager.TargetControls.Clear();
            }

            // if Events Selection Type only get the 1st N Events
            var colEvents = new ArrayList();
            var lstEvent = default(EventListObject);
            var indexID = 0;
            foreach (EventInfo tempLoopVar_objEvent in _selectedEvents)
            {
                objEvent = tempLoopVar_objEvent;
                var tcc = new TokenReplaceControllerClass(ModuleId, LocalResourceFile);
                var objCtlEventRecurMaster = new EventRecurMasterController();
                var fmtRowEnd = "";
                var fmtRowBegin = "";
                fmtRowEnd = tcc.TokenParameters(fmtEventTimeEnd, objEvent, Settings);
                fmtRowBegin = tcc.TokenParameters(fmtEventTimeBegin, objEvent, Settings);

                lstEvent = new EventListObject();
                lstEvent.EventID = objEvent.EventID;
                lstEvent.CreatedByID = objEvent.CreatedByID;
                lstEvent.OwnerID = objEvent.OwnerID;
                lstEvent.IndexId = indexID;
                // Get Dates (automatically converted to User's Timezone)
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

                var isEvtEditor = IsEventEditor(objEvent, false);

                var templatedescr = "";
                var iconString = "";

                if (!IsPrivateNotModerator || UserId == objEvent.OwnerID)
                {
                    templatedescr = tcc.TokenReplaceEvent(objEvent, tmpListDescription, null, false, isEvtEditor);
                    lstEvent.CategoryColor = GetColor(objEvent.Color);
                    lstEvent.CategoryFontColor = GetColor(objEvent.FontColor);

                    iconString = CreateIconString(objEvent, Settings.IconListPrio, Settings.IconListRec,
                                                       Settings.IconListReminder, Settings.IconListEnroll);
                }

                lstEvent.EventName = CreateEventName(objEvent, "[event:title]");
                lstEvent.EventDesc = objEvent.EventDesc;
                // RWJS - not sure why replace ' with \' - lstEvent.DecodedDesc = System.Web.HttpUtility.HtmlDecode(objEvent.EventDesc).Replace(Environment.NewLine, "").Trim.Replace("'", "\'")
                lstEvent.DecodedDesc =
                    Convert.ToString(HttpUtility.HtmlDecode(templatedescr).Replace(Environment.NewLine, ""));

                var objEventRRULE = default(EventRRULEInfo);
                objEventRRULE = objCtlEventRecurMaster.DecomposeRRULE(objEvent.RRULE, objEvent.EventTimeBegin);
                lstEvent.RecurText =
                    objCtlEventRecurMaster.RecurrenceText(objEventRRULE, LocalResourceFile, culture,
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
                if (Settings.Eventimage && objEvent.ImageURL != null && objEvent.ImageDisplay)
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
                lstEvent.DisplayDuration = Convert.ToInt32(Conversion.Int((double) objEvent.Duration / 1440 + 1));
                lstEvent.CategoryName = objEvent.CategoryName;
                lstEvent.LocationName = tcc.TokenReplaceEvent(objEvent, tmpListLocation);
                lstEvent.CustomField1 = objEvent.CustomField1;
                lstEvent.CustomField2 = objEvent.CustomField2;
                lstEvent.RecurMasterID = objEvent.RecurMasterID;

                if (Settings.Eventtooltiplist)
                {
                    lstEvent.Tooltip = ToolTipCreate(objEvent, Settings.Templates.txtTooltipTemplateTitle,
                                                          Settings.Templates.txtTooltipTemplateBody, isEvtEditor);
                }

                lstEvent.EditVisibility = false;
                if (isEvtEditor)
                {
                    lstEvent.EditVisibility = true;
                    editColumnVisible = true;
                }

                colEvents.Add(lstEvent);
                indexID++;
            }

            //Determine which fields get displayed
            if (!IsPrivateNotModerator)
            {
                if (Settings.EventsListFields.LastIndexOf("EB", StringComparison.Ordinal) < 0 ||
                    editColumnVisible == false)
                {
                    gvEvents.Columns[0].Visible = false;
                }
                else
                {
                    gvEvents.Columns[0].Visible = true;
                }
                if (Settings.EventsListFields.LastIndexOf("BD", StringComparison.Ordinal) < 0)
                {
                    gvEvents.Columns[1].Visible = false;
                }
                if (Settings.EventsListFields.LastIndexOf("ED", StringComparison.Ordinal) < 0)
                {
                    gvEvents.Columns[2].Visible = false;
                }
                if (Settings.EventsListFields.LastIndexOf("EN", StringComparison.Ordinal) < 0)
                {
                    gvEvents.Columns[3].Visible = false;
                }
                if (Settings.EventsListFields.LastIndexOf("IM", StringComparison.Ordinal) < 0)
                {
                    gvEvents.Columns[4].Visible = false;
                }
                if (Settings.EventsListFields.LastIndexOf("DU", StringComparison.Ordinal) < 0)
                {
                    gvEvents.Columns[5].Visible = false;
                }
                if (Settings.EventsListFields.LastIndexOf("CA", StringComparison.Ordinal) < 0)
                {
                    gvEvents.Columns[6].Visible = false;
                }
                if (Settings.EventsListFields.LastIndexOf("LO", StringComparison.Ordinal) < 0)
                {
                    gvEvents.Columns[7].Visible = false;
                }
                if (!Settings.EventsCustomField1 ||
                    Settings.EventsListFields.LastIndexOf("C1", StringComparison.Ordinal) < 0)
                {
                    gvEvents.Columns[8].Visible = false;
                }
                if (!Settings.EventsCustomField2 ||
                    Settings.EventsListFields.LastIndexOf("C2", StringComparison.Ordinal) < 0)
                {
                    gvEvents.Columns[9].Visible = false;
                }
                if (Settings.EventsListFields.LastIndexOf("DE", StringComparison.Ordinal) < 0)
                {
                    gvEvents.Columns[10].Visible = false;
                }
                if (Settings.EventsListFields.LastIndexOf("RT", StringComparison.Ordinal) < 0)
                {
                    gvEvents.Columns[11].Visible = false;
                }
                if (Settings.EventsListFields.LastIndexOf("RU", StringComparison.Ordinal) < 0)
                {
                    gvEvents.Columns[12].Visible = false;
                }
            }
            else
            {
                // Set Defaults
                gvEvents.Columns[0].Visible = false; // Edit Buttom
                gvEvents.Columns[1].Visible = true; // Begin Date
                gvEvents.Columns[2].Visible = true; // End Date
                gvEvents.Columns[3].Visible = true; // Title
                gvEvents.Columns[4].Visible = false; // Image
                gvEvents.Columns[5].Visible = false; // Duration
                gvEvents.Columns[6].Visible = false; // Category
                gvEvents.Columns[7].Visible = false; // Location
                gvEvents.Columns[8].Visible = false; // Custom Field 1
                gvEvents.Columns[9].Visible = false; // Custom Field 2
                gvEvents.Columns[10].Visible = false; // Description
                gvEvents.Columns[11].Visible = false; // Recurrence Pattern
                gvEvents.Columns[12].Visible = false; // Recur Until
            }

            EventListObject.SortExpression = sortExpression;
            EventListObject.SortDirection = sortDirection;
            colEvents.Sort();

            gvEvents.DataKeyNames = new[] {"IndexId", "EventID", "EventDateBegin"};
            gvEvents.DataSource = colEvents;
            gvEvents.DataBind();
        }

        #endregion

        #region Control Events

        public void gvEvents_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var eventListObject = e.Row.DataItem as EventListObject;
                if (eventListObject != null)
                {
                    if (Settings.Eventtooltiplist && !string.IsNullOrEmpty(eventListObject.Tooltip))
                    {
                        var tooltip = eventListObject.Tooltip;
                        e.Row.Attributes.Add("title", tooltip);
                    }
                    var backColor = eventListObject.CategoryColor;
                    if (backColor.Name != "0")
                    {
                        for (var i = 0; i <= e.Row.Cells.Count - 1; i++)
                        {
                            if (e.Row.Cells[i].Visible && !(gvEvents.Columns[i].SortExpression == "Description"))
                            {
                                e.Row.Cells[i].BackColor = backColor;
                            }
                        }
                    }
                }
                if (IsPrivateNotModerator &&
                    !(UserId == eventListObject.OwnerID))
                {
                    var lnkevent = (HyperLink) e.Row.FindControl("lnkEvent");
                    lnkevent.Style.Add("cursor", "text");
                    lnkevent.Style.Add("text-decoration", "none");
                    lnkevent.Attributes.Add("onclick", "javascript:return false;");
                }
            }
        }

        protected void gvEvents_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if ((e.Row.RowType == DataControlRowType.DataRow) & Settings.Eventtooltiplist)
            {
                toolTipManager.TargetControls.Add(e.Row.ClientID, true);
            }
        }

        protected void SelectCategory_CategorySelected(object sender, CommandEventArgs e)
        {
            //Store the other selection(s) too.
            ((SelectLocation)SelectLocation).StoreLocations();
            BindDataGrid();
        }

        protected void SelectLocation_LocationSelected(object sender, CommandEventArgs e)
        {
            //Store the other selection(s) too.
            ((SelectCategory)SelectCategory).StoreCategories();
            BindDataGrid();
        }

        protected void gvEvents_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            //Set page index
            gvEvents.PageIndex = e.NewPageIndex;

            //Binddata
            BindDataGrid();
        }

        protected void gvEvents_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Edit":
                    var iItemID = Convert.ToInt32(e.CommandArgument);
                    //set selected row editable
                    var objEventInfoHelper =
                        new EventInfoHelper(ModuleId, TabId, PortalId, Settings);
                    Response.Redirect(
                        objEventInfoHelper.GetEditURL(iItemID, GetUrlGroupId(), GetUrlUserId()));
                    break;
            }
        }

        protected void gvEvents_Sorting(object sender, GridViewSortEventArgs e)
        {
            //Get the sort expression
            var sortExpression = GetListSortExpression(e.SortExpression);

            //HACK Change sortdirection
            var sortDirection = e.SortDirection;
            if (!ReferenceEquals(ViewState["SortExpression"], null) && sortExpression ==
                (EventListObject.SortFilter) ViewState["SortExpression"])
            {
                if ((SortDirection) ViewState["SortDirection"] == SortDirection.Ascending)
                {
                    sortDirection = SortDirection.Descending;
                }
                else
                {
                    sortDirection = SortDirection.Ascending;
                }
            }

            //Cache direction en expression
            ViewState["SortExpression"] = sortExpression;
            ViewState["SortDirection"] = sortDirection;

            //Binddata
            BindDataGrid(sortExpression, sortDirection);
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
            InitializeComponent();
        }

        #endregion
    }
}