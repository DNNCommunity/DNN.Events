using DotNetNuke.Services.Exceptions;
using System.Diagnostics;
using DotNetNuke.Framework;
using Microsoft.VisualBasic;
using System.Web.UI.WebControls;
using System.Collections;
using DotNetNuke.Services.Localization;
using System;

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
		
		public partial class EventMyEnrollments : EventBase
		{
			
#region  Web Form Designer Generated Code
			
			//This call is required by the Web Form Designer.
			[DebuggerStepThrough()]private void InitializeComponent()
			{
				
			}
			
			private void Page_Init(System.Object sender, EventArgs e)
			{
				//CODEGEN: This method call is required by the Web Form Designer
				//Do not modify it using the code editor.
				InitializeComponent();
			}
			
#endregion
			
#region Private Area
#endregion
			
#region Event Handlers
			private void Page_Load(System.Object sender, EventArgs e)
			{
				try
				{
					//EVT-4499 if not login, redirect user to login page
					if (!Request.IsAuthenticated)
					{
						RedirectToLogin();
					}
					
					LocalizeAll();
					
					// Setup Icon Bar for use
					SetUpIconBar(EventIcons, EventIcons2);
					
					lnkSelectedDelete.Attributes.Add("onclick", "javascript:return confirm('" + Localization.GetString("ConfirmDeleteSelected", LocalResourceFile) + "');");
					
					if (Page.IsPostBack == false)
					{
						BindData();
					}
					
				}
				catch (Exception exc) //Module failed to load
				{
					Exceptions.ProcessModuleLoadException(this, exc);
				}
			}
#endregion
			
#region Helper Methods
			private void BindData()
			{
				try
				{
					DateTime moduleStartDate = DateAndTime.DateAdd(DateInterval.Day, System.Convert.ToDouble(- Settings.EnrolListDaysBefore), ModuleNow());
					DateTime moduleEndDate = DateAndTime.DateAdd(DateInterval.Day, Settings.EnrolListDaysAfter, ModuleNow());
					DateTime displayStartDate = DateAndTime.DateAdd(DateInterval.Day, System.Convert.ToDouble(- Settings.EnrolListDaysBefore), DisplayNow());
					DateTime displayEndDate = DateAndTime.DateAdd(DateInterval.Day, Settings.EnrolListDaysAfter, DisplayNow());
					
					//Default sort from settings
					SortDirection sortDirection = Settings.EnrolListSortDirection;
					EventSignupsInfo.SortFilter sortExpression = GetSignupsSortExpression("EventTimeBegin");
					
					ArrayList inCategoryIDs = new ArrayList();
					inCategoryIDs.Add("-1");
					EventInfoHelper objEventInfoHelper = new EventInfoHelper(ModuleId, TabId, PortalId, Settings);
					string categoryIDs = objEventInfoHelper.CreateCategoryFilter(inCategoryIDs);
					
					ArrayList eventSignups = default(ArrayList);
					EventSignupsController objCtlEventSignups = new EventSignupsController();
					
					eventSignups = objCtlEventSignups.EventsSignupsMyEnrollments(ModuleId, UserId, GetUrlGroupId(), categoryIDs, moduleStartDate, moduleEndDate);
					
					EventTimeZoneUtilities objEventTimeZoneUtilities = new EventTimeZoneUtilities();
					ArrayList displayEventSignups = new ArrayList();
					foreach (EventSignupsInfo eventSignup in eventSignups)
					{
						string displayTimeZoneId = GetDisplayTimeZoneId();
						eventSignup.EventTimeBegin = objEventTimeZoneUtilities.ConvertToDisplayTimeZone(eventSignup.EventTimeBegin, eventSignup.EventTimeZoneId, PortalId, displayTimeZoneId).EventDate;
						eventSignup.EventTimeEnd = objEventTimeZoneUtilities.ConvertToDisplayTimeZone(eventSignup.EventTimeEnd, eventSignup.EventTimeZoneId, PortalId, displayTimeZoneId).EventDate;
						if (eventSignup.EventTimeBegin > displayEndDate || eventSignup.EventTimeEnd < displayStartDate)
						{
							continue;
						}
						displayEventSignups.Add(eventSignup);
					}
					
					EventSignupsInfo.SortExpression = sortExpression;
					EventSignupsInfo.SortDirection = sortDirection;
					displayEventSignups.Sort();
					
					//Get data for selected date and fill grid
					grdEnrollment.DataSource = displayEventSignups;
					grdEnrollment.DataBind();
					if (eventSignups.Count < 1)
					{
						divMessage.Visible = true;
						grdEnrollment.Visible = false;
						//"No Events/Enrollments found..."
						lblMessage.Text = Localization.GetString("MsgNoMyEventsOrEnrollment", LocalResourceFile);
					}
					else
					{
						for (int i = 0; i <= eventSignups.Count - 1; i++)
						{
							decimal decTotal = (decimal) (System.Convert.ToDecimal(grdEnrollment.Items[i].Cells[7].Text) / System.Convert.ToDecimal(grdEnrollment.Items[i].Cells[8].Text));
							DateTime dtStartTime = System.Convert.ToDateTime(grdEnrollment.Items[i].Cells[1].Text);
							// ReSharper disable LocalizableElement
							((Label) (grdEnrollment.Items[i].FindControl("lblAmount"))).Text = string.Format("{0:F2}", decTotal) + " " + PortalSettings.Currency;
							((Label) (grdEnrollment.Items[i].FindControl("lblTotal"))).Text = string.Format("{0:F2}", System.Convert.ToDecimal(grdEnrollment.Items[i].Cells[7].Text)) + " " + PortalSettings.Currency;
							// ReSharper restore LocalizableElement
							if (decTotal > 0 || dtStartTime < ModuleNow().AddDays(Settings.Enrolcanceldays))
							{
								((CheckBox) (grdEnrollment.Items[i].FindControl("chkSelect"))).Enabled = false;
							}
						}
					}
				}
				catch (Exception exc)
				{
					Exceptions.ProcessModuleLoadException(this, exc);
				}
			}
			
			private void LocalizeAll()
			{
				grdEnrollment.Columns[0].HeaderText = Localization.GetString("plSelect", LocalResourceFile);
				grdEnrollment.Columns[1].HeaderText = Localization.GetString("plDate", LocalResourceFile);
				grdEnrollment.Columns[2].HeaderText = Localization.GetString("plTime", LocalResourceFile);
				grdEnrollment.Columns[3].HeaderText = Localization.GetString("plEvent", LocalResourceFile);
				grdEnrollment.Columns[4].HeaderText = Localization.GetString("plApproved", LocalResourceFile);
				grdEnrollment.Columns[6].HeaderText = Localization.GetString("plAmount", LocalResourceFile);
				grdEnrollment.Columns[8].HeaderText = Localization.GetString("plNoEnrolees", LocalResourceFile);
				grdEnrollment.Columns[9].HeaderText = Localization.GetString("plTotal", LocalResourceFile);
				lnkSelectedDelete.ToolTip = string.Format(Localization.GetString("CancelEnrolments", LocalResourceFile), Settings.Enrolcanceldays);
			}
#endregion
			
#region Grid and Other Events
			
			public void grdEnrollment_ItemCommand(System.Object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
			{
				try
				{
					switch (e.CommandName)
					{
						case "Select":
							EventSignupsInfo objEnroll = default(EventSignupsInfo);
							EventSignupsController objCtlEventSignups = new EventSignupsController();
							objEnroll = objCtlEventSignups.EventsSignupsGet(System.Convert.ToInt32(grdEnrollment.DataKeys[e.Item.ItemIndex]), ModuleId, false);
							int iItemID = objEnroll.EventID;
							
							EventInfoHelper objEventInfoHelper = new EventInfoHelper(ModuleId, TabId, PortalId, Settings);
							Response.Redirect(objEventInfoHelper.GetDetailPageRealURL(iItemID, GetUrlGroupId(), GetUrlUserId()));
							break;
					}
				}
				catch (Exception exc) //Module failed to load
				{
					Exceptions.ProcessModuleLoadException(this, exc);
				}
				BindData();
			}
			
			protected void lnkSelectedDelete_Click(object sender, EventArgs e)
			{
				DataGridItem item = default(DataGridItem);
				EventSignupsInfo objEnroll = default(EventSignupsInfo);
				EventSignupsController objCtlEventSignups = new EventSignupsController();
				EventInfo objEvent = new EventInfo();
				EventController objCtlEvent = new EventController();
				int eventID = 0;
				
				foreach (DataGridItem tempLoopVar_item in grdEnrollment.Items)
				{
					item = tempLoopVar_item;
					if (((CheckBox) (item.FindControl("chkSelect"))).Checked)
					{
						objEnroll = objCtlEventSignups.EventsSignupsGet(System.Convert.ToInt32(grdEnrollment.DataKeys[item.ItemIndex]), ModuleId, false);
						if (eventID != objEnroll.EventID)
						{
							objEvent = objCtlEvent.EventsGet(objEnroll.EventID, ModuleId);
						}
						eventID = objEnroll.EventID;
						
						// Delete Selected Enrollee
						DeleteEnrollment(System.Convert.ToInt32(grdEnrollment.DataKeys[item.ItemIndex]), objEvent.ModuleID, objEvent.EventID);
						
						// Mail users
						if (Settings.SendEnrollMessageDeleted)
						{
							EventEmailInfo objEventEmailInfo = new EventEmailInfo();
							EventEmails objEventEmail = new EventEmails(PortalId, ModuleId, LocalResourceFile, ((PageBase) Page).PageCulture.Name);
							objEventEmailInfo.TxtEmailSubject = Settings.Templates.txtEnrollMessageSubject;
							objEventEmailInfo.TxtEmailBody = Settings.Templates.txtEnrollMessageDeleted;
							objEventEmailInfo.TxtEmailFrom = Settings.StandardEmail;
							objEventEmailInfo.UserEmails.Add(PortalSettings.UserInfo.Email);
							objEventEmailInfo.UserLocales.Add(PortalSettings.UserInfo.Profile.PreferredLocale);
							objEventEmailInfo.UserTimeZoneIds.Add(PortalSettings.UserInfo.Profile.PreferredTimeZone.Id);
							objEventEmailInfo.UserIDs.Add(objEvent.OwnerID);
							objEventEmail.SendEmails(objEventEmailInfo, objEvent, objEnroll);
						}
						
					}
				}
				
				BindData();
				
			}
			
			protected void returnButton_Click(System.Object sender, EventArgs e)
			{
				string[] cntrl = Settings.DefaultView.Split('.');
				int socialGroupId = GetUrlGroupId();
				if (socialGroupId > 0)
				{
					Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(TabId, "", "mctl=" + cntrl[0], "ModuleID=" + ModuleId.ToString(), "groupid=" + socialGroupId.ToString()));
				}
				else
				{
					Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(TabId, "", "mctl=" + cntrl[0], "ModuleID=" + ModuleId.ToString()));
				}
			}
			
#endregion
			
		}
	}
	
