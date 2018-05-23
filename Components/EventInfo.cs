

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
	    using System.Drawing;
	    using System.Web.UI.WebControls;
	    using DotNetNuke.Modules.Events;
	    using Microsoft.VisualBasic;

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
					return this._portalID;
				}
				set
				{
					this._portalID = value;
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
					return this._eventID;
				}
				set
				{
					this._eventID = value;
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
					return this._recurMasterID;
				}
				set
				{
					this._recurMasterID = value;
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
					return this._moduleID;
				}
				set
				{
					this._moduleID = value;
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
					return this._eventDateEnd;
				}
				set
				{
					this._eventDateEnd = value;
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
					return this._eventTimeBegin;
				}
				set
				{
					this._eventTimeBegin = value;
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
					return this.EventTimeBegin.AddMinutes(this.Duration);
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
					return this._duration;
				}
				set
				{
					this._duration = value;
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
					return this._eventName;
				}
				set
				{
					this._eventName = value;
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
					return this._eventDesc;
				}
				set
				{
					this._eventDesc = value;
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
					return ((Priority) this._importance);
				}
				set
				{
					this._importance = (int) value;
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
					return this._createdDate;
				}
				set
				{
					this._createdDate = value;
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
					return this._createdBy;
				}
				set
				{
					this._createdBy = value;
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
					return this._createdByID;
				}
				set
				{
					this._createdByID = value;
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
					return this._every;
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
					return this._period;
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
					return this._repeatType;
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
					return this._notify;
				}
				set
				{
					this._notify = value;
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
					return this._approved;
				}
				set
				{
					this._approved = value;
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
					return this._signups;
				}
				set
				{
					this._signups = value;
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
					return this._maxEnrollment;
				}
				set
				{
					this._maxEnrollment = value;
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
					if (this._enrolled < 0)
					{
						return 0;
					}
					return this._enrolled;
				}
				set
				{
					this._enrolled = value;
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
					return this._noOfRecurrences;
				}
				set
				{
					this._noOfRecurrences = value;
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
					return this._lastRecurrence;
				}
				set
				{
					this._lastRecurrence = value;
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
					return this._enrollRoleID;
				}
				set
				{
					this._enrollRoleID = value;
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
					return this._enrollType;
				}
				set
				{
					this._enrollType = value;
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
					return this._enrollFee;
				}
				set
				{
					this._enrollFee = value;
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
					return this._payPalAccount;
				}
				set
				{
					this._payPalAccount = value;
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
					return this._cancelled;
				}
				set
				{
					this._cancelled = value;
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
					return this._detailPage;
				}
				set
				{
					this._detailPage = value;
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
					return this._detailNewWin;
				}
				set
				{
					this._detailNewWin = value;
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
					return this._detailURL;
				}
				set
				{
					this._detailURL = value;
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
					return this._imageURL;
				}
				set
				{
					this._imageURL = value;
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
					return this._imageType;
				}
				set
				{
					this._imageType = value;
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
					return this._imageWidth;
				}
				set
				{
					this._imageWidth = value;
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
					return this._imageHeight;
				}
				set
				{
					this._imageHeight = value;
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
					return this._location;
				}
				set
				{
					this._location = value;
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
					return this._locationName;
				}
				set
				{
					this._locationName = value;
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
					return this._mapURL;
				}
				set
				{
					this._mapURL = value;
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
					return this._category;
				}
				set
				{
					this._category = value;
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
					return this._categoryName;
				}
				set
				{
					this._categoryName = value;
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
					return this._color;
				}
				set
				{
					this._color = value;
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
					return this._fontColor;
				}
				set
				{
					this._fontColor = value;
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
					return this._imageDisplay;
				}
				set
				{
					this._imageDisplay = value;
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
					return this._sendReminder;
				}
				set
				{
					this._sendReminder = value;
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
					return this._reminderTime;
				}
				set
				{
					this._reminderTime = value;
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
					return this._reminderTimeMeasurement;
				}
				set
				{
					this._reminderTimeMeasurement = value;
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
					return this._reminder;
				}
				set
				{
					this._reminder = value;
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
					return this._reminderFrom;
				}
				set
				{
					this._reminderFrom = value;
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
					return this._searchSubmitted;
				}
				set
				{
					this._searchSubmitted = value;
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
					return this._moduleTitle;
				}
				set
				{
					this._moduleTitle = value;
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
					return this._customField1;
				}
				set
				{
					this._customField1 = value;
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
					return this._customField2;
				}
				set
				{
					this._customField2 = value;
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
					return this._enrollListView;
				}
				set
				{
					this._enrollListView = value;
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
					return this._displayEndDate;
				}
				set
				{
					this._displayEndDate = value;
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
					return this._allDayEvent;
				}
				set
				{
					this._allDayEvent = value;
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
					return this._ownerID;
				}
				set
				{
					this._ownerID = value;
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
					return this._ownerName;
				}
				set
				{
					this._ownerName = value;
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
					return this._lastUpdatedAt;
				}
				set
				{
					this._lastUpdatedAt = value;
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
					return this._lastUpdatedBy;
				}
				set
				{
					this._lastUpdatedBy = value;
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
					return this._lastUpdatedID;
				}
				set
				{
					this._lastUpdatedID = value;
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
					return this._originalDateBegin;
				}
				set
				{
					this._originalDateBegin = value;
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
					return this._updateStatus;
				}
				set
				{
					this._updateStatus = value;
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
					return this._rrule;
				}
				set
				{
					this._rrule = value;
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
					return this._rmOwnerID;
				}
				set
				{
					this._rmOwnerID = value;
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
					return this._newEventEmailSent;
				}
				set
				{
					this._newEventEmailSent = value;
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
					return this._isPrivate;
				}
				set
				{
					this._isPrivate = value;
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
					if (string.IsNullOrEmpty(this._eventTimeZoneId))
					{
						EventModuleSettings modSettings = EventModuleSettings.GetEventModuleSettings(this._moduleID, null);
						this._eventTimeZoneId = modSettings.TimeZoneId;
					}
					return this._eventTimeZoneId;
				}
				set
				{
					this._eventTimeZoneId = value;
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
					if (ReferenceEquals(this._otherTimeZoneId, null))
					{
						this._otherTimeZoneId = "UTC";
					}
					return this._otherTimeZoneId;
				}
				set
				{
					this._otherTimeZoneId = value;
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
					return this._allowAnonEnroll;
				}
				set
				{
					this._allowAnonEnroll = value;
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
					return this._contentItemId;
				}
				set
				{
					this._contentItemId = value;
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
					return this._journalItem;
				}
				set
				{
					this._journalItem = value;
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
					return this._socialGroupId;
				}
				set
				{
					this._socialGroupId = value;
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
					return this._socialUserId;
				}
				set
				{
					this._socialUserId = value;
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
					return this._summary;
				}
				set
				{
					this._summary = value;
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
					return this._sequence;
				}
				set
				{
					this._sequence = value;
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
					return this._rmSequence;
				}
				set
				{
					this._rmSequence = value;
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
					return this._socialUserUserName;
				}
				set
				{
					this._socialUserUserName = value;
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
					return this._socialUserDisplayName;
				}
				set
				{
					this._socialUserDisplayName = value;
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
				string xCompare = this.EventName + Strings.Format(this.EventID, "00000000");
				string yCompare = o.EventName + Strings.Format(o.EventID, "00000000");
				if (SortExpression == SortFilter.CategoryName)
				{
					xCompare = this.CategoryName + Strings.Format(this.EventID, "00000000");
					yCompare = o.CategoryName + Strings.Format(o.EventID, "00000000");
				}
				else if (SortExpression == SortFilter.CustomField1)
				{
					xCompare = this.CustomField1 + Strings.Format(this.EventID, "00000000");
					yCompare = o.CustomField1 + Strings.Format(o.EventID, "00000000");
				}
				else if (SortExpression == SortFilter.CustomField2)
				{
					xCompare = this.CustomField2 + Strings.Format(this.EventID, "00000000");
					yCompare = o.CustomField2 + Strings.Format(o.EventID, "00000000");
				}
				else if (SortExpression == SortFilter.Description)
				{
					xCompare = this.EventDesc + Strings.Format(this.EventID, "00000000");
					yCompare = o.EventDesc + Strings.Format(o.EventID, "00000000");
				}
				else if (SortExpression == SortFilter.Duration)
				{
					xCompare = System.Convert.ToString(Strings.Format(this.Duration, "000000") + Strings.Format(this.EventID, "00000000"));
					yCompare = System.Convert.ToString(Strings.Format(o.Duration, "000000") + Strings.Format(o.EventID, "00000000"));
				}
				else if (SortExpression == SortFilter.EventDateBegin)
				{
					xCompare = System.Convert.ToString(Strings.Format(this.EventTimeBegin, "yyyyMMddHHmm") + Strings.Format(this.EventID, "00000000"));
					yCompare = System.Convert.ToString(Strings.Format(o.EventTimeBegin, "yyyyMMddHHmm") + Strings.Format(o.EventID, "00000000"));
				}
				else if (SortExpression == SortFilter.EventDateEnd)
				{
					xCompare = System.Convert.ToString(Strings.Format(this.EventTimeEnd, "yyyyMMddHHmm") + Strings.Format(this.EventID, "00000000"));
					yCompare = System.Convert.ToString(Strings.Format(o.EventTimeEnd, "yyyyMMddHHmm") + Strings.Format(o.EventID, "00000000"));
				}
				else if (SortExpression == SortFilter.LocationName)
				{
					xCompare = this.LocationName + Strings.Format(this.EventID, "00000000");
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
				return ((EventInfo) (this.MemberwiseClone()));
				
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
					return this._portalID;
				}
				set
				{
					this._portalID = value;
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
					return this._masterID;
				}
				set
				{
					this._masterID = value;
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
					return this._moduleID;
				}
				set
				{
					this._moduleID = value;
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
					return this._subEventID;
				}
				set
				{
					this._subEventID = value;
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
					return this._subEventTitle;
				}
				set
				{
					this._subEventTitle = value;
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
					return this._eventID;
				}
				set
				{
					this._eventID = value;
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
					return this._moduleID;
				}
				set
				{
					this._moduleID = value;
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
					return this._signupID;
				}
				set
				{
					this._signupID = value;
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
					return this._userID;
				}
				set
				{
					this._userID = value;
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
					return this._approved;
				}
				set
				{
					this._approved = value;
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
					return this._userName;
				}
				set
				{
					this._userName = value;
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
					return this._email;
				}
				set
				{
					this._email = value;
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
					return this._emailVisible;
				}
				set
				{
					this._emailVisible = value;
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
					return this._telephone;
				}
				set
				{
					this._telephone = value;
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
					return this._eventTimeBegin.Date;
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
					return this._eventTimeBegin;
				}
				set
				{
					this._eventTimeBegin = value;
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
					return this._eventTimeEnd;
				}
				set
				{
					this._eventTimeEnd = value;
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
					return this._duration;
				}
				set
				{
					this._duration = value;
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
					return this._eventName;
				}
				set
				{
					this._eventName = value;
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
					return ((Priority) this._importance);
				}
				set
				{
					this._importance = (int) value;
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
					return this._eventApproved;
				}
				set
				{
					this._eventApproved = value;
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
					return this._maxEnrollment;
				}
				set
				{
					this._maxEnrollment = value;
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
					return this._enrolled;
				}
				set
				{
					this._enrolled = value;
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
					return this._payPalStatus;
				}
				set
				{
					this._payPalStatus = value;
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
					return this._payPalReason;
				}
				set
				{
					this._payPalReason = value;
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
					return this._payPalTransID;
				}
				set
				{
					this._payPalTransID = value;
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
					return this._payPalPayerID;
				}
				set
				{
					this._payPalPayerID = value;
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
					return this._payPalPayerStatus;
				}
				set
				{
					this._payPalPayerStatus = value;
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
					return this._payPalRecieverEmail;
				}
				set
				{
					this._payPalRecieverEmail = value;
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
					return this._payPalUserEmail;
				}
				set
				{
					this._payPalUserEmail = value;
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
					return this._payPalPayerEmail;
				}
				set
				{
					this._payPalPayerEmail = value;
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
					return this._payPalFirstName;
				}
				set
				{
					this._payPalFirstName = value;
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
					return this._payPalLastName;
				}
				set
				{
					this._payPalLastName = value;
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
					return this._payPalAddress;
				}
				set
				{
					this._payPalAddress = value;
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
					return this._payPalCity;
				}
				set
				{
					this._payPalCity = value;
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
					return this._payPalState;
				}
				set
				{
					this._payPalState = value;
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
					return this._payPalZip;
				}
				set
				{
					this._payPalZip = value;
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
					return this._payPalCountry;
				}
				set
				{
					this._payPalCountry = value;
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
					return this._payPalCurrency;
				}
				set
				{
					this._payPalCurrency = value;
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
					return this._payPalPaymentDate;
				}
				set
				{
					this._payPalPaymentDate = value;
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
					return this._payPalAmount;
				}
				set
				{
					this._payPalAmount = value;
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
					return this._payPalFee;
				}
				set
				{
					this._payPalFee = value;
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
					return this._noEnrolees;
				}
				set
				{
					this._noEnrolees = value;
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
					return this._eventTimeZoneId;
				}
				set
				{
					this._eventTimeZoneId = value;
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
					return this._anonEmail;
				}
				set
				{
					this._anonEmail = value;
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
					return this._anonName;
				}
				set
				{
					this._anonName = value;
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
					return this._anonTelephone;
				}
				set
				{
					this._anonTelephone = value;
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
					return this._anonCulture;
				}
				set
				{
					this._anonCulture = value;
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
					return this._anonTimeZoneId;
				}
				set
				{
					this._anonTimeZoneId = value;
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
					return this._firstName;
				}
				set
				{
					this._firstName = value;
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
					return this._lastName;
				}
				set
				{
					this._lastName = value;
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
					return this._company;
				}
				set
				{
					this._company = value;
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
					return this._jobTitle;
				}
				set
				{
					this._jobTitle = value;
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
					return this._referenceNumber;
				}
				set
				{
					this._referenceNumber = value;
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
					return this._remarks;
				}
				set
				{
					this._remarks = value;
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
					return this._street;
				}
				set
				{
					this._street = value;
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
					return this._postalCode;
				}
				set
				{
					this._postalCode = value;
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
					return this._city;
				}
				set
				{
					this._city = value;
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
					return this._region;
				}
				set
				{
					this._region = value;
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
					return this._country;
				}
				set
				{
					this._country = value;
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
				string xCompare = this.EventName + Strings.Format(this.EventID, "00000000");
				string yCompare = o.EventName + Strings.Format(o.EventID, "00000000");
				if (SortExpression == SortFilter.Duration)
				{
					xCompare = System.Convert.ToString(Strings.Format(this.Duration, "000000") + Strings.Format(this.EventID, "00000000"));
					yCompare = System.Convert.ToString(Strings.Format(o.Duration, "000000") + Strings.Format(o.EventID, "00000000"));
				}
				else if (SortExpression == SortFilter.EventTimeBegin)
				{
					xCompare = System.Convert.ToString(Strings.Format(this.EventTimeBegin, "yyyyMMddHHmm") + Strings.Format(this.EventID, "00000000"));
					yCompare = System.Convert.ToString(Strings.Format(o.EventTimeBegin, "yyyyMMddHHmm") + Strings.Format(o.EventID, "00000000"));
				}
				else if (SortExpression == SortFilter.EventTimeEnd)
				{
					xCompare = System.Convert.ToString(Strings.Format(this.EventTimeEnd, "yyyyMMddHHmm") + Strings.Format(this.EventID, "00000000"));
					yCompare = System.Convert.ToString(Strings.Format(o.EventTimeEnd, "yyyyMMddHHmm") + Strings.Format(o.EventID, "00000000"));
				}
				else if (SortExpression == SortFilter.Approved)
				{
					xCompare = this.Approved.ToString() + Strings.Format(this.EventID, "00000000");
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
				return ((EventSignupsInfo) (this.MemberwiseClone()));
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
					return this._payPalID;
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
					return this._signupID;
				}
				set
				{
					this._signupID = value;
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
					return this._createDate;
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
					return this._payPalStatus;
				}
				set
				{
					this._payPalStatus = value;
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
					return this._payPalReason;
				}
				set
				{
					this._payPalReason = value;
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
					return this._payPalTransID;
				}
				set
				{
					this._payPalTransID = value;
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
					return this._payPalPayerID;
				}
				set
				{
					this._payPalPayerID = value;
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
					return this._payPalPayerStatus;
				}
				set
				{
					this._payPalPayerStatus = value;
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
					return this._payPalRecieverEmail;
				}
				set
				{
					this._payPalRecieverEmail = value;
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
					return this._payPalUserEmail;
				}
				set
				{
					this._payPalUserEmail = value;
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
					return this._payPalPayerEmail;
				}
				set
				{
					this._payPalPayerEmail = value;
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
					return this._payPalFirstName;
				}
				set
				{
					this._payPalFirstName = value;
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
					return this._payPalLastName;
				}
				set
				{
					this._payPalLastName = value;
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
					return this._payPalAddress;
				}
				set
				{
					this._payPalAddress = value;
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
					return this._payPalCity;
				}
				set
				{
					this._payPalCity = value;
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
					return this._payPalState;
				}
				set
				{
					this._payPalState = value;
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
					return this._payPalZip;
				}
				set
				{
					this._payPalZip = value;
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
					return this._payPalCountry;
				}
				set
				{
					this._payPalCountry = value;
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
					return this._payPalCurrency;
				}
				set
				{
					this._payPalCurrency = value;
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
					return this._payPalPaymentDate;
				}
				set
				{
					this._payPalPaymentDate = value;
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
					return this._payPalAmount;
				}
				set
				{
					this._payPalAmount = value;
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
					return this._payPalFee;
				}
				set
				{
					this._payPalFee = value;
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
					return this._portalID;
				}
				set
				{
					this._portalID = value;
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
					return this._category;
				}
				set
				{
					this._category = value;
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
					return this._categoryName;
				}
				set
				{
					this._categoryName = value;
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
					return this._color;
				}
				set
				{
					this._color = value;
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
					return this._fontColor;
				}
				set
				{
					this._fontColor = value;
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
					return this._portalID;
				}
				set
				{
					this._portalID = value;
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
					return this._location;
				}
				set
				{
					this._location = value;
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
					return this._locationName;
				}
				set
				{
					this._locationName = value;
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
					return this._mapURL;
				}
				set
				{
					this._mapURL = value;
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
					return this._street;
				}
				set
				{
					this._street = value;
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
					return this._postalCode;
				}
				set
				{
					this._postalCode = value;
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
					return this._city;
				}
				set
				{
					this._city = value;
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
					return this._region;
				}
				set
				{
					this._region = value;
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
					return this._country;
				}
				set
				{
					this._country = value;
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
					return this._eventID;
				}
				set
				{
					this._eventID = value;
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
					return this._portalAliasID;
				}
				set
				{
					this._portalAliasID = value;
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
					return this._notificationID;
				}
				set
				{
					this._notificationID = value;
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
					return this._userEmail;
				}
				set
				{
					this._userEmail = value;
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
					return this._notificationSent;
				}
				set
				{
					this._notificationSent = value;
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
					return this._notifyByDateTime;
				}
				set
				{
					this._notifyByDateTime = value;
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
					return this._eventTimeBegin;
				}
				set
				{
					this._eventTimeBegin = value;
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
					return this._notifyLanguage;
				}
				set
				{
					this._notifyLanguage = value;
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
					return this._moduleID;
				}
				set
				{
					this._moduleID = value;
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
					return this._tabID;
				}
				set
				{
					this._tabID = value;
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
					return this._recurMasterID;
				}
				set
				{
					this._recurMasterID = value;
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
					return this._moduleID;
				}
				set
				{
					this._moduleID = value;
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
					return this._portalID;
				}
				set
				{
					this._portalID = value;
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
					return this._rrule;
				}
				set
				{
					this._rrule = value;
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
					return this._dtstart;
				}
				set
				{
					this._dtstart = value;
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
					return this._duration;
				}
				set
				{
					this._duration = value;
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
					return this._until;
				}
				set
				{
					this._until = value;
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
					return this._eventName;
				}
				set
				{
					this._eventName = value;
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
					return this._eventDesc;
				}
				set
				{
					this._eventDesc = value;
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
					return ((Priority) this._importance);
				}
				set
				{
					this._importance = (int) value;
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
					return this._notify;
				}
				set
				{
					this._notify = value;
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
					return this._approved;
				}
				set
				{
					this._approved = value;
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
					return this._signups;
				}
				set
				{
					this._signups = value;
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
					return this._maxEnrollment;
				}
				set
				{
					this._maxEnrollment = value;
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
					if (this._enrolled < 0)
					{
						return 0;
					}
					return this._enrolled;
				}
				set
				{
					this._enrolled = value;
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
					return this._enrollRoleID;
				}
				set
				{
					this._enrollRoleID = value;
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
					return this._enrollType;
				}
				set
				{
					this._enrollType = value;
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
					return this._enrollFee;
				}
				set
				{
					this._enrollFee = value;
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
					return this._payPalAccount;
				}
				set
				{
					this._payPalAccount = value;
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
					return this._detailPage;
				}
				set
				{
					this._detailPage = value;
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
					return this._detailNewWin;
				}
				set
				{
					this._detailNewWin = value;
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
					return this._detailURL;
				}
				set
				{
					this._detailURL = value;
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
					return this._imageURL;
				}
				set
				{
					this._imageURL = value;
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
					return this._imageType;
				}
				set
				{
					this._imageType = value;
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
					return this._imageWidth;
				}
				set
				{
					this._imageWidth = value;
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
					return this._imageHeight;
				}
				set
				{
					this._imageHeight = value;
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
					return this._location;
				}
				set
				{
					this._location = value;
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
					return this._category;
				}
				set
				{
					this._category = value;
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
					return this._imageDisplay;
				}
				set
				{
					this._imageDisplay = value;
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
					return this._sendReminder;
				}
				set
				{
					this._sendReminder = value;
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
					return this._reminderTime;
				}
				set
				{
					this._reminderTime = value;
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
					return this._reminderTimeMeasurement;
				}
				set
				{
					this._reminderTimeMeasurement = value;
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
					return this._reminder;
				}
				set
				{
					this._reminder = value;
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
					return this._reminderFrom;
				}
				set
				{
					this._reminderFrom = value;
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
					return this._customField1;
				}
				set
				{
					this._customField1 = value;
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
					return this._customField2;
				}
				set
				{
					this._customField2 = value;
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
					return this._enrollListView;
				}
				set
				{
					this._enrollListView = value;
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
					return this._displayEndDate;
				}
				set
				{
					this._displayEndDate = value;
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
					return this._allDayEvent;
				}
				set
				{
					this._allDayEvent = value;
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
					return this._cultureName;
				}
				set
				{
					this._cultureName = value;
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
					return this._ownerID;
				}
				set
				{
					this._ownerID = value;
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
					return this._createdByID;
				}
				set
				{
					this._createdByID = value;
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
					return this._createdDate;
				}
				set
				{
					this._createdDate = value;
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
					return this._updatedByID;
				}
				set
				{
					this._updatedByID = value;
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
					return this._updatedDate;
				}
				set
				{
					this._updatedDate = value;
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
					return this._firstEventID;
				}
				set
				{
					this._firstEventID = value;
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
					if (string.IsNullOrEmpty(this._eventTimeZoneId))
					{
						EventModuleSettings modSettings = EventModuleSettings.GetEventModuleSettings(this._moduleID, null);
						this._eventTimeZoneId = modSettings.TimeZoneId;
					}
					return this._eventTimeZoneId;
				}
				set
				{
					this._eventTimeZoneId = value;
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
					return this._allowAnonEnroll;
				}
				set
				{
					this._allowAnonEnroll = value;
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
					return this._contentItemId;
				}
				set
				{
					this._contentItemId = value;
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
					return this._journalItem;
				}
				set
				{
					this._journalItem = value;
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
					return this._socialGroupId;
				}
				set
				{
					this._socialGroupId = value;
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
					return this._socialUserId;
				}
				set
				{
					this._socialUserId = value;
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
					return this._summary;
				}
				set
				{
					this._summary = value;
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
					return this._sequence;
				}
				set
				{
					this._sequence = value;
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
					return this._freq;
				}
				set
				{
					this._freq = value;
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
					return this._interval;
				}
				set
				{
					this._interval = value;
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
					return this._byDay;
				}
				set
				{
					this._byDay = value;
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
					return this._byMonthDay;
				}
				set
				{
					this._byMonthDay = value;
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
					return this._byMonth;
				}
				set
				{
					this._byMonth = value;
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
					return this._su;
				}
				set
				{
					this._su = value;
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
					return this._suNo;
				}
				set
				{
					this._suNo = value;
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
					return this._mo;
				}
				set
				{
					this._mo = value;
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
					return this._moNo;
				}
				set
				{
					this._moNo = value;
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
					return this._tu;
				}
				set
				{
					this._tu = value;
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
					return this._tuNo;
				}
				set
				{
					this._tuNo = value;
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
					return this._we;
				}
				set
				{
					this._we = value;
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
					return this._weNo;
				}
				set
				{
					this._weNo = value;
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
					return this._th;
				}
				set
				{
					this._th = value;
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
					return this._thNo;
				}
				set
				{
					this._thNo = value;
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
					return this._fr;
				}
				set
				{
					this._fr = value;
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
					return this._frNo;
				}
				set
				{
					this._frNo = value;
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
					return this._sa;
				}
				set
				{
					this._sa = value;
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
					return this._saNo;
				}
				set
				{
					this._saNo = value;
				}
			}
			
			public bool FreqBasic
			{
				get
				{
					return this._freqBasic;
				}
				set
				{
					this._freqBasic = value;
				}
			}
			
			public string Wkst
			{
				get
				{
					return this._wkst;
				}
				set
				{
					this._wkst = value;
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
					return this._txtEmailSubject;
				}
				set
				{
					this._txtEmailSubject = value;
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
					return this._txtEmailBody;
				}
				set
				{
					this._txtEmailBody = value;
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
					return this._txtEmailFrom;
				}
				set
				{
					this._txtEmailFrom = value;
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
					return this._userIDs;
				}
				set
				{
					this._userIDs = value;
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
					return this._userEmails;
				}
				set
				{
					this._userEmails = value;
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
					return this._userLocales;
				}
				set
				{
					this._userLocales = value;
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
					return this._userTimeZoneIds;
				}
				set
				{
					this._userTimeZoneIds = value;
				}
			}
			
			/// <summary>
			/// Initializes a new instance of the <see cref="EventEmailInfo" /> class.
			/// </summary>
			public EventEmailInfo()
			{
				ArrayList newUserEmails = new ArrayList();
				this.UserEmails = newUserEmails;
				
				ArrayList newUserIDs = new ArrayList();
				this.UserIDs = newUserIDs;
				
				ArrayList newUserLocales = new ArrayList();
				this.UserLocales = newUserLocales;
				
				ArrayList newUserTimeZoneIds = new ArrayList();
				this.UserTimeZoneIds = newUserTimeZoneIds;
				
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
					return this._portalID;
				}
				set
				{
					this._portalID = value;
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
					return this._subscriptionID;
				}
				set
				{
					this._subscriptionID = value;
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
					return this._moduleID;
				}
				set
				{
					this._moduleID = value;
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
					return this._userID;
				}
				set
				{
					this._userID = value;
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
					return this._indexId;
				}
				set
				{
					this._indexId = value;
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
					return this._eventID;
				}
				set
				{
					this._eventID = value;
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
					return this._createdByID;
				}
				set
				{
					this._createdByID = value;
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
					return this._ownerID;
				}
				set
				{
					this._ownerID = value;
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
					return this._moduleID;
				}
				set
				{
					this._moduleID = value;
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
					return this._eventDateBegin;
				}
				set
				{
					this._eventDateBegin = value;
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
					return this._eventDateEnd;
				}
				set
				{
					this._eventDateEnd = value;
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
					return this._txtEventDateEnd;
				}
				set
				{
					this._txtEventDateEnd = value;
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
					return this._eventTimeBegin;
				}
				set
				{
					this._eventTimeBegin = value;
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
					return this._txtEventTimeBegin;
				}
				set
				{
					this._txtEventTimeBegin = value;
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
					return this._recurUntil;
				}
				set
				{
					this._recurUntil = value;
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
					return this._duration;
				}
				set
				{
					this._duration = value;
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
					return this._eventName;
				}
				set
				{
					this._eventName = value;
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
					return this._eventDesc;
				}
				set
				{
					this._eventDesc = value;
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
					return this._decodedDesc;
				}
				set
				{
					this._decodedDesc = value;
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
					return this._recurText;
				}
				set
				{
					this._recurText = value;
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
					return this._url;
				}
				set
				{
					this._url = value;
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
					return this._target;
				}
				set
				{
					this._target = value;
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
					return this._imageURL;
				}
				set
				{
					this._imageURL = value;
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
					return this._categoryName;
				}
				set
				{
					this._categoryName = value;
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
					return this._locationName;
				}
				set
				{
					this._locationName = value;
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
					return this._customField1;
				}
				set
				{
					this._customField1 = value;
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
					return this._customField2;
				}
				set
				{
					this._customField2 = value;
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
					return this._editVisibility;
				}
				set
				{
					this._editVisibility = value;
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
					return this._categoryColor;
				}
				set
				{
					this._categoryColor = value;
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
					return this._categoryFontColor;
				}
				set
				{
					this._categoryFontColor = value;
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
					return this._displayDuration;
				}
				set
				{
					this._displayDuration = value;
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
					return this._recurMasterID;
				}
				set
				{
					this._recurMasterID = value;
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
					return this._icons;
				}
				set
				{
					this._icons = value;
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
					return this._tooltip;
				}
				set
				{
					this._tooltip = value;
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
				string xCompare = this.EventName + Strings.Format(this.EventID, "00000000");
				string yCompare = o.EventName + Strings.Format(o.EventID, "00000000");
				if (SortExpression == SortFilter.CategoryName)
				{
					xCompare = this.CategoryName + Strings.Format(this.EventID, "00000000");
					yCompare = o.CategoryName + Strings.Format(o.EventID, "00000000");
				}
				else if (SortExpression == SortFilter.CustomField1)
				{
					xCompare = this.CustomField1 + Strings.Format(this.EventID, "00000000");
					yCompare = o.CustomField1 + Strings.Format(o.EventID, "00000000");
				}
				else if (SortExpression == SortFilter.CustomField2)
				{
					xCompare = this.CustomField2 + Strings.Format(this.EventID, "00000000");
					yCompare = o.CustomField2 + Strings.Format(o.EventID, "00000000");
				}
				else if (SortExpression == SortFilter.Description)
				{
					xCompare = this.EventDesc + Strings.Format(this.EventID, "00000000");
					yCompare = o.EventDesc + Strings.Format(o.EventID, "00000000");
				}
				else if (SortExpression == SortFilter.Duration)
				{
					xCompare = System.Convert.ToString(Strings.Format(this.Duration, "000000") + Strings.Format(this.EventID, "00000000"));
					yCompare = System.Convert.ToString(Strings.Format(o.Duration, "000000") + Strings.Format(o.EventID, "00000000"));
				}
				else if (SortExpression == SortFilter.EventDateBegin)
				{
					xCompare = System.Convert.ToString(Strings.Format(this.EventDateBegin, "yyyyMMddHHmm") + Strings.Format(this.EventID, "00000000"));
					yCompare = System.Convert.ToString(Strings.Format(o.EventDateBegin, "yyyyMMddHHmm") + Strings.Format(o.EventID, "00000000"));
				}
				else if (SortExpression == SortFilter.EventDateEnd)
				{
					xCompare = System.Convert.ToString(Strings.Format(this.EventDateEnd, "yyyyMMddHHmm") + Strings.Format(this.EventID, "00000000"));
					yCompare = System.Convert.ToString(Strings.Format(o.EventDateEnd, "yyyyMMddHHmm") + Strings.Format(o.EventID, "00000000"));
				}
				else if (SortExpression == SortFilter.LocationName)
				{
					xCompare = this.LocationName + Strings.Format(this.EventID, "00000000");
					yCompare = o.LocationName + Strings.Format(o.EventID, "00000000");
				}
				else if (SortExpression == SortFilter.EventID)
				{
					xCompare = Strings.Format(this.EventID, "00000000");
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
					return this._userID;
				}
				set
				{
					this._userID = value;
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
					return this._displayName;
				}
				set
				{
					this._displayName = value;
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
					return this._profileURL;
				}
				set
				{
					this._profileURL = value;
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
					return this._displayNameURL;
				}
				set
				{
					this._displayNameURL = value;
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
					return this._signupID;
				}
				set
				{
					this._signupID = value;
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
					return this._enrollUserName;
				}
				set
				{
					this._enrollUserName = value;
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
					return this._enrollDisplayName;
				}
				set
				{
					this._enrollDisplayName = value;
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
					return this._enrollEmail;
				}
				set
				{
					this._enrollEmail = value;
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
					return this._enrollPhone;
				}
				set
				{
					this._enrollPhone = value;
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
					return this._enrollApproved;
				}
				set
				{
					this._enrollApproved = value;
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
					return this._enrollNo;
				}
				set
				{
					this._enrollNo = value;
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
					return this._enrollTimeBegin;
				}
				set
				{
					this._enrollTimeBegin = value;
				}
			}
		}
#endregion
		
	}
