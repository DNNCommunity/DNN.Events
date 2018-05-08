using DotNetNuke.Entities.Users;
using System.Web.UI.WebControls;
using System.Collections;
using DotNetNuke.Services.Localization;
using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security.Roles;

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
		[System.ComponentModel.DefaultEvent("Refreshed")]public partial class EventUserGrid : EventBase
		{
			//  Inherits Framework.UserControlBase
			
			private ArrayList _users = new ArrayList();
			private static readonly string _myFileName = typeof(EventIcons).BaseType.Name +".ascx";
			// ReSharper disable EventNeverInvoked
			public delegate void RefreshedEventHandler(object sender, EventArgs e);
			private RefreshedEventHandler RefreshedEvent;
			
			public event RefreshedEventHandler Refreshed
			{
				add
				{
					RefreshedEvent = (RefreshedEventHandler) System.Delegate.Combine(RefreshedEvent, value);
				}
				remove
				{
					RefreshedEvent = (RefreshedEventHandler) System.Delegate.Remove(RefreshedEvent, value);
				}
			}
			
			// ReSharper restore EventNeverInvoked
			public delegate void AddSelectedUsersEventHandler(object sender, EventArgs e, ArrayList arrUsers);
			private AddSelectedUsersEventHandler AddSelectedUsersEvent;
			
			public event AddSelectedUsersEventHandler AddSelectedUsers
			{
				add
				{
					AddSelectedUsersEvent = (AddSelectedUsersEventHandler) System.Delegate.Combine(AddSelectedUsersEvent, value);
				}
				remove
				{
					AddSelectedUsersEvent = (AddSelectedUsersEventHandler) System.Delegate.Remove(AddSelectedUsersEvent, value);
				}
			}
			
			
			protected ArrayList Users
			{
				get
				{
					return _users;
				}
				set
				{
					_users = value;
				}
			}
			
			new protected string LocalResourceFile
			{
				get
				{
					return Localization.GetResourceFile(this, _myFileName);
				}
			}
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// Gets the Page Size for the Grid
			/// </summary>
			/// <history>
			/// 	[cnurse]	03/02/2006  Created
			/// </history>
			/// -----------------------------------------------------------------------------
			protected int PageSize
			{
				get
				{
					object setting = UserModuleBase.GetSetting(PortalId, "Records_PerPage");
					return System.Convert.ToInt32(setting);
				}
			}
			
			
			protected int ItemID
			{
				get
				{
					return System.Convert.ToInt32(Request.QueryString["ItemID"]);
				}
			}
			
			protected void Page_Load(object sender, EventArgs e)
			{
				if (!Page.IsPostBack)
				{
					
				}
			}
			
			private void Localize_Text()
			{
				string localText = "";
				localText = Localization.GetString("lblStartswith.Text", LocalResourceFile);
				if (!string.IsNullOrEmpty(localText))
				{
					lblStartswith.Text = localText;
				}
				localText = Localization.GetString("cmdSelectedAddUser.Text", LocalResourceFile);
				if (!string.IsNullOrEmpty(localText))
				{
					cmdSelectedAddUser.Text = localText;
				}
				localText = Localization.GetString("cmdRefreshList.Text", LocalResourceFile);
				if (!string.IsNullOrEmpty(localText))
				{
					cmdRefreshList.Text = localText;
				}
				localText = Localization.GetString("Select.Header", LocalResourceFile);
				if (!string.IsNullOrEmpty(localText))
				{
					gvUsersToEnroll.Columns[0].HeaderText = localText;
				}
				localText = Localization.GetString("Username.Header", LocalResourceFile);
				if (!string.IsNullOrEmpty(localText))
				{
					gvUsersToEnroll.Columns[1].HeaderText = localText;
				}
				localText = Localization.GetString("Displayname.Header", LocalResourceFile);
				if (!string.IsNullOrEmpty(localText))
				{
					gvUsersToEnroll.Columns[2].HeaderText = localText;
				}
				localText = Localization.GetString("Emailaddress.Header", LocalResourceFile);
				if (!string.IsNullOrEmpty(localText))
				{
					gvUsersToEnroll.Columns[3].HeaderText = localText;
				}
				
			}
			
			public void RefreshGrid()
			{
				const string csname = "ChangeScrip";
				Type cstype = System.Reflection.MethodBase.GetCurrentMethod().GetType();
				string cstext = System.Convert.ToString("function ChangedropdownFilterItem(event) {" + "\r\n" + 
					"var DropDownFilterItem = document.getElementById('" + dropdownFilterItem.ClientID + "');" + 
					"var lblStartswith = document.getElementById('" + lblStartswith.ClientID + "');" + 
					"if (DropDownFilterItem.value =='1') lblStartswith.style.display = 'none';" + "\r\n" + 
					"else lblStartswith.style.display = '';" + "\r\n" + "}");
				
				if (!Page.ClientScript.IsClientScriptBlockRegistered(csname))
				{
					Page.ClientScript.RegisterClientScriptBlock(cstype, csname, cstext, true);
				}
				
				Localize_Text();
				BindData(txtFilterUsers.Text, dropdownFilterItem.Value);
			}
			
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// BindData gets the users from the Database and binds them to the DataGrid
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
				
				gvUsersToEnroll.PageSize = PageSize;
				
				RoleController objCtlRole = new RoleController();
				RoleInfo objRole = objCtlRole.GetRoleByName(PortalId, PortalSettings.RegisteredRoleName);
				string roleName = "";
				string regRoleName = "";
				if (!ReferenceEquals(objRole, null))
				{
					roleName = objRole.RoleName;
					regRoleName = roleName;
				}
				DropDownList ddEnrollRoles = (DropDownList) (Parent.FindControl("ddEnrollRoles"));
				if (ddEnrollRoles.SelectedValue != "-1")
				{
					roleName = ddEnrollRoles.SelectedItem.Text;
				}
				
				dropdownFilterItem.Items.Clear();
				dropdownFilterItem.Items.Add(new ListItem(Localization.GetString("dropdownFilterItem00.Text", LocalResourceFile), "0"));
				dropdownFilterItem.Items.Add(new ListItem(Localization.GetString("dropdownFilterItem02.Text", LocalResourceFile), "2"));
				if (roleName == regRoleName)
				{
					dropdownFilterItem.Items.Add(new ListItem(Localization.GetString("dropdownFilterItem01.Text", LocalResourceFile), "1"));
				}
				
				ArrayList tmpUsers = default(ArrayList);
				if (roleName != regRoleName || searchField != "1")
				{
					tmpUsers = objCtlRole.GetUsersByRoleName(PortalId, roleName);
				}
				else
				{
					tmpUsers = objCtlRole.GetUsersByRoleName(PortalId, searchText);
				}
				
				EventSignupsController objCtlEventSignups = new EventSignupsController();
				ArrayList lstSignups = objCtlEventSignups.EventsSignupsGetEvent(ItemID, ModuleId);
				
				Users = new ArrayList();
				if (searchText != "None")
				{
					foreach (UserInfo objUser in tmpUsers)
					{
						switch (searchField)
						{
							case "0": //username
								if (objUser.Username.Substring(0, searchText.Length).ToLower() == searchText.ToLower())
								{
									UserAdd(objUser, lstSignups);
								}
								break;
							case "1": //Groupname
								UserAdd(objUser, lstSignups);
								break;
							case "2": //Lastname
								if (objUser.LastName.Substring(0, searchText.Length).ToLower() == searchText.ToLower())
								{
									UserAdd(objUser, lstSignups);
								}
								break;
							default:
								UserAdd(objUser, lstSignups);
								break;
						}
					}
				}
				if (Users.Count > 0)
				{
					gvUsersToEnroll.Visible = true;
					cmdSelectedAddUser.Visible = true;
				}
				else
				{
					gvUsersToEnroll.Visible = false;
					cmdSelectedAddUser.Visible = false;
				}
				gvUsersToEnroll.DataSource = Users;
				gvUsersToEnroll.DataBind();
			}
			
			private void UserAdd(UserInfo inUser, ArrayList lstSignups)
			{
				bool blAdd = true;
				foreach (EventSignupsInfo objEventSignup in lstSignups)
				{
					if (inUser.UserID == objEventSignup.UserID)
					{
						blAdd = false;
					}
				}
				if (blAdd)
				{
					Users.Add(inUser);
				}
				
			}
			
			protected void gvUsersToEnroll_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
			{
				gvUsersToEnroll.PageIndex = e.NewPageIndex;
				BindData(txtFilterUsers.Text, dropdownFilterItem.Value);
			}
			
			protected void cmdSelectedAddUser_Click(object sender, EventArgs e)
			{
				GridViewRow row = default(GridViewRow);
				ArrayList arrUsers = new ArrayList();
				try
				{
					foreach (GridViewRow tempLoopVar_row in gvUsersToEnroll.Rows)
					{
						row = tempLoopVar_row;
						if (((CheckBox) (row.FindControl("chkSelectUser"))).Checked)
						{
							arrUsers.Add(System.Convert.ToInt32(gvUsersToEnroll.DataKeys[row.RowIndex].Value));
						}
					}
					if (AddSelectedUsersEvent != null)
						AddSelectedUsersEvent(this, new EventArgs(), arrUsers);
				}
				catch (Exception)
				{
					
				}
				
			}
			
			protected void cmdRefreshList_Click(object sender, EventArgs e)
			{
				gvUsersToEnroll.PageIndex = 0;
				BindData(txtFilterUsers.Text, dropdownFilterItem.Value);
				if (dropdownFilterItem.Value == "1")
				{
					lblStartswith.Attributes.Add("style", "display: none");
				}
				else
				{
					lblStartswith.Attributes.Remove("style");
				}
				
			}
		}
	}

