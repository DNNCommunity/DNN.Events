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
    using System.Reflection;
    using System.Web.UI.WebControls;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Security;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Localization;
    using global::Components;

    [DNNtc.ModuleControlProperties("Categories", "Edit Event Categories", DNNtc.ControlType.View, "https://dnnevents.codeplex.com/documentation", false, true)]
    public partial class EventEditCategories : EventBase
    {
        #region Event Handler

        private void Page_Load(object sender, EventArgs e)
        {
            this.LocalizeAll();

            if (PortalSecurity.IsInRole(this.PortalSettings.AdministratorRoleName) || this.IsCategoryEditor())
            { }
            else
            {
                this.Response.Redirect(this.GetSocialNavigateUrl(), true);
            }

            // Set the selected theme
            this.SetTheme(this.pnlEventsModuleCategories);

            if (!this.Page.IsPostBack)
            {
                this.SetDefaultValues();
                this.BindData();
            }
        }

        #endregion

        #region Private Functions

        private void SetDefaultValues()
        {
            //Back to normal (add) mode
            this.txtCategoryName.Text = "";
            this.txtCategoryColor.Text = "";
            this.txtCategoryFontColor.Text = "";
            this.ViewState.Remove("Category");
            this.cmdUpdate.Visible = false;
            this.cpBackColor.SelectedColor = ColorTranslator.FromHtml(DefaultBackColor);
            this.cpForeColor.SelectedColor = ColorTranslator.FromHtml(DefaultFontColor);
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

            const string csname2 = "LoadPreview";
            var cstype2 = MethodBase.GetCurrentMethod().GetType();
            var cstext2 = "<script type=\"text/javascript\">byid('" + this.lblPreviewCat.ClientID +
                          "').innerHTML = byid('" + this.txtCategoryName.ClientID + "').value;byid('" +
                          this.lblPreviewCat.ClientID + "').style.color = byid('" + this.txtCategoryFontColor.ClientID +
                          "').value;byid('" + this.previewpane.ClientID + "').style.backgroundColor = byid('" +
                          this.txtCategoryColor.ClientID + "').value;</script>";
            this.Page.ClientScript.RegisterStartupScript(cstype2, csname2, cstext2, false);

            var previewScript = "";
            previewScript = "CategoryPreviewPane('" + this.cpBackColor.ClientID + "','" + this.cpForeColor.ClientID +
                            "','" + this.previewpane.ClientID + "','" + this.lblPreviewCat.ClientID + "','" +
                            this.txtCategoryFontColor.ClientID + "','" + this.txtCategoryColor.ClientID + "','" +
                            this.txtCategoryName.ClientID + "','" +
                            Localization.GetString("InvalidColor", this.LocalResourceFile) + "');";

            const string csname3 = "ColorPicker";
            var cstype3 = MethodBase.GetCurrentMethod().GetType();
            var cstext3 = "<script type=\"text/javascript\">";
            cstext3 += " function HandleColorChange(sender,eventargs) { $get(\"" + this.txtCategoryColor.ClientID +
                       "\").value = sender.get_selectedColor(); " + previewScript + "}";
            cstext3 += " function HandleColorFontChange(sender,eventargs) { $get(\"" +
                       this.txtCategoryFontColor.ClientID + "\").value = sender.get_selectedColor(); " + previewScript +
                       "}";
            cstext3 += "  </script>";
            if (!this.Page.ClientScript.IsClientScriptBlockRegistered(csname3))
            {
                this.Page.ClientScript.RegisterClientScriptBlock(cstype3, csname3, cstext3, false);
            }

            this.txtCategoryName.Attributes.Add(
                "onchange", "byid('" + this.lblPreviewCat.ClientID + "').innerHTML = this.value;");

            this.txtCategoryFontColor.Attributes.Add("onchange", previewScript);
            this.txtCategoryColor.Attributes.Add("onchange", previewScript);


            //ColorPicker Icons
        }

        #endregion

        #region Private Area

        //Private itemId As Integer
        private readonly EventCategoryController _objCtlCategory = new EventCategoryController();

        private EventCategoryInfo _objCategory = new EventCategoryInfo();
        private ArrayList _colCategories;
        private const string DefaultBackColor = "#ffffff";
        private const string DefaultFontColor = "#000000";

        #endregion

        #region Helper Routines

        private void LocalizeAll()
        {
            this.GrdCategories.Columns[3].HeaderText = Localization.GetString("plCategoryName", this.LocalResourceFile);
        }

        private void BindData()
        {
            var i = 0;

            this._colCategories = this._objCtlCategory.EventsCategoryList(this.PortalId);
            this.GrdCategories.DataSource = this._colCategories;
            this.GrdCategories.DataBind();
            if (this.GrdCategories.Items.Count > 0)
            {
                this.GrdCategories.Visible = true;
                for (i = 0; i <= this.GrdCategories.Items.Count - 1; i++)
                {
                    ((ImageButton) this.GrdCategories.Items[i].FindControl("DeleteButton")).Attributes.Add(
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

        #endregion

        #region Control Events

        protected void GrdCategories_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Select":
                    int category = Convert.ToInt16(this.GrdCategories.DataKeys[e.Item.ItemIndex]);
                    this._objCategory = this._objCtlCategory.EventCategoryGet(category, this.PortalId);
                    this.txtCategoryName.Text = this._objCategory.CategoryName;
                    if (this._objCategory.Color != "")
                    {
                        this.txtCategoryColor.Text = this._objCategory.Color;
                        this.cpBackColor.SelectedColor = ColorTranslator.FromHtml(this.txtCategoryColor.Text);
                    }
                    else
                    {
                        this.txtCategoryColor.Text = "";
                        this.cpBackColor.SelectedColor = ColorTranslator.FromHtml(DefaultBackColor);
                    }
                    if (this._objCategory.FontColor != "")
                    {
                        this.txtCategoryFontColor.Text = this._objCategory.FontColor;
                        this.cpForeColor.SelectedColor = ColorTranslator.FromHtml(this.txtCategoryFontColor.Text);
                    }
                    else
                    {
                        this.txtCategoryFontColor.Text = "";
                        this.cpForeColor.SelectedColor = ColorTranslator.FromHtml(DefaultFontColor);
                    }

                    //Remember that we might use update
                    this.ViewState.Add("Category", this._objCategory.Category.ToString());
                    this.cmdUpdate.Visible = true;

                    this.BindData();
                    break;
            }
        }

        protected void GrdCategories_DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            var category = 0;
            category = Convert.ToInt32(this.GrdCategories.DataKeys[e.Item.ItemIndex]);
            this.divDeleteError.Visible = false;

            var objDesktopModule = default(DesktopModuleInfo);
            var objModules = new ModuleController();
            objDesktopModule = DesktopModuleController.GetDesktopModuleByModuleName("DNN_Events", this.PortalId);
            var lstModules = objModules.GetModulesByDefinition(this.PortalId, objDesktopModule.FriendlyName);
            foreach (ModuleInfo objModule in lstModules)
            {
                var categories = this.Settings.ModuleCategoryIDs;
                //EventModuleSettings ems = new EventModuleSettings();
                //ArrayList categories = ems.GetEventModuleSettings(objModule.ModuleID, null).ModuleCategoryIDs;
                if (categories.Contains(category))
                {
                    this.lblDeleteError.Text =
                        string.Format(Localization.GetString("lblDeleteError", this.LocalResourceFile),
                                      objModule.ModuleTitle);
                    this.divDeleteError.Visible = true;
                    return;
                }
            }


            this._objCtlCategory.EventsCategoryDelete(category, this.PortalId);
            this.BindData();

            //Be sure we cannot update any more
            this.txtCategoryName.Text = "";
            this.txtCategoryColor.Text = "";
            this.txtCategoryFontColor.Text = "";
            this.ViewState.Remove("Category");
            this.cmdUpdate.Visible = false;
            this.cpBackColor.SelectedColor = ColorTranslator.FromHtml(DefaultBackColor);
            this.cpForeColor.SelectedColor = ColorTranslator.FromHtml(DefaultFontColor);
        }

        protected void cmdUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                // Only Update if the Entered Data is Valid
                if (this.Page.IsValid && !string.IsNullOrEmpty(this.txtCategoryName.Text))
                {
                    var objCategory = new EventCategoryInfo();
                    var objSecurity = new PortalSecurity();
                    var categoryName = "";

                    // Filter text for non-admins
                    if (PortalSecurity.IsInRole(this.PortalSettings.AdministratorRoleName))
                    {
                        categoryName = this.txtCategoryName.Text;
                    }
                    else
                    {
                        categoryName =
                            objSecurity.InputFilter(this.txtCategoryName.Text, PortalSecurity.FilterFlag.NoScripting);
                    }

                    //bind text values to object

                    objCategory.Category = Convert.ToInt32(this.ViewState["Category"]);
                    objCategory.PortalID = this.PortalId;
                    objCategory.CategoryName = categoryName;
                    objCategory.Color = this.txtCategoryColor.Text;
                    objCategory.FontColor = this.txtCategoryFontColor.Text;
                    this._objCtlCategory.EventsCategorySave(objCategory);

                    this.SetDefaultValues();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

            this.BindData();
        }

        protected void cmdAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Only Update if the Entered Data is Valid
                if (this.Page.IsValid && !string.IsNullOrEmpty(this.txtCategoryName.Text))
                {
                    var objCategory = new EventCategoryInfo();
                    var objSecurity = new PortalSecurity();
                    var categoryName = "";

                    // Filter text for non-admins
                    if (PortalSecurity.IsInRole(this.PortalSettings.AdministratorRoleName))
                    {
                        categoryName = this.txtCategoryName.Text;
                    }
                    else
                    {
                        categoryName =
                            objSecurity.InputFilter(this.txtCategoryName.Text, PortalSecurity.FilterFlag.NoScripting);
                    }

                    //bind text values to object

                    objCategory.Category = 0;
                    objCategory.PortalID = this.PortalId;
                    objCategory.CategoryName = categoryName;
                    objCategory.Color = this.txtCategoryColor.Text;
                    objCategory.FontColor = this.txtCategoryFontColor.Text;
                    this._objCtlCategory.EventsCategorySave(objCategory);

                    this.SetDefaultValues();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

            this.BindData();
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