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
    using System.Reflection;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Services.Localization;

    #region EventTemplates

    [Serializable]
    public class EventTemplates
    {
        #region  Constructors

        public EventTemplates(int moduleID, Hashtable allsettings, string localResourceFile)
        {
            var t = GetType();
            var p = default(PropertyInfo);
            foreach (var tempLoopVar_p in t.GetProperties())
            {
                p = tempLoopVar_p;
                var pn = p.Name;
                string pv = null;
                if (!ReferenceEquals(allsettings[pn], null))
                {
                    pv = Convert.ToString(allsettings[pn]);
                    if (pv.Length > 1900)
                    {
                        if (!ReferenceEquals(allsettings[pn + "2"], null))
                        {
                            pv = Convert.ToString(pv + Convert.ToString(allsettings[pn + "2"]));
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(localResourceFile))
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

        #region  Private Members

        // ReSharper disable InconsistentNaming
        private string _EventDetailsTemplate;

        private string _NewEventTemplate;
        private string _txtToolTipTemplateTitleNT;
        private string _txtToolTipTemplateBodyNT;
        private string _txtToolTipTemplateTitle;
        private string _txtToolTipTemplateBody;
        private string _moderateemailsubject;
        private string _moderateemailmessage;
        private string _txtEmailSubject;
        private string _txtEmailMessage;
        private string _txtEnrollMessageSubject;
        private string _txtEnrollMessageApproved;
        private string _txtEnrollMessageWaiting;
        private string _txtEnrollMessageDenied;
        private string _txtEnrollMessageAdded;
        private string _txtEnrollMessageDeleted;
        private string _txtEnrollMessagePaying;
        private string _txtEnrollMessagePending;
        private string _txtEnrollMessagePaid;
        private string _txtEnrollMessageIncorrect;
        private string _txtEnrollMessageCancelled;
        private string _txtEditViewEmailSubject;
        private string _txtEditViewEmailBody;
        private string _txtSubject;
        private string _txtMessage;
        private string _txtNewEventEmailSubject;
        private string _txtNewEventEmailMessage;
        private string _txtListEventTimeBegin;
        private string _txtListEventTimeEnd;
        private string _txtListLocation;
        private string _txtListEventDescription;
        private string _txtListRptHeader;
        private string _txtListRptBody;
        private string _txtListRptFooter;
        private string _txtDayEventTimeBegin;
        private string _txtDayEventTimeEnd;
        private string _txtDayLocation;
        private string _txtDayEventDescription;
        private string _txtWeekEventText;
        private string _txtWeekTitleDate;
        private string _txtMonthEventText;
        private string _txtMonthDayEventCount;
        private string _txtRSSTitle;
        private string _txtRSSDescription;
        private string _txtSEOPageTitle;
        private string _txtSEOPageDescription;
        private string _EventiCalSubject;

        private string _EventiCalBody;
        // ReSharper restore InconsistentNaming

        #endregion

        #region  Public Methods

        public string GetTemplate(string templateName)
        {
            var t = GetType();
            var p = default(PropertyInfo);
            foreach (var tempLoopVar_p in t.GetProperties())
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
            var t = GetType();
            var p = default(PropertyInfo);
            foreach (var tempLoopVar_p in t.GetProperties())
            {
                p = tempLoopVar_p;
                if (p.Name == templateName)
                {
                    p.SetValue(this, templateValue, null);
                    var objModules = new ModuleController();

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
            var cacheKey = "EventsSettings" + moduleID;
            DataCache.ClearCache(cacheKey);
        }

        public void ResetTemplate(int moduleID, string templateName, string localResourceFile)
        {
            var templateValue = Localization.GetString(templateName, localResourceFile);
            SaveTemplate(moduleID, templateName, templateValue);
        }

        #endregion

        #region  Private Methods

        #endregion

        #region  Properties

        // ReSharper disable InconsistentNaming
        public string EventDetailsTemplate
        {
            get { return _EventDetailsTemplate; }
            set { _EventDetailsTemplate = value; }
        }

        public string NewEventTemplate
        {
            get { return _NewEventTemplate; }
            set { _NewEventTemplate = value; }
        }

        public string txtTooltipTemplateTitleNT
        {
            get { return _txtToolTipTemplateTitleNT; }
            set { _txtToolTipTemplateTitleNT = value; }
        }

        public string txtTooltipTemplateBodyNT
        {
            get { return _txtToolTipTemplateBodyNT; }
            set { _txtToolTipTemplateBodyNT = value; }
        }

        public string txtTooltipTemplateTitle
        {
            get { return _txtToolTipTemplateTitle; }
            set { _txtToolTipTemplateTitle = value; }
        }

        public string txtTooltipTemplateBody
        {
            get { return _txtToolTipTemplateBody; }
            set { _txtToolTipTemplateBody = value; }
        }

        public string moderateemailsubject
        {
            get { return _moderateemailsubject; }
            set { _moderateemailsubject = value; }
        }

        public string moderateemailmessage
        {
            get { return _moderateemailmessage; }
            set { _moderateemailmessage = value; }
        }

        public string txtEmailSubject
        {
            get { return _txtEmailSubject; }
            set { _txtEmailSubject = value; }
        }

        public string txtEmailMessage
        {
            get { return _txtEmailMessage; }
            set { _txtEmailMessage = value; }
        }

        public string txtEnrollMessageSubject
        {
            get { return _txtEnrollMessageSubject; }
            set { _txtEnrollMessageSubject = value; }
        }

        public string txtEnrollMessageApproved
        {
            get { return _txtEnrollMessageApproved; }
            set { _txtEnrollMessageApproved = value; }
        }

        public string txtEnrollMessageWaiting
        {
            get { return _txtEnrollMessageWaiting; }
            set { _txtEnrollMessageWaiting = value; }
        }

        public string txtEnrollMessageDenied
        {
            get { return _txtEnrollMessageDenied; }
            set { _txtEnrollMessageDenied = value; }
        }

        public string txtEnrollMessageAdded
        {
            get { return _txtEnrollMessageAdded; }
            set { _txtEnrollMessageAdded = value; }
        }

        public string txtEnrollMessageDeleted
        {
            get { return _txtEnrollMessageDeleted; }
            set { _txtEnrollMessageDeleted = value; }
        }

        public string txtEnrollMessagePaying
        {
            get { return _txtEnrollMessagePaying; }
            set { _txtEnrollMessagePaying = value; }
        }

        public string txtEnrollMessagePending
        {
            get { return _txtEnrollMessagePending; }
            set { _txtEnrollMessagePending = value; }
        }

        public string txtEnrollMessagePaid
        {
            get { return _txtEnrollMessagePaid; }
            set { _txtEnrollMessagePaid = value; }
        }

        public string txtEnrollMessageIncorrect
        {
            get { return _txtEnrollMessageIncorrect; }
            set { _txtEnrollMessageIncorrect = value; }
        }

        public string txtEnrollMessageCancelled
        {
            get { return _txtEnrollMessageCancelled; }
            set { _txtEnrollMessageCancelled = value; }
        }

        public string txtEditViewEmailSubject
        {
            get { return _txtEditViewEmailSubject; }
            set { _txtEditViewEmailSubject = value; }
        }

        public string txtEditViewEmailBody
        {
            get { return _txtEditViewEmailBody; }
            set { _txtEditViewEmailBody = value; }
        }

        public string txtSubject
        {
            get { return _txtSubject; }
            set { _txtSubject = value; }
        }

        public string txtMessage
        {
            get { return _txtMessage; }
            set { _txtMessage = value; }
        }

        public string txtNewEventEmailSubject
        {
            get { return _txtNewEventEmailSubject; }
            set { _txtNewEventEmailSubject = value; }
        }

        public string txtNewEventEmailMessage
        {
            get { return _txtNewEventEmailMessage; }
            set { _txtNewEventEmailMessage = value; }
        }

        public string txtListEventTimeBegin
        {
            get { return _txtListEventTimeBegin; }
            set { _txtListEventTimeBegin = value; }
        }

        public string txtListEventTimeEnd
        {
            get { return _txtListEventTimeEnd; }
            set { _txtListEventTimeEnd = value; }
        }

        public string txtListLocation
        {
            get { return _txtListLocation; }
            set { _txtListLocation = value; }
        }

        public string txtListEventDescription
        {
            get { return _txtListEventDescription; }
            set { _txtListEventDescription = value; }
        }

        public string txtListRptHeader
        {
            get { return _txtListRptHeader; }
            set { _txtListRptHeader = value; }
        }

        public string txtListRptBody
        {
            get { return _txtListRptBody; }
            set { _txtListRptBody = value; }
        }

        public string txtListRptFooter
        {
            get { return _txtListRptFooter; }
            set { _txtListRptFooter = value; }
        }

        public string txtDayEventTimeBegin
        {
            get { return _txtDayEventTimeBegin; }
            set { _txtDayEventTimeBegin = value; }
        }

        public string txtDayEventTimeEnd
        {
            get { return _txtDayEventTimeEnd; }
            set { _txtDayEventTimeEnd = value; }
        }

        public string txtDayLocation
        {
            get { return _txtDayLocation; }
            set { _txtDayLocation = value; }
        }

        public string txtDayEventDescription
        {
            get { return _txtDayEventDescription; }
            set { _txtDayEventDescription = value; }
        }

        public string txtWeekEventText
        {
            get { return _txtWeekEventText; }
            set { _txtWeekEventText = value; }
        }

        public string txtWeekTitleDate
        {
            get { return _txtWeekTitleDate; }
            set { _txtWeekTitleDate = value; }
        }

        public string txtMonthEventText
        {
            get { return _txtMonthEventText; }
            set { _txtMonthEventText = value; }
        }

        public string txtMonthDayEventCount
        {
            get { return _txtMonthDayEventCount; }
            set { _txtMonthDayEventCount = value; }
        }

        public string txtRSSTitle
        {
            get { return _txtRSSTitle; }
            set { _txtRSSTitle = value; }
        }

        public string txtRSSDescription
        {
            get { return _txtRSSDescription; }
            set { _txtRSSDescription = value; }
        }

        public string txtSEOPageTitle
        {
            get { return _txtSEOPageTitle; }
            set { _txtSEOPageTitle = value; }
        }

        public string txtSEOPageDescription
        {
            get { return _txtSEOPageDescription; }
            set { _txtSEOPageDescription = value; }
        }

        public string EventiCalSubject
        {
            get { return _EventiCalSubject; }
            set { _EventiCalSubject = value; }
        }

        public string EventiCalBody
        {
            get { return _EventiCalBody; }
            set { _EventiCalBody = value; }
        }
        // ReSharper restore InconsistentNaming

        #endregion
    }

    #endregion
}