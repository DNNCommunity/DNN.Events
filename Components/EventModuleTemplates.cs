using System.Collections;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Localization;
using System;
using DotNetNuke.Entities.Modules;


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
#region EventTemplates
		
		[Serializable()]
			public class EventTemplates
			{
			
#region  Private Members
			// ReSharper disable InconsistentNaming
			private string _EventDetailsTemplate = null;
			private string _NewEventTemplate = null;
			private string _txtToolTipTemplateTitleNT = null;
			private string _txtToolTipTemplateBodyNT = null;
			private string _txtToolTipTemplateTitle = null;
			private string _txtToolTipTemplateBody = null;
			private string _moderateemailsubject = null;
			private string _moderateemailmessage = null;
			private string _txtEmailSubject = null;
			private string _txtEmailMessage = null;
			private string _txtEnrollMessageSubject = null;
			private string _txtEnrollMessageApproved = null;
			private string _txtEnrollMessageWaiting = null;
			private string _txtEnrollMessageDenied = null;
			private string _txtEnrollMessageAdded = null;
			private string _txtEnrollMessageDeleted = null;
			private string _txtEnrollMessagePaying = null;
			private string _txtEnrollMessagePending = null;
			private string _txtEnrollMessagePaid = null;
			private string _txtEnrollMessageIncorrect = null;
			private string _txtEnrollMessageCancelled = null;
			private string _txtEditViewEmailSubject = null;
			private string _txtEditViewEmailBody = null;
			private string _txtSubject = null;
			private string _txtMessage = null;
			private string _txtNewEventEmailSubject = null;
			private string _txtNewEventEmailMessage = null;
			private string _txtListEventTimeBegin = null;
			private string _txtListEventTimeEnd = null;
			private string _txtListLocation = null;
			private string _txtListEventDescription = null;
			private string _txtListRptHeader = null;
			private string _txtListRptBody = null;
			private string _txtListRptFooter = null;
			private string _txtDayEventTimeBegin = null;
			private string _txtDayEventTimeEnd = null;
			private string _txtDayLocation = null;
			private string _txtDayEventDescription = null;
			private string _txtWeekEventText = null;
			private string _txtWeekTitleDate = null;
			private string _txtMonthEventText = null;
			private string _txtMonthDayEventCount = null;
			private string _txtRSSTitle = null;
			private string _txtRSSDescription = null;
			private string _txtSEOPageTitle = null;
			private string _txtSEOPageDescription = null;
			private string _EventiCalSubject = null;
			private string _EventiCalBody = null;
			// ReSharper restore InconsistentNaming
			
#endregion
			
#region  Constructors
			
			public EventTemplates(int moduleID, Hashtable allsettings, string localResourceFile)
			{
				
				Type t = this.GetType();
				System.Reflection.PropertyInfo p = default(System.Reflection.PropertyInfo);
				foreach (System.Reflection.PropertyInfo tempLoopVar_p in t.GetProperties())
				{
					p = tempLoopVar_p;
					string pn = p.Name;
					string pv = null;
					if (!ReferenceEquals(allsettings[pn], null))
					{
						pv = System.Convert.ToString(allsettings[pn]);
						if (pv.Length > 1900)
						{
							if (!ReferenceEquals(allsettings[pn + "2"], null))
							{
								pv = System.Convert.ToString(pv + System.Convert.ToString(allsettings[pn + "2"]));
							}
						}
					}
					else
					{
						if (!ReferenceEquals(localResourceFile, null))
						{
							pv = Localization.GetString(pn, localResourceFile);
							if (moduleID > 0)
							{
								SaveTemplate(moduleID, pn, pv);
							}
						}
					}
					p.SetValue(this, pv, null);
				}
				
			}
			
#endregion
			
#region  Public Methods
			public string GetTemplate(string templateName)
			{
				Type t = this.GetType();
				System.Reflection.PropertyInfo p = default(System.Reflection.PropertyInfo);
				foreach (System.Reflection.PropertyInfo tempLoopVar_p in t.GetProperties())
				{
					p = tempLoopVar_p;
					if (p.Name == templateName)
					{
						return p.GetValue(this, null).ToString();
					}
				}
				return "";
			}
			
			public void SaveTemplate(int moduleID, string templateName, string templateValue)
			{
				Type t = this.GetType();
				System.Reflection.PropertyInfo p = default(System.Reflection.PropertyInfo);
				foreach (System.Reflection.PropertyInfo tempLoopVar_p in t.GetProperties())
				{
					p = tempLoopVar_p;
					if (p.Name == templateName)
					{
						p.SetValue(this, templateValue, null);
						ModuleController objModules = new ModuleController();
						
						if (templateValue.Length > 2000)
						{
							objModules.UpdateModuleSetting(moduleID, templateName, templateValue.Substring(0, 2000));
							objModules.UpdateModuleSetting(moduleID, templateName + "2", templateValue.Substring(2000));
						}
						else
						{
							objModules.UpdateModuleSetting(moduleID, templateName, templateValue.Trim());
							objModules.DeleteModuleSetting(moduleID, templateName + "2");
						}
					}
				}
				string cacheKey = "EventsSettings" + moduleID.ToString();
				DataCache.ClearCache(cacheKey);
			}
			
			public void ResetTemplate(int moduleID, string templateName, string localResourceFile)
			{
				string templateValue = Localization.GetString(templateName, localResourceFile);
				SaveTemplate(moduleID, templateName, templateValue);
				
			}
			
#endregion
			
#region  Private Methods
			
#endregion
			
#region  Properties
			// ReSharper disable InconsistentNaming
			public string EventDetailsTemplate
			{
				get
				{
					return _EventDetailsTemplate;
				}
				set
				{
					_EventDetailsTemplate = value;
				}
			}
			
			public string NewEventTemplate
			{
				get
				{
					return _NewEventTemplate;
				}
				set
				{
					_NewEventTemplate = value;
				}
			}
			
			public string txtTooltipTemplateTitleNT
			{
				get
				{
					return _txtToolTipTemplateTitleNT;
				}
				set
				{
					_txtToolTipTemplateTitleNT = value;
				}
			}
			
			public string txtTooltipTemplateBodyNT
			{
				get
				{
					return _txtToolTipTemplateBodyNT;
				}
				set
				{
					_txtToolTipTemplateBodyNT = value;
				}
			}
			
			public string txtTooltipTemplateTitle
			{
				get
				{
					return _txtToolTipTemplateTitle;
				}
				set
				{
					_txtToolTipTemplateTitle = value;
				}
			}
			
			public string txtTooltipTemplateBody
			{
				get
				{
					return _txtToolTipTemplateBody;
				}
				set
				{
					_txtToolTipTemplateBody = value;
				}
			}
			
			public string moderateemailsubject
			{
				get
				{
					return _moderateemailsubject;
				}
				set
				{
					_moderateemailsubject = value;
				}
			}
			
			public string moderateemailmessage
			{
				get
				{
					return _moderateemailmessage;
				}
				set
				{
					_moderateemailmessage = value;
				}
			}
			
			public string txtEmailSubject
			{
				get
				{
					return _txtEmailSubject;
				}
				set
				{
					_txtEmailSubject = value;
				}
			}
			
			public string txtEmailMessage
			{
				get
				{
					return _txtEmailMessage;
				}
				set
				{
					_txtEmailMessage = value;
				}
			}
			
			public string txtEnrollMessageSubject
			{
				get
				{
					return _txtEnrollMessageSubject;
				}
				set
				{
					_txtEnrollMessageSubject = value;
				}
			}
			
			public string txtEnrollMessageApproved
			{
				get
				{
					return _txtEnrollMessageApproved;
				}
				set
				{
					_txtEnrollMessageApproved = value;
				}
			}
			
			public string txtEnrollMessageWaiting
			{
				get
				{
					return _txtEnrollMessageWaiting;
				}
				set
				{
					_txtEnrollMessageWaiting = value;
				}
			}
			
			public string txtEnrollMessageDenied
			{
				get
				{
					return _txtEnrollMessageDenied;
				}
				set
				{
					_txtEnrollMessageDenied = value;
				}
			}
			
			public string txtEnrollMessageAdded
			{
				get
				{
					return _txtEnrollMessageAdded;
				}
				set
				{
					_txtEnrollMessageAdded = value;
				}
			}
			
			public string txtEnrollMessageDeleted
			{
				get
				{
					return _txtEnrollMessageDeleted;
				}
				set
				{
					_txtEnrollMessageDeleted = value;
				}
			}
			
			public string txtEnrollMessagePaying
			{
				get
				{
					return _txtEnrollMessagePaying;
				}
				set
				{
					_txtEnrollMessagePaying = value;
				}
			}
			
			public string txtEnrollMessagePending
			{
				get
				{
					return _txtEnrollMessagePending;
				}
				set
				{
					_txtEnrollMessagePending = value;
				}
			}
			
			public string txtEnrollMessagePaid
			{
				get
				{
					return _txtEnrollMessagePaid;
				}
				set
				{
					_txtEnrollMessagePaid = value;
				}
			}
			
			public string txtEnrollMessageIncorrect
			{
				get
				{
					return _txtEnrollMessageIncorrect;
				}
				set
				{
					_txtEnrollMessageIncorrect = value;
				}
			}
			
			public string txtEnrollMessageCancelled
			{
				get
				{
					return _txtEnrollMessageCancelled;
				}
				set
				{
					_txtEnrollMessageCancelled = value;
				}
			}
			
			public string txtEditViewEmailSubject
			{
				get
				{
					return _txtEditViewEmailSubject;
				}
				set
				{
					_txtEditViewEmailSubject = value;
				}
			}
			
			public string txtEditViewEmailBody
			{
				get
				{
					return _txtEditViewEmailBody;
				}
				set
				{
					_txtEditViewEmailBody = value;
				}
			}
			
			public string txtSubject
			{
				get
				{
					return _txtSubject;
				}
				set
				{
					_txtSubject = value;
				}
			}
			
			public string txtMessage
			{
				get
				{
					return _txtMessage;
				}
				set
				{
					_txtMessage = value;
				}
			}
			
			public string txtNewEventEmailSubject
			{
				get
				{
					return _txtNewEventEmailSubject;
				}
				set
				{
					_txtNewEventEmailSubject = value;
				}
			}
			
			public string txtNewEventEmailMessage
			{
				get
				{
					return _txtNewEventEmailMessage;
				}
				set
				{
					_txtNewEventEmailMessage = value;
				}
			}
			
			public string txtListEventTimeBegin
			{
				get
				{
					return _txtListEventTimeBegin;
				}
				set
				{
					_txtListEventTimeBegin = value;
				}
			}
			
			public string txtListEventTimeEnd
			{
				get
				{
					return _txtListEventTimeEnd;
				}
				set
				{
					_txtListEventTimeEnd = value;
				}
			}
			
			public string txtListLocation
			{
				get
				{
					return _txtListLocation;
				}
				set
				{
					_txtListLocation = value;
				}
			}
			
			public string txtListEventDescription
			{
				get
				{
					return _txtListEventDescription;
				}
				set
				{
					_txtListEventDescription = value;
				}
			}
			
			public string txtListRptHeader
			{
				get
				{
					return _txtListRptHeader;
				}
				set
				{
					_txtListRptHeader = value;
				}
			}
			
			public string txtListRptBody
			{
				get
				{
					return _txtListRptBody;
				}
				set
				{
					_txtListRptBody = value;
				}
			}
			
			public string txtListRptFooter
			{
				get
				{
					return _txtListRptFooter;
				}
				set
				{
					_txtListRptFooter = value;
				}
			}
			
			public string txtDayEventTimeBegin
			{
				get
				{
					return _txtDayEventTimeBegin;
				}
				set
				{
					_txtDayEventTimeBegin = value;
				}
			}
			
			public string txtDayEventTimeEnd
			{
				get
				{
					return _txtDayEventTimeEnd;
				}
				set
				{
					_txtDayEventTimeEnd = value;
				}
			}
			
			public string txtDayLocation
			{
				get
				{
					return _txtDayLocation;
				}
				set
				{
					_txtDayLocation = value;
				}
			}
			
			public string txtDayEventDescription
			{
				get
				{
					return _txtDayEventDescription;
				}
				set
				{
					_txtDayEventDescription = value;
				}
			}
			
			public string txtWeekEventText
			{
				get
				{
					return _txtWeekEventText;
				}
				set
				{
					_txtWeekEventText = value;
				}
			}
			
			public string txtWeekTitleDate
			{
				get
				{
					return _txtWeekTitleDate;
				}
				set
				{
					_txtWeekTitleDate = value;
				}
			}
			
			public string txtMonthEventText
			{
				get
				{
					return _txtMonthEventText;
				}
				set
				{
					_txtMonthEventText = value;
				}
			}
			
			public string txtMonthDayEventCount
			{
				get
				{
					return _txtMonthDayEventCount;
				}
				set
				{
					_txtMonthDayEventCount = value;
				}
			}
			
			public string txtRSSTitle
			{
				get
				{
					return _txtRSSTitle;
				}
				set
				{
					_txtRSSTitle = value;
				}
			}
			
			public string txtRSSDescription
			{
				get
				{
					return _txtRSSDescription;
				}
				set
				{
					_txtRSSDescription = value;
				}
			}
			
			public string txtSEOPageTitle
			{
				get
				{
					return _txtSEOPageTitle;
				}
				set
				{
					_txtSEOPageTitle = value;
				}
			}
			
			public string txtSEOPageDescription
			{
				get
				{
					return _txtSEOPageDescription;
				}
				set
				{
					_txtSEOPageDescription = value;
				}
			}
			
			public string EventiCalSubject
			{
				get
				{
					return _EventiCalSubject;
				}
				set
				{
					_EventiCalSubject = value;
				}
			}
			
			public string EventiCalBody
			{
				get
				{
					return _EventiCalBody;
				}
				set
				{
					_EventiCalBody = value;
				}
			}
			// ReSharper restore InconsistentNaming
			
#endregion
			
		}
#endregion
		
	}
	
