using System.Drawing;
using Microsoft.VisualBasic;
using System.Web.UI.WebControls;
using System.Collections;
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
		
#region EventInfo Class
		
		public class EventInfo : IComparable
		{
			/// <summary>
			/// Priority setting for events, can be used to display appropriate icons in an event
			/// </summary>
			public enum Priority
			{
				/// <summary>
				/// High priority
				/// </summary>
				High = 1,
				/// <summary>
				/// Medium priority
				/// </summary>
				Medium = 2,
				/// <summary>
				/// Low priority
				/// </summary>
				Low = 3
			}
			
#region Private Members
			private int _portalID;
			private int _eventID;
			private int _recurMasterID;
			private int _moduleID;
			private DateTime _eventDateEnd;
			private DateTime _eventTimeBegin;
			private int _duration;
			private string _eventName = string.Empty;
			private string _eventDesc = string.Empty;
			private int _importance;
			private DateTime _createdDate;
			private string _createdBy = string.Empty;
			private int _createdByID;
			// ReSharper disable UnassignedField.Local
			private int _every;
			private string _period = string.Empty;
			private string _repeatType = string.Empty;
			// ReSharper restore UnassignedField.Local
			private string _notify = string.Empty;
			private bool _approved;
			private int _maxEnrollment;
			private bool _signups;
			private int _enrolled;
			private int _noOfRecurrences;
			private DateTime _lastRecurrence;
			private int _enrollRoleID = 0;
			private string _enrollType = string.Empty;
			private decimal _enrollFee;
			private string _payPalAccount = string.Empty;
			private bool _cancelled;
			private bool _detailPage;
			private bool _detailNewWin;
			private string _detailURL = string.Empty;
			private string _imageURL = string.Empty;
			private string _imageType = string.Empty;
			private int _imageWidth;
			private int _imageHeight;
			private int _location;
			private string _locationName = string.Empty;
			private string _mapURL = string.Empty;
			private int _category;
			private string _categoryName = string.Empty;
			private string _color = string.Empty;
			private string _fontColor = string.Empty;
			private bool _imageDisplay;
			private bool _sendReminder;
			private int _reminderTime;
			private string _reminderTimeMeasurement = string.Empty;
			private string _reminder = string.Empty;
			private string _reminderFrom = string.Empty;
			private bool _searchSubmitted;
			private string _moduleTitle = string.Empty;
			private string _customField1 = string.Empty;
			private string _customField2 = string.Empty;
			private bool _enrollListView;
			private bool _displayEndDate;
			private bool _allDayEvent;
			private int _ownerID;
			private string _ownerName = string.Empty;
			private DateTime _lastUpdatedAt;
			private string _lastUpdatedBy = string.Empty;
			private int _lastUpdatedID;
			private DateTime _originalDateBegin;
			private string _updateStatus = string.Empty;
			private string _rrule = string.Empty;
			private int _rmOwnerID;
			private bool _newEventEmailSent;
			private bool _isPrivate;
			private string _eventTimeZoneId = string.Empty;
			private string _otherTimeZoneId = string.Empty;
			private bool _allowAnonEnroll;
			private int _contentItemId;
			private bool _journalItem;
			private int _socialGroupId;
			private int _socialUserId;
			private string _summary = string.Empty;
			private int _sequence;
			private int _rmSequence;
			private string _socialUserUserName = string.Empty;
			private string _socialUserDisplayName = string.Empty;
			
			
#endregion
			
#region Constructors
			
			// initialization
			/// <summary>
			/// Initializes a new instance of the <see cref="EventInfo" /> class.
			/// </summary>
			public EventInfo()
			{
			}
#endregion
			
#region Public Properties
			
			// public properties
			/// <summary>
			/// Gets or sets the portal ID.
			/// </summary>
			/// <value>The portal ID.</value>
			public int PortalID
			{
				get
				{
					return _portalID;
				}
				set
				{
					_portalID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the event ID.
			/// </summary>
			/// <value>The event ID.</value>
			public int EventID
			{
				get
				{
					return _eventID;
				}
				set
				{
					_eventID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the recurring master ID.
			/// </summary>
			/// <value>The recur master ID.</value>
			public int RecurMasterID
			{
				get
				{
					return _recurMasterID;
				}
				set
				{
					_recurMasterID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the module ID.
			/// </summary>
			/// <value>The module ID.</value>
			public int ModuleID
			{
				get
				{
					return _moduleID;
				}
				set
				{
					_moduleID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the event date end.
			/// </summary>
			/// <value>The event date end.</value>
			[Obsolete("EventDateEnd is only retained for upgrade from versions prior to 4.1.0")]
				public DateTime EventDateEnd
				{
				get
				{
					return _eventDateEnd;
				}
				set
				{
					_eventDateEnd = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the event time begin.
			/// </summary>
			/// <value>The event time begin.</value>
			public DateTime EventTimeBegin
			{
				get
				{
					return _eventTimeBegin;
				}
				set
				{
					_eventTimeBegin = value;
				}
			}
			
			/// <summary>
			/// Gets the event time end.
			/// </summary>
			/// <value>The event time end.</value>
			public DateTime EventTimeEnd
			{
				get
				{
					return EventTimeBegin.AddMinutes(Duration);
				}
			}
			
			/// <summary>
			/// Gets or sets the duration of an event.
			/// </summary>
			/// <value>The duration.</value>
			public int Duration
			{
				get
				{
					return _duration;
				}
				set
				{
					_duration = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the name of the event.
			/// </summary>
			/// <value>The name of the event.</value>
			public string EventName
			{
				get
				{
					return _eventName;
				}
				set
				{
					_eventName = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the event description
			/// </summary>
			/// <value>The event description.</value>
			public string EventDesc
			{
				get
				{
					return _eventDesc;
				}
				set
				{
					_eventDesc = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the importance of an event
			/// </summary>
			/// <value>The importance of an event.</value>
			public Priority Importance
			{
				get
				{
					return ((Priority) _importance);
				}
				set
				{
					_importance = (int) value;
				}
			}
			
			/// <summary>
			/// Gets or sets the created date of an event
			/// </summary>
			/// <value>The created date of an event.</value>
			public DateTime CreatedDate
			{
				get
				{
					return _createdDate;
				}
				set
				{
					_createdDate = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the created by of an event.
			/// </summary>
			/// <value>The created by of an event.</value>
			public string CreatedBy
			{
				get
				{
					return _createdBy;
				}
				set
				{
					_createdBy = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the created by UserID.
			/// </summary>
			/// <value>The created by UserID.</value>
			public int CreatedByID
			{
				get
				{
					return _createdByID;
				}
				set
				{
					_createdByID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the recurrence unit of an event
			/// </summary>
			/// <value>The recurrence unit of an event.</value>
			[Obsolete("Every is only retained for upgrade from versions prior to 4.1.0")]
				public int Every
				{
				get
				{
					return _every;
				}
			}
			
			/// <summary>
			/// Gets or sets the period of a recurrence.
			/// </summary>
			/// <value>The period of a recurrnence.</value>
			[Obsolete("Period is only retained for upgrade from versions prior to 4.1.0")]
				public string Period
				{
				get
				{
					return _period;
				}
			}
			
			/// <summary>
			/// Gets or sets the type of the repeat of a recurrence
			/// </summary>
			/// <value>The type of the repeat of a recurrence.</value>
			[Obsolete("RepeatType is only retained for upgrade from versions prior to 4.1.0")]
				public string RepeatType
				{
				get
				{
					return _repeatType;
				}
			}
			
			/// <summary>
			/// Gets or sets the notification text
			/// </summary>
			/// <value>The notification text.</value>
			public string Notify
			{
				get
				{
					return _notify;
				}
				set
				{
					_notify = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="EventInfo" /> is approved.
			/// </summary>
			/// <value><c>true</c> if approved; otherwise, <c>false</c>.</value>
			public bool Approved
			{
				get
				{
					return _approved;
				}
				set
				{
					_approved = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="EventInfo" /> can have signups.
			/// </summary>
			/// <value><c>true</c> can have signups; otherwise, <c>false</c>.</value>
			public bool Signups
			{
				get
				{
					return _signups;
				}
				set
				{
					_signups = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the max # of enrollments
			/// </summary>
			/// <value>The max # of enrollments</value>
			public int MaxEnrollment
			{
				get
				{
					return _maxEnrollment;
				}
				set
				{
					_maxEnrollment = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the # of enrolled people
			/// </summary>
			/// <value>The # enrolled of enrolled people.</value>
			public int Enrolled
			{
				get
				{
					if (_enrolled < 0)
					{
						return 0;
					}
					return _enrolled;
				}
				set
				{
					_enrolled = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the # f recurrences.
			/// </summary>
			/// <value>The # of recurrences.</value>
			public int NoOfRecurrences
			{
				get
				{
					return _noOfRecurrences;
				}
				set
				{
					_noOfRecurrences = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the last recurrence date
			/// </summary>
			/// <value>The last recurrence date.</value>
			public DateTime LastRecurrence
			{
				get
				{
					return _lastRecurrence;
				}
				set
				{
					_lastRecurrence = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the enroll role ID.
			/// </summary>
			/// <value>The enroll role ID.</value>
			public int EnrollRoleID
			{
				get
				{
					return _enrollRoleID;
				}
				set
				{
					_enrollRoleID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the type of the enrollment
			/// </summary>
			/// <value>The type of the enrollment</value>
			public string EnrollType
			{
				get
				{
					return _enrollType;
				}
				set
				{
					_enrollType = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the enroll fee.
			/// </summary>
			/// <value>The enroll fee.</value>
			public decimal EnrollFee
			{
				get
				{
					return _enrollFee;
				}
				set
				{
					_enrollFee = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the PayPal account.
			/// </summary>
			/// <value>The PayPal account.</value>
			public string PayPalAccount
			{
				get
				{
					return _payPalAccount;
				}
				set
				{
					_payPalAccount = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="EventInfo" /> is cancelled.
			/// </summary>
			/// <value><c>true</c> if cancelled; otherwise, <c>false</c>.</value>
			public bool Cancelled
			{
				get
				{
					return _cancelled;
				}
				set
				{
					_cancelled = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether a detail page must be openend.
			/// </summary>
			/// <value><c>true</c> if detail page must be opened; otherwise, <c>false</c>.</value>
			public bool DetailPage
			{
				get
				{
					return _detailPage;
				}
				set
				{
					_detailPage = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether detail page must appear in a new page.
			/// </summary>
			/// <value><c>true</c> if new page; otherwise, <c>false</c>.</value>
			public bool DetailNewWin
			{
				get
				{
					return _detailNewWin;
				}
				set
				{
					_detailNewWin = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the detail URL.
			/// </summary>
			/// <value>The detail URL.</value>
			public string DetailURL
			{
				get
				{
					return _detailURL;
				}
				set
				{
					_detailURL = value;
				}
			}
			/// <summary>
			/// Gets or sets the image URL.
			/// </summary>
			/// <value>The image URL.</value>
			public string ImageURL
			{
				get
				{
					return _imageURL;
				}
				set
				{
					_imageURL = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the type of the image.
			/// </summary>
			/// <value>The type of the image.</value>
			public string ImageType
			{
				get
				{
					return _imageType;
				}
				set
				{
					_imageType = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the width of the image.
			/// </summary>
			/// <value>The width of the image.</value>
			public int ImageWidth
			{
				get
				{
					return _imageWidth;
				}
				set
				{
					_imageWidth = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the height of the image.
			/// </summary>
			/// <value>The height of the image.</value>
			public int ImageHeight
			{
				get
				{
					return _imageHeight;
				}
				set
				{
					_imageHeight = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the location of an event
			/// </summary>
			/// <value>The location of an event.</value>
			public int Location
			{
				get
				{
					return _location;
				}
				set
				{
					_location = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the name of the location of an event.
			/// </summary>
			/// <value>The name of the location of an event.</value>
			public string LocationName
			{
				get
				{
					return _locationName;
				}
				set
				{
					_locationName = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the map URL.
			/// </summary>
			/// <value>The map URL.</value>
			public string MapURL
			{
				get
				{
					return _mapURL;
				}
				set
				{
					_mapURL = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the category of an event
			/// </summary>
			/// <value>The category of an event.</value>
			public int Category
			{
				get
				{
					return _category;
				}
				set
				{
					_category = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the name of the category.
			/// </summary>
			/// <value>The name of the category.</value>
			public string CategoryName
			{
				get
				{
					return _categoryName;
				}
				set
				{
					_categoryName = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the category color to be used for display.
			/// </summary>
			/// <value>The color of the category.</value>
			public string Color
			{
				get
				{
					return _color;
				}
				set
				{
					_color = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the category color of the font (foreground color).
			/// </summary>
			/// <value>The color of the font.</value>
			public string FontColor
			{
				get
				{
					return _fontColor;
				}
				set
				{
					_fontColor = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether an image must be displayed.
			/// </summary>
			/// <value><c>true</c> if image must be displayed; otherwise, <c>false</c>.</value>
			public bool ImageDisplay
			{
				get
				{
					return _imageDisplay;
				}
				set
				{
					_imageDisplay = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether to send a reminder.
			/// </summary>
			/// <value><c>true</c> if send reminder; otherwise, <c>false</c>.</value>
			public bool SendReminder
			{
				get
				{
					return _sendReminder;
				}
				set
				{
					_sendReminder = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the reminder time.
			/// </summary>
			/// <value>The reminder time.</value>
			public int ReminderTime
			{
				get
				{
					return _reminderTime;
				}
				set
				{
					_reminderTime = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the reminder time measurement.
			/// </summary>
			/// <value>The reminder time measurement.</value>
			public string ReminderTimeMeasurement
			{
				get
				{
					return _reminderTimeMeasurement;
				}
				set
				{
					_reminderTimeMeasurement = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the reminder.
			/// </summary>
			/// <value>The reminder.</value>
			public string Reminder
			{
				get
				{
					return _reminder;
				}
				set
				{
					_reminder = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the reminder from e-mail address
			/// </summary>
			/// <value>The reminder from e-mail address.</value>
			public string ReminderFrom
			{
				get
				{
					return _reminderFrom;
				}
				set
				{
					_reminderFrom = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether the event is submitted to the search
			/// </summary>
			/// <value><c>true</c> if search submitted; otherwise, <c>false</c>.</value>
			public bool SearchSubmitted
			{
				get
				{
					return _searchSubmitted;
				}
				set
				{
					_searchSubmitted = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the module title.
			/// </summary>
			/// <value>The module title.</value>
			public string ModuleTitle
			{
				get
				{
					return _moduleTitle;
				}
				set
				{
					_moduleTitle = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the custom field1.
			/// </summary>
			/// <value>The custom field1.</value>
			public string CustomField1
			{
				get
				{
					return _customField1;
				}
				set
				{
					_customField1 = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the custom field2.
			/// </summary>
			/// <value>The custom field2.</value>
			public string CustomField2
			{
				get
				{
					return _customField2;
				}
				set
				{
					_customField2 = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether enroll list view is to be displayed.
			/// </summary>
			/// <value><c>true</c> if [enroll list view]; otherwise, <c>false</c>.</value>
			public bool EnrollListView
			{
				get
				{
					return _enrollListView;
				}
				set
				{
					_enrollListView = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether end date is to be displayed
			/// </summary>
			/// <value><c>true</c> if [display end date]; otherwise, <c>false</c>.</value>
			public bool DisplayEndDate
			{
				get
				{
					return _displayEndDate;
				}
				set
				{
					_displayEndDate = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether the event is an all day event.
			/// </summary>
			/// <value><c>true</c> if [all day event]; otherwise, <c>false</c>.</value>
			public bool AllDayEvent
			{
				get
				{
					return _allDayEvent;
				}
				set
				{
					_allDayEvent = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the owner ID (userID).
			/// </summary>
			/// <value>The owner ID.</value>
			public int OwnerID
			{
				get
				{
					return _ownerID;
				}
				set
				{
					_ownerID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the name of the owner of the event
			/// </summary>
			/// <value>The name of the owner.</value>
			public string OwnerName
			{
				get
				{
					return _ownerName;
				}
				set
				{
					_ownerName = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the last updated at.
			/// </summary>
			/// <value>The last updated at.</value>
			public DateTime LastUpdatedAt
			{
				get
				{
					return _lastUpdatedAt;
				}
				set
				{
					_lastUpdatedAt = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the last updated by.
			/// </summary>
			/// <value>The last updated by.</value>
			public string LastUpdatedBy
			{
				get
				{
					return _lastUpdatedBy;
				}
				set
				{
					_lastUpdatedBy = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the last updated userID
			/// </summary>
			/// <value>The last updated userID.</value>
			public int LastUpdatedID
			{
				get
				{
					return _lastUpdatedID;
				}
				set
				{
					_lastUpdatedID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the original date begin.
			/// </summary>
			/// <value>The original date begin.</value>
			public DateTime OriginalDateBegin
			{
				get
				{
					return _originalDateBegin;
				}
				set
				{
					_originalDateBegin = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the update status.
			/// </summary>
			/// <value>The update status.</value>
			public string UpdateStatus
			{
				get
				{
					return _updateStatus;
				}
				set
				{
					_updateStatus = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the Recurrence Rule.
			/// </summary>
			/// <value>The Recurrence Rule.</value>
			public string RRULE
			{
				get
				{
					return _rrule;
				}
				set
				{
					_rrule = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the RM owner ID.
			/// </summary>
			/// <value>The RM owner ID.</value>
			public int RmOwnerID
			{
				get
				{
					return _rmOwnerID;
				}
				set
				{
					_rmOwnerID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether [new event email sent].
			/// </summary>
			/// <value><c>true</c> if [new event email sent]; otherwise, <c>false</c>.</value>
			public bool NewEventEmailSent
			{
				get
				{
					return _newEventEmailSent;
				}
				set
				{
					_newEventEmailSent = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether [is private].
			/// </summary>
			/// <value><c>true</c> if [is private]; otherwise, <c>false</c>.</value>
			public bool IsPrivate
			{
				get
				{
					return _isPrivate;
				}
				set
				{
					_isPrivate = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating what the [event timezoneid] is
			/// </summary>
			/// <value>The event timezoneid.</value>
			public string EventTimeZoneId
			{
				get
				{
					if (string.IsNullOrEmpty(_eventTimeZoneId))
					{
						EventModuleSettings modSettings = EventModuleSettings.GetEventModuleSettings(_moduleID, null);
						_eventTimeZoneId = modSettings.TimeZoneId;
					}
					return _eventTimeZoneId;
				}
				set
				{
					_eventTimeZoneId = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating what the [other timezoneid] is
			/// </summary>
			/// <value>The other timezoneid.</value>
			public string OtherTimeZoneId
			{
				get
				{
					if (ReferenceEquals(_otherTimeZoneId, null))
					{
						_otherTimeZoneId = "UTC";
					}
					return _otherTimeZoneId;
				}
				set
				{
					_otherTimeZoneId = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether [allow anonymous enrollment].
			/// </summary>
			/// <value><c>true</c> if [allow anonymous enrollment]; otherwise, <c>false</c>.</value>
			public bool AllowAnonEnroll
			{
				get
				{
					return _allowAnonEnroll;
				}
				set
				{
					_allowAnonEnroll = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating the [content item id].
			/// </summary>
			/// <value>The contentitemid.</value>
			public int ContentItemID
			{
				get
				{
					return _contentItemId;
				}
				set
				{
					_contentItemId = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether [event has journal item].
			/// </summary>
			/// <value><c>true</c> if [event has journal item]; otherwise, <c>false</c>.</value>
			public bool JournalItem
			{
				get
				{
					return _journalItem;
				}
				set
				{
					_journalItem = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating the [soccialgroup id].
			/// </summary>
			/// <value>The SocialGroupId.</value>
			public int SocialGroupId
			{
				get
				{
					return _socialGroupId;
				}
				set
				{
					_socialGroupId = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating the [user id].
			/// </summary>
			/// <value>The SocialUserId.</value>
			public int SocialUserId
			{
				get
				{
					return _socialUserId;
				}
				set
				{
					_socialUserId = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the summary.
			/// </summary>
			/// <value>The summary.</value>
			public string Summary
			{
				get
				{
					return _summary;
				}
				set
				{
					_summary = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating the [sequence].
			/// </summary>
			/// <value>The Sequence.</value>
			public int Sequence
			{
				get
				{
					return _sequence;
				}
				set
				{
					_sequence = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating the [recur master sequence].
			/// </summary>
			/// <value>The Recur MasterSequence.</value>
			public int RmSequence
			{
				get
				{
					return _rmSequence;
				}
				set
				{
					_rmSequence = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the SocialUserUserName.
			/// </summary>
			/// <value>The SocialUserUserName.</value>
			public string SocialUserUserName
			{
				get
				{
					return _socialUserUserName;
				}
				set
				{
					_socialUserUserName = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the SocialUserDisplayName.
			/// </summary>
			/// <value>The SocialUserDisplayName.</value>
			public string SocialUserDisplayName
			{
				get
				{
					return _socialUserDisplayName;
				}
				set
				{
					_socialUserDisplayName = value;
				}
			}
			
			private static SortFilter _sortExpression;
			/// <summary>
			/// Gets or sets the sort expression.
			/// </summary>
			/// <value>The sort expression.</value>
			public static SortFilter SortExpression
			{
				get
				{
					return _sortExpression;
				}
				set
				{
					_sortExpression = value;
				}
			}
			
			private static SortDirection _sortDirection;
			/// <summary>
			/// Gets or sets the sort direction.
			/// </summary>
			/// <value>The sort direction.</value>
			public static SortDirection SortDirection
			{
				get
				{
					return _sortDirection;
				}
				set
				{
					_sortDirection = value;
				}
			}
			
			
			/// <summary>
			/// Sorting enumeration
			/// </summary>
			public enum SortFilter
			{
				/// <summary>
				/// By EventID
				/// </summary>
				EventID,
				/// <summary>
				/// By Date beging
				/// </summary>
				EventDateBegin,
				/// <summary>
				/// By Date end
				/// </summary>
				EventDateEnd,
				/// <summary>
				/// Bu Name
				/// </summary>
				EventName,
				/// <summary>
				/// By duration
				/// </summary>
				Duration,
				/// <summary>
				/// Bu category name
				/// </summary>
				CategoryName,
				/// <summary>
				/// By customfield1
				/// </summary>
				CustomField1,
				/// <summary>
				/// By customfield2
				/// </summary>
				CustomField2,
				/// <summary>
				/// By description
				/// </summary>
				Description,
				/// <summary>
				/// By Location name
				/// </summary>
				LocationName
			}
			
			/// <summary>
			/// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
			/// </summary>
			/// <param name="obj">An object to compare with this instance.</param>
			/// <returns>
			/// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings:
			/// Value
			/// Meaning
			/// Less than zero
			/// This instance is less than <paramref name="obj" />.
			/// Zero
			/// This instance is equal to <paramref name="obj" />.
			/// Greater than zero
			/// This instance is greater than <paramref name="obj" />.
			/// </returns>
			/// <exception cref="T:System.ArgumentException">
			/// <paramref name="obj" /> is not the same type as this instance.
			/// </exception>
			public int CompareTo(object obj)
			{
				EventInfo o = (EventInfo) obj;
				string xCompare = EventName + Strings.Format(EventID, "00000000");
				string yCompare = o.EventName + Strings.Format(o.EventID, "00000000");
				if (SortExpression == SortFilter.CategoryName)
				{
					xCompare = CategoryName + Strings.Format(EventID, "00000000");
					yCompare = o.CategoryName + Strings.Format(o.EventID, "00000000");
				}
				else if (SortExpression == SortFilter.CustomField1)
				{
					xCompare = CustomField1 + Strings.Format(EventID, "00000000");
					yCompare = o.CustomField1 + Strings.Format(o.EventID, "00000000");
				}
				else if (SortExpression == SortFilter.CustomField2)
				{
					xCompare = CustomField2 + Strings.Format(EventID, "00000000");
					yCompare = o.CustomField2 + Strings.Format(o.EventID, "00000000");
				}
				else if (SortExpression == SortFilter.Description)
				{
					xCompare = EventDesc + Strings.Format(EventID, "00000000");
					yCompare = o.EventDesc + Strings.Format(o.EventID, "00000000");
				}
				else if (SortExpression == SortFilter.Duration)
				{
					xCompare = System.Convert.ToString(Strings.Format(Duration, "000000") + Strings.Format(EventID, "00000000"));
					yCompare = System.Convert.ToString(Strings.Format(o.Duration, "000000") + Strings.Format(o.EventID, "00000000"));
				}
				else if (SortExpression == SortFilter.EventDateBegin)
				{
					xCompare = System.Convert.ToString(Strings.Format(EventTimeBegin, "yyyyMMddHHmm") + Strings.Format(EventID, "00000000"));
					yCompare = System.Convert.ToString(Strings.Format(o.EventTimeBegin, "yyyyMMddHHmm") + Strings.Format(o.EventID, "00000000"));
				}
				else if (SortExpression == SortFilter.EventDateEnd)
				{
					xCompare = System.Convert.ToString(Strings.Format(EventTimeEnd, "yyyyMMddHHmm") + Strings.Format(EventID, "00000000"));
					yCompare = System.Convert.ToString(Strings.Format(o.EventTimeEnd, "yyyyMMddHHmm") + Strings.Format(o.EventID, "00000000"));
				}
				else if (SortExpression == SortFilter.LocationName)
				{
					xCompare = LocationName + Strings.Format(EventID, "00000000");
					yCompare = o.LocationName + Strings.Format(o.EventID, "00000000");
				}
				if (SortDirection == System.Web.UI.WebControls.SortDirection.Ascending)
				{
					return System.String.Compare(xCompare, yCompare, StringComparison.CurrentCulture);
				}
				else
				{
					return System.String.Compare(yCompare, xCompare, StringComparison.CurrentCulture);
				}
			}
			
			
#endregion
			
#region Public Methods
			
			/// <summary>
			/// Clones an instance of the events object
			/// </summary>
			/// <returns>Cloned Eventsinfo object</returns>
			public EventInfo Clone()
			{
				return ((EventInfo) (MemberwiseClone()));
				
			}
#endregion
		}
		
#endregion
		
#region EventMasterInfo Class
		public class EventMasterInfo
		{
			
			private int _portalID;
			private int _masterID;
			private int _moduleID;
			private int _subEventID;
			private string _subEventTitle;
			
			/// <summary>
			/// Gets or sets the portal ID.
			/// </summary>
			/// <value>The portal ID.</value>
			public int PortalID
			{
				get
				{
					return _portalID;
				}
				set
				{
					_portalID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the master ID.
			/// </summary>
			/// <value>The master ID.</value>
			public int MasterID
			{
				get
				{
					return _masterID;
				}
				set
				{
					_masterID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the module ID.
			/// </summary>
			/// <value>The module ID.</value>
			public int ModuleID
			{
				get
				{
					return _moduleID;
				}
				set
				{
					_moduleID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the sub event ID.
			/// </summary>
			/// <value>The sub event ID.</value>
			public int SubEventID
			{
				get
				{
					return _subEventID;
				}
				set
				{
					_subEventID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the sub event title.
			/// </summary>
			/// <value>The sub event title.</value>
			public string SubEventTitle
			{
				get
				{
					return _subEventTitle;
				}
				set
				{
					_subEventTitle = value;
				}
			}
		}
#endregion
		
#region EventSignupsInfo Class
		/// <summary>
		/// Information about the users that have signed up for a particular event
		/// </summary>
		public class EventSignupsInfo : IComparable
		{
			
			/// <summary>
			/// Priority of the event
			/// </summary>
			public enum Priority
			{
				/// <summary>
				/// Hign priority
				/// </summary>
				High = 1,
				/// <summary>
				/// Medium priority
				/// </summary>
				Medium = 2,
				/// <summary>
				/// Low priority
				/// </summary>
				Low = 3
			}
			
			private int _eventID;
			private int _moduleID;
			private int _signupID;
			private int _userID;
			private bool _approved = false;
			private string _userName;
			private string _email;
			private bool _emailVisible;
			private string _telephone;
			private DateTime _eventTimeBegin = DateTime.Now;
			private DateTime _eventTimeEnd;
			private int _duration;
			private string _eventName;
			private int _importance;
			private bool _eventApproved = false;
			private int _maxEnrollment = 0;
			private int _enrolled;
			private string _payPalStatus;
			private string _payPalReason;
			private string _payPalTransID;
			private string _payPalPayerID;
			private string _payPalPayerStatus;
			private string _payPalRecieverEmail;
			private string _payPalUserEmail;
			private string _payPalPayerEmail;
			private string _payPalFirstName;
			private string _payPalLastName;
			private string _payPalAddress;
			private string _payPalCity;
			private string _payPalState;
			private string _payPalZip;
			private string _payPalCountry;
			private string _payPalCurrency;
			private DateTime _payPalPaymentDate = DateTime.Now;
			private decimal _payPalAmount = 0;
			private decimal _payPalFee = 0;
			private int _noEnrolees = 1;
			private string _eventTimeZoneId;
			private string _anonEmail;
			private string _anonName;
			private string _anonTelephone;
			private string _anonCulture;
			private string _anonTimeZoneId;
			private string _firstName;
			private string _lastName;
			private string _company;
			private string _jobTitle;
			private string _referenceNumber;
			private string _remarks;
			private string _street;
			private string _postalCode;
			private string _city;
			private string _region;
			private string _country;
			
			// initialization
			/// <summary>
			/// Initializes a new instance of the <see cref="EventSignupsInfo" /> class.
			/// </summary>
			public EventSignupsInfo()
			{
			}
			
			// public properties
			/// <summary>
			/// Gets or sets the event ID.
			/// </summary>
			/// <value>The event ID.</value>
			public int EventID
			{
				get
				{
					return _eventID;
				}
				set
				{
					_eventID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the module ID.
			/// </summary>
			/// <value>The module ID.</value>
			public int ModuleID
			{
				get
				{
					return _moduleID;
				}
				set
				{
					_moduleID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the signup ID.
			/// </summary>
			/// <value>The signup ID.</value>
			public int SignupID
			{
				get
				{
					return _signupID;
				}
				set
				{
					_signupID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the user ID.
			/// </summary>
			/// <value>The user ID.</value>
			public int UserID
			{
				get
				{
					return _userID;
				}
				set
				{
					_userID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="EventSignupsInfo" /> is approved.
			/// </summary>
			/// <value><c>true</c> if approved; otherwise, <c>false</c>.</value>
			public bool Approved
			{
				get
				{
					return _approved;
				}
				set
				{
					_approved = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the name of the user for the signup
			/// </summary>
			/// <value>The name of the user.</value>
			public string UserName
			{
				get
				{
					return _userName;
				}
				set
				{
					_userName = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the email of the signup
			/// </summary>
			/// <value>The email.</value>
			public string Email
			{
				get
				{
					return _email;
				}
				set
				{
					_email = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether email of the signup is visible.
			/// </summary>
			/// <value><c>true</c> if [email visible]; otherwise, <c>false</c>.</value>
			public bool EmailVisible
			{
				get
				{
					return _emailVisible;
				}
				set
				{
					_emailVisible = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the telephone of the signup
			/// </summary>
			/// <value>The telephone.</value>
			public string Telephone
			{
				get
				{
					return _telephone;
				}
				set
				{
					_telephone = value;
				}
			}
			
			/// <summary>
			/// Gets the event date begin.
			/// </summary>
			/// <value>The event date begin.</value>
			public DateTime EventDateBegin
			{
				get
				{
					return _eventTimeBegin.Date;
				}
			}
			
			/// <summary>
			/// Gets or sets the event time begin.
			/// </summary>
			/// <value>The event time begin.</value>
			public DateTime EventTimeBegin
			{
				get
				{
					return _eventTimeBegin;
				}
				set
				{
					_eventTimeBegin = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the event time end.
			/// </summary>
			/// <value>The event time end.</value>
			public DateTime EventTimeEnd
			{
				get
				{
					return _eventTimeEnd;
				}
				set
				{
					_eventTimeEnd = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the duration.
			/// </summary>
			/// <value>The duration.</value>
			public int Duration
			{
				get
				{
					return _duration;
				}
				set
				{
					_duration = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the name of the event.
			/// </summary>
			/// <value>The name of the event.</value>
			public string EventName
			{
				get
				{
					return _eventName;
				}
				set
				{
					_eventName = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the importance of the event
			/// </summary>
			/// <value>The importance.</value>
			public Priority Importance
			{
				get
				{
					return ((Priority) _importance);
				}
				set
				{
					_importance = (int) value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether the event is approved.
			/// </summary>
			/// <value><c>true</c> if [event approved]; otherwise, <c>false</c>.</value>
			public bool EventApproved
			{
				get
				{
					return _eventApproved;
				}
				set
				{
					_eventApproved = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the max # of enrollments
			/// </summary>
			/// <value>The max # of enrollments.</value>
			public int MaxEnrollment
			{
				get
				{
					return _maxEnrollment;
				}
				set
				{
					_maxEnrollment = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the # of enrolled.
			/// </summary>
			/// <value>The # of enrolled.</value>
			public int Enrolled
			{
				get
				{
					return _enrolled;
				}
				set
				{
					_enrolled = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the PayPal status.
			/// </summary>
			/// <value>The PayPal status.</value>
			public string PayPalStatus
			{
				get
				{
					return _payPalStatus;
				}
				set
				{
					_payPalStatus = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the PayPal result reason
			/// </summary>
			/// <value>The PayPal reason.</value>
			public string PayPalReason
			{
				get
				{
					return _payPalReason;
				}
				set
				{
					_payPalReason = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the PayPal transaction ID.
			/// </summary>
			/// <value>The PayPal transaction ID.</value>
			public string PayPalTransID
			{
				get
				{
					return _payPalTransID;
				}
				set
				{
					_payPalTransID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the PayPal payer ID.
			/// </summary>
			/// <value>The PayPal payer ID.</value>
			public string PayPalPayerID
			{
				get
				{
					return _payPalPayerID;
				}
				set
				{
					_payPalPayerID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the PayPal payer status.
			/// </summary>
			/// <value>The pay PayPal status.</value>
			public string PayPalPayerStatus
			{
				get
				{
					return _payPalPayerStatus;
				}
				set
				{
					_payPalPayerStatus = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the PayPal reciever email.
			/// </summary>
			/// <value>The PayPal reciever email.</value>
			public string PayPalRecieverEmail
			{
				get
				{
					return _payPalRecieverEmail;
				}
				set
				{
					_payPalRecieverEmail = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the PayPal user email.
			/// </summary>
			/// <value>The PayPal user email.</value>
			public string PayPalUserEmail
			{
				get
				{
					return _payPalUserEmail;
				}
				set
				{
					_payPalUserEmail = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the PayPal payer email.
			/// </summary>
			/// <value>The PayPal payer email.</value>
			public string PayPalPayerEmail
			{
				get
				{
					return _payPalPayerEmail;
				}
				set
				{
					_payPalPayerEmail = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the first name of the PayPal.
			/// </summary>
			/// <value>The first name of the PayPal.</value>
			public string PayPalFirstName
			{
				get
				{
					return _payPalFirstName;
				}
				set
				{
					_payPalFirstName = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the last name of the PayPal.
			/// </summary>
			/// <value>The last name of the PayPal.</value>
			public string PayPalLastName
			{
				get
				{
					return _payPalLastName;
				}
				set
				{
					_payPalLastName = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the PayPal address.
			/// </summary>
			/// <value>The PayPal address.</value>
			public string PayPalAddress
			{
				get
				{
					return _payPalAddress;
				}
				set
				{
					_payPalAddress = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the PayPal city.
			/// </summary>
			/// <value>The PayPal city.</value>
			public string PayPalCity
			{
				get
				{
					return _payPalCity;
				}
				set
				{
					_payPalCity = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the state of the PayPal.
			/// </summary>
			/// <value>The state of the PayPal.</value>
			public string PayPalState
			{
				get
				{
					return _payPalState;
				}
				set
				{
					_payPalState = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the PayPal zip.
			/// </summary>
			/// <value>The PayPal zip.</value>
			public string PayPalZip
			{
				get
				{
					return _payPalZip;
				}
				set
				{
					_payPalZip = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the PayPal country.
			/// </summary>
			/// <value>The PayPal country.</value>
			public string PayPalCountry
			{
				get
				{
					return _payPalCountry;
				}
				set
				{
					_payPalCountry = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the PayPal currency.
			/// </summary>
			/// <value>The PayPal currency.</value>
			public string PayPalCurrency
			{
				get
				{
					return _payPalCurrency;
				}
				set
				{
					_payPalCurrency = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the PayPal payment date.
			/// </summary>
			/// <value>The PayPal payment date.</value>
			public DateTime PayPalPaymentDate
			{
				get
				{
					return _payPalPaymentDate;
				}
				set
				{
					_payPalPaymentDate = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the PayPal amount.
			/// </summary>
			/// <value>The PayPal amount.</value>
			public decimal PayPalAmount
			{
				get
				{
					return _payPalAmount;
				}
				set
				{
					_payPalAmount = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the PayPal fee.
			/// </summary>
			/// <value>The PayPal fee.</value>
			public decimal PayPalFee
			{
				get
				{
					return _payPalFee;
				}
				set
				{
					_payPalFee = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the # of  enrolees.
			/// </summary>
			/// <value>The # of enrolees.</value>
			public int NoEnrolees
			{
				get
				{
					return _noEnrolees;
				}
				set
				{
					_noEnrolees = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating what the [event timezoneid] is
			/// </summary>
			/// <value>The event timezoneid.</value>
			public string EventTimeZoneId
			{
				get
				{
					return _eventTimeZoneId;
				}
				set
				{
					_eventTimeZoneId = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating what the [anonymous email] is
			/// </summary>
			/// <value>The event timezoneid.</value>
			public string AnonEmail
			{
				get
				{
					return _anonEmail;
				}
				set
				{
					_anonEmail = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating what the [anonymous name] is
			/// </summary>
			/// <value>The event timezoneid.</value>
			public string AnonName
			{
				get
				{
					return _anonName;
				}
				set
				{
					_anonName = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating what the [anonymous telephone] is
			/// </summary>
			/// <value>The event timezoneid.</value>
			public string AnonTelephone
			{
				get
				{
					return _anonTelephone;
				}
				set
				{
					_anonTelephone = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating what the [anonymous Culture] is
			/// </summary>
			/// <value>The event timezoneid.</value>
			public string AnonCulture
			{
				get
				{
					return _anonCulture;
				}
				set
				{
					_anonCulture = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating what the [anonymous TimeZoneId] is
			/// </summary>
			/// <value>The event timezoneid.</value>
			public string AnonTimeZoneId
			{
				get
				{
					return _anonTimeZoneId;
				}
				set
				{
					_anonTimeZoneId = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the enrollee's dirst name.
			/// </summary>
			/// <value>The enrollee's first name.</value>
			public string FirstName
			{
				get
				{
					return _firstName;
				}
				set
				{
					_firstName = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the enrollee's last name.
			/// </summary>
			/// <value>The enrollee's last name.</value>
			public string LastName
			{
				get
				{
					return _lastName;
				}
				set
				{
					_lastName = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the enrollee's company.
			/// </summary>
			/// <value>The enrollee's company.</value>
			public string Company
			{
				get
				{
					return _company;
				}
				set
				{
					_company = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the enrollee's job title.
			/// </summary>
			/// <value>The enrollee's job title.</value>
			public string JobTitle
			{
				get
				{
					return _jobTitle;
				}
				set
				{
					_jobTitle = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the enrollee's reference number.
			/// </summary>
			/// <value>The enrollee's reference number.</value>
			public string ReferenceNumber
			{
				get
				{
					return _referenceNumber;
				}
				set
				{
					_referenceNumber = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the enrollee's remarks.
			/// </summary>
			/// <value>The enrollee's remarks.</value>
			public string Remarks
			{
				get
				{
					return _remarks;
				}
				set
				{
					_remarks = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the street of the enrollee's address.
			/// </summary>
			/// <value>The street of the enrollee's address.</value>
			public string Street
			{
				get
				{
					return _street;
				}
				set
				{
					_street = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the postal code of the enrollee's address.
			/// </summary>
			/// <value>The postal code of the enrollee's address.</value>
			public string PostalCode
			{
				get
				{
					return _postalCode;
				}
				set
				{
					_postalCode = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the city of the enrollee's address.
			/// </summary>
			/// <value>The city of the enrollee's address.</value>
			public string City
			{
				get
				{
					return _city;
				}
				set
				{
					_city = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the region of the enrollee's address.
			/// </summary>
			/// <value>The region of the enrollee's address.</value>
			public string Region
			{
				get
				{
					return _region;
				}
				set
				{
					_region = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the country of the enrollee's address.
			/// </summary>
			/// <value>The country of the enrollee's address.</value>
			public string Country
			{
				get
				{
					return _country;
				}
				set
				{
					_country = value;
				}
			}
#region Sorting
			private static SortFilter _sortExpression;
			/// <summary>
			/// Gets or sets the sort expression.
			/// </summary>
			/// <value>The sort expression.</value>
			public static SortFilter SortExpression
			{
				get
				{
					return _sortExpression;
				}
				set
				{
					_sortExpression = value;
				}
			}
			
			private static SortDirection _sortDirection;
			/// <summary>
			/// Gets or sets the sort direction.
			/// </summary>
			/// <value>The sort direction.</value>
			public static SortDirection SortDirection
			{
				get
				{
					return _sortDirection;
				}
				set
				{
					_sortDirection = value;
				}
			}
			
			
			/// <summary>
			/// Sorting enumeration
			/// </summary>
			public enum SortFilter
			{
				/// <summary>
				/// By EventID
				/// </summary>
				EventID,
				/// <summary>
				/// By Date beging
				/// </summary>
				EventTimeBegin,
				/// <summary>
				/// By Date end
				/// </summary>
				EventTimeEnd,
				/// <summary>
				/// Bu Name
				/// </summary>
				EventName,
				/// <summary>
				/// By duration
				/// </summary>
				Duration,
				/// <summary>
				/// By approved
				/// </summary>
				Approved
			}
			
			/// <summary>
			/// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
			/// </summary>
			/// <param name="obj">An object to compare with this instance.</param>
			/// <returns>
			/// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings:
			/// Value
			/// Meaning
			/// Less than zero
			/// This instance is less than <paramref name="obj" />.
			/// Zero
			/// This instance is equal to <paramref name="obj" />.
			/// Greater than zero
			/// This instance is greater than <paramref name="obj" />.
			/// </returns>
			/// <exception cref="T:System.ArgumentException">
			/// <paramref name="obj" /> is not the same type as this instance.
			/// </exception>
			public int CompareTo(object obj)
			{
				EventSignupsInfo o = (EventSignupsInfo) obj;
				string xCompare = EventName + Strings.Format(EventID, "00000000");
				string yCompare = o.EventName + Strings.Format(o.EventID, "00000000");
				if (SortExpression == SortFilter.Duration)
				{
					xCompare = System.Convert.ToString(Strings.Format(Duration, "000000") + Strings.Format(EventID, "00000000"));
					yCompare = System.Convert.ToString(Strings.Format(o.Duration, "000000") + Strings.Format(o.EventID, "00000000"));
				}
				else if (SortExpression == SortFilter.EventTimeBegin)
				{
					xCompare = System.Convert.ToString(Strings.Format(EventTimeBegin, "yyyyMMddHHmm") + Strings.Format(EventID, "00000000"));
					yCompare = System.Convert.ToString(Strings.Format(o.EventTimeBegin, "yyyyMMddHHmm") + Strings.Format(o.EventID, "00000000"));
				}
				else if (SortExpression == SortFilter.EventTimeEnd)
				{
					xCompare = System.Convert.ToString(Strings.Format(EventTimeEnd, "yyyyMMddHHmm") + Strings.Format(EventID, "00000000"));
					yCompare = System.Convert.ToString(Strings.Format(o.EventTimeEnd, "yyyyMMddHHmm") + Strings.Format(o.EventID, "00000000"));
				}
				else if (SortExpression == SortFilter.Approved)
				{
					xCompare = Approved.ToString() + Strings.Format(EventID, "00000000");
					yCompare = o.Approved.ToString() + Strings.Format(o.EventID, "00000000");
				}
				if (SortDirection == System.Web.UI.WebControls.SortDirection.Ascending)
				{
					return System.String.Compare(xCompare, yCompare, StringComparison.CurrentCulture);
				}
				else
				{
					return System.String.Compare(yCompare, xCompare, StringComparison.CurrentCulture);
				}
			}
			
#endregion
			
#region Public Methods
			
			/// <summary>
			/// Clones an instance of the eventssignups object
			/// </summary>
			/// <returns>Cloned EventsSignupsinfo object</returns>
			public EventSignupsInfo Clone()
			{
				// create the object
				return ((EventSignupsInfo) (MemberwiseClone()));
			}
#endregion
			
		}
#endregion
		
#region EventPPErrorLogInfo Class
		/// <summary>
		/// Information  about any infomartion during PayPal payments
		/// </summary>
		public class EventPpErrorLogInfo
		{
			// ReSharper disable ConvertToConstant.Local
			private int _payPalID = 0;
			// ReSharper restore ConvertToConstant.Local
			private int _signupID = 0;
			private DateTime _createDate = DateTime.Now;
			private string _payPalStatus;
			private string _payPalReason;
			private string _payPalTransID;
			private string _payPalPayerID;
			private string _payPalPayerStatus;
			private string _payPalRecieverEmail;
			private string _payPalUserEmail;
			private string _payPalPayerEmail;
			private string _payPalFirstName;
			private string _payPalLastName;
			private string _payPalAddress;
			private string _payPalCity;
			private string _payPalState;
			private string _payPalZip;
			private string _payPalCountry;
			private string _payPalCurrency;
			private DateTime _payPalPaymentDate = DateTime.Now;
			private double _payPalAmount = 0.0;
			private double _payPalFee = 0.0;
			
			// public properties
			/// <summary>
			/// Gets the PayPal ID.
			/// </summary>
			/// <value>The PayPal ID.</value>
			public int PayPalID
			{
				get
				{
					return _payPalID;
				}
			}
			
			/// <summary>
			/// Gets or sets the signup ID.
			/// </summary>
			/// <value>The signup ID.</value>
			public int SignupID
			{
				get
				{
					return _signupID;
				}
				set
				{
					_signupID = value;
				}
			}
			
			/// <summary>
			/// Gets the create date.
			/// </summary>
			/// <value>The create date.</value>
			public DateTime CreateDate
			{
				get
				{
					return _createDate;
				}
			}
			
			/// <summary>
			/// Gets or sets the PayPal status.
			/// </summary>
			/// <value>The PayPal status.</value>
			public string PayPalStatus
			{
				get
				{
					return _payPalStatus;
				}
				set
				{
					_payPalStatus = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the PayPal reason.
			/// </summary>
			/// <value>The PayPal reason.</value>
			public string PayPalReason
			{
				get
				{
					return _payPalReason;
				}
				set
				{
					_payPalReason = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the PayPal trans ID.
			/// </summary>
			/// <value>The PayPal trans ID.</value>
			public string PayPalTransID
			{
				get
				{
					return _payPalTransID;
				}
				set
				{
					_payPalTransID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the PayPal payer ID.
			/// </summary>
			/// <value>The PayPal payer ID.</value>
			public string PayPalPayerID
			{
				get
				{
					return _payPalPayerID;
				}
				set
				{
					_payPalPayerID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the PayPal payer status.
			/// </summary>
			/// <value>The PayPal payer status.</value>
			public string PayPalPayerStatus
			{
				get
				{
					return _payPalPayerStatus;
				}
				set
				{
					_payPalPayerStatus = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the PayPal reciever email.
			/// </summary>
			/// <value>The PayPal reciever email.</value>
			public string PayPalRecieverEmail
			{
				get
				{
					return _payPalRecieverEmail;
				}
				set
				{
					_payPalRecieverEmail = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the PayPal user email.
			/// </summary>
			/// <value>The PayPal user email.</value>
			public string PayPalUserEmail
			{
				get
				{
					return _payPalUserEmail;
				}
				set
				{
					_payPalUserEmail = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the PayPal payer email.
			/// </summary>
			/// <value>The PayPal payer email.</value>
			public string PayPalPayerEmail
			{
				get
				{
					return _payPalPayerEmail;
				}
				set
				{
					_payPalPayerEmail = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the first name of the PayPal.
			/// </summary>
			/// <value>The first name of the PayPal.</value>
			public string PayPalFirstName
			{
				get
				{
					return _payPalFirstName;
				}
				set
				{
					_payPalFirstName = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the last name of the PayPal.
			/// </summary>
			/// <value>The last name of the PayPal.</value>
			public string PayPalLastName
			{
				get
				{
					return _payPalLastName;
				}
				set
				{
					_payPalLastName = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the PayPal address.
			/// </summary>
			/// <value>The PayPal address.</value>
			public string PayPalAddress
			{
				get
				{
					return _payPalAddress;
				}
				set
				{
					_payPalAddress = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the PayPal city.
			/// </summary>
			/// <value>The PayPal city.</value>
			public string PayPalCity
			{
				get
				{
					return _payPalCity;
				}
				set
				{
					_payPalCity = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the state of the PayPal.
			/// </summary>
			/// <value>The state of the PayPal.</value>
			public string PayPalState
			{
				get
				{
					return _payPalState;
				}
				set
				{
					_payPalState = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the PayPal zip.
			/// </summary>
			/// <value>The PayPal zip.</value>
			public string PayPalZip
			{
				get
				{
					return _payPalZip;
				}
				set
				{
					_payPalZip = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the PayPal country.
			/// </summary>
			/// <value>The PayPal country.</value>
			public string PayPalCountry
			{
				get
				{
					return _payPalCountry;
				}
				set
				{
					_payPalCountry = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the PayPal currency.
			/// </summary>
			/// <value>The PayPal currency.</value>
			public string PayPalCurrency
			{
				get
				{
					return _payPalCurrency;
				}
				set
				{
					_payPalCurrency = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the PayPal payment date.
			/// </summary>
			/// <value>The PayPal payment date.</value>
			public DateTime PayPalPaymentDate
			{
				get
				{
					return _payPalPaymentDate;
				}
				set
				{
					_payPalPaymentDate = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the PayPal amount.
			/// </summary>
			/// <value>The PayPal amount.</value>
			public double PayPalAmount
			{
				get
				{
					return _payPalAmount;
				}
				set
				{
					_payPalAmount = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the PayPal fee.
			/// </summary>
			/// <value>The PayPal fee.</value>
			public double PayPalFee
			{
				get
				{
					return _payPalFee;
				}
				set
				{
					_payPalFee = value;
				}
			}
			
		}
#endregion
		
#region EventCategoryInfo Class
		/// <summary>
		/// Information about the (optional) category of the envent
		/// </summary>
		public class EventCategoryInfo
		{
			
			private int _portalID;
			private int _category;
			private string _categoryName;
			private string _color;
			private string _fontColor;
			
			// initialization
			/// <summary>
			/// Initializes a new instance of the <see cref="EventCategoryInfo" /> class.
			/// </summary>
			public EventCategoryInfo()
			{
			}
			
			// public properties
			/// <summary>
			/// Gets or sets the portal ID.
			/// </summary>
			/// <value>The portal ID.</value>
			public int PortalID
			{
				get
				{
					return _portalID;
				}
				set
				{
					_portalID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the category ID.
			/// </summary>
			/// <value>The category ID.</value>
			public int Category
			{
				get
				{
					return _category;
				}
				set
				{
					_category = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the name of the category.
			/// </summary>
			/// <value>The name of the category.</value>
			public string CategoryName
			{
				get
				{
					return _categoryName;
				}
				set
				{
					_categoryName = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the color.
			/// </summary>
			/// <value>The color.</value>
			public string Color
			{
				get
				{
					return _color;
				}
				set
				{
					_color = value;
				}
			}
			/// <summary>
			/// Gets or sets the color of the font.
			/// </summary>
			/// <value>The color of the font.</value>
			public string FontColor
			{
				get
				{
					return _fontColor;
				}
				set
				{
					_fontColor = value;
				}
			}
		}
#endregion
		
#region EventLocationInfo Class
		/// <summary>
		/// Information about the (optional) location of the event
		/// </summary>
		public class EventLocationInfo
		{
			
			private int _portalID;
			private int _location;
			private string _locationName;
			private string _mapURL;
			private string _street;
			private string _postalCode;
			private string _city;
			private string _region;
			private string _country;
			
			// initialization
			/// <summary>
			/// Initializes a new instance of the <see cref="EventLocationInfo" /> class.
			/// </summary>
			public EventLocationInfo()
			{
			}
			
			// public properties
			/// <summary>
			/// Gets or sets the portal ID.
			/// </summary>
			/// <value>The portal ID.</value>
			public int PortalID
			{
				get
				{
					return _portalID;
				}
				set
				{
					_portalID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the location.
			/// </summary>
			/// <value>The location.</value>
			public int Location
			{
				get
				{
					return _location;
				}
				set
				{
					_location = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the name of the location.
			/// </summary>
			/// <value>The name of the location.</value>
			public string LocationName
			{
				get
				{
					return _locationName;
				}
				set
				{
					_locationName = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the map URL.
			/// </summary>
			/// <value>The map URL.</value>
			public string MapURL
			{
				get
				{
					return _mapURL;
				}
				set
				{
					_mapURL = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the street of the location's address.
			/// </summary>
			/// <value>The street of the location's address.</value>
			public string Street
			{
				get
				{
					return _street;
				}
				set
				{
					_street = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the postal code of the location's address.
			/// </summary>
			/// <value>The postal code of the location's address.</value>
			public string PostalCode
			{
				get
				{
					return _postalCode;
				}
				set
				{
					_postalCode = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the city of the location's address.
			/// </summary>
			/// <value>The city of the location's address.</value>
			public string City
			{
				get
				{
					return _city;
				}
				set
				{
					_city = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the region of the location's address.
			/// </summary>
			/// <value>The region of the location's address.</value>
			public string Region
			{
				get
				{
					return _region;
				}
				set
				{
					_region = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the country of the location's address.
			/// </summary>
			/// <value>The country of the location's address.</value>
			public string Country
			{
				get
				{
					return _country;
				}
				set
				{
					_country = value;
				}
			}
		}
#endregion
		
#region EventNotificationInfo Class
		/// <summary>
		/// Information for emial notification of events
		/// </summary>
		public class EventNotificationInfo
		{
			
			private int _eventID;
			private int _portalAliasID;
			private int _notificationID;
			private string _userEmail;
			private bool _notificationSent = false;
			private DateTime _notifyByDateTime;
			private DateTime _eventTimeBegin;
			private string _notifyLanguage;
			private int _moduleID;
			private int _tabID;
			
			// initialization
			/// <summary>
			/// Initializes a new instance of the <see cref="EventNotificationInfo" /> class.
			/// </summary>
			public EventNotificationInfo()
			{
			}
			
			// public properties
			/// <summary>
			/// Gets or sets the event ID.
			/// </summary>
			/// <value>The event ID.</value>
			public int EventID
			{
				get
				{
					return _eventID;
				}
				set
				{
					_eventID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the portal alias ID.
			/// </summary>
			/// <value>The portal alias ID.</value>
			public int PortalAliasID
			{
				get
				{
					return _portalAliasID;
				}
				set
				{
					_portalAliasID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the notification ID.
			/// </summary>
			/// <value>The notification ID.</value>
			public int NotificationID
			{
				get
				{
					return _notificationID;
				}
				set
				{
					_notificationID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the user email.
			/// </summary>
			/// <value>The user email.</value>
			public string UserEmail
			{
				get
				{
					return _userEmail;
				}
				set
				{
					_userEmail = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether [notification sent].
			/// </summary>
			/// <value><c>true</c> if [notification sent]; otherwise, <c>false</c>.</value>
			public bool NotificationSent
			{
				get
				{
					return _notificationSent;
				}
				set
				{
					_notificationSent = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the notify by date time.
			/// </summary>
			/// <value>The notify by date time.</value>
			public DateTime NotifyByDateTime
			{
				get
				{
					return _notifyByDateTime;
				}
				set
				{
					_notifyByDateTime = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the event time begin.
			/// </summary>
			/// <value>The event time begin.</value>
			public DateTime EventTimeBegin
			{
				get
				{
					return _eventTimeBegin;
				}
				set
				{
					_eventTimeBegin = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the notify language.
			/// </summary>
			/// <value>The notify language.</value>
			public string NotifyLanguage
			{
				get
				{
					return _notifyLanguage;
				}
				set
				{
					_notifyLanguage = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the module ID.
			/// </summary>
			/// <value>The module ID.</value>
			public int ModuleID
			{
				get
				{
					return _moduleID;
				}
				set
				{
					_moduleID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the tab ID.
			/// </summary>
			/// <value>The tab ID.</value>
			public int TabID
			{
				get
				{
					return _tabID;
				}
				set
				{
					_tabID = value;
				}
			}
			
		}
#endregion
		
#region EventRecurMasterInfo Class
		/// <summary>
		/// Master record for recurring events, holds a set of events together
		/// </summary>
		public class EventRecurMasterInfo
		{
			
			/// <summary>
			/// Priority of the (master) events
			/// </summary>
			public enum Priority
			{
				/// <summary>
				/// High priority
				/// </summary>
				High = 1,
				/// <summary>
				/// Medium priority
				/// </summary>
				Medium = 2,
				/// <summary>
				/// Low priority
				/// </summary>
				Low = 3
			}
			
			private int _recurMasterID;
			private int _moduleID;
			private int _portalID;
			private string _rrule = string.Empty;
			private DateTime _dtstart;
			private string _duration = string.Empty;
			private DateTime _until;
			private string _eventName = string.Empty;
			private string _eventDesc = string.Empty;
			private int _importance;
			private string _notify = string.Empty;
			private bool _approved;
			private int _maxEnrollment;
			private bool _signups;
			private int _enrolled;
			private int _enrollRoleID = 0;
			private string _enrollType = string.Empty;
			private decimal _enrollFee;
			private string _payPalAccount = string.Empty;
			private bool _detailPage;
			private bool _detailNewWin;
			private string _detailURL = string.Empty;
			private string _imageURL = string.Empty;
			private string _imageType = string.Empty;
			private int _imageWidth;
			private int _imageHeight;
			private int _location;
			private int _category;
			private bool _imageDisplay;
			private bool _sendReminder;
			private int _reminderTime;
			private string _reminderTimeMeasurement = string.Empty;
			private string _reminder = string.Empty;
			private string _reminderFrom = string.Empty;
			private string _customField1 = string.Empty;
			private string _customField2 = string.Empty;
			private bool _enrollListView;
			private bool _displayEndDate;
			private bool _allDayEvent;
			private string _cultureName = string.Empty;
			private int _ownerID;
			private int _createdByID;
			private DateTime _createdDate;
			private int _updatedByID;
			private DateTime _updatedDate;
			private int _firstEventID;
			private string _eventTimeZoneId = string.Empty;
			private bool _allowAnonEnroll;
			private int _contentItemId;
			private bool _journalItem;
			private int _socialGroupId;
			private int _socialUserId;
			private string _summary = string.Empty;
			private int _sequence;
			
			/// <summary>
			/// Gets or sets the recur master ID.
			/// </summary>
			/// <value>The recur master ID.</value>
			public int RecurMasterID
			{
				get
				{
					return _recurMasterID;
				}
				set
				{
					_recurMasterID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the module ID.
			/// </summary>
			/// <value>The module ID.</value>
			public int ModuleID
			{
				get
				{
					return _moduleID;
				}
				set
				{
					_moduleID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the portal ID.
			/// </summary>
			/// <value>The portal ID.</value>
			public int PortalID
			{
				get
				{
					return _portalID;
				}
				set
				{
					_portalID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the Recurrence rule.
			/// </summary>
			/// <value>The recurrence rule.</value>
			public string RRULE
			{
				get
				{
					return _rrule;
				}
				set
				{
					_rrule = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the Date start.
			/// </summary>
			/// <value>The start date.</value>
			public DateTime Dtstart
			{
				get
				{
					return _dtstart;
				}
				set
				{
					_dtstart = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the duration.
			/// </summary>
			/// <value>The duration.</value>
			public string Duration
			{
				get
				{
					return _duration;
				}
				set
				{
					_duration = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the until.
			/// </summary>
			/// <value>The until.</value>
			public DateTime Until
			{
				get
				{
					return _until;
				}
				set
				{
					_until = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the name of the event.
			/// </summary>
			/// <value>The name of the event.</value>
			public string EventName
			{
				get
				{
					return _eventName;
				}
				set
				{
					_eventName = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the event description
			/// </summary>
			/// <value>The event description.</value>
			public string EventDesc
			{
				get
				{
					return _eventDesc;
				}
				set
				{
					_eventDesc = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the importance.
			/// </summary>
			/// <value>The importance.</value>
			public Priority Importance
			{
				get
				{
					return ((Priority) _importance);
				}
				set
				{
					_importance = (int) value;
				}
			}
			
			/// <summary>
			/// Gets or sets the notify.
			/// </summary>
			/// <value>The notify.</value>
			public string Notify
			{
				get
				{
					return _notify;
				}
				set
				{
					_notify = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="EventRecurMasterInfo" /> is approved.
			/// </summary>
			/// <value><c>true</c> if approved; otherwise, <c>false</c>.</value>
			public bool Approved
			{
				get
				{
					return _approved;
				}
				set
				{
					_approved = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="EventRecurMasterInfo" /> is signups.
			/// </summary>
			/// <value><c>true</c> if signups; otherwise, <c>false</c>.</value>
			public bool Signups
			{
				get
				{
					return _signups;
				}
				set
				{
					_signups = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the max # of enrollment.
			/// </summary>
			/// <value>The max # of enrollment.</value>
			public int MaxEnrollment
			{
				get
				{
					return _maxEnrollment;
				}
				set
				{
					_maxEnrollment = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the # enrolled.
			/// </summary>
			/// <value>The #enrolled.</value>
			public int Enrolled
			{
				get
				{
					if (_enrolled < 0)
					{
						return 0;
					}
					return _enrolled;
				}
				set
				{
					_enrolled = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the enroll role ID.
			/// </summary>
			/// <value>The enroll role ID.</value>
			public int EnrollRoleID
			{
				get
				{
					return _enrollRoleID;
				}
				set
				{
					_enrollRoleID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the type of the enroll.
			/// </summary>
			/// <value>The type of the enroll.</value>
			public string EnrollType
			{
				get
				{
					return _enrollType;
				}
				set
				{
					_enrollType = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the enroll fee.
			/// </summary>
			/// <value>The enroll fee.</value>
			public decimal EnrollFee
			{
				get
				{
					return _enrollFee;
				}
				set
				{
					_enrollFee = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the pay pal account.
			/// </summary>
			/// <value>The pay pal account.</value>
			public string PayPalAccount
			{
				get
				{
					return _payPalAccount;
				}
				set
				{
					_payPalAccount = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether to display on a detail page.
			/// </summary>
			/// <value><c>true</c> if [detail page]; otherwise, <c>false</c>.</value>
			public bool DetailPage
			{
				get
				{
					return _detailPage;
				}
				set
				{
					_detailPage = value;
				}
			}
			/// <summary>
			/// Gets or sets a value indicating whether to dispay the event in a new page.
			/// </summary>
			/// <value><c>true</c> if [detail new page]; otherwise, <c>false</c>.</value>
			public bool DetailNewWin
			{
				get
				{
					return _detailNewWin;
				}
				set
				{
					_detailNewWin = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the detail URL.
			/// </summary>
			/// <value>The detail URL.</value>
			public string DetailURL
			{
				get
				{
					return _detailURL;
				}
				set
				{
					_detailURL = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the image URL.
			/// </summary>
			/// <value>The image URL.</value>
			public string ImageURL
			{
				get
				{
					return _imageURL;
				}
				set
				{
					_imageURL = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the type of the image.
			/// </summary>
			/// <value>The type of the image.</value>
			public string ImageType
			{
				get
				{
					return _imageType;
				}
				set
				{
					_imageType = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the width of the image.
			/// </summary>
			/// <value>The width of the image.</value>
			public int ImageWidth
			{
				get
				{
					return _imageWidth;
				}
				set
				{
					_imageWidth = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the height of the image.
			/// </summary>
			/// <value>The height of the image.</value>
			public int ImageHeight
			{
				get
				{
					return _imageHeight;
				}
				set
				{
					_imageHeight = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the location.
			/// </summary>
			/// <value>The location.</value>
			public int Location
			{
				get
				{
					return _location;
				}
				set
				{
					_location = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the category.
			/// </summary>
			/// <value>The category.</value>
			public int Category
			{
				get
				{
					return _category;
				}
				set
				{
					_category = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether [image display].
			/// </summary>
			/// <value><c>true</c> if [image display]; otherwise, <c>false</c>.</value>
			public bool ImageDisplay
			{
				get
				{
					return _imageDisplay;
				}
				set
				{
					_imageDisplay = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether [send reminder].
			/// </summary>
			/// <value><c>true</c> if [send reminder]; otherwise, <c>false</c>.</value>
			public bool SendReminder
			{
				get
				{
					return _sendReminder;
				}
				set
				{
					_sendReminder = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the reminder time.
			/// </summary>
			/// <value>The reminder time.</value>
			public int ReminderTime
			{
				get
				{
					return _reminderTime;
				}
				set
				{
					_reminderTime = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the reminder time measurement.
			/// </summary>
			/// <value>The reminder time measurement.</value>
			public string ReminderTimeMeasurement
			{
				get
				{
					return _reminderTimeMeasurement;
				}
				set
				{
					_reminderTimeMeasurement = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the reminder.
			/// </summary>
			/// <value>The reminder.</value>
			public string Reminder
			{
				get
				{
					return _reminder;
				}
				set
				{
					_reminder = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the reminder from.
			/// </summary>
			/// <value>The reminder from.</value>
			public string ReminderFrom
			{
				get
				{
					return _reminderFrom;
				}
				set
				{
					_reminderFrom = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the custom field1.
			/// </summary>
			/// <value>The custom field1.</value>
			public string CustomField1
			{
				get
				{
					return _customField1;
				}
				set
				{
					_customField1 = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the custom field2.
			/// </summary>
			/// <value>The custom field2.</value>
			public string CustomField2
			{
				get
				{
					return _customField2;
				}
				set
				{
					_customField2 = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether [enroll list view].
			/// </summary>
			/// <value><c>true</c> if [enroll list view]; otherwise, <c>false</c>.</value>
			public bool EnrollListView
			{
				get
				{
					return _enrollListView;
				}
				set
				{
					_enrollListView = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether [display end date].
			/// </summary>
			/// <value><c>true</c> if [display end date]; otherwise, <c>false</c>.</value>
			public bool DisplayEndDate
			{
				get
				{
					return _displayEndDate;
				}
				set
				{
					_displayEndDate = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether [all day event].
			/// </summary>
			/// <value><c>true</c> if [all day event]; otherwise, <c>false</c>.</value>
			public bool AllDayEvent
			{
				get
				{
					return _allDayEvent;
				}
				set
				{
					_allDayEvent = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the name of the culture.
			/// </summary>
			/// <value>The name of the culture.</value>
			public string CultureName
			{
				get
				{
					return _cultureName;
				}
				set
				{
					_cultureName = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the owner ID.
			/// </summary>
			/// <value>The owner ID.</value>
			public int OwnerID
			{
				get
				{
					return _ownerID;
				}
				set
				{
					_ownerID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the created by ID.
			/// </summary>
			/// <value>The created by ID.</value>
			public int CreatedByID
			{
				get
				{
					return _createdByID;
				}
				set
				{
					_createdByID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the created date.
			/// </summary>
			/// <value>The created date.</value>
			public DateTime CreatedDate
			{
				get
				{
					return _createdDate;
				}
				set
				{
					_createdDate = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the updated by ID.
			/// </summary>
			/// <value>The updated by ID.</value>
			public int UpdatedByID
			{
				get
				{
					return _updatedByID;
				}
				set
				{
					_updatedByID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the updated date.
			/// </summary>
			/// <value>The updated date.</value>
			public DateTime UpdatedDate
			{
				get
				{
					return _updatedDate;
				}
				set
				{
					_updatedDate = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the first event ID.
			/// </summary>
			/// <value>The first event ID.</value>
			public int FirstEventID
			{
				get
				{
					return _firstEventID;
				}
				set
				{
					_firstEventID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating what the [event timezoneid] is
			/// </summary>
			/// <value>The event timezoneid.</value>
			public string EventTimeZoneId
			{
				get
				{
					if (string.IsNullOrEmpty(_eventTimeZoneId))
					{
						EventModuleSettings modSettings = EventModuleSettings.GetEventModuleSettings(_moduleID, null);
						_eventTimeZoneId = modSettings.TimeZoneId;
					}
					return _eventTimeZoneId;
				}
				set
				{
					_eventTimeZoneId = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether [allow anonymous enrollment].
			/// </summary>
			/// <value><c>true</c> if [allow anonymous enrollment]; otherwise, <c>false</c>.</value>
			public bool AllowAnonEnroll
			{
				get
				{
					return _allowAnonEnroll;
				}
				set
				{
					_allowAnonEnroll = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating the [content item id].
			/// </summary>
			/// <value>The contentitemid.</value>
			public int ContentItemID
			{
				get
				{
					return _contentItemId;
				}
				set
				{
					_contentItemId = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether [event has journal item].
			/// </summary>
			/// <value><c>true</c> if [event has journal item]; otherwise, <c>false</c>.</value>
			public bool JournalItem
			{
				get
				{
					return _journalItem;
				}
				set
				{
					_journalItem = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating the [socialgroup id].
			/// </summary>
			/// <value>The SocialGroupid.</value>
			public int SocialGroupID
			{
				get
				{
					return _socialGroupId;
				}
				set
				{
					_socialGroupId = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating the [social user id].
			/// </summary>
			/// <value>The SocialUserid.</value>
			public int SocialUserID
			{
				get
				{
					return _socialUserId;
				}
				set
				{
					_socialUserId = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the summary.
			/// </summary>
			/// <value>The summary.</value>
			public string Summary
			{
				get
				{
					return _summary;
				}
				set
				{
					_summary = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating the [sequence].
			/// </summary>
			/// <value>The Sequence.</value>
			public int Sequence
			{
				get
				{
					return _sequence;
				}
				set
				{
					_sequence = value;
				}
			}
			
		}
#endregion
		
#region EventRRULEInfo Class
		/// <summary>
		/// Information about the recurrrence rules
		/// </summary>
		public class EventRRULEInfo
		{
			
			private string _freq;
			private int _interval;
			private string _byDay;
			private int _byMonthDay;
			private int _byMonth;
			private bool _su;
			private bool _mo;
			private bool _tu;
			private bool _we;
			private bool _th;
			private bool _fr;
			private bool _sa;
			private int _suNo;
			private int _moNo;
			private int _tuNo;
			private int _weNo;
			private int _thNo;
			private int _frNo;
			private int _saNo;
			private bool _freqBasic;
			private string _wkst;
			
			/// <summary>
			/// Gets or sets the frequency
			/// </summary>
			/// <value>The frequency.</value>
			public string Freq
			{
				get
				{
					return _freq;
				}
				set
				{
					_freq = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the interval.
			/// </summary>
			/// <value>The interval.</value>
			public int Interval
			{
				get
				{
					return _interval;
				}
				set
				{
					_interval = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the by day.
			/// </summary>
			/// <value>The by day.</value>
			public string ByDay
			{
				get
				{
					return _byDay;
				}
				set
				{
					_byDay = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the by day of the month.
			/// </summary>
			/// <value>The by day of the month.</value>
			public int ByMonthDay
			{
				get
				{
					return _byMonthDay;
				}
				set
				{
					_byMonthDay = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the by month.
			/// </summary>
			/// <value>The by month.</value>
			public int ByMonth
			{
				get
				{
					return _byMonth;
				}
				set
				{
					_byMonth = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="EventRRULEInfo" /> is sunday
			/// </summary>
			/// <value><c>true</c> if sunday; otherwise, <c>false</c>.</value>
			public bool Su
			{
				get
				{
					return _su;
				}
				set
				{
					_su = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the no sunday
			/// </summary>
			/// <value>The no sunday</value>
			public int SuNo
			{
				get
				{
					return _suNo;
				}
				set
				{
					_suNo = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="EventRRULEInfo" /> is monday
			/// </summary>
			/// <value><c>true</c> if monday; otherwise, <c>false</c>.</value>
			public bool Mo
			{
				get
				{
					return _mo;
				}
				set
				{
					_mo = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the no monday
			/// </summary>
			/// <value>The no monday</value>
			public int MoNo
			{
				get
				{
					return _moNo;
				}
				set
				{
					_moNo = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="EventRRULEInfo" /> is tuesday
			/// </summary>
			/// <value><c>true</c> if tuesday; otherwise, <c>false</c>.</value>
			public bool Tu
			{
				get
				{
					return _tu;
				}
				set
				{
					_tu = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the no tuesday
			/// </summary>
			/// <value>The no tuesday.</value>
			public int TuNo
			{
				get
				{
					return _tuNo;
				}
				set
				{
					_tuNo = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="EventRRULEInfo" /> is wednessday
			/// </summary>
			/// <value><c>true</c> if wednessday; otherwise, <c>false</c>.</value>
			public bool We
			{
				get
				{
					return _we;
				}
				set
				{
					_we = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the no wednessday.
			/// </summary>
			/// <value>The no wednessday.</value>
			public int WeNo
			{
				get
				{
					return _weNo;
				}
				set
				{
					_weNo = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="EventRRULEInfo" /> is thursday.
			/// </summary>
			/// <value><c>true</c> if thursday; otherwise, <c>false</c>.</value>
			public bool Th
			{
				get
				{
					return _th;
				}
				set
				{
					_th = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the no thursday
			/// </summary>
			/// <value>The no thursday.</value>
			public int ThNo
			{
				get
				{
					return _thNo;
				}
				set
				{
					_thNo = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="EventRRULEInfo" /> is friday.
			/// </summary>
			/// <value><c>true</c> if friday; otherwise, <c>false</c>.</value>
			public bool Fr
			{
				get
				{
					return _fr;
				}
				set
				{
					_fr = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the no friday.
			/// </summary>
			/// <value>The no friday.</value>
			public int FrNo
			{
				get
				{
					return _frNo;
				}
				set
				{
					_frNo = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="EventRRULEInfo" /> is saturday.
			/// </summary>
			/// <value><c>true</c> if saturday; otherwise, <c>false</c>.</value>
			public bool Sa
			{
				get
				{
					return _sa;
				}
				set
				{
					_sa = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the no saturday.
			/// </summary>
			/// <value>The no saturday.</value>
			public int SaNo
			{
				get
				{
					return _saNo;
				}
				set
				{
					_saNo = value;
				}
			}
			
			public bool FreqBasic
			{
				get
				{
					return _freqBasic;
				}
				set
				{
					_freqBasic = value;
				}
			}
			
			public string Wkst
			{
				get
				{
					return _wkst;
				}
				set
				{
					_wkst = value;
				}
			}
			
		}
#endregion
		
#region EventEmailInfo Class
		/// <summary>
		/// Information abotu e-mails related to events
		/// </summary>
		public class EventEmailInfo
		{
			private string _txtEmailSubject;
			private string _txtEmailBody;
			private string _txtEmailFrom;
			private ArrayList _userIDs;
			private ArrayList _userEmails;
			private ArrayList _userLocales;
			private ArrayList _userTimeZoneIds;
			
			/// <summary>
			/// Gets or sets the  email subject.
			/// </summary>
			/// <value>The  email subject.</value>
			public string TxtEmailSubject
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
			
			/// <summary>
			/// Gets or sets the  email body.
			/// </summary>
			/// <value>The email body.</value>
			public string TxtEmailBody
			{
				get
				{
					return _txtEmailBody;
				}
				set
				{
					_txtEmailBody = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the email from.
			/// </summary>
			/// <value>The email from.</value>
			public string TxtEmailFrom
			{
				get
				{
					return _txtEmailFrom;
				}
				set
				{
					_txtEmailFrom = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the user I ds.
			/// </summary>
			/// <value>The user I ds.</value>
			public ArrayList UserIDs
			{
				get
				{
					return _userIDs;
				}
				set
				{
					_userIDs = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the user emails.
			/// </summary>
			/// <value>The user emails.</value>
			public ArrayList UserEmails
			{
				get
				{
					return _userEmails;
				}
				set
				{
					_userEmails = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the user locales.
			/// </summary>
			/// <value>The user locales.</value>
			public ArrayList UserLocales
			{
				get
				{
					return _userLocales;
				}
				set
				{
					_userLocales = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the user timezoneids.
			/// </summary>
			/// <value>The user timezoneids.</value>
			public ArrayList UserTimeZoneIds
			{
				get
				{
					return _userTimeZoneIds;
				}
				set
				{
					_userTimeZoneIds = value;
				}
			}
			
			/// <summary>
			/// Initializes a new instance of the <see cref="EventEmailInfo" /> class.
			/// </summary>
			public EventEmailInfo()
			{
				ArrayList newUserEmails = new ArrayList();
				UserEmails = newUserEmails;
				
				ArrayList newUserIDs = new ArrayList();
				UserIDs = newUserIDs;
				
				ArrayList newUserLocales = new ArrayList();
				UserLocales = newUserLocales;
				
				ArrayList newUserTimeZoneIds = new ArrayList();
				UserTimeZoneIds = newUserTimeZoneIds;
				
			}
		}
#endregion
		
#region EventSubscriptionInfo Class
		/// <summary>
		/// Information about subscription o events
		/// </summary>
		public class EventSubscriptionInfo
		{
			
			private int _portalID;
			private int _moduleID;
			private int _userID;
			private int _subscriptionID;
			
			// initialization
			/// <summary>
			/// Initializes a new instance of the <see cref="EventSubscriptionInfo" /> class.
			/// </summary>
			public EventSubscriptionInfo()
			{
			}
			
			// public properties
			/// <summary>
			/// Gets or sets the portal ID.
			/// </summary>
			/// <value>The portal ID.</value>
			public int PortalID
			{
				get
				{
					return _portalID;
				}
				set
				{
					_portalID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the subscription ID.
			/// </summary>
			/// <value>The subscription ID.</value>
			public int SubscriptionID
			{
				get
				{
					return _subscriptionID;
				}
				set
				{
					_subscriptionID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the module ID.
			/// </summary>
			/// <value>The module ID.</value>
			public int ModuleID
			{
				get
				{
					return _moduleID;
				}
				set
				{
					_moduleID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the user ID.
			/// </summary>
			/// <value>The user ID.</value>
			public int UserID
			{
				get
				{
					return _userID;
				}
				set
				{
					_userID = value;
				}
			}
			
		}
#endregion
		
#region EventListObject Class
		/// <summary>
		/// Object for listing the events
		/// </summary>
		public class EventListObject : IComparable
		{
			
			// public properties
			private int _indexId;
			/// <summary>
			/// Gets or sets the index id.
			/// </summary>
			/// <value>The index id.</value>
			public int IndexId
			{
				get
				{
					return _indexId;
				}
				set
				{
					_indexId = value;
				}
			}
			private int _eventID;
			/// <summary>
			/// Gets or sets the event ID.
			/// </summary>
			/// <value>The event ID.</value>
			public int EventID
			{
				get
				{
					return _eventID;
				}
				set
				{
					_eventID = value;
				}
			}
			
			private int _createdByID;
			/// <summary>
			/// Gets or sets the created by ID.
			/// </summary>
			/// <value>The created by ID.</value>
			public int CreatedByID
			{
				get
				{
					return _createdByID;
				}
				set
				{
					_createdByID = value;
				}
			}
			
			private int _ownerID;
			/// <summary>
			/// Gets or sets the owner ID.
			/// </summary>
			/// <value>The owner ID.</value>
			public int OwnerID
			{
				get
				{
					return _ownerID;
				}
				set
				{
					_ownerID = value;
				}
			}
			
			private int _moduleID;
			/// <summary>
			/// Gets or sets the module ID.
			/// </summary>
			/// <value>The module ID.</value>
			public int ModuleID
			{
				get
				{
					return _moduleID;
				}
				set
				{
					_moduleID = value;
				}
			}
			
			private DateTime _eventDateBegin;
			/// <summary>
			/// Gets or sets the event date begin.
			/// </summary>
			/// <value>The event date begin.</value>
			public DateTime EventDateBegin
			{
				get
				{
					return _eventDateBegin;
				}
				set
				{
					_eventDateBegin = value;
				}
			}
			
			private DateTime _eventDateEnd;
			/// <summary>
			/// Gets or sets the event date end.
			/// </summary>
			/// <value>The event date end.</value>
			public DateTime EventDateEnd
			{
				get
				{
					return _eventDateEnd;
				}
				set
				{
					_eventDateEnd = value;
				}
			}
			
			private string _txtEventDateEnd;
			/// <summary>
			/// Gets or sets the event date end.
			/// </summary>
			/// <value>The event date end.</value>
			public string TxtEventDateEnd
			{
				get
				{
					return _txtEventDateEnd;
				}
				set
				{
					_txtEventDateEnd = value;
				}
			}
			
			private DateTime _eventTimeBegin;
			/// <summary>
			/// Gets or sets the event time begin.
			/// </summary>
			/// <value>The event time begin.</value>
			public DateTime EventTimeBegin
			{
				get
				{
					return _eventTimeBegin;
				}
				set
				{
					_eventTimeBegin = value;
				}
			}
			
			private string _txtEventTimeBegin;
			/// <summary>
			/// Gets or sets the event time begin.
			/// </summary>
			/// <value>The event time begin.</value>
			public string TxtEventTimeBegin
			{
				get
				{
					return _txtEventTimeBegin;
				}
				set
				{
					_txtEventTimeBegin = value;
				}
			}
			
			private string _recurUntil;
			/// <summary>
			/// Gets or sets the recurrence until.
			/// </summary>
			/// <value>The recurrence until.</value>
			public string RecurUntil
			{
				get
				{
					return _recurUntil;
				}
				set
				{
					_recurUntil = value;
				}
			}
			
			private int _duration;
			/// <summary>
			/// Gets or sets the duration.
			/// </summary>
			/// <value>The duration.</value>
			public int Duration
			{
				get
				{
					return _duration;
				}
				set
				{
					_duration = value;
				}
			}
			
			private string _eventName;
			/// <summary>
			/// Gets or sets the name of the event.
			/// </summary>
			/// <value>The name of the event.</value>
			public string EventName
			{
				get
				{
					return _eventName;
				}
				set
				{
					_eventName = value;
				}
			}
			
			private string _eventDesc;
			/// <summary>
			/// Gets or sets the event description
			/// </summary>
			/// <value>The event description.</value>
			public string EventDesc
			{
				get
				{
					return _eventDesc;
				}
				set
				{
					_eventDesc = value;
				}
			}
			
			private string _decodedDesc;
			/// <summary>
			/// Gets or sets the decoded description.
			/// </summary>
			/// <value>The decoded description.</value>
			public string DecodedDesc
			{
				get
				{
					return _decodedDesc;
				}
				set
				{
					_decodedDesc = value;
				}
			}
			
			private string _recurText;
			/// <summary>
			/// Gets or sets the recurrence text.
			/// </summary>
			/// <value>The recurrence text.</value>
			public string RecurText
			{
				get
				{
					return _recurText;
				}
				set
				{
					_recurText = value;
				}
			}
			
			private string _url;
			/// <summary>
			/// Gets or sets the URL.
			/// </summary>
			/// <value>The URL.</value>
			public string URL
			{
				get
				{
					return _url;
				}
				set
				{
					_url = value;
				}
			}
			
			private string _target;
			/// <summary>
			/// Gets or sets the target.
			/// </summary>
			/// <value>The target.</value>
			public string Target
			{
				get
				{
					return _target;
				}
				set
				{
					_target = value;
				}
			}
			
			private string _imageURL;
			/// <summary>
			/// Gets or sets the image URL.
			/// </summary>
			/// <value>The image URL.</value>
			public string ImageURL
			{
				get
				{
					return _imageURL;
				}
				set
				{
					_imageURL = value;
				}
			}
			
			
			private string _categoryName;
			/// <summary>
			/// Gets or sets the name of the category.
			/// </summary>
			/// <value>The name of the category.</value>
			public string CategoryName
			{
				get
				{
					return _categoryName;
				}
				set
				{
					_categoryName = value;
				}
			}
			
			private string _locationName;
			/// <summary>
			/// Gets or sets the name of the location.
			/// </summary>
			/// <value>The name of the location.</value>
			public string LocationName
			{
				get
				{
					return _locationName;
				}
				set
				{
					_locationName = value;
				}
			}
			
			private string _customField1;
			/// <summary>
			/// Gets or sets the custom field1.
			/// </summary>
			/// <value>The custom field1.</value>
			public string CustomField1
			{
				get
				{
					return _customField1;
				}
				set
				{
					_customField1 = value;
				}
			}
			
			private string _customField2;
			/// <summary>
			/// Gets or sets the custom field2.
			/// </summary>
			/// <value>The custom field2.</value>
			public string CustomField2
			{
				get
				{
					return _customField2;
				}
				set
				{
					_customField2 = value;
				}
			}
			
			private bool _editVisibility;
			/// <summary>
			/// Gets or sets a value indicating whether visibility is editable.
			/// </summary>
			/// <value><c>true</c> if [edit visibility]; otherwise, <c>false</c>.</value>
			public bool EditVisibility
			{
				get
				{
					return _editVisibility;
				}
				set
				{
					_editVisibility = value;
				}
			}
			
			private Color _categoryColor;
			/// <summary>
			/// Gets or sets the color of the category.
			/// </summary>
			/// <value>The color of the category.</value>
			public Color CategoryColor
			{
				get
				{
					return _categoryColor;
				}
				set
				{
					_categoryColor = value;
				}
			}
			
			private Color _categoryFontColor;
			/// <summary>
			/// Gets or sets the color of the category font.
			/// </summary>
			/// <value>The color of the category font.</value>
			public Color CategoryFontColor
			{
				get
				{
					return _categoryFontColor;
				}
				set
				{
					_categoryFontColor = value;
				}
			}
			
			private int _displayDuration;
			/// <summary>
			/// Gets or sets the display duration.
			/// </summary>
			/// <value>The display duration.</value>
			public int DisplayDuration
			{
				get
				{
					return _displayDuration;
				}
				set
				{
					_displayDuration = value;
				}
			}
			
			private int _recurMasterID;
			/// <summary>
			/// Gets or sets the recur master ID.
			/// </summary>
			/// <value>The recur master ID.</value>
			public int RecurMasterID
			{
				get
				{
					return _recurMasterID;
				}
				set
				{
					_recurMasterID = value;
				}
			}
			
			private string _icons;
			/// <summary>
			/// Gets or sets the icons.
			/// </summary>
			/// <value>The icons.</value>
			public string Icons
			{
				get
				{
					return _icons;
				}
				set
				{
					_icons = value;
				}
			}
			
			private string _tooltip;
			/// <summary>
			/// Gets or sets the tooltip.
			/// </summary>
			/// <value>The tooltip.</value>
			public string Tooltip
			{
				get
				{
					return _tooltip;
				}
				set
				{
					_tooltip = value;
				}
			}
			
			private static SortFilter _sortExpression;
			/// <summary>
			/// Gets or sets the sort expression.
			/// </summary>
			/// <value>The sort expression.</value>
			public static SortFilter SortExpression
			{
				get
				{
					return _sortExpression;
				}
				set
				{
					_sortExpression = value;
				}
			}
			
			private static SortDirection _sortDirection;
			/// <summary>
			/// Gets or sets the sort direction.
			/// </summary>
			/// <value>The sort direction.</value>
			public static SortDirection SortDirection
			{
				get
				{
					return _sortDirection;
				}
				set
				{
					_sortDirection = value;
				}
			}
			
			
			/// <summary>
			/// Sorting enumeration
			/// </summary>
			public enum SortFilter
			{
				/// <summary>
				/// By EventID
				/// </summary>
				EventID,
				/// <summary>
				/// By Date beging
				/// </summary>
				EventDateBegin,
				/// <summary>
				/// By Date end
				/// </summary>
				EventDateEnd,
				/// <summary>
				/// Bu Name
				/// </summary>
				EventName,
				/// <summary>
				/// By duration
				/// </summary>
				Duration,
				/// <summary>
				/// Bu category name
				/// </summary>
				CategoryName,
				/// <summary>
				/// By customfield1
				/// </summary>
				CustomField1,
				/// <summary>
				/// By customfield2
				/// </summary>
				CustomField2,
				/// <summary>
				/// By description
				/// </summary>
				Description,
				/// <summary>
				/// By Location name
				/// </summary>
				LocationName
			}
			
			/// <summary>
			/// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
			/// </summary>
			/// <param name="obj">An object to compare with this instance.</param>
			/// <returns>
			/// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings:
			/// Value
			/// Meaning
			/// Less than zero
			/// This instance is less than <paramref name="obj" />.
			/// Zero
			/// This instance is equal to <paramref name="obj" />.
			/// Greater than zero
			/// This instance is greater than <paramref name="obj" />.
			/// </returns>
			/// <exception cref="T:System.ArgumentException">
			/// <paramref name="obj" /> is not the same type as this instance.
			/// </exception>
			public int CompareTo(object obj)
			{
				EventListObject o = (EventListObject) obj;
				string xCompare = EventName + Strings.Format(EventID, "00000000");
				string yCompare = o.EventName + Strings.Format(o.EventID, "00000000");
				if (SortExpression == SortFilter.CategoryName)
				{
					xCompare = CategoryName + Strings.Format(EventID, "00000000");
					yCompare = o.CategoryName + Strings.Format(o.EventID, "00000000");
				}
				else if (SortExpression == SortFilter.CustomField1)
				{
					xCompare = CustomField1 + Strings.Format(EventID, "00000000");
					yCompare = o.CustomField1 + Strings.Format(o.EventID, "00000000");
				}
				else if (SortExpression == SortFilter.CustomField2)
				{
					xCompare = CustomField2 + Strings.Format(EventID, "00000000");
					yCompare = o.CustomField2 + Strings.Format(o.EventID, "00000000");
				}
				else if (SortExpression == SortFilter.Description)
				{
					xCompare = EventDesc + Strings.Format(EventID, "00000000");
					yCompare = o.EventDesc + Strings.Format(o.EventID, "00000000");
				}
				else if (SortExpression == SortFilter.Duration)
				{
					xCompare = System.Convert.ToString(Strings.Format(Duration, "000000") + Strings.Format(EventID, "00000000"));
					yCompare = System.Convert.ToString(Strings.Format(o.Duration, "000000") + Strings.Format(o.EventID, "00000000"));
				}
				else if (SortExpression == SortFilter.EventDateBegin)
				{
					xCompare = System.Convert.ToString(Strings.Format(EventDateBegin, "yyyyMMddHHmm") + Strings.Format(EventID, "00000000"));
					yCompare = System.Convert.ToString(Strings.Format(o.EventDateBegin, "yyyyMMddHHmm") + Strings.Format(o.EventID, "00000000"));
				}
				else if (SortExpression == SortFilter.EventDateEnd)
				{
					xCompare = System.Convert.ToString(Strings.Format(EventDateEnd, "yyyyMMddHHmm") + Strings.Format(EventID, "00000000"));
					yCompare = System.Convert.ToString(Strings.Format(o.EventDateEnd, "yyyyMMddHHmm") + Strings.Format(o.EventID, "00000000"));
				}
				else if (SortExpression == SortFilter.LocationName)
				{
					xCompare = LocationName + Strings.Format(EventID, "00000000");
					yCompare = o.LocationName + Strings.Format(o.EventID, "00000000");
				}
				else if (SortExpression == SortFilter.EventID)
				{
					xCompare = Strings.Format(EventID, "00000000");
					yCompare = Strings.Format(o.EventID, "00000000");
				}
				if (SortDirection == System.Web.UI.WebControls.SortDirection.Ascending)
				{
					return System.String.Compare(xCompare, yCompare, StringComparison.CurrentCulture);
				}
				else
				{
					return System.String.Compare(yCompare, xCompare, StringComparison.CurrentCulture);
				}
			}
		}
#endregion
#region EventUser Class
		/// <summary>
		/// user related to an event
		/// </summary>
		public class EventUser
		{
			private int _userID;
			private string _displayName;
			private string _profileURL;
			private string _displayNameURL;
			
			/// <summary>
			/// Gets or sets the user ID.
			/// </summary>
			/// <value>The user ID.</value>
			public int UserID
			{
				get
				{
					return _userID;
				}
				set
				{
					_userID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the display name.
			/// </summary>
			/// <value>The display name.</value>
			public string DisplayName
			{
				get
				{
					return _displayName;
				}
				set
				{
					_displayName = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the profile URL.
			/// </summary>
			/// <value>The profile URL.</value>
			public string ProfileURL
			{
				get
				{
					return _profileURL;
				}
				set
				{
					_profileURL = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the display name URL.
			/// </summary>
			/// <value>The display name URL.</value>
			public string DisplayNameURL
			{
				get
				{
					return _displayNameURL;
				}
				set
				{
					_displayNameURL = value;
				}
			}
			
		}
#endregion
		
#region EventEnrollList Class
		/// <summary>
		/// List of users enrolled into an event
		/// </summary>
		public class EventEnrollList
		{
			private int _signupID;
			private string _enrollUserName;
			private string _enrollDisplayName;
			private string _enrollEmail;
			private string _enrollPhone;
			private bool _enrollApproved;
			private int _enrollNo;
			private DateTime _enrollTimeBegin;
			
			/// <summary>
			/// Gets or sets the signup ID.
			/// </summary>
			/// <value>The signup ID.</value>
			public int SignupID
			{
				get
				{
					return _signupID;
				}
				set
				{
					_signupID = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the name of the enroll user.
			/// </summary>
			/// <value>The name of the enroll user.</value>
			public string EnrollUserName
			{
				get
				{
					return _enrollUserName;
				}
				set
				{
					_enrollUserName = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the display name of the enroll.
			/// </summary>
			/// <value>The display name of the enroll.</value>
			public string EnrollDisplayName
			{
				get
				{
					return _enrollDisplayName;
				}
				set
				{
					_enrollDisplayName = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the enroll email.
			/// </summary>
			/// <value>The enroll email.</value>
			public string EnrollEmail
			{
				get
				{
					return _enrollEmail;
				}
				set
				{
					_enrollEmail = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the enroll phone.
			/// </summary>
			/// <value>The enroll phone.</value>
			public string EnrollPhone
			{
				get
				{
					return _enrollPhone;
				}
				set
				{
					_enrollPhone = value;
				}
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether [enroll approved].
			/// </summary>
			/// <value><c>true</c> if [enroll approved]; otherwise, <c>false</c>.</value>
			public bool EnrollApproved
			{
				get
				{
					return _enrollApproved;
				}
				set
				{
					_enrollApproved = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the enroll no.
			/// </summary>
			/// <value>The enroll no.</value>
			public int EnrollNo
			{
				get
				{
					return _enrollNo;
				}
				set
				{
					_enrollNo = value;
				}
			}
			
			/// <summary>
			/// Gets or sets the enroll time begin.
			/// </summary>
			/// <value>The enroll time begin.</value>
			public DateTime EnrollTimeBegin
			{
				get
				{
					return _enrollTimeBegin;
				}
				set
				{
					_enrollTimeBegin = value;
				}
			}
		}
#endregion
		
	}
