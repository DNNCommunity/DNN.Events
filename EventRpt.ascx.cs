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
    using System.Data;
    using System.Diagnostics;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Localization;
    using global::Components;

    public partial class EventRpt : EventBase
    {
        #region Event Handlers

        private ArrayList _selectedEvents;


        private void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetupViewControls(this.EventIcons, this.EventIcons2, this.SelectCategory, this.SelectLocation);

                if (this.Page.IsPostBack == false)
                {
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

        private int _rptItemCount;

        public int PageNumber
        {
            get
                {
                    if (!ReferenceEquals(this.ViewState["PageNumber"], null))
                    {
                        return Convert.ToInt32(this.ViewState["PageNumber"]);
                    }
                    return 0;
                }
            set { this.ViewState["PageNumber"] = value; }
        }

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

            var sortExpression = (EventInfo.SortFilter) this.GetListSortExpression(this.Settings.EventsListSortColumn);

            // Get Events/Sub-Calendar Events
            this._selectedEvents = this.Get_ListView_Events(this.SelectCategory.SelectedCategory,
                                                            this.SelectLocation.SelectedLocation);

            EventInfo.SortExpression = sortExpression;
            EventInfo.SortDirection = sortDirection;
            this._selectedEvents.Sort();

            var tcc = new TokenReplaceControllerClass(this.ModuleId, this.LocalResourceFile);
            var eventTable = new DataTable("Events");
            eventTable.Columns.Add("EventText", Type.GetType("System.String"));
            eventTable.Columns.Add("Tooltip", Type.GetType("System.String"));

            if (this.Settings.Eventtooltiplist)
            {
                this.toolTipManager.TargetControls.Clear();
            }

            var dgRow = default(DataRow);
            var clientIdCount = 1;
            foreach (EventInfo objEvent in this._selectedEvents)
            {
                dgRow = eventTable.NewRow();
                var blAddSubModuleName = false;
                if (objEvent.ModuleID != this.ModuleId && objEvent.ModuleTitle != null &&
                    this.Settings.Addsubmodulename)
                {
                    blAddSubModuleName = true;
                }
                var isEvtEditor = this.IsEventEditor(objEvent, false);
                var tmpText = this.Settings.Templates.txtListRptBody;
                var tmpTooltip = "";
                if (this.Settings.Eventtooltiplist)
                {
                    tmpTooltip = this.ToolTipCreate(objEvent, this.Settings.Templates.txtTooltipTemplateTitle,
                                                    this.Settings.Templates.txtTooltipTemplateBody, isEvtEditor);
                    dgRow["Tooltip"] = tmpTooltip;
                }
                if (!this.Settings.ListViewTable)
                {
                    var tooltip = HttpUtility.HtmlEncode(tmpTooltip);
                    tmpText = this.AddTooltip(clientIdCount, tooltip, tmpText);
                    clientIdCount++;
                }

                dgRow["EventText"] = tcc.TokenReplaceEvent(objEvent, tmpText, null, blAddSubModuleName, isEvtEditor);

                eventTable.Rows.Add(dgRow);
            }

            var pgEvents = new PagedDataSource();
            var dvEvents = new DataView(eventTable);
            pgEvents.DataSource = dvEvents;
            pgEvents.AllowPaging = true;
            pgEvents.PageSize = this.Settings.RptColumns * this.Settings.RptRows;
            pgEvents.CurrentPageIndex = this.PageNumber;
            if (pgEvents.PageCount > 1)
            {
                this.rptTRPager.Visible = true;
                var pages = new ArrayList();
                for (var i = 0; i <= pgEvents.PageCount - 1; i++)
                {
                    pages.Add(i + 1);
                }
                this.rptPager.DataSource = pages;
                this.rptPager.DataBind();
            }
            else
            {
                this.rptTRPager.Visible = false;
            }

            if (pgEvents.CurrentPageIndex + 1 < pgEvents.PageCount)
            {
                this._rptItemCount = pgEvents.PageSize;
            }
            else
            {
                this._rptItemCount = eventTable.Rows.Count - pgEvents.CurrentPageIndex * pgEvents.PageSize;
            }

            this.rptEvents.DataSource = pgEvents;
            this.rptEvents.DataBind();
        }

        #endregion

        #region Control Events

        private void SelectCategory_CategorySelected(object sender, CommandEventArgs e)
        {
            //Store the other selection(s) too.
            this.SelectLocation.StoreLocations();
            this.BindDataGrid();
        }

        private void SelectLocation_LocationSelected(object sender, CommandEventArgs e)
        {
            //Store the other selection(s) too.
            this.SelectCategory.StoreCategories();
            this.BindDataGrid();
        }

        private int _rptCurrentItemCount;
        private bool _rptAlternate = true;

        private void rptEvents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var rptColumns = this.Settings.RptColumns;
            var columnWidth = "\"" + Convert.ToInt32((double) 100 / this.Settings.RptColumns) + "%\"";
            switch (e.Item.ItemType)
            {
                case ListItemType.Header:
                    const string rptHeaderTable = "<table class=\"RptRepeater\">";
                    var rptHeaderStart =
                        "<tr id=\"rptTRHeader\" ><th id=\"rptTDHeader\" class=\"RptHeader\" colspan=\"" +
                        this.Settings.RptColumns + "\">";
                    const string rptHeaderEnd = "</th></tr>";

                    var rptHeaderBody =
                        this.Settings.Templates.txtListRptHeader.Replace(
                            "[event:repeaterheadertext]",
                            Localization.GetString("TokenListRptHeader", this.LocalResourceFile));
                    rptHeaderBody = rptHeaderBody.Replace("[event:repeaterzeroeventstext]",
                                                          Localization.GetString(
                                                              "TokenListRptHeaderZeroEvents", this.LocalResourceFile));
                    var tcc = new TokenReplaceControllerClass(this.ModuleId, this.LocalResourceFile);
                    if (this._rptItemCount == 0)
                    {
                        rptHeaderBody = tcc.TokenOneParameter(rptHeaderBody, "IFZEROEVENTS", true);
                    }
                    else
                    {
                        rptHeaderBody = tcc.TokenOneParameter(rptHeaderBody, "IFZEROEVENTS", false);
                    }

                    var rptHeader = (Literal) e.Item.FindControl("rptHeader");
                    if (this.Settings.ListViewTable)
                    {
                        rptHeader.Text = rptHeaderTable;
                        if (!string.IsNullOrEmpty(rptHeaderBody))
                        {
                            rptHeader.Text = rptHeader.Text + rptHeaderStart + rptHeaderBody + rptHeaderEnd;
                        }
                    }
                    else
                    {
                        rptHeader.Text = rptHeaderBody;
                    }
                    break;
                case ListItemType.Footer:
                    var rptFooterStart =
                        "<tr id=\"rptTRFooter\"><td id=\"rptTDFooter\" class=\"RptFooter\" colspan=\"" +
                        this.Settings.RptColumns + "\">";
                    const string rptFooterEnd = "</td></tr>";
                    const string rptFooterTable = "</table>";
                    var rptFooterBody =
                        this.Settings.Templates.txtListRptFooter.Replace(
                            "[event:repeaterfootertext]",
                            Localization.GetString("TokenListRptFooter", this.LocalResourceFile));
                    var rptFooter = (Literal) e.Item.FindControl("rptFooter");
                    if (this.Settings.ListViewTable)
                    {
                        if (!string.IsNullOrEmpty(rptFooterBody))
                        {
                            rptFooter.Text = rptFooterStart + rptFooterBody + rptFooterEnd;
                        }
                        rptFooter.Text = rptFooter.Text + rptFooterTable;
                    }
                    else
                    {
                        rptFooter.Text = rptFooterBody;
                    }
                    break;
                default:
                    var rptBody = (Literal) e.Item.FindControl("rptBody");
                    var rptRowBody = Convert.ToString(DataBinder.Eval(e.Item.DataItem, "EventText"));
                    this._rptCurrentItemCount++;
                    if (this.Settings.ListViewTable)
                    {
                        var rptBodyStart = "<td [event:repeatertooltip] width=" + columnWidth + ">";

                        const string rptBodyEnd = "</td>";
                        var rptRowStart = "";
                        if ((this._rptCurrentItemCount - 1) % rptColumns == 0)
                        {
                            this._rptAlternate = !this._rptAlternate;
                            var rptCellClass = "RptNormal";
                            if (this._rptAlternate)
                            {
                                rptCellClass = "RptAlternate";
                            }
                            rptRowStart = "<tr class=\"" + rptCellClass + "\" >" + rptBodyStart;
                        }
                        else
                        {
                            rptRowStart = rptBodyStart;
                        }

                        var rptRowEnd = "";
                        if (this._rptCurrentItemCount % rptColumns == 0)
                        {
                            rptRowEnd = rptBodyEnd + "</tr>";
                        }
                        else if (this._rptItemCount == this._rptCurrentItemCount)
                        {
                            // ReSharper disable RedundantAssignment
                            for (var i = 1; i <= rptColumns - this._rptCurrentItemCount % rptColumns; i++)
                            {
                                // ReSharper restore RedundantAssignment
                                rptRowEnd += "<td width=" + columnWidth + " ></td>";
                            }
                            rptRowEnd += "</tr>";
                        }
                        else
                        {
                            rptRowEnd = rptBodyEnd;
                        }
                        var tooltip = "";
                        if (this.Settings.Eventtooltiplist)
                        {
                            tooltip = HttpUtility.HtmlEncode(
                                Convert.ToString(DataBinder.Eval(e.Item.DataItem, "Tooltip")));
                        }
                        rptBody.Text = this.AddTooltip(this._rptCurrentItemCount, tooltip, rptRowStart) + rptRowBody +
                                       rptRowEnd;
                    }
                    else
                    {
                        rptBody.Text = rptRowBody;
                    }
                    break;
            }
        }

        private string AddTooltip(int itemCount, string toolTip, string body)
        {
            var fullTooltip = "";
            if (this.Settings.Eventtooltiplist)
            {
                var ttClientId = "ctlEvents_Mod_" + this.ModuleId + "_RptRowBody_" + itemCount;
                fullTooltip = "ID=\"" + ttClientId + "\" title=\"" + toolTip + "\"";
                this.toolTipManager.TargetControls.Add(this.ClientID, true);
            }
            return body.Replace("[event:repeatertooltip]", fullTooltip);
        }

        private void rptPages_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            switch (e.Item.ItemType)
            {
                case ListItemType.Header:
                    break;
                case ListItemType.Footer:
                    break;
                default:
                    var lnkPage = (LinkButton) e.Item.FindControl("cmdPage");
                    if (int.Parse(lnkPage.CommandArgument) == this.PageNumber + 1)
                    {
                        lnkPage.Style.Add("cursor", "text");
                        lnkPage.Style.Add("text-decoration", "none");
                        lnkPage.Attributes.Add("onclick", "javascript:return false;");
                        lnkPage.CssClass = "RptPagerCurrentPage";
                    }
                    break;
            }
        }

        private void rptPages_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            this.PageNumber = Convert.ToInt32(e.CommandArgument) - 1;
            this.BindDataGrid();
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