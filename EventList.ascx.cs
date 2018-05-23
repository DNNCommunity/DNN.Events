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

    public partial class EventList : EventBase
    {
        #region Event Handlers

        private ArrayList _selectedEvents;


        private void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetupViewControls(this.EventIcons, this.EventIcons2, this.SelectCategory, this.SelectLocation);

                this.gvEvents.PageSize = this.Settings.EventsListPageSize;

                if (this.Page.IsPostBack == false)
                {
                    if (this.Settings.EventsListShowHeader != "No")
                    {
                        this.gvEvents.ShowHeader = true;
                        Localization.LocalizeGridView(ref this.gvEvents, this.LocalResourceFile);
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

        #region Helper Functions

        private void BindDataGrid()
        {
            //Default sort from settings
            var sortDirection = default(SortDirection);
            if (this.Settings.EventsListSortDirection == "ASC")
            {
                sortDirection = SortDirection.Ascending;
            }
            else
            {
                sortDirection = SortDirection.Descending;
            }

            var sortExpression = this.GetListSortExpression(this.Settings.EventsListSortColumn);

            //Show header - or not
            if (this.Settings.EventsListShowHeader == "Yes")
            {
                this.gvEvents.ShowHeader = true;
            }
            else
            {
                this.gvEvents.ShowHeader = false;
            }

            //Get cached sort settings
            if (!ReferenceEquals(this.ViewState["SortExpression"], null) &&
                this.ViewState["SortExpression"] is EventListObject.SortFilter)
            {
                sortExpression = (EventListObject.SortFilter) this.ViewState["SortExpression"];
            }
            if (!ReferenceEquals(this.ViewState["SortDirection"], null) &&
                this.ViewState["SortDirection"] is SortDirection)
            {
                sortDirection = (SortDirection) this.ViewState["SortDirection"];
            }

            this.BindDataGrid(sortExpression, sortDirection);
        }

        private void BindDataGrid(EventListObject.SortFilter sortExpression, SortDirection sortDirection)
        {
            var culture = Thread.CurrentThread.CurrentCulture;
            var objEvent = default(EventInfo);
            var objEventInfoHelper = new EventInfoHelper(this.ModuleId, this.TabId, this.PortalId, this.Settings);
            var editColumnVisible = false;

            // Get Events/Sub-Calendar Events
            this._selectedEvents = this.Get_ListView_Events(this.SelectCategory.SelectedCategory,
                                                            this.SelectLocation.SelectedLocation);

            var fmtEventTimeBegin = this.Settings.Templates.txtListEventTimeBegin;
            if (string.IsNullOrEmpty(fmtEventTimeBegin))
            {
                fmtEventTimeBegin = "g";
            }

            var fmtEventTimeEnd = this.Settings.Templates.txtListEventTimeEnd;
            if (string.IsNullOrEmpty(fmtEventTimeEnd))
            {
                fmtEventTimeEnd = "g";
            }

            var tmpListDescription = this.Settings.Templates.txtListEventDescription;
            var tmpListLocation = this.Settings.Templates.txtListLocation;

            if (this._selectedEvents.Count == 0)
            {
                this.gvEvents.Visible = false;
                this.divNoEvents.Visible = true;
                return;
            }
            this.gvEvents.Visible = true;
            this.divNoEvents.Visible = false;

            if (this.Settings.Eventtooltiplist)
            {
                this.toolTipManager.TargetControls.Clear();
            }

            // if Events Selection Type only get the 1st N Events
            var colEvents = new ArrayList();
            var lstEvent = default(EventListObject);
            var indexID = 0;
            foreach (EventInfo tempLoopVar_objEvent in this._selectedEvents)
            {
                objEvent = tempLoopVar_objEvent;
                var tcc = new TokenReplaceControllerClass(this.ModuleId, this.LocalResourceFile);
                var objCtlEventRecurMaster = new EventRecurMasterController();
                var fmtRowEnd = "";
                var fmtRowBegin = "";
                fmtRowEnd = tcc.TokenParameters(fmtEventTimeEnd, objEvent, this.Settings);
                fmtRowBegin = tcc.TokenParameters(fmtEventTimeBegin, objEvent, this.Settings);

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

                var isEvtEditor = this.IsEventEditor(objEvent, false);

                var templatedescr = "";
                var iconString = "";

                if (!this.IsPrivateNotModerator || this.UserId == objEvent.OwnerID)
                {
                    templatedescr = tcc.TokenReplaceEvent(objEvent, tmpListDescription, null, false, isEvtEditor);
                    lstEvent.CategoryColor = this.GetColor(objEvent.Color);
                    lstEvent.CategoryFontColor = this.GetColor(objEvent.FontColor);

                    iconString = this.CreateIconString(objEvent, this.Settings.IconListPrio, this.Settings.IconListRec,
                                                       this.Settings.IconListReminder, this.Settings.IconListEnroll);
                }

                lstEvent.EventName = this.CreateEventName(objEvent, "[event:title]");
                lstEvent.EventDesc = objEvent.EventDesc;
                // RWJS - not sure why replace ' with \' - lstEvent.DecodedDesc = System.Web.HttpUtility.HtmlDecode(objEvent.EventDesc).Replace(Environment.NewLine, "").Trim.Replace("'", "\'")
                lstEvent.DecodedDesc =
                    Convert.ToString(HttpUtility.HtmlDecode(templatedescr).Replace(Environment.NewLine, ""));

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
                    lstEvent.ImageURL = this.ImageInfo(objEvent.ImageURL, objEvent.ImageHeight, objEvent.ImageWidth);
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

                if (this.Settings.Eventtooltiplist)
                {
                    lstEvent.Tooltip = this.ToolTipCreate(objEvent, this.Settings.Templates.txtTooltipTemplateTitle,
                                                          this.Settings.Templates.txtTooltipTemplateBody, isEvtEditor);
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
            if (!this.IsPrivateNotModerator)
            {
                if (this.Settings.EventsListFields.LastIndexOf("EB", StringComparison.Ordinal) < 0 ||
                    editColumnVisible == false)
                {
                    this.gvEvents.Columns[0].Visible = false;
                }
                else
                {
                    this.gvEvents.Columns[0].Visible = true;
                }
                if (this.Settings.EventsListFields.LastIndexOf("BD", StringComparison.Ordinal) < 0)
                {
                    this.gvEvents.Columns[1].Visible = false;
                }
                if (this.Settings.EventsListFields.LastIndexOf("ED", StringComparison.Ordinal) < 0)
                {
                    this.gvEvents.Columns[2].Visible = false;
                }
                if (this.Settings.EventsListFields.LastIndexOf("EN", StringComparison.Ordinal) < 0)
                {
                    this.gvEvents.Columns[3].Visible = false;
                }
                if (this.Settings.EventsListFields.LastIndexOf("IM", StringComparison.Ordinal) < 0)
                {
                    this.gvEvents.Columns[4].Visible = false;
                }
                if (this.Settings.EventsListFields.LastIndexOf("DU", StringComparison.Ordinal) < 0)
                {
                    this.gvEvents.Columns[5].Visible = false;
                }
                if (this.Settings.EventsListFields.LastIndexOf("CA", StringComparison.Ordinal) < 0)
                {
                    this.gvEvents.Columns[6].Visible = false;
                }
                if (this.Settings.EventsListFields.LastIndexOf("LO", StringComparison.Ordinal) < 0)
                {
                    this.gvEvents.Columns[7].Visible = false;
                }
                if (!this.Settings.EventsCustomField1 ||
                    this.Settings.EventsListFields.LastIndexOf("C1", StringComparison.Ordinal) < 0)
                {
                    this.gvEvents.Columns[8].Visible = false;
                }
                if (!this.Settings.EventsCustomField2 ||
                    this.Settings.EventsListFields.LastIndexOf("C2", StringComparison.Ordinal) < 0)
                {
                    this.gvEvents.Columns[9].Visible = false;
                }
                if (this.Settings.EventsListFields.LastIndexOf("DE", StringComparison.Ordinal) < 0)
                {
                    this.gvEvents.Columns[10].Visible = false;
                }
                if (this.Settings.EventsListFields.LastIndexOf("RT", StringComparison.Ordinal) < 0)
                {
                    this.gvEvents.Columns[11].Visible = false;
                }
                if (this.Settings.EventsListFields.LastIndexOf("RU", StringComparison.Ordinal) < 0)
                {
                    this.gvEvents.Columns[12].Visible = false;
                }
            }
            else
            {
                // Set Defaults
                this.gvEvents.Columns[0].Visible = false; // Edit Buttom
                this.gvEvents.Columns[1].Visible = true; // Begin Date
                this.gvEvents.Columns[2].Visible = true; // End Date
                this.gvEvents.Columns[3].Visible = true; // Title
                this.gvEvents.Columns[4].Visible = false; // Image
                this.gvEvents.Columns[5].Visible = false; // Duration
                this.gvEvents.Columns[6].Visible = false; // Category
                this.gvEvents.Columns[7].Visible = false; // Location
                this.gvEvents.Columns[8].Visible = false; // Custom Field 1
                this.gvEvents.Columns[9].Visible = false; // Custom Field 2
                this.gvEvents.Columns[10].Visible = false; // Description
                this.gvEvents.Columns[11].Visible = false; // Recurrence Pattern
                this.gvEvents.Columns[12].Visible = false; // Recur Until
            }

            EventListObject.SortExpression = sortExpression;
            EventListObject.SortDirection = sortDirection;
            colEvents.Sort();

            this.gvEvents.DataKeyNames = new[] {"IndexId", "EventID", "EventDateBegin"};
            this.gvEvents.DataSource = colEvents;
            this.gvEvents.DataBind();
        }

        #endregion

        #region Control Events

        public void gvEvents_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (this.Settings.Eventtooltiplist)
                {
                    var tooltip = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "Tooltip"));
                    e.Row.Attributes.Add("title", tooltip);
                }
                var backColor = (Color) DataBinder.Eval(e.Row.DataItem, "CategoryColor");
                if (backColor.Name != "0")
                {
                    for (var i = 0; i <= e.Row.Cells.Count - 1; i++)
                    {
                        if (e.Row.Cells[i].Visible && !(this.gvEvents.Columns[i].SortExpression == "Description"))
                        {
                            e.Row.Cells[i].BackColor = backColor;
                        }
                    }
                }
                if (this.IsPrivateNotModerator &&
                    !(this.UserId == Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "OwnerID"))))
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
            if ((e.Row.RowType == DataControlRowType.DataRow) & this.Settings.Eventtooltiplist)
            {
                this.toolTipManager.TargetControls.Add(e.Row.ClientID, true);
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

        protected void gvEvents_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            //Set page index
            this.gvEvents.PageIndex = e.NewPageIndex;

            //Binddata
            this.BindDataGrid();
        }

        protected void gvEvents_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Edit":
                    var iItemID = Convert.ToInt32(e.CommandArgument);
                    //set selected row editable
                    var objEventInfoHelper =
                        new EventInfoHelper(this.ModuleId, this.TabId, this.PortalId, this.Settings);
                    this.Response.Redirect(
                        objEventInfoHelper.GetEditURL(iItemID, this.GetUrlGroupId(), this.GetUrlUserId()));
                    break;
            }
        }

        protected void gvEvents_Sorting(object sender, GridViewSortEventArgs e)
        {
            //Get the sort expression
            var sortExpression = this.GetListSortExpression(e.SortExpression);

            //HACK Change sortdirection
            var sortDirection = e.SortDirection;
            if (!ReferenceEquals(this.ViewState["SortExpression"], null) && sortExpression ==
                (EventListObject.SortFilter) this.ViewState["SortExpression"])
            {
                if ((SortDirection) this.ViewState["SortDirection"] == SortDirection.Ascending)
                {
                    sortDirection = SortDirection.Descending;
                }
                else
                {
                    sortDirection = SortDirection.Ascending;
                }
            }

            //Cache direction en expression
            this.ViewState["SortExpression"] = sortExpression;
            this.ViewState["SortDirection"] = sortDirection;

            //Binddata
            this.BindDataGrid(sortExpression, sortDirection);
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