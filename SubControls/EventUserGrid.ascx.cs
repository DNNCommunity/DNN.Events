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
    using System.ComponentModel;
    using System.Reflection;
    using System.Web.UI.WebControls;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Entities.Users;
    using DotNetNuke.Security.Roles;
    using DotNetNuke.Services.Localization;
    using global::Components;

    [DefaultEvent("Refreshed")]
    public partial class EventUserGrid : EventBase
    {
        public delegate void AddSelectedUsersEventHandler(object sender, EventArgs e, ArrayList arrUsers);

        private static readonly string _myFileName = typeof(EventIcons).BaseType.Name + ".ascx";
        //  Inherits Framework.UserControlBase

        private AddSelectedUsersEventHandler AddSelectedUsersEvent;


        protected ArrayList Users { get; set; } = new ArrayList();

        protected new string LocalResourceFile => Localization.GetResourceFile(this, _myFileName);

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     Gets the Page Size for the Grid
        /// </summary>
        /// <history>
        ///     [cnurse]	03/02/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected int PageSize
        {
            get
                {
                    var setting = UserModuleBase.GetSetting(this.PortalId, "Records_PerPage");
                    return Convert.ToInt32(setting);
                }
        }


        protected int ItemID => Convert.ToInt32(this.Request.QueryString["ItemID"]);

        public event AddSelectedUsersEventHandler AddSelectedUsers
        {
            add
                {
                    this.AddSelectedUsersEvent =
                        (AddSelectedUsersEventHandler) Delegate.Combine(this.AddSelectedUsersEvent, value);
                }
            remove
                {
                    this.AddSelectedUsersEvent =
                        (AddSelectedUsersEventHandler) Delegate.Remove(this.AddSelectedUsersEvent, value);
                }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            { }
        }

        private void Localize_Text()
        {
            var localText = "";
            localText = Localization.GetString("lblStartswith.Text", this.LocalResourceFile);
            if (!string.IsNullOrEmpty(localText))
            {
                this.lblStartswith.Text = localText;
            }
            localText = Localization.GetString("cmdSelectedAddUser.Text", this.LocalResourceFile);
            if (!string.IsNullOrEmpty(localText))
            {
                this.cmdSelectedAddUser.Text = localText;
            }
            localText = Localization.GetString("cmdRefreshList.Text", this.LocalResourceFile);
            if (!string.IsNullOrEmpty(localText))
            {
                this.cmdRefreshList.Text = localText;
            }
            localText = Localization.GetString("Select.Header", this.LocalResourceFile);
            if (!string.IsNullOrEmpty(localText))
            {
                this.gvUsersToEnroll.Columns[0].HeaderText = localText;
            }
            localText = Localization.GetString("Username.Header", this.LocalResourceFile);
            if (!string.IsNullOrEmpty(localText))
            {
                this.gvUsersToEnroll.Columns[1].HeaderText = localText;
            }
            localText = Localization.GetString("Displayname.Header", this.LocalResourceFile);
            if (!string.IsNullOrEmpty(localText))
            {
                this.gvUsersToEnroll.Columns[2].HeaderText = localText;
            }
            localText = Localization.GetString("Emailaddress.Header", this.LocalResourceFile);
            if (!string.IsNullOrEmpty(localText))
            {
                this.gvUsersToEnroll.Columns[3].HeaderText = localText;
            }
        }

        public void RefreshGrid()
        {
            const string csname = "ChangeScrip";
            var cstype = MethodBase.GetCurrentMethod().GetType();
            var cstext = Convert.ToString("function ChangedropdownFilterItem(event) {" + "\r\n" +
                                          "var DropDownFilterItem = document.getElementById('" +
                                          this.dropdownFilterItem.ClientID + "');" +
                                          "var lblStartswith = document.getElementById('" +
                                          this.lblStartswith.ClientID + "');" +
                                          "if (DropDownFilterItem.value =='1') lblStartswith.style.display = 'none';" +
                                          "\r\n" +
                                          "else lblStartswith.style.display = '';" + "\r\n" + "}");

            if (!this.Page.ClientScript.IsClientScriptBlockRegistered(csname))
            {
                this.Page.ClientScript.RegisterClientScriptBlock(cstype, csname, cstext, true);
            }

            this.Localize_Text();
            this.BindData(this.txtFilterUsers.Text, this.dropdownFilterItem.Value);
        }


        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     BindData gets the users from the Database and binds them to the DataGrid
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="searchText">Text to Search</param>
        /// <param name="searchField">Field to Search</param>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------
        private void BindData(string searchText, string searchField)
        {
            this.gvUsersToEnroll.PageSize = this.PageSize;

            var objCtlRole = new RoleController();
            var objRole = objCtlRole.GetRoleByName(this.PortalId, this.PortalSettings.RegisteredRoleName);
            var roleName = "";
            var regRoleName = "";
            if (!ReferenceEquals(objRole, null))
            {
                roleName = objRole.RoleName;
                regRoleName = roleName;
            }
            var ddEnrollRoles = (DropDownList) this.Parent.FindControl("ddEnrollRoles");
            if (ddEnrollRoles.SelectedValue != "-1")
            {
                roleName = ddEnrollRoles.SelectedItem.Text;
            }

            this.dropdownFilterItem.Items.Clear();
            this.dropdownFilterItem.Items.Add(
                new ListItem(Localization.GetString("dropdownFilterItem00.Text", this.LocalResourceFile), "0"));
            this.dropdownFilterItem.Items.Add(
                new ListItem(Localization.GetString("dropdownFilterItem02.Text", this.LocalResourceFile), "2"));
            if (roleName == regRoleName)
            {
                this.dropdownFilterItem.Items.Add(
                    new ListItem(Localization.GetString("dropdownFilterItem01.Text", this.LocalResourceFile),
                                 "1"));
            }

            var tmpUsers = default(ArrayList);
            if (roleName != regRoleName || searchField != "1")
            {
                tmpUsers = objCtlRole.GetUsersByRoleName(this.PortalId, roleName);
            }
            else
            {
                tmpUsers = objCtlRole.GetUsersByRoleName(this.PortalId, searchText);
            }

            var objCtlEventSignups = new EventSignupsController();
            var lstSignups = objCtlEventSignups.EventsSignupsGetEvent(this.ItemID, this.ModuleId);

            this.Users = new ArrayList();
            if (searchText != "None")
            {
                foreach (UserInfo objUser in tmpUsers)
                {
                    switch (searchField)
                    {
                        case "0": //username
                            if (objUser.Username.Substring(0, searchText.Length).ToLower() == searchText.ToLower())
                            {
                                this.UserAdd(objUser, lstSignups);
                            }
                            break;
                        case "1": //Groupname
                            this.UserAdd(objUser, lstSignups);
                            break;
                        case "2": //Lastname
                            if (objUser.LastName.Substring(0, searchText.Length).ToLower() == searchText.ToLower())
                            {
                                this.UserAdd(objUser, lstSignups);
                            }
                            break;
                        default:
                            this.UserAdd(objUser, lstSignups);
                            break;
                    }
                }
            }
            if (this.Users.Count > 0)
            {
                this.gvUsersToEnroll.Visible = true;
                this.cmdSelectedAddUser.Visible = true;
            }
            else
            {
                this.gvUsersToEnroll.Visible = false;
                this.cmdSelectedAddUser.Visible = false;
            }
            this.gvUsersToEnroll.DataSource = this.Users;
            this.gvUsersToEnroll.DataBind();
        }

        private void UserAdd(UserInfo inUser, ArrayList lstSignups)
        {
            var blAdd = true;
            foreach (EventSignupsInfo objEventSignup in lstSignups)
            {
                if (inUser.UserID == objEventSignup.UserID)
                {
                    blAdd = false;
                }
            }
            if (blAdd)
            {
                this.Users.Add(inUser);
            }
        }

        protected void gvUsersToEnroll_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.gvUsersToEnroll.PageIndex = e.NewPageIndex;
            this.BindData(this.txtFilterUsers.Text, this.dropdownFilterItem.Value);
        }

        protected void cmdSelectedAddUser_Click(object sender, EventArgs e)
        {
            var row = default(GridViewRow);
            var arrUsers = new ArrayList();
            try
            {
                foreach (GridViewRow tempLoopVar_row in this.gvUsersToEnroll.Rows)
                {
                    row = tempLoopVar_row;
                    if (((CheckBox) row.FindControl("chkSelectUser")).Checked)
                    {
                        arrUsers.Add(Convert.ToInt32(this.gvUsersToEnroll.DataKeys[row.RowIndex].Value));
                    }
                }
                if (this.AddSelectedUsersEvent != null)
                {
                    this.AddSelectedUsersEvent(this, new EventArgs(), arrUsers);
                }
            }
            catch (Exception)
            { }
        }

        protected void cmdRefreshList_Click(object sender, EventArgs e)
        {
            this.gvUsersToEnroll.PageIndex = 0;
            this.BindData(this.txtFilterUsers.Text, this.dropdownFilterItem.Value);
            if (this.dropdownFilterItem.Value == "1")
            {
                this.lblStartswith.Attributes.Add("style", "display: none");
            }
            else
            {
                this.lblStartswith.Attributes.Remove("style");
            }
        }

        // ReSharper disable EventNeverInvoked
        public delegate void RefreshedEventHandler(object sender, EventArgs e);

        private RefreshedEventHandler RefreshedEvent;

        public event RefreshedEventHandler Refreshed
        {
            add { this.RefreshedEvent = (RefreshedEventHandler) Delegate.Combine(this.RefreshedEvent, value); }
            remove { this.RefreshedEvent = (RefreshedEventHandler) Delegate.Remove(this.RefreshedEvent, value); }
        }

        // ReSharper restore EventNeverInvoked
    }
}