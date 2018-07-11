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
    using System.Linq;
    using System.Reflection;
    using System.Web.UI.WebControls;
    using DotNetNuke.Common;
    using DotNetNuke.Common.Lists;
    using DotNetNuke.Security;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Localization;
    using global::Components;

    [DNNtc.ModuleControlProperties("Locations", "Edit Event Locations", DNNtc.ControlType.View, "https://dnnevents.codeplex.com/documentation", true, true)]
    public partial class EventEditLocations : EventBase
    {
        #region Event Handlers

        private void Page_Load(object sender, EventArgs e)
        {
            this.LocalizeAll();

            if (PortalSecurity.IsInRole(this.PortalSettings.AdministratorRoleName) || this.IsLocationEditor())
            { }
            else
            {
                this.Response.Redirect(this.GetSocialNavigateUrl(), true);
            }

            // Set the selected theme
            this.SetTheme(this.pnlEventsModuleLocations);

            if (!this.Page.IsPostBack)
            {
                this.BindData();
                this.BindCountry();
                this.BindRegion();
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

            //Add the external Validation.js to the Page
            const string csname = "ExtValidationScriptFile";
            var cstype = MethodBase.GetCurrentMethod().GetType();
            var cstext = "<script src=\"" + this.ResolveUrl("~/DesktopModules/Events/Scripts/Validation.js") +
                         "\" type=\"text/javascript\"></script>";
            if (!this.Page.ClientScript.IsClientScriptBlockRegistered(csname))
            {
                this.Page.ClientScript.RegisterClientScriptBlock(cstype, csname, cstext, false);
            }
        }

        #endregion

        #region Private Area

        private readonly EventLocationController _objCtlLocation = new EventLocationController();
        private EventLocationInfo _objLocation = new EventLocationInfo();
        private ArrayList _colLocations;

        #endregion

        #region Helper Routines

        private void LocalizeAll()
        {
            this.GrdLocations.Columns[3].HeaderText = Localization.GetString("plLocationName", this.LocalResourceFile);
            this.GrdLocations.Columns[4].HeaderText = Localization.GetString("plMapURL", this.LocalResourceFile);
        }

        private void BindData()
        {
            //Grid
            var i = 0;
            this._colLocations = this._objCtlLocation.EventsLocationList(this.PortalId);
            this.GrdLocations.DataSource = this._colLocations;
            this.GrdLocations.DataBind();
            if (this.GrdLocations.Items.Count > 0)
            {
                this.GrdLocations.Visible = true;
                for (i = 0; i <= this.GrdLocations.Items.Count - 1; i++)
                {
                    ((ImageButton) this.GrdLocations.Items[i].FindControl("DeleteButton")).Attributes.Add(
                        "onclick",
                        "javascript:return confirm('" +
                        Localization.GetString(
                            "AreYouSureYouWishToDelete.Text",
                            this
                                .LocalResourceFile) +
                        "');");
                }
            }
        }

        private void BindCountry()
        {
            var ctlEntry = new ListController();
            var entryCollection = ctlEntry.GetListEntryInfoItems("Country");

            this.cboCountry.DataSource = entryCollection;
            this.cboCountry.DataBind();
            this.cboCountry.Items.Insert(
                0,
                new ListItem("<" + Localization.GetString("Not_Specified", Localization.SharedResourceFile) + ">",
                             ""));
            if (!this.Page.IsPostBack)
            {
                this.cboCountry.SelectedIndex = 0;
            }
        }

        private void BindRegion()
        {
            var ctlEntry = new ListController();
            var countryCode = Convert.ToString(this.cboCountry.SelectedItem.Value);
            var listKey = "Country." + countryCode; // listKey in format "Country.US:Region"
            var entryCollection = ctlEntry.GetListEntryInfoItems("Region", listKey);

            if (entryCollection.Any())
            {
                this.txtRegion.Visible = false;
                this.cboRegion.Visible = true;

                this.cboRegion.Items.Clear();
                this.cboRegion.DataSource = entryCollection;
                this.cboRegion.DataBind();
                this.cboRegion.Items.Insert(
                    0,
                    new ListItem(
                        "<" + Localization.GetString("Not_Specified", Localization.SharedResourceFile) + ">", ""));

                if (countryCode.ToLower() == "us")
                {
                    this.lblRegionCap.Text = Localization.GetString("plState.Text", this.LocalResourceFile);
                    this.lblRegionCap.HelpText = Localization.GetString("plState.Help", this.LocalResourceFile);
                    this.lblPostalCodeCap.Text = Localization.GetString("plZipCode.Text", this.LocalResourceFile);
                    this.lblPostalCodeCap.HelpText = Localization.GetString("plZipCode.Help", this.LocalResourceFile);
                }
                else
                {
                    this.lblRegionCap.Text = Localization.GetString("plRegion.Text", this.LocalResourceFile);
                    this.lblRegionCap.HelpText = Localization.GetString("plRegion.Help", this.LocalResourceFile);
                    this.lblPostalCodeCap.Text = Localization.GetString("plPostalCode.Text", this.LocalResourceFile);
                    this.lblPostalCodeCap.HelpText =
                        Localization.GetString("plPostalCode.Help", this.LocalResourceFile);
                }
            }
            else
            {
                this.txtRegion.Visible = true;
                this.cboRegion.Visible = false;

                this.lblRegionCap.Text = Localization.GetString("plRegion.Text", this.LocalResourceFile);
                this.lblRegionCap.HelpText = Localization.GetString("plRegion.Help", this.LocalResourceFile);
                this.lblPostalCodeCap.Text = Localization.GetString("plPostalCode.Text", this.LocalResourceFile);
                this.lblPostalCodeCap.HelpText = Localization.GetString("plPostalCode.Help", this.LocalResourceFile);
            }
        }

        private string PutHTTPInFront(string myURL)
        {
            if (myURL.ToLower().StartsWith("http://") || myURL.ToLower().StartsWith("https://"))
            {
                return myURL;
            }
            if (myURL.Trim() == "")
            {
                return "";
            }
            return Globals.AddHTTP(myURL);
        }

        protected void cmdUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                this.SaveLocation(Convert.ToInt32(this.ViewState["Location"]));
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            this.BindData();
        }

        public void cmdAdd_Click(object sender, EventArgs e)
        {
            try
            {
                this.SaveLocation(0);
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            this.BindData();
        }

        private void SaveLocation(int locationID)
        {
            // Only Update if the Entered Data is Valid
            if (this.Page.IsValid && !string.IsNullOrEmpty(this.txtLocationName.Text))
            {
                var objLocation = new EventLocationInfo();
                var objSecurity = new PortalSecurity();
                var locationName = "";
                var mapURL = "";
                var street = "";
                var postalCode = "";
                var city = "";
                string region = null;
                string country = null;

                // Filter text for non-admins
                if (PortalSecurity.IsInRole(this.PortalSettings.AdministratorRoleName))
                {
                    locationName = this.txtLocationName.Text;
                    mapURL = this.PutHTTPInFront(this.txtMapURL.Text);
                    street = this.txtStreet.Text;
                    postalCode = this.txtPostalCode.Text;
                    city = this.txtCity.Text;
                    if (this.cboRegion.SelectedIndex > 0)
                    {
                        region = this.cboRegion.SelectedItem.Text;
                    }
                    else
                    {
                        region = this.txtRegion.Text;
                    }
                    if (this.cboCountry.SelectedIndex > 0)
                    {
                        country = Convert.ToString(this.cboCountry.SelectedItem.Text);
                    }
                }
                else
                {
                    locationName =
                        objSecurity.InputFilter(this.txtLocationName.Text, PortalSecurity.FilterFlag.NoScripting);
                    mapURL = objSecurity.InputFilter(this.PutHTTPInFront(this.txtMapURL.Text),
                                                     PortalSecurity.FilterFlag.NoScripting);
                    street = objSecurity.InputFilter(this.txtStreet.Text, PortalSecurity.FilterFlag.NoScripting);
                    postalCode =
                        objSecurity.InputFilter(this.txtPostalCode.Text, PortalSecurity.FilterFlag.NoScripting);
                    city = objSecurity.InputFilter(this.txtCity.Text, PortalSecurity.FilterFlag.NoScripting);
                    if (this.cboRegion.SelectedIndex > 0)
                    {
                        region = objSecurity.InputFilter(this.cboRegion.SelectedItem.Text,
                                                         PortalSecurity.FilterFlag.NoScripting);
                    }
                    else
                    {
                        region = objSecurity.InputFilter(this.txtRegion.Text, PortalSecurity.FilterFlag.NoScripting);
                    }
                    if (this.cboCountry.SelectedIndex > 0)
                    {
                        country = objSecurity.InputFilter(Convert.ToString(this.cboCountry.SelectedItem.Text),
                                                          PortalSecurity.FilterFlag.NoScripting);
                    }
                }

                //bind text values to object
                objLocation.Location = locationID;
                objLocation.PortalID = this.PortalId;
                objLocation.LocationName = locationName;
                objLocation.MapURL = mapURL;
                objLocation.Street = street;
                objLocation.PostalCode = postalCode;
                objLocation.City = city;
                objLocation.Region = region;
                objLocation.Country = country;
                this._objCtlLocation.EventsLocationSave(objLocation);

                //Back to normal (add) mode
                this.txtLocationName.Text = "";
                this.txtMapURL.Text = "";
                this.txtStreet.Text = "";
                this.txtPostalCode.Text = "";
                this.txtCity.Text = "";
                this.txtRegion.Text = "";
                this.cboRegion.ClearSelection();
                this.cboCountry.ClearSelection();

                this.ViewState.Remove("Location");
                this.cmdUpdate.Visible = false;
            }
        }

        protected void OnCountryIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.BindRegion();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion

        #region Control Events

        public void GrdLocations_DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            var location = 0;
            location = Convert.ToInt32(this.GrdLocations.DataKeys[e.Item.ItemIndex]);
            this._objCtlLocation.EventsLocationDelete(location, this.PortalId);
            this.BindData();

            //Back to normal (add) mode
            this.txtLocationName.Text = "";
            this.txtMapURL.Text = "";
            this.txtStreet.Text = "";
            this.txtPostalCode.Text = "";
            this.txtCity.Text = "";
            this.txtRegion.Text = "";
            this.cboRegion.ClearSelection();
            this.cboCountry.ClearSelection();

            this.ViewState.Remove("Location");
            this.cmdUpdate.Visible = false;
        }

        public void GrdLocations_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Select":
                    int location = Convert.ToInt16(this.GrdLocations.DataKeys[e.Item.ItemIndex]);
                    this._objLocation = this._objCtlLocation.EventsLocationGet(location, this.PortalId);

                    // Clear selections.
                    this.cboCountry.ClearSelection();
                    this.cboRegion.ClearSelection();
                    this.txtRegion.Text = string.Empty;

                    // Fill fields.
                    this.txtLocationName.Text = this._objLocation.LocationName;
                    this.txtMapURL.Text = this._objLocation.MapURL;
                    this.txtStreet.Text = this._objLocation.Street;
                    this.txtPostalCode.Text = this._objLocation.PostalCode;
                    this.txtCity.Text = this._objLocation.City;

                    if (this.cboCountry.Items.FindByText(this._objLocation.Country) != null)
                    {
                        this.cboCountry.Items.FindByText(this._objLocation.Country).Selected = true;
                    }
                    this.BindRegion();
                    if (this.cboRegion.Items.FindByText(this._objLocation.Region) != null)
                    {
                        this.cboRegion.Items.FindByText(this._objLocation.Region).Selected = true;
                    }
                    else
                    {
                        this.txtRegion.Text = this._objLocation.Region;
                    }

                    this.ViewState.Add("Location", this._objLocation.Location.ToString());
                    this.BindData();

                    //We can update
                    this.cmdUpdate.Visible = true;
                    break;
            }
        }

        protected void returnButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.Response.Redirect(this.GetSocialNavigateUrl(), true);
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion
    }
}