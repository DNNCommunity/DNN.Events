
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
    using System.Reflection;
    using System.Web.UI.WebControls;
    using DotNetNuke.Security;
    using DotNetNuke.Services.Localization;
    using global::Components;
    using Telerik.Web.UI;
    using EventInfo = global::Components.EventInfo;

    public partial class SelectCategory : EventBase
    {
        #region Public Methods

        public void StoreCategories()
        {
            this.SelectedCategory.Clear();
            var lstCategories = new ArrayList();
            if (this.Settings.Enablecategories == EventModuleSettings.DisplayCategories.SingleSelect)
            {
                lstCategories.Add(this.ddlCategories.SelectedValue);
            }
            else
            {
                if (this.ddlCategories.CheckedItems.Count != this.ddlCategories.Items.Count)
                {
                    foreach (var item in this.ddlCategories.CheckedItems)
                    {
                        lstCategories.Add(item.Value);
                    }
                }
                else
                {
                    lstCategories.Add("-1");
                }
            }
            this.SelectedCategory = lstCategories;
        }

        #endregion

        #region Properties

        private ArrayList _selectedCategory = new ArrayList();
        private bool _gotCategories;

        public ArrayList SelectedCategory
        {
            get
                {
                    //have selected the category before
                    if (!this._gotCategories)
                    {
                        this._gotCategories = true;
                        this._selectedCategory.Clear();
                        //is there a default module category when category select has been disabled
                        //if not has it been passed in as a parameter
                        //if not is there a default module category when category select has not been disabled
                        //if not is there as setting in cookies available
                        if (this.Settings.Enablecategories == EventModuleSettings.DisplayCategories.DoNotDisplay)
                        {
                            if (this.Settings.ModuleCategoriesSelected == EventModuleSettings.CategoriesSelected.All)
                            {
                                this._selectedCategory.Clear();
                                this._selectedCategory.Add("-1");
                            }
                            else
                            {
                                this._selectedCategory.Clear();
                                foreach (int category in this.Settings.ModuleCategoryIDs)
                                {
                                    this._selectedCategory.Add(category);
                                }
                            }
                        }
                        else if (!(this.Request.Params["Category"] == null))
                        {
                            var objSecurity = new PortalSecurity();
                            var tmpCategory = this.Request.Params["Category"];
                            tmpCategory = objSecurity.InputFilter(tmpCategory, PortalSecurity.FilterFlag.NoScripting);
                            tmpCategory = objSecurity.InputFilter(tmpCategory, PortalSecurity.FilterFlag.NoSQL);
                            var oCntrlEventCategory = new EventCategoryController();
                            var oEventCategory =
                                oCntrlEventCategory.EventCategoryGetByName(tmpCategory, this.PortalSettings.PortalId);
                            if (!ReferenceEquals(oEventCategory, null))
                            {
                                this._selectedCategory.Add(oEventCategory.Category);
                            }
                        }
                        else if (this.Settings.ModuleCategoriesSelected != EventModuleSettings.CategoriesSelected.All)
                        {
                            this._selectedCategory.Clear();
                            foreach (int category in this.Settings.ModuleCategoryIDs)
                            {
                                this._selectedCategory.Add(category);
                            }
                        }
                        else if (ReferenceEquals(this.Request.Cookies["DNNEvents"], null))
                        {
                            this._selectedCategory.Clear();
                            this._selectedCategory.Add("-1");
                        }
                        else
                        {
                            //Do we have a special one for this module
                            if (ReferenceEquals(
                                this.Request.Cookies["DNNEvents"]["EventCategory" + Convert.ToString(this.ModuleId)],
                                null))
                            {
                                this._selectedCategory.Clear();
                                this._selectedCategory.Add("-1");
                            }
                            else
                            {
                                //Yes there is one!
                                var objSecurity = new PortalSecurity();
                                var tmpCategory =
                                    Convert.ToString(
                                        this.Request.Cookies["DNNEvents"][
                                            "EventCategory" + Convert.ToString(this.ModuleId)]);
                                tmpCategory =
                                    objSecurity.InputFilter(tmpCategory, PortalSecurity.FilterFlag.NoScripting);
                                tmpCategory = objSecurity.InputFilter(tmpCategory, PortalSecurity.FilterFlag.NoSQL);
                                var tmpArray = tmpCategory.Split(',');
                                for (var i = 0; i <= tmpArray.Length - 1; i++)
                                {
                                    if (tmpArray[i] != "")
                                    {
                                        this._selectedCategory.Add(int.Parse(tmpArray[i]));
                                    }
                                }
                            }
                        }
                    }
                    return this._selectedCategory;
                }
            set
                {
                    try
                    {
                        this._selectedCategory = value;
                        this._gotCategories = true;
                        this.Response.Cookies["DNNEvents"]["EventCategory" + Convert.ToString(this.ModuleId)] =
                            string.Join(",", (string[]) this._selectedCategory.ToArray(typeof(string)));
                        this.Response.Cookies["DNNEvents"].Expires = DateTime.Now.AddMinutes(2);
                        this.Response.Cookies["DNNEvents"].Path = "/";
                    }
                    catch (Exception)
                    { }
                }
        }

        public ArrayList ModuleCategoryIDs { get; set; } = new ArrayList();

        #endregion

        #region Event Handlers

        private void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Add the external Validation.js to the Page
                const string csname = "ExtValidationScriptFile";
                var cstype = MethodBase.GetCurrentMethod().GetType();
                var cstext = "<script src=\"" + this.ResolveUrl("~/DesktopModules/Events/Scripts/Validation.js") +
                             "\" type=\"text/javascript\"></script>";
                if (!this.Page.ClientScript.IsClientScriptBlockRegistered(csname))
                {
                    this.Page.ClientScript.RegisterClientScriptBlock(cstype, csname, cstext, false);
                }

                this.ddlCategories.EmptyMessage = Localization.GetString("NoCategories", this.LocalResourceFile);
                this.ddlCategories.Localization.AllItemsCheckedString =
                    Localization.GetString("AllCategories", this.LocalResourceFile);
                this.ddlCategories.Localization.CheckAllString =
                    Localization.GetString("SelectAllCategories", this.LocalResourceFile);
                if (this.Settings.Enablecategories == EventModuleSettings.DisplayCategories.SingleSelect)
                {
                    this.ddlCategories.CheckBoxes = false;
                }

                if (!this.Page.IsPostBack)
                {
                    //Bind DDL
                    var ctrlEventCategories = new EventCategoryController();
                    var lstCategories = ctrlEventCategories.EventsCategoryList(this.PortalId);

                    var arrCategories = new ArrayList();
                    if (this.Settings.Restrictcategories)
                    {
                        foreach (EventCategoryInfo dbCategory in lstCategories)
                        {
                            foreach (int category in this.Settings.ModuleCategoryIDs)
                            {
                                if (dbCategory.Category == category)
                                {
                                    arrCategories.Add(dbCategory);
                                }
                            }
                        }
                    }
                    else
                    {
                        arrCategories.AddRange(lstCategories);
                    }

                    if (lstCategories.Count == 0)
                    {
                        this.Visible = false;
                        this.SelectedCategory.Clear();
                        return;
                    }

                    //Restrict categories by events in time frame.
                    if (this.Settings.RestrictCategoriesToTimeFrame)
                    {
                        //Only for list view.
                        var whichView = string.Empty;
                        if (!(this.Request.QueryString["mctl"] == null) && this.ModuleId ==
                            Convert.ToInt32(this.Request.QueryString["ModuleID"]))
                        {
                            if (this.Request["mctl"].EndsWith(".ascx"))
                            {
                                whichView = this.Request["mctl"];
                            }
                            else
                            {
                                whichView = this.Request["mctl"] + ".ascx";
                            }
                        }
                        if (whichView.Length == 0)
                        {
                            if (!ReferenceEquals(
                                    this.Request.Cookies.Get("DNNEvents" + Convert.ToString(this.ModuleId)), null))
                            {
                                whichView = this
                                    .Request.Cookies.Get("DNNEvents" + Convert.ToString(this.ModuleId)).Value;
                            }
                            else
                            {
                                whichView = this.Settings.DefaultView;
                            }
                        }

                        if (whichView == "EventList.ascx" || whichView == "EventRpt.ascx")
                        {
                            var objEventInfoHelper =
                                new EventInfoHelper(this.ModuleId, this.TabId, this.PortalId, this.Settings);
                            var lstEvents = default(ArrayList);

                            var getSubEvents = this.Settings.MasterEvent;
                            var numDays = this.Settings.EventsListEventDays;
                            var displayDate = default(DateTime);
                            var startDate = default(DateTime);
                            var endDate = default(DateTime);
                            if (this.Settings.ListViewUseTime)
                            {
                                displayDate = this.DisplayNow();
                            }
                            else
                            {
                                displayDate = this.DisplayNow().Date;
                            }
                            if (this.Settings.EventsListSelectType == "DAYS")
                            {
                                startDate = displayDate.AddDays(this.Settings.EventsListBeforeDays * -1);
                                endDate = displayDate.AddDays(this.Settings.EventsListAfterDays * 1);
                            }
                            else
                            {
                                startDate = displayDate;
                                endDate = displayDate.AddDays(numDays);
                            }

                            lstEvents = objEventInfoHelper.GetEvents(startDate, endDate, getSubEvents,
                                                                     new ArrayList(Convert.ToInt32(new[] {"-1"})),
                                                                     new ArrayList(Convert.ToInt32(new[] {"-1"})), -1,
                                                                     -1);

                            var eventCategoryIds = new ArrayList();
                            foreach (EventInfo lstEvent in lstEvents)
                            {
                                eventCategoryIds.Add(lstEvent.Category);
                            }
                            foreach (EventCategoryInfo lstCategory in lstCategories)
                            {
                                if (!eventCategoryIds.Contains(lstCategory.Category))
                                {
                                    arrCategories.Remove(lstCategory);
                                }
                            }
                        }
                    }

                    //Bind categories.
                    this.ddlCategories.DataSource = arrCategories;
                    this.ddlCategories.DataBind();

                    if (this.Settings.Enablecategories == EventModuleSettings.DisplayCategories.SingleSelect)
                    {
                        this.ddlCategories.Items.Insert(
                            0,
                            new RadComboBoxItem(Localization.GetString("AllCategories", this.LocalResourceFile),
                                                "-1"));
                        this.ddlCategories.SelectedIndex = 0;
                    }
                    this.ddlCategories.OnClientDropDownClosed =
                        "function() { btnUpdateClick('" + this.btnUpdate.UniqueID + "','" +
                        this.ddlCategories.ClientID + "');}";
                    this.ddlCategories.OnClientLoad = "function() { storeText('" + this.ddlCategories.ClientID + "');}";
                    if (this.Settings.Enablecategories == EventModuleSettings.DisplayCategories.SingleSelect)
                    {
                        foreach (int category in this.SelectedCategory)
                        {
                            this.ddlCategories.SelectedIndex =
                                this.ddlCategories.FindItemByValue(category.ToString()).Index;
                            break;
                        }
                    }
                    else
                    {
                        foreach (int category in this.SelectedCategory)
                        {
                            foreach (RadComboBoxItem item in this.ddlCategories.Items)
                            {
                                if (item.Value == category.ToString())
                                {
                                    item.Checked = true;
                                }
                            }
                        }

                        if (Convert.ToInt32(this.SelectedCategory[0]) == -1)
                        {
                            foreach (RadComboBoxItem item in this.ddlCategories.Items)
                            {
                                item.Checked = true;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                //ProcessModuleLoadException(Me, exc)
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            this.StoreCategories();

            // Fire the CategorySelected event...
            var args = new CommandEventArgs(this.SelectedCategory.ToString(), null);
            if (this.CategorySelectedChangedEvent != null)
            {
                this.CategorySelectedChangedEvent(this, args);
            }
        }

        public delegate void CategorySelectedChangedEventHandler(object sender, CommandEventArgs e);

        private CategorySelectedChangedEventHandler CategorySelectedChangedEvent;

        public event CategorySelectedChangedEventHandler CategorySelectedChanged
        {
            add
                {
                    this.CategorySelectedChangedEvent =
                        (CategorySelectedChangedEventHandler) Delegate.Combine(
                            this.CategorySelectedChangedEvent, value);
                }
            remove
                {
                    this.CategorySelectedChangedEvent =
                        (CategorySelectedChangedEventHandler) Delegate.Remove(this.CategorySelectedChangedEvent, value);
                }
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