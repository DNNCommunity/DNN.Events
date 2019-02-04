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
using System.Drawing;
using System.Reflection;
using System.Web.UI.WebControls;
using Components;
using DNNtc;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.Modules.Events
{
    [DNNtc.ModuleControlProperties("Categories", "Edit Event Categories", ControlType.View, "https://github.com/DNNCommunity/DNN.Events/wiki", false, true)]
    public partial class EventEditCategories : EventBase
    {
        #region Event Handler

        private void Page_Load(object sender, EventArgs e)
        {
            LocalizeAll();

            if (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName) || IsCategoryEditor())
            { }
            else
            {
                Response.Redirect(GetSocialNavigateUrl(), true);
            }

            // Set the selected theme
            SetTheme(pnlEventsModuleCategories);

            if (!Page.IsPostBack)
            {
                SetDefaultValues();
                BindData();
            }
        }

        #endregion

        #region Private Functions

        private void SetDefaultValues()
        {
            //Back to normal (add) mode
            txtCategoryName.Text = "";
            txtCategoryColor.Text = "";
            txtCategoryFontColor.Text = "";
            ViewState.Remove("Category");
            cmdUpdate.Visible = false;
            cpBackColor.SelectedColor = ColorTranslator.FromHtml(DefaultBackColor);
            cpForeColor.SelectedColor = ColorTranslator.FromHtml(DefaultFontColor);
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

            //Add the external Validation.js to the Page
            const string csname = "ExtValidationScriptFile";
            var cstype = MethodBase.GetCurrentMethod().GetType();
            var cstext = "<script src=\"" + ResolveUrl("~/DesktopModules/Events/Scripts/Validation.js") +
                         "\" type=\"text/javascript\"></script>";
            if (!Page.ClientScript.IsClientScriptBlockRegistered(csname))
            {
                Page.ClientScript.RegisterClientScriptBlock(cstype, csname, cstext, false);
            }

            const string csname2 = "LoadPreview";
            var cstype2 = MethodBase.GetCurrentMethod().GetType();
            var cstext2 = "<script type=\"text/javascript\">byid('" + lblPreviewCat.ClientID +
                          "').innerHTML = byid('" + txtCategoryName.ClientID + "').value;byid('" +
                          lblPreviewCat.ClientID + "').style.color = byid('" + txtCategoryFontColor.ClientID +
                          "').value;byid('" + previewpane.ClientID + "').style.backgroundColor = byid('" +
                          txtCategoryColor.ClientID + "').value;</script>";
            Page.ClientScript.RegisterStartupScript(cstype2, csname2, cstext2, false);

            var previewScript = "";
            previewScript = "CategoryPreviewPane('" + cpBackColor.ClientID + "','" + cpForeColor.ClientID +
                            "','" + previewpane.ClientID + "','" + lblPreviewCat.ClientID + "','" +
                            txtCategoryFontColor.ClientID + "','" + txtCategoryColor.ClientID + "','" +
                            txtCategoryName.ClientID + "','" +
                            Localization.GetString("InvalidColor", LocalResourceFile) + "');";

            const string csname3 = "ColorPicker";
            var cstype3 = MethodBase.GetCurrentMethod().GetType();
            var cstext3 = "<script type=\"text/javascript\">";
            cstext3 += " function HandleColorChange(sender,eventargs) { $get(\"" + txtCategoryColor.ClientID +
                       "\").value = sender.get_selectedColor(); " + previewScript + "}";
            cstext3 += " function HandleColorFontChange(sender,eventargs) { $get(\"" +
                       txtCategoryFontColor.ClientID + "\").value = sender.get_selectedColor(); " + previewScript +
                       "}";
            cstext3 += "  </script>";
            if (!Page.ClientScript.IsClientScriptBlockRegistered(csname3))
            {
                Page.ClientScript.RegisterClientScriptBlock(cstype3, csname3, cstext3, false);
            }

            txtCategoryName.Attributes.Add(
                "onchange", "byid('" + lblPreviewCat.ClientID + "').innerHTML = this.value;");

            txtCategoryFontColor.Attributes.Add("onchange", previewScript);
            txtCategoryColor.Attributes.Add("onchange", previewScript);


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
            GrdCategories.Columns[3].HeaderText = Localization.GetString("plCategoryName", LocalResourceFile);
        }

        private void BindData()
        {
            var i = 0;

            _colCategories = _objCtlCategory.EventsCategoryList(PortalId);
            GrdCategories.DataSource = _colCategories;
            GrdCategories.DataBind();
            if (GrdCategories.Items.Count > 0)
            {
                GrdCategories.Visible = true;
                for (i = 0; i <= GrdCategories.Items.Count - 1; i++)
                {
                    ((ImageButton) GrdCategories.Items[i].FindControl("DeleteButton")).Attributes.Add(
                        "onclick",
                        "javascript:return confirm('" +
                        Localization.GetString(
                            "AreYouSureYouWishToDelete.Text",
                            LocalResourceFile) +
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
                    int category = Convert.ToInt16(GrdCategories.DataKeys[e.Item.ItemIndex]);
                    _objCategory = _objCtlCategory.EventCategoryGet(category, PortalId);
                    txtCategoryName.Text = _objCategory.CategoryName;
                    if (_objCategory.Color != "")
                    {
                        txtCategoryColor.Text = _objCategory.Color;
                        cpBackColor.SelectedColor = ColorTranslator.FromHtml(txtCategoryColor.Text);
                    }
                    else
                    {
                        txtCategoryColor.Text = "";
                        cpBackColor.SelectedColor = ColorTranslator.FromHtml(DefaultBackColor);
                    }
                    if (_objCategory.FontColor != "")
                    {
                        txtCategoryFontColor.Text = _objCategory.FontColor;
                        cpForeColor.SelectedColor = ColorTranslator.FromHtml(txtCategoryFontColor.Text);
                    }
                    else
                    {
                        txtCategoryFontColor.Text = "";
                        cpForeColor.SelectedColor = ColorTranslator.FromHtml(DefaultFontColor);
                    }

                    //Remember that we might use update
                    ViewState.Add("Category", _objCategory.Category.ToString());
                    cmdUpdate.Visible = true;

                    BindData();
                    break;
            }
        }

        protected void GrdCategories_DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            var category = 0;
            category = Convert.ToInt32(GrdCategories.DataKeys[e.Item.ItemIndex]);
            divDeleteError.Visible = false;

            var objDesktopModule = default(DesktopModuleInfo);
            var objModules = new ModuleController();
            objDesktopModule = DesktopModuleController.GetDesktopModuleByModuleName("DNN_Events", PortalId);
            var lstModules = objModules.GetModulesByDefinition(PortalId, objDesktopModule.FriendlyName);
            foreach (ModuleInfo objModule in lstModules)
            {
                var categories = Settings.ModuleCategoryIDs;
                //EventModuleSettings ems = new EventModuleSettings();
                //ArrayList categories = ems.GetEventModuleSettings(objModule.ModuleID, null).ModuleCategoryIDs;
                if (categories.Contains(category))
                {
                    lblDeleteError.Text =
                        string.Format(Localization.GetString("lblDeleteError", LocalResourceFile),
                                      objModule.ModuleTitle);
                    divDeleteError.Visible = true;
                    return;
                }
            }


            _objCtlCategory.EventsCategoryDelete(category, PortalId);
            BindData();

            //Be sure we cannot update any more
            txtCategoryName.Text = "";
            txtCategoryColor.Text = "";
            txtCategoryFontColor.Text = "";
            ViewState.Remove("Category");
            cmdUpdate.Visible = false;
            cpBackColor.SelectedColor = ColorTranslator.FromHtml(DefaultBackColor);
            cpForeColor.SelectedColor = ColorTranslator.FromHtml(DefaultFontColor);
        }

        protected void cmdUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                // Only Update if the Entered Data is Valid
                if (Page.IsValid && !string.IsNullOrEmpty(txtCategoryName.Text))
                {
                    var objCategory = new EventCategoryInfo();
                    var objSecurity = new PortalSecurity();
                    var categoryName = "";

                    // Filter text for non-admins
                    if (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName))
                    {
                        categoryName = txtCategoryName.Text;
                    }
                    else
                    {
                        categoryName =
                            objSecurity.InputFilter(txtCategoryName.Text, PortalSecurity.FilterFlag.NoScripting);
                    }

                    //bind text values to object

                    objCategory.Category = Convert.ToInt32(ViewState["Category"]);
                    objCategory.PortalID = PortalId;
                    objCategory.CategoryName = categoryName;
                    objCategory.Color = txtCategoryColor.Text;
                    objCategory.FontColor = txtCategoryFontColor.Text;
                    _objCtlCategory.EventsCategorySave(objCategory);

                    SetDefaultValues();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

            BindData();
        }

        protected void cmdAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Only Update if the Entered Data is Valid
                if (Page.IsValid && !string.IsNullOrEmpty(txtCategoryName.Text))
                {
                    var objCategory = new EventCategoryInfo();
                    var objSecurity = new PortalSecurity();
                    var categoryName = "";

                    // Filter text for non-admins
                    if (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName))
                    {
                        categoryName = txtCategoryName.Text;
                    }
                    else
                    {
                        categoryName =
                            objSecurity.InputFilter(txtCategoryName.Text, PortalSecurity.FilterFlag.NoScripting);
                    }

                    //bind text values to object

                    objCategory.Category = 0;
                    objCategory.PortalID = PortalId;
                    objCategory.CategoryName = categoryName;
                    objCategory.Color = txtCategoryColor.Text;
                    objCategory.FontColor = txtCategoryFontColor.Text;
                    _objCtlCategory.EventsCategorySave(objCategory);

                    SetDefaultValues();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

            BindData();
        }

        protected void returnButton_Click(object sender, EventArgs e)
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