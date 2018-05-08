using DotNetNuke.Services.Exceptions;
using System.Diagnostics;
using System.Web.UI.WebControls;
using System.Collections;
using DotNetNuke.Services.Localization;
using System;
using DotNetNuke.Security;
using DotNetNuke.Common.Lists;
using System.Collections.Generic;
using System.Linq;


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

    [DNNtc.ModuleControlProperties("Locations", "Edit Event Locations", DNNtc.ControlType.View, "https://dnnevents.codeplex.com/documentation", true, true)]
    public partial class EventEditLocations : EventBase
    {

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

            //Add the external Validation.js to the Page
            const string csname = "ExtValidationScriptFile";
            Type cstype = System.Reflection.MethodBase.GetCurrentMethod().GetType();
            string cstext = "<script src=\"" + ResolveUrl("~/DesktopModules/Events/Scripts/Validation.js") + "\" type=\"text/javascript\"></script>";
            if (!Page.ClientScript.IsClientScriptBlockRegistered(csname))
            {
                Page.ClientScript.RegisterClientScriptBlock(cstype, csname, cstext, false);
            }

        }

        #endregion

        #region Private Area

        private EventLocationController _objCtlLocation = new EventLocationController();
        private EventLocationInfo _objLocation = new EventLocationInfo();
        private ArrayList _colLocations;
        #endregion

        #region Event Handlers
        private void Page_Load(System.Object sender, EventArgs e)
        {
            LocalizeAll();

            if (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName.ToString()) ||
                IsLocationEditor())
            {
            }
            else
            {
                Response.Redirect(GetSocialNavigateUrl(), true);
            }

            // Set the selected theme
            SetTheme(pnlEventsModuleLocations);

            if (!Page.IsPostBack)
            {
                BindData();
                BindCountry();
                BindRegion();
            }
        }
        #endregion

        #region Helper Routines
        private void LocalizeAll()
        {
            GrdLocations.Columns[3].HeaderText = Localization.GetString("plLocationName", LocalResourceFile);
            GrdLocations.Columns[4].HeaderText = Localization.GetString("plMapURL", LocalResourceFile);
        }

        private void BindData()
        {
            //Grid
            int i = 0;
            _colLocations = _objCtlLocation.EventsLocationList(PortalId);
            GrdLocations.DataSource = _colLocations;
            GrdLocations.DataBind();
            if (GrdLocations.Items.Count > 0)
            {
                GrdLocations.Visible = true;
                for (i = 0; i <= GrdLocations.Items.Count - 1; i++)
                {
                    ((ImageButton)(GrdLocations.Items[i].FindControl("DeleteButton"))).Attributes.Add("onclick", "javascript:return confirm('" + Localization.GetString("AreYouSureYouWishToDelete.Text", LocalResourceFile) + "');");
                }
            }
        }

        private void BindCountry()
        {
            ListController ctlEntry = new ListController();
            IEnumerable<ListEntryInfo> entryCollection = ctlEntry.GetListEntryInfoItems("Country");

            cboCountry.DataSource = entryCollection;
            cboCountry.DataBind();
            cboCountry.Items.Insert(0, new ListItem("<" + Localization.GetString("Not_Specified", Localization.SharedResourceFile) + ">", ""));
            if (!Page.IsPostBack)
            {
                cboCountry.SelectedIndex = 0;
            }
        }

        private void BindRegion()
        {
            ListController ctlEntry = new ListController();
            string countryCode = System.Convert.ToString(cboCountry.SelectedItem.Value);
            string listKey = "Country." + countryCode; // listKey in format "Country.US:Region"
            System.Collections.Generic.IEnumerable<ListEntryInfo> entryCollection = ctlEntry.GetListEntryInfoItems("Region", listKey);

            if (entryCollection.Any())
            {
                txtRegion.Visible = false;
                cboRegion.Visible = true;

                cboRegion.Items.Clear();
                cboRegion.DataSource = entryCollection;
                cboRegion.DataBind();
                cboRegion.Items.Insert(0, new ListItem("<" + Localization.GetString("Not_Specified", Localization.SharedResourceFile) + ">", ""));

                if (countryCode.ToLower() == "us")
                {
                    lblRegionCap.Text = Localization.GetString("plState.Text", LocalResourceFile);
                    lblRegionCap.HelpText = Localization.GetString("plState.Help", LocalResourceFile);
                    lblPostalCodeCap.Text = Localization.GetString("plZipCode.Text", LocalResourceFile);
                    lblPostalCodeCap.HelpText = Localization.GetString("plZipCode.Help", LocalResourceFile);
                }
                else
                {
                    lblRegionCap.Text = Localization.GetString("plRegion.Text", LocalResourceFile);
                    lblRegionCap.HelpText = Localization.GetString("plRegion.Help", LocalResourceFile);
                    lblPostalCodeCap.Text = Localization.GetString("plPostalCode.Text", LocalResourceFile);
                    lblPostalCodeCap.HelpText = Localization.GetString("plPostalCode.Help", LocalResourceFile);
                }
            }
            else
            {
                txtRegion.Visible = true;
                cboRegion.Visible = false;

                lblRegionCap.Text = Localization.GetString("plRegion.Text", LocalResourceFile);
                lblRegionCap.HelpText = Localization.GetString("plRegion.Help", LocalResourceFile);
                lblPostalCodeCap.Text = Localization.GetString("plPostalCode.Text", LocalResourceFile);
                lblPostalCodeCap.HelpText = Localization.GetString("plPostalCode.Help", LocalResourceFile);
            }
        }

        private string PutHTTPInFront(string myURL)
        {
            if (myURL.ToLower().StartsWith("http://") || myURL.ToLower().StartsWith("https://"))
            {
                return myURL;
            }
            else if (myURL.Trim() == "")
            {
                return "";
            }
            else
            {
                return DotNetNuke.Common.Globals.AddHTTP(myURL);
            }
        }

        protected void cmdUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                SaveLocation(System.Convert.ToInt32(ViewState["Location"]));
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            BindData();
        }

        public void cmdAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SaveLocation(0);
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            BindData();
        }

        private void SaveLocation(int locationID)
        {
            // Only Update if the Entered Data is Valid
            if (Page.IsValid && !string.IsNullOrEmpty(txtLocationName.Text))
            {

                EventLocationInfo objLocation = new EventLocationInfo();
                PortalSecurity objSecurity = new PortalSecurity();
                string locationName = "";
                string mapURL = "";
                string street = "";
                string postalCode = "";
                string city = "";
                string region = null;
                string country = null;

                // Filter text for non-admins
                if (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName.ToString()) == true)
                {
                    locationName = txtLocationName.Text;
                    mapURL = PutHTTPInFront(txtMapURL.Text);
                    street = txtStreet.Text;
                    postalCode = txtPostalCode.Text;
                    city = txtCity.Text;
                    if (cboRegion.SelectedIndex > 0)
                    {
                        region = cboRegion.SelectedItem.Text;
                    }
                    else
                    {
                        region = txtRegion.Text;
                    }
                    if (cboCountry.SelectedIndex > 0)
                    {
                        country = System.Convert.ToString(cboCountry.SelectedItem.Text);
                    }
                }
                else
                {
                    locationName = objSecurity.InputFilter(txtLocationName.Text, PortalSecurity.FilterFlag.NoScripting);
                    mapURL = objSecurity.InputFilter(PutHTTPInFront(txtMapURL.Text), PortalSecurity.FilterFlag.NoScripting);
                    street = objSecurity.InputFilter(txtStreet.Text, PortalSecurity.FilterFlag.NoScripting);
                    postalCode = objSecurity.InputFilter(txtPostalCode.Text, PortalSecurity.FilterFlag.NoScripting);
                    city = objSecurity.InputFilter(txtCity.Text, PortalSecurity.FilterFlag.NoScripting);
                    if (cboRegion.SelectedIndex > 0)
                    {
                        region = objSecurity.InputFilter(cboRegion.SelectedItem.Text, PortalSecurity.FilterFlag.NoScripting);
                    }
                    else
                    {
                        region = objSecurity.InputFilter(txtRegion.Text, PortalSecurity.FilterFlag.NoScripting);
                    }
                    if (cboCountry.SelectedIndex > 0)
                    {
                        country = objSecurity.InputFilter(System.Convert.ToString(cboCountry.SelectedItem.Text), PortalSecurity.FilterFlag.NoScripting);
                    }
                }

                //bind text values to object
                objLocation.Location = locationID;
                objLocation.PortalID = PortalId;
                objLocation.LocationName = locationName;
                objLocation.MapURL = mapURL;
                objLocation.Street = street;
                objLocation.PostalCode = postalCode;
                objLocation.City = city;
                objLocation.Region = region;
                objLocation.Country = country;
                _objCtlLocation.EventsLocationSave(objLocation);

                //Back to normal (add) mode
                txtLocationName.Text = "";
                txtMapURL.Text = "";
                txtStreet.Text = "";
                txtPostalCode.Text = "";
                txtCity.Text = "";
                txtRegion.Text = "";
                cboRegion.ClearSelection();
                cboCountry.ClearSelection();

                ViewState.Remove("Location");
                cmdUpdate.Visible = false;
            }
        }

        protected void OnCountryIndexChanged(object sender, EventArgs e)
        {
            try
            {
                BindRegion();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        #endregion

        #region Control Events
        public void GrdLocations_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            int location = 0;
            location = System.Convert.ToInt32(GrdLocations.DataKeys[e.Item.ItemIndex]);
            _objCtlLocation.EventsLocationDelete(location, PortalId);
            BindData();

            //Back to normal (add) mode
            txtLocationName.Text = "";
            txtMapURL.Text = "";
            txtStreet.Text = "";
            txtPostalCode.Text = "";
            txtCity.Text = "";
            txtRegion.Text = "";
            cboRegion.ClearSelection();
            cboCountry.ClearSelection();

            ViewState.Remove("Location");
            cmdUpdate.Visible = false;
        }

        public void GrdLocations_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Select":
                    int location = System.Convert.ToInt16(GrdLocations.DataKeys[e.Item.ItemIndex]);
                    _objLocation = _objCtlLocation.EventsLocationGet(location, PortalId);

                    // Clear selections.
                    cboCountry.ClearSelection();
                    cboRegion.ClearSelection();
                    txtRegion.Text = string.Empty;

                    // Fill fields.
                    txtLocationName.Text = _objLocation.LocationName;
                    txtMapURL.Text = _objLocation.MapURL;
                    txtStreet.Text = _objLocation.Street;
                    txtPostalCode.Text = _objLocation.PostalCode;
                    txtCity.Text = _objLocation.City;

                    if (cboCountry.Items.FindByText(_objLocation.Country) != null)
                    {
                        cboCountry.Items.FindByText(_objLocation.Country).Selected = true;
                    }
                    BindRegion();
                    if (cboRegion.Items.FindByText(_objLocation.Region) != null)
                    {
                        cboRegion.Items.FindByText(_objLocation.Region).Selected = true;
                    }
                    else
                    {
                        txtRegion.Text = _objLocation.Region;
                    }

                    ViewState.Add("Location", _objLocation.Location.ToString());
                    BindData();

                    //We can update
                    cmdUpdate.Visible = true;
                    break;
            }
        }

        protected void returnButton_Click(System.Object sender, EventArgs e)
        {
            try
            {
                Response.Redirect(GetSocialNavigateUrl(), true);

            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion

    }

}


