


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


namespace Components
{
    using System;
    using System.Collections;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Services.Localization;

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
								this.SaveTemplate(moduleID, pn, pv);
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
				this.SaveTemplate(moduleID, templateName, templateValue);
				
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
					return this._EventDetailsTemplate;
				}
				set
				{
					this._EventDetailsTemplate = value;
				}
			}
			
			public string NewEventTemplate
			{
				get
				{
					return this._NewEventTemplate;
				}
				set
				{
					this._NewEventTemplate = value;
				}
			}
			
			public string txtTooltipTemplateTitleNT
			{
				get
				{
					return this._txtToolTipTemplateTitleNT;
				}
				set
				{
					this._txtToolTipTemplateTitleNT = value;
				}
			}
			
			public string txtTooltipTemplateBodyNT
			{
				get
				{
					return this._txtToolTipTemplateBodyNT;
				}
				set
				{
					this._txtToolTipTemplateBodyNT = value;
				}
			}
			
			public string txtTooltipTemplateTitle
			{
				get
				{
					return this._txtToolTipTemplateTitle;
				}
				set
				{
					this._txtToolTipTemplateTitle = value;
				}
			}
			
			public string txtTooltipTemplateBody
			{
				get
				{
					return this._txtToolTipTemplateBody;
				}
				set
				{
					this._txtToolTipTemplateBody = value;
				}
			}
			
			public string moderateemailsubject
			{
				get
				{
					return this._moderateemailsubject;
				}
				set
				{
					this._moderateemailsubject = value;
				}
			}
			
			public string moderateemailmessage
			{
				get
				{
					return this._moderateemailmessage;
				}
				set
				{
					this._moderateemailmessage = value;
				}
			}
			
			public string txtEmailSubject
			{
				get
				{
					return this._txtEmailSubject;
				}
				set
				{
					this._txtEmailSubject = value;
				}
			}
			
			public string txtEmailMessage
			{
				get
				{
					return this._txtEmailMessage;
				}
				set
				{
					this._txtEmailMessage = value;
				}
			}
			
			public string txtEnrollMessageSubject
			{
				get
				{
					return this._txtEnrollMessageSubject;
				}
				set
				{
					this._txtEnrollMessageSubject = value;
				}
			}
			
			public string txtEnrollMessageApproved
			{
				get
				{
					return this._txtEnrollMessageApproved;
				}
				set
				{
					this._txtEnrollMessageApproved = value;
				}
			}
			
			public string txtEnrollMessageWaiting
			{
				get
				{
					return this._txtEnrollMessageWaiting;
				}
				set
				{
					this._txtEnrollMessageWaiting = value;
				}
			}
			
			public string txtEnrollMessageDenied
			{
				get
				{
					return this._txtEnrollMessageDenied;
				}
				set
				{
					this._txtEnrollMessageDenied = value;
				}
			}
			
			public string txtEnrollMessageAdded
			{
				get
				{
					return this._txtEnrollMessageAdded;
				}
				set
				{
					this._txtEnrollMessageAdded = value;
				}
			}
			
			public string txtEnrollMessageDeleted
			{
				get
				{
					return this._txtEnrollMessageDeleted;
				}
				set
				{
					this._txtEnrollMessageDeleted = value;
				}
			}
			
			public string txtEnrollMessagePaying
			{
				get
				{
					return this._txtEnrollMessagePaying;
				}
				set
				{
					this._txtEnrollMessagePaying = value;
				}
			}
			
			public string txtEnrollMessagePending
			{
				get
				{
					return this._txtEnrollMessagePending;
				}
				set
				{
					this._txtEnrollMessagePending = value;
				}
			}
			
			public string txtEnrollMessagePaid
			{
				get
				{
					return this._txtEnrollMessagePaid;
				}
				set
				{
					this._txtEnrollMessagePaid = value;
				}
			}
			
			public string txtEnrollMessageIncorrect
			{
				get
				{
					return this._txtEnrollMessageIncorrect;
				}
				set
				{
					this._txtEnrollMessageIncorrect = value;
				}
			}
			
			public string txtEnrollMessageCancelled
			{
				get
				{
					return this._txtEnrollMessageCancelled;
				}
				set
				{
					this._txtEnrollMessageCancelled = value;
				}
			}
			
			public string txtEditViewEmailSubject
			{
				get
				{
					return this._txtEditViewEmailSubject;
				}
				set
				{
					this._txtEditViewEmailSubject = value;
				}
			}
			
			public string txtEditViewEmailBody
			{
				get
				{
					return this._txtEditViewEmailBody;
				}
				set
				{
					this._txtEditViewEmailBody = value;
				}
			}
			
			public string txtSubject
			{
				get
				{
					return this._txtSubject;
				}
				set
				{
					this._txtSubject = value;
				}
			}
			
			public string txtMessage
			{
				get
				{
					return this._txtMessage;
				}
				set
				{
					this._txtMessage = value;
				}
			}
			
			public string txtNewEventEmailSubject
			{
				get
				{
					return this._txtNewEventEmailSubject;
				}
				set
				{
					this._txtNewEventEmailSubject = value;
				}
			}
			
			public string txtNewEventEmailMessage
			{
				get
				{
					return this._txtNewEventEmailMessage;
				}
				set
				{
					this._txtNewEventEmailMessage = value;
				}
			}
			
			public string txtListEventTimeBegin
			{
				get
				{
					return this._txtListEventTimeBegin;
				}
				set
				{
					this._txtListEventTimeBegin = value;
				}
			}
			
			public string txtListEventTimeEnd
			{
				get
				{
					return this._txtListEventTimeEnd;
				}
				set
				{
					this._txtListEventTimeEnd = value;
				}
			}
			
			public string txtListLocation
			{
				get
				{
					return this._txtListLocation;
				}
				set
				{
					this._txtListLocation = value;
				}
			}
			
			public string txtListEventDescription
			{
				get
				{
					return this._txtListEventDescription;
				}
				set
				{
					this._txtListEventDescription = value;
				}
			}
			
			public string txtListRptHeader
			{
				get
				{
					return this._txtListRptHeader;
				}
				set
				{
					this._txtListRptHeader = value;
				}
			}
			
			public string txtListRptBody
			{
				get
				{
					return this._txtListRptBody;
				}
				set
				{
					this._txtListRptBody = value;
				}
			}
			
			public string txtListRptFooter
			{
				get
				{
					return this._txtListRptFooter;
				}
				set
				{
					this._txtListRptFooter = value;
				}
			}
			
			public string txtDayEventTimeBegin
			{
				get
				{
					return this._txtDayEventTimeBegin;
				}
				set
				{
					this._txtDayEventTimeBegin = value;
				}
			}
			
			public string txtDayEventTimeEnd
			{
				get
				{
					return this._txtDayEventTimeEnd;
				}
				set
				{
					this._txtDayEventTimeEnd = value;
				}
			}
			
			public string txtDayLocation
			{
				get
				{
					return this._txtDayLocation;
				}
				set
				{
					this._txtDayLocation = value;
				}
			}
			
			public string txtDayEventDescription
			{
				get
				{
					return this._txtDayEventDescription;
				}
				set
				{
					this._txtDayEventDescription = value;
				}
			}
			
			public string txtWeekEventText
			{
				get
				{
					return this._txtWeekEventText;
				}
				set
				{
					this._txtWeekEventText = value;
				}
			}
			
			public string txtWeekTitleDate
			{
				get
				{
					return this._txtWeekTitleDate;
				}
				set
				{
					this._txtWeekTitleDate = value;
				}
			}
			
			public string txtMonthEventText
			{
				get
				{
					return this._txtMonthEventText;
				}
				set
				{
					this._txtMonthEventText = value;
				}
			}
			
			public string txtMonthDayEventCount
			{
				get
				{
					return this._txtMonthDayEventCount;
				}
				set
				{
					this._txtMonthDayEventCount = value;
				}
			}
			
			public string txtRSSTitle
			{
				get
				{
					return this._txtRSSTitle;
				}
				set
				{
					this._txtRSSTitle = value;
				}
			}
			
			public string txtRSSDescription
			{
				get
				{
					return this._txtRSSDescription;
				}
				set
				{
					this._txtRSSDescription = value;
				}
			}
			
			public string txtSEOPageTitle
			{
				get
				{
					return this._txtSEOPageTitle;
				}
				set
				{
					this._txtSEOPageTitle = value;
				}
			}
			
			public string txtSEOPageDescription
			{
				get
				{
					return this._txtSEOPageDescription;
				}
				set
				{
					this._txtSEOPageDescription = value;
				}
			}
			
			public string EventiCalSubject
			{
				get
				{
					return this._EventiCalSubject;
				}
				set
				{
					this._EventiCalSubject = value;
				}
			}
			
			public string EventiCalBody
			{
				get
				{
					return this._EventiCalBody;
				}
				set
				{
					this._EventiCalBody = value;
				}
			}
			// ReSharper restore InconsistentNaming
			
#endregion
			
		}
#endregion
		
	}
	
