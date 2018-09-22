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
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Components;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.Modules.Events
{
    public partial class EventRpt : EventBase
    {
        #region Event Handlers

        private ArrayList _selectedEvents;


        private void Page_Load(object sender, EventArgs e)
        {
            try
            {
                SetupViewControls(EventIcons, EventIcons2, SelectCategory, SelectLocation);

                if (Page.IsPostBack == false)
                {
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

        private int _rptItemCount;

        public int PageNumber
        {
            get
                {
                    if (!ReferenceEquals(ViewState["PageNumber"], null))
                    {
                        return Convert.ToInt32(ViewState["PageNumber"]);
                    }
                    return 0;
                }
            set { ViewState["PageNumber"] = value; }
        }

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

            var sortExpression = (EventInfo.SortFilter) GetListSortExpression(Settings.EventsListSortColumn);

            // Get Events/Sub-Calendar Events
            _selectedEvents = Get_ListView_Events(SelectCategory.SelectedCategory,
                                                            SelectLocation.SelectedLocation);

            EventInfo.SortExpression = sortExpression;
            EventInfo.SortDirection = sortDirection;
            _selectedEvents.Sort();

            var tcc = new TokenReplaceControllerClass(ModuleId, LocalResourceFile);
            var eventTable = new DataTable("Events");
            eventTable.Columns.Add("EventText", Type.GetType("System.String"));
            eventTable.Columns.Add("Tooltip", Type.GetType("System.String"));

            if (Settings.Eventtooltiplist)
            {
                toolTipManager.TargetControls.Clear();
            }

            var dgRow = default(DataRow);
            var clientIdCount = 1;
            foreach (EventInfo objEvent in _selectedEvents)
            {
                dgRow = eventTable.NewRow();
                var blAddSubModuleName = false;
                if (objEvent.ModuleID != ModuleId && objEvent.ModuleTitle != null &&
                    Settings.Addsubmodulename)
                {
                    blAddSubModuleName = true;
                }
                var isEvtEditor = IsEventEditor(objEvent, false);
                var tmpText = Settings.Templates.txtListRptBody;
                var tmpTooltip = "";
                if (Settings.Eventtooltiplist)
                {
                    tmpTooltip = ToolTipCreate(objEvent, Settings.Templates.txtTooltipTemplateTitle,
                                                    Settings.Templates.txtTooltipTemplateBody, isEvtEditor);
                    dgRow["Tooltip"] = tmpTooltip;
                }
                if (!Settings.ListViewTable)
                {
                    var tooltip = HttpUtility.HtmlEncode(tmpTooltip);
                    tmpText = AddTooltip(clientIdCount, tooltip, tmpText);
                    clientIdCount++;
                }

                dgRow["EventText"] = tcc.TokenReplaceEvent(objEvent, tmpText, null, blAddSubModuleName, isEvtEditor);

                eventTable.Rows.Add(dgRow);
            }

            var pgEvents = new PagedDataSource();
            var dvEvents = new DataView(eventTable);
            pgEvents.DataSource = dvEvents;
            pgEvents.AllowPaging = true;
            pgEvents.PageSize = Settings.RptColumns * Settings.RptRows;
            pgEvents.CurrentPageIndex = PageNumber;
            if (pgEvents.PageCount > 1)
            {
                rptTRPager.Visible = true;
                var pages = new ArrayList();
                for (var i = 0; i <= pgEvents.PageCount - 1; i++)
                {
                    pages.Add(i + 1);
                }
                rptPager.DataSource = pages;
                rptPager.DataBind();
            }
            else
            {
                rptTRPager.Visible = false;
            }

            if (pgEvents.CurrentPageIndex + 1 < pgEvents.PageCount)
            {
                _rptItemCount = pgEvents.PageSize;
            }
            else
            {
                _rptItemCount = eventTable.Rows.Count - pgEvents.CurrentPageIndex * pgEvents.PageSize;
            }

            rptEvents.DataSource = pgEvents;
            rptEvents.DataBind();
        }

        #endregion

        #region Control Events

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

        private int _rptCurrentItemCount;
        private bool _rptAlternate = true;

        protected void rptEvents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var rptColumns = Settings.RptColumns;
            var columnWidth = "\"" + Convert.ToInt32((double) 100 / Settings.RptColumns) + "%\"";
            switch (e.Item.ItemType)
            {
                case ListItemType.Header:
                    const string rptHeaderTable = "<table class=\"RptRepeater\">";
                    var rptHeaderStart =
                        "<tr id=\"rptTRHeader\" ><th id=\"rptTDHeader\" class=\"RptHeader\" colspan=\"" +
                        Settings.RptColumns + "\">";
                    const string rptHeaderEnd = "</th></tr>";

                    var rptHeaderBody =
                        Settings.Templates.txtListRptHeader.Replace(
                            "[event:repeaterheadertext]",
                            Localization.GetString("TokenListRptHeader", LocalResourceFile));
                    rptHeaderBody = rptHeaderBody.Replace("[event:repeaterzeroeventstext]",
                                                          Localization.GetString(
                                                              "TokenListRptHeaderZeroEvents", LocalResourceFile));
                    var tcc = new TokenReplaceControllerClass(ModuleId, LocalResourceFile);
                    if (_rptItemCount == 0)
                    {
                        rptHeaderBody = tcc.TokenOneParameter(rptHeaderBody, "IFZEROEVENTS", true);
                    }
                    else
                    {
                        rptHeaderBody = tcc.TokenOneParameter(rptHeaderBody, "IFZEROEVENTS", false);
                    }

                    var rptHeader = (Literal) e.Item.FindControl("rptHeader");
                    if (Settings.ListViewTable)
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
                        Settings.RptColumns + "\">";
                    const string rptFooterEnd = "</td></tr>";
                    const string rptFooterTable = "</table>";
                    var rptFooterBody =
                        Settings.Templates.txtListRptFooter.Replace(
                            "[event:repeaterfootertext]",
                            Localization.GetString("TokenListRptFooter", LocalResourceFile));
                    var rptFooter = (Literal) e.Item.FindControl("rptFooter");
                    if (Settings.ListViewTable)
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
                    _rptCurrentItemCount++;
                    if (Settings.ListViewTable)
                    {
                        var rptBodyStart = "<td [event:repeatertooltip] width=" + columnWidth + ">";

                        const string rptBodyEnd = "</td>";
                        var rptRowStart = "";
                        if ((_rptCurrentItemCount - 1) % rptColumns == 0)
                        {
                            _rptAlternate = !_rptAlternate;
                            var rptCellClass = "RptNormal";
                            if (_rptAlternate)
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
                        if (_rptCurrentItemCount % rptColumns == 0)
                        {
                            rptRowEnd = rptBodyEnd + "</tr>";
                        }
                        else if (_rptItemCount == _rptCurrentItemCount)
                        {
                            // ReSharper disable RedundantAssignment
                            for (var i = 1; i <= rptColumns - _rptCurrentItemCount % rptColumns; i++)
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
                        if (Settings.Eventtooltiplist)
                        {
                            tooltip = HttpUtility.HtmlEncode(
                                Convert.ToString(DataBinder.Eval(e.Item.DataItem, "Tooltip")));
                        }
                        rptBody.Text = AddTooltip(_rptCurrentItemCount, tooltip, rptRowStart) + rptRowBody +
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
            if (Settings.Eventtooltiplist)
            {
                var ttClientId = "ctlEvents_Mod_" + ModuleId + "_RptRowBody_" + itemCount;
                fullTooltip = "ID=\"" + ttClientId + "\" title=\"" + toolTip + "\"";
                toolTipManager.TargetControls.Add(ClientID, true);
            }
            return body.Replace("[event:repeatertooltip]", fullTooltip);
        }

        protected void rptPages_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            switch (e.Item.ItemType)
            {
                case ListItemType.Header:
                    break;
                case ListItemType.Footer:
                    break;
                default:
                    var lnkPage = (LinkButton) e.Item.FindControl("cmdPage");
                    if (int.Parse(lnkPage.CommandArgument) == PageNumber + 1)
                    {
                        lnkPage.Style.Add("cursor", "text");
                        lnkPage.Style.Add("text-decoration", "none");
                        lnkPage.Attributes.Add("onclick", "javascript:return false;");
                        lnkPage.CssClass = "RptPagerCurrentPage";
                    }
                    break;
            }
        }

        protected void rptPages_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            PageNumber = Convert.ToInt32(e.CommandArgument) - 1;
            BindDataGrid();
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