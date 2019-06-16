
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
    using Security;
    using Services.Localization;
    using global::Components;
    using Telerik.Web.UI;
    using EventInfo = global::Components.EventInfo;

    public partial class SelectCategory : EventBase
    {
        #region Public Methods

        public void StoreCategories()
        {
            SelectedCategory.Clear();
            var lstCategories = new ArrayList();
            if (Settings.Enablecategories == EventModuleSettings.DisplayCategories.SingleSelect)
            {
                lstCategories.Add(ddlCategories.SelectedValue);
            }
            else
            {
                if (ddlCategories.CheckedItems.Count > 0 && ddlCategories.CheckedItems.Count != ddlCategories.Items.Count)
                {
                    foreach (var item in ddlCategories.CheckedItems)
                    {
                        lstCategories.Add(item.Value);
                    }
                }
                else
                {
                    lstCategories.Add("-1");
                }
            }
            SelectedCategory = lstCategories;
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
                    if (!_gotCategories)
                    {
                        _gotCategories = true;
                        _selectedCategory.Clear();
                        //is there a default module category when category select has been disabled
                        //if not has it been passed in as a parameter
                        //if not is there a default module category when category select has not been disabled
                        //if not is there as setting in cookies available
                        if (Settings.Enablecategories == EventModuleSettings.DisplayCategories.DoNotDisplay)
                        {
                            if (Settings.ModuleCategoriesSelected == EventModuleSettings.CategoriesSelected.All)
                            {
                                _selectedCategory.Clear();
                                _selectedCategory.Add("-1");
                            }
                            else
                            {
                                _selectedCategory.Clear();
                                foreach (var category in Settings.ModuleCategoryIDs)
                                {
                                    _selectedCategory.Add(category);
                                }
                            }
                        }
                        else if (!(Request.Params["Category"] == null))
                        {
                            var objSecurity = new PortalSecurity();
                            var tmpCategory = Request.Params["Category"];
                            tmpCategory = objSecurity.InputFilter(tmpCategory, PortalSecurity.FilterFlag.NoScripting);
                            tmpCategory = objSecurity.InputFilter(tmpCategory, PortalSecurity.FilterFlag.NoSQL);
                            var oCntrlEventCategory = new EventCategoryController();
                            var oEventCategory =
                                oCntrlEventCategory.EventCategoryGetByName(tmpCategory, PortalSettings.PortalId);
                            if (!ReferenceEquals(oEventCategory, null))
                            {
                                _selectedCategory.Add(oEventCategory.Category);
                            }
                        }
                        else if (Settings.ModuleCategoriesSelected != EventModuleSettings.CategoriesSelected.All)
                        {
                            _selectedCategory.Clear();
                            foreach (var category in Settings.ModuleCategoryIDs)
                            {
                                _selectedCategory.Add(category);
                            }
                        }
                        else if (ReferenceEquals(Request.Cookies["DNNEvents"], null))
                        {
                            _selectedCategory.Clear();
                            _selectedCategory.Add("-1");
                        }
                        else
                        {
                            //Do we have a special one for this module
                            if (ReferenceEquals(
                                Request.Cookies["DNNEvents"]["EventCategory" + Convert.ToString(ModuleId)],
                                null))
                            {
                                _selectedCategory.Clear();
                                _selectedCategory.Add("-1");
                            }
                            else
                            {
                                //Yes there is one!
                                var objSecurity = new PortalSecurity();
                                var tmpCategory =
                                    Convert.ToString(
                                        Request.Cookies["DNNEvents"][
                                            "EventCategory" + Convert.ToString(ModuleId)]);
                                tmpCategory =
                                    objSecurity.InputFilter(tmpCategory, PortalSecurity.FilterFlag.NoScripting);
                                tmpCategory = objSecurity.InputFilter(tmpCategory, PortalSecurity.FilterFlag.NoSQL);
                                var tmpArray = tmpCategory.Split(',');
                                for (var i = 0; i <= tmpArray.Length - 1; i++)
                                {
                                    if (tmpArray[i] != "")
                                    {
                                        _selectedCategory.Add(int.Parse(tmpArray[i]));
                                    }
                                }
                            }
                        }
                    }
                    return _selectedCategory;
                }
            set
                {
                    try
                    {
                        _selectedCategory = value;
                        _gotCategories = true;
                        Response.Cookies["DNNEvents"]["EventCategory" + Convert.ToString(ModuleId)] =
                            string.Join(",", (string[]) _selectedCategory.ToArray(typeof(string)));
                        Response.Cookies["DNNEvents"].Expires = DateTime.Now.AddMinutes(2);
                        Response.Cookies["DNNEvents"].Path = "/";
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
                var cstext = "<script src=\"" + ResolveUrl("~/DesktopModules/Events/Scripts/Validation.js") +
                             "\" type=\"text/javascript\"></script>";
                if (!Page.ClientScript.IsClientScriptBlockRegistered(csname))
                {
                    Page.ClientScript.RegisterClientScriptBlock(cstype, csname, cstext, false);
                }

                ddlCategories.EmptyMessage = Localization.GetString("NoCategories", LocalResourceFile);
                ddlCategories.Localization.AllItemsCheckedString =
                    Localization.GetString("AllCategories", LocalResourceFile);
                ddlCategories.Localization.CheckAllString =
                    Localization.GetString("SelectAllCategories", LocalResourceFile);
                if (Settings.Enablecategories == EventModuleSettings.DisplayCategories.SingleSelect)
                {
                    ddlCategories.CheckBoxes = false;
                }

                if (!Page.IsPostBack)
                {
                    //Bind DDL
                    var ctrlEventCategories = new EventCategoryController();
                    var lstCategories = ctrlEventCategories.EventsCategoryList(PortalId);

                    var arrCategories = new ArrayList();
                    if (Settings.Restrictcategories)
                    {
                        foreach (EventCategoryInfo dbCategory in lstCategories)
                        {
                            foreach (int category in Settings.ModuleCategoryIDs)
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
                        Visible = false;
                        SelectedCategory.Clear();
                        return;
                    }

                    //Restrict categories by events in time frame.
                    if (Settings.RestrictCategoriesToTimeFrame)
                    {
                        //Only for list view.
                        var whichView = string.Empty;
                        if (!(Request.QueryString["mctl"] == null) && ModuleId ==
                            Convert.ToInt32(Request.QueryString["ModuleID"]))
                        {
                            if (Request["mctl"].EndsWith(".ascx"))
                            {
                                whichView = Request["mctl"];
                            }
                            else
                            {
                                whichView = Request["mctl"] + ".ascx";
                            }
                        }
                        if (whichView.Length == 0)
                        {
                            if (!ReferenceEquals(
                                    Request.Cookies.Get("DNNEvents" + Convert.ToString(ModuleId)), null))
                            {
                                whichView = Request.Cookies.Get("DNNEvents" + Convert.ToString(ModuleId)).Value;
                            }
                            else
                            {
                                whichView = Settings.DefaultView;
                            }
                        }

                        if (whichView == "EventList.ascx" || whichView == "EventRpt.ascx")
                        {
                            var objEventInfoHelper =
                                new EventInfoHelper(ModuleId, TabId, PortalId, Settings);
                            var lstEvents = default(ArrayList);

                            var getSubEvents = Settings.MasterEvent;
                            var numDays = Settings.EventsListEventDays;
                            var displayDate = default(DateTime);
                            var startDate = default(DateTime);
                            var endDate = default(DateTime);
                            if (Settings.ListViewUseTime)
                            {
                                displayDate = DisplayNow();
                            }
                            else
                            {
                                displayDate = DisplayNow().Date;
                            }
                            if (Settings.EventsListSelectType == "DAYS")
                            {
                                startDate = displayDate.AddDays(Settings.EventsListBeforeDays * -1);
                                endDate = displayDate.AddDays(Settings.EventsListAfterDays * 1);
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
                    ddlCategories.DataSource = arrCategories;
                    ddlCategories.DataBind();

                    if (Settings.Enablecategories == EventModuleSettings.DisplayCategories.SingleSelect)
                    {
                        ddlCategories.Items.Insert(
                            0,
                            new RadComboBoxItem(Localization.GetString("AllCategories", LocalResourceFile),
                                                "-1"));
                        ddlCategories.SelectedIndex = 0;
                    }
                    ddlCategories.OnClientDropDownClosed =
                        "function() { btnUpdateClick('" + btnUpdate.UniqueID + "','" +
                        ddlCategories.ClientID + "');}";
                    ddlCategories.OnClientLoad = "function() { storeText('" + ddlCategories.ClientID + "');}";
                    if (Settings.Enablecategories == EventModuleSettings.DisplayCategories.SingleSelect)
                    {
                        foreach (int category in SelectedCategory)
                        {
                            ddlCategories.SelectedIndex =
                                ddlCategories.FindItemByValue(category.ToString()).Index;
                            break;
                        }
                    }
                    else
                    {
                        foreach (int category in SelectedCategory)
                        {
                            foreach (RadComboBoxItem item in ddlCategories.Items)
                            {
                                if (item.Value == category.ToString())
                                {
                                    item.Checked = true;
                                }
                            }
                        }

                        if (Convert.ToInt32(SelectedCategory[0]) == -1)
                        {
                            foreach (RadComboBoxItem item in ddlCategories.Items)
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
            StoreCategories();

            // Fire the CategorySelected event...
            var args = new CommandEventArgs(SelectedCategory.ToString(), null);
            if (CategorySelectedChangedEvent != null)
            {
                CategorySelectedChangedEvent(this, args);
            }
        }

        public delegate void CategorySelectedChangedEventHandler(object sender, CommandEventArgs e);

        private CategorySelectedChangedEventHandler CategorySelectedChangedEvent;

        public event CategorySelectedChangedEventHandler CategorySelectedChanged
        {
            add
                {
                    CategorySelectedChangedEvent =
                        (CategorySelectedChangedEventHandler) Delegate.Combine(
                            CategorySelectedChangedEvent, value);
                }
            remove
                {
                    CategorySelectedChangedEvent =
                        (CategorySelectedChangedEventHandler) Delegate.Remove(CategorySelectedChangedEvent, value);
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
            InitializeComponent();
        }

        #endregion
    }
}