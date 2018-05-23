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

    public partial class SelectLocation : EventBase
    {
        #region Public Methods

        public void StoreLocations()
        {
            this.SelectedLocation.Clear();
            var lstLocations = new ArrayList();
            if (this.Settings.Enablelocations == EventModuleSettings.DisplayLocations.SingleSelect)
            {
                lstLocations.Add(this.ddlLocations.SelectedValue);
            }
            else
            {
                if (this.ddlLocations.CheckedItems.Count != this.ddlLocations.Items.Count)
                {
                    foreach (var item in this.ddlLocations.CheckedItems)
                    {
                        lstLocations.Add(item.Value);
                    }
                }
                else
                {
                    lstLocations.Add("-1");
                }
            }
            this.SelectedLocation = lstLocations;
        }

        #endregion

        #region Properties

        private ArrayList _selectedLocation = new ArrayList();
        private bool _gotLocations;

        public ArrayList SelectedLocation
        {
            get
                {
                    //have selected the location before
                    if (!this._gotLocations)
                    {
                        this._gotLocations = true;
                        this._selectedLocation.Clear();

                        //is there a default module location when location select has been disabled
                        //if not has it been passed in as a parameter
                        //if not is there a default module location when location select has not been disabled
                        //if not is there as setting in cookies available
                        if (this.Settings.Enablelocations == EventModuleSettings.DisplayLocations.DoNotDisplay)
                        {
                            if (this.Settings.ModuleLocationsSelected == EventModuleSettings.LocationsSelected.All)
                            {
                                this._selectedLocation.Clear();
                                this._selectedLocation.Add("-1");
                            }
                            else
                            {
                                this._selectedLocation.Clear();
                                foreach (int location in this.Settings.ModuleLocationIDs)
                                {
                                    this._selectedLocation.Add(location);
                                }
                            }
                        }
                        else if (!(this.Request.Params["Location"] == null))
                        {
                            var objSecurity = new PortalSecurity();
                            var tmpLocation = this.Request.Params["Location"];
                            tmpLocation = objSecurity.InputFilter(tmpLocation, PortalSecurity.FilterFlag.NoScripting);
                            tmpLocation = objSecurity.InputFilter(tmpLocation, PortalSecurity.FilterFlag.NoSQL);
                            var oCntrlEventLocation = new EventLocationController();
                            var oEventLocation =
                                oCntrlEventLocation.EventsLocationGetByName(tmpLocation, this.PortalSettings.PortalId);
                            if (!ReferenceEquals(oEventLocation, null))
                            {
                                this._selectedLocation.Add(oEventLocation.Location);
                            }
                        }
                        else if (this.Settings.ModuleLocationsSelected != EventModuleSettings.LocationsSelected.All)
                        {
                            this._selectedLocation.Clear();
                            foreach (int location in this.Settings.ModuleLocationIDs)
                            {
                                this._selectedLocation.Add(location);
                            }
                        }
                        else if (ReferenceEquals(this.Request.Cookies["DNNEvents"], null))
                        {
                            this._selectedLocation.Clear();
                            this._selectedLocation.Add("-1");
                        }
                        else
                        {
                            //Do we have a special one for this module
                            if (ReferenceEquals(
                                this.Request.Cookies["DNNEvents"]["EventLocation" + Convert.ToString(this.ModuleId)],
                                null))
                            {
                                this._selectedLocation.Clear();
                                this._selectedLocation.Add("-1");
                            }
                            else
                            {
                                //Yes there is one!
                                var objSecurity = new PortalSecurity();
                                var tmpLocation =
                                    Convert.ToString(
                                        this.Request.Cookies["DNNEvents"][
                                            "EventLocation" + Convert.ToString(this.ModuleId)]);
                                tmpLocation =
                                    objSecurity.InputFilter(tmpLocation, PortalSecurity.FilterFlag.NoScripting);
                                tmpLocation = objSecurity.InputFilter(tmpLocation, PortalSecurity.FilterFlag.NoSQL);
                                var tmpArray = tmpLocation.Split(',');
                                for (var i = 0; i <= tmpArray.Length - 1; i++)
                                {
                                    if (tmpArray[i] != "")
                                    {
                                        this._selectedLocation.Add(int.Parse(tmpArray[i]));
                                    }
                                }
                            }
                        }
                    }
                    return this._selectedLocation;
                }
            set
                {
                    try
                    {
                        this._selectedLocation = value;
                        this._gotLocations = true;
                        this.Response.Cookies["DNNEvents"]["EventLocation" + Convert.ToString(this.ModuleId)] =
                            string.Join(",", (string[]) this._selectedLocation.ToArray(typeof(string)));
                        this.Response.Cookies["DNNEvents"].Expires = DateTime.Now.AddMinutes(2);
                        this.Response.Cookies["DNNEvents"].Path = "/";
                    }
                    catch (Exception)
                    { }
                }
        }

        public ArrayList ModuleLocationIDs { get; set; } = new ArrayList();

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

                this.ddlLocations.EmptyMessage = Localization.GetString("NoLocations", this.LocalResourceFile);
                this.ddlLocations.Localization.AllItemsCheckedString =
                    Localization.GetString("AllLocations", this.LocalResourceFile);
                this.ddlLocations.Localization.CheckAllString =
                    Localization.GetString("SelectAllLocations", this.LocalResourceFile);
                if (this.Settings.Enablelocations == EventModuleSettings.DisplayLocations.SingleSelect)
                {
                    this.ddlLocations.CheckBoxes = false;
                }

                if (!this.Page.IsPostBack)
                {
                    //Bind DDL
                    var ctrlEventLocations = new EventLocationController();
                    var lstLocations = ctrlEventLocations.EventsLocationList(this.PortalId);

                    var arrLocations = new ArrayList();
                    if (this.Settings.Restrictlocations)
                    {
                        foreach (EventLocationInfo dbLocation in lstLocations)
                        {
                            foreach (int location in this.Settings.ModuleLocationIDs)
                            {
                                if (dbLocation.Location == location)
                                {
                                    arrLocations.Add(dbLocation);
                                }
                            }
                        }
                    }
                    else
                    {
                        arrLocations.AddRange(lstLocations);
                    }

                    if (lstLocations.Count == 0)
                    {
                        this.Visible = false;
                        this.SelectedLocation.Clear();
                        return;
                    }

                    //Restrict locations by events in time frame.
                    if (this.Settings.RestrictLocationsToTimeFrame)
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

                            var eventLocationIds = new ArrayList();
                            foreach (EventInfo lstEvent in lstEvents)
                            {
                                eventLocationIds.Add(lstEvent.Location);
                            }
                            foreach (EventLocationInfo lstLocation in lstLocations)
                            {
                                if (!eventLocationIds.Contains(lstLocation.Location))
                                {
                                    arrLocations.Remove(lstLocation);
                                }
                            }
                        }
                    }

                    //Bind locations.
                    this.ddlLocations.DataSource = arrLocations;
                    this.ddlLocations.DataBind();

                    if (this.Settings.Enablelocations == EventModuleSettings.DisplayLocations.SingleSelect)
                    {
                        this.ddlLocations.Items.Insert(
                            0,
                            new RadComboBoxItem(Localization.GetString("AllLocations", this.LocalResourceFile),
                                                "-1"));
                        this.ddlLocations.SelectedIndex = 0;
                    }
                    this.ddlLocations.OnClientDropDownClosed =
                        "function() { btnUpdateClick('" + this.btnUpdate.UniqueID + "','" + this.ddlLocations.ClientID +
                        "');}";
                    this.ddlLocations.OnClientLoad = "function() { storeText('" + this.ddlLocations.ClientID + "');}";
                    if (this.Settings.Enablelocations == EventModuleSettings.DisplayLocations.SingleSelect)
                    {
                        foreach (int location in this.SelectedLocation)
                        {
                            this.ddlLocations.SelectedIndex =
                                this.ddlLocations.FindItemByValue(location.ToString()).Index;
                            break;
                        }
                    }
                    else
                    {
                        foreach (int location in this.SelectedLocation)
                        {
                            foreach (RadComboBoxItem item in this.ddlLocations.Items)
                            {
                                if (item.Value == location.ToString())
                                {
                                    item.Checked = true;
                                }
                            }
                        }

                        if (Convert.ToInt32(this.SelectedLocation[0]) == -1)
                        {
                            foreach (RadComboBoxItem item in this.ddlLocations.Items)
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

        public void btnUpdate_Click(object sender, EventArgs e)
        {
            this.StoreLocations();

            // Fire the LocationSelected event...
            var args = new CommandEventArgs(this.SelectedLocation.ToString(), null);
            if (this.LocationSelectedChangedEvent != null)
            {
                this.LocationSelectedChangedEvent(this, args);
            }
        }

        public delegate void LocationSelectedChangedEventHandler(object sender, CommandEventArgs e);

        private LocationSelectedChangedEventHandler LocationSelectedChangedEvent;

        public event LocationSelectedChangedEventHandler LocationSelectedChanged
        {
            add
                {
                    this.LocationSelectedChangedEvent =
                        (LocationSelectedChangedEventHandler) Delegate.Combine(
                            this.LocationSelectedChangedEvent, value);
                }
            remove
                {
                    this.LocationSelectedChangedEvent =
                        (LocationSelectedChangedEventHandler) Delegate.Remove(this.LocationSelectedChangedEvent, value);
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