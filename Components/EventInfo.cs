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
    using Microsoft.VisualBasic;

    #region EventInfo Class

    public class EventInfo : IComparable
    {
        /// <summary>
        ///     Priority setting for events, can be used to display appropriate icons in an event
        /// </summary>
        public enum Priority
        {
            /// <summary>
            ///     High priority
            /// </summary>
            High = 1,

            /// <summary>
            ///     Medium priority
            /// </summary>
            Medium = 2,

            /// <summary>
            ///     Low priority
            /// </summary>
            Low = 3
        }

        #region Constructors

        // initialization

        #endregion

        #region Public Methods

        /// <summary>
        ///     Clones an instance of the events object
        /// </summary>
        /// <returns>Cloned Eventsinfo object</returns>
        public EventInfo Clone()
        {
            return (EventInfo) this.MemberwiseClone();
        }

        #endregion

        #region Private Members

        private int _importance;

        // ReSharper disable UnassignedField.Local

        // ReSharper restore UnassignedField.Local

        private int _enrolled;
        private string _eventTimeZoneId = string.Empty;
        private string _otherTimeZoneId = string.Empty;

        #endregion

        #region Public Properties

        // public properties
        /// <summary>
        ///     Gets or sets the portal ID.
        /// </summary>
        /// <value>The portal ID.</value>
        public int PortalID { get; set; }

        /// <summary>
        ///     Gets or sets the event ID.
        /// </summary>
        /// <value>The event ID.</value>
        public int EventID { get; set; }

        /// <summary>
        ///     Gets or sets the recurring master ID.
        /// </summary>
        /// <value>The recur master ID.</value>
        public int RecurMasterID { get; set; }

        /// <summary>
        ///     Gets or sets the module ID.
        /// </summary>
        /// <value>The module ID.</value>
        public int ModuleID { get; set; }

        /// <summary>
        ///     Gets or sets the event date end.
        /// </summary>
        /// <value>The event date end.</value>
        [Obsolete("EventDateEnd is only retained for upgrade from versions prior to 4.1.0")]
        public DateTime EventDateEnd { get; set; }

        /// <summary>
        ///     Gets or sets the event time begin.
        /// </summary>
        /// <value>The event time begin.</value>
        public DateTime EventTimeBegin { get; set; }

        /// <summary>
        ///     Gets the event time end.
        /// </summary>
        /// <value>The event time end.</value>
        public DateTime EventTimeEnd => this.EventTimeBegin.AddMinutes(this.Duration);

        /// <summary>
        ///     Gets or sets the duration of an event.
        /// </summary>
        /// <value>The duration.</value>
        public int Duration { get; set; }

        /// <summary>
        ///     Gets or sets the name of the event.
        /// </summary>
        /// <value>The name of the event.</value>
        public string EventName { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the event description
        /// </summary>
        /// <value>The event description.</value>
        public string EventDesc { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the importance of an event
        /// </summary>
        /// <value>The importance of an event.</value>
        public Priority Importance
        {
            get { return (Priority) this._importance; }
            set { this._importance = (int) value; }
        }

        /// <summary>
        ///     Gets or sets the created date of an event
        /// </summary>
        /// <value>The created date of an event.</value>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        ///     Gets or sets the created by of an event.
        /// </summary>
        /// <value>The created by of an event.</value>
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the created by UserID.
        /// </summary>
        /// <value>The created by UserID.</value>
        public int CreatedByID { get; set; }

        /// <summary>
        ///     Gets or sets the recurrence unit of an event
        /// </summary>
        /// <value>The recurrence unit of an event.</value>
        [Obsolete("Every is only retained for upgrade from versions prior to 4.1.0")]
        public int Every { get; }

        /// <summary>
        ///     Gets or sets the period of a recurrence.
        /// </summary>
        /// <value>The period of a recurrnence.</value>
        [Obsolete("Period is only retained for upgrade from versions prior to 4.1.0")]
        public string Period { get; } = string.Empty;

        /// <summary>
        ///     Gets or sets the type of the repeat of a recurrence
        /// </summary>
        /// <value>The type of the repeat of a recurrence.</value>
        [Obsolete("RepeatType is only retained for upgrade from versions prior to 4.1.0")]
        public string RepeatType { get; } = string.Empty;

        /// <summary>
        ///     Gets or sets the notification text
        /// </summary>
        /// <value>The notification text.</value>
        public string Notify { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="EventInfo" /> is approved.
        /// </summary>
        /// <value><c>true</c> if approved; otherwise, <c>false</c>.</value>
        public bool Approved { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="EventInfo" /> can have signups.
        /// </summary>
        /// <value><c>true</c> can have signups; otherwise, <c>false</c>.</value>
        public bool Signups { get; set; }

        /// <summary>
        ///     Gets or sets the max # of enrollments
        /// </summary>
        /// <value>The max # of enrollments</value>
        public int MaxEnrollment { get; set; }

        /// <summary>
        ///     Gets or sets the # of enrolled people
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
            set { this._enrolled = value; }
        }

        /// <summary>
        ///     Gets or sets the # f recurrences.
        /// </summary>
        /// <value>The # of recurrences.</value>
        public int NoOfRecurrences { get; set; }

        /// <summary>
        ///     Gets or sets the last recurrence date
        /// </summary>
        /// <value>The last recurrence date.</value>
        public DateTime LastRecurrence { get; set; }

        /// <summary>
        ///     Gets or sets the enroll role ID.
        /// </summary>
        /// <value>The enroll role ID.</value>
        public int EnrollRoleID { get; set; } = 0;

        /// <summary>
        ///     Gets or sets the type of the enrollment
        /// </summary>
        /// <value>The type of the enrollment</value>
        public string EnrollType { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the enroll fee.
        /// </summary>
        /// <value>The enroll fee.</value>
        public decimal EnrollFee { get; set; }

        /// <summary>
        ///     Gets or sets the PayPal account.
        /// </summary>
        /// <value>The PayPal account.</value>
        public string PayPalAccount { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="EventInfo" /> is cancelled.
        /// </summary>
        /// <value><c>true</c> if cancelled; otherwise, <c>false</c>.</value>
        public bool Cancelled { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether a detail page must be openend.
        /// </summary>
        /// <value><c>true</c> if detail page must be opened; otherwise, <c>false</c>.</value>
        public bool DetailPage { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether detail page must appear in a new page.
        /// </summary>
        /// <value><c>true</c> if new page; otherwise, <c>false</c>.</value>
        public bool DetailNewWin { get; set; }

        /// <summary>
        ///     Gets or sets the detail URL.
        /// </summary>
        /// <value>The detail URL.</value>
        public string DetailURL { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the image URL.
        /// </summary>
        /// <value>The image URL.</value>
        public string ImageURL { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the type of the image.
        /// </summary>
        /// <value>The type of the image.</value>
        public string ImageType { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the width of the image.
        /// </summary>
        /// <value>The width of the image.</value>
        public int ImageWidth { get; set; }

        /// <summary>
        ///     Gets or sets the height of the image.
        /// </summary>
        /// <value>The height of the image.</value>
        public int ImageHeight { get; set; }

        /// <summary>
        ///     Gets or sets the location of an event
        /// </summary>
        /// <value>The location of an event.</value>
        public int Location { get; set; }

        /// <summary>
        ///     Gets or sets the name of the location of an event.
        /// </summary>
        /// <value>The name of the location of an event.</value>
        public string LocationName { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the map URL.
        /// </summary>
        /// <value>The map URL.</value>
        public string MapURL { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the category of an event
        /// </summary>
        /// <value>The category of an event.</value>
        public int Category { get; set; }

        /// <summary>
        ///     Gets or sets the name of the category.
        /// </summary>
        /// <value>The name of the category.</value>
        public string CategoryName { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the category color to be used for display.
        /// </summary>
        /// <value>The color of the category.</value>
        public string Color { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the category color of the font (foreground color).
        /// </summary>
        /// <value>The color of the font.</value>
        public string FontColor { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets a value indicating whether an image must be displayed.
        /// </summary>
        /// <value><c>true</c> if image must be displayed; otherwise, <c>false</c>.</value>
        public bool ImageDisplay { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to send a reminder.
        /// </summary>
        /// <value><c>true</c> if send reminder; otherwise, <c>false</c>.</value>
        public bool SendReminder { get; set; }

        /// <summary>
        ///     Gets or sets the reminder time.
        /// </summary>
        /// <value>The reminder time.</value>
        public int ReminderTime { get; set; }

        /// <summary>
        ///     Gets or sets the reminder time measurement.
        /// </summary>
        /// <value>The reminder time measurement.</value>
        public string ReminderTimeMeasurement { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the reminder.
        /// </summary>
        /// <value>The reminder.</value>
        public string Reminder { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the reminder from e-mail address
        /// </summary>
        /// <value>The reminder from e-mail address.</value>
        public string ReminderFrom { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets a value indicating whether the event is submitted to the search
        /// </summary>
        /// <value><c>true</c> if search submitted; otherwise, <c>false</c>.</value>
        public bool SearchSubmitted { get; set; }

        /// <summary>
        ///     Gets or sets the module title.
        /// </summary>
        /// <value>The module title.</value>
        public string ModuleTitle { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the custom field1.
        /// </summary>
        /// <value>The custom field1.</value>
        public string CustomField1 { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the custom field2.
        /// </summary>
        /// <value>The custom field2.</value>
        public string CustomField2 { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets a value indicating whether enroll list view is to be displayed.
        /// </summary>
        /// <value><c>true</c> if [enroll list view]; otherwise, <c>false</c>.</value>
        public bool EnrollListView { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether end date is to be displayed
        /// </summary>
        /// <value><c>true</c> if [display end date]; otherwise, <c>false</c>.</value>
        public bool DisplayEndDate { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the event is an all day event.
        /// </summary>
        /// <value><c>true</c> if [all day event]; otherwise, <c>false</c>.</value>
        public bool AllDayEvent { get; set; }

        /// <summary>
        ///     Gets or sets the owner ID (userID).
        /// </summary>
        /// <value>The owner ID.</value>
        public int OwnerID { get; set; }

        /// <summary>
        ///     Gets or sets the name of the owner of the event
        /// </summary>
        /// <value>The name of the owner.</value>
        public string OwnerName { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the last updated at.
        /// </summary>
        /// <value>The last updated at.</value>
        public DateTime LastUpdatedAt { get; set; }

        /// <summary>
        ///     Gets or sets the last updated by.
        /// </summary>
        /// <value>The last updated by.</value>
        public string LastUpdatedBy { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the last updated userID
        /// </summary>
        /// <value>The last updated userID.</value>
        public int LastUpdatedID { get; set; }

        /// <summary>
        ///     Gets or sets the original date begin.
        /// </summary>
        /// <value>The original date begin.</value>
        public DateTime OriginalDateBegin { get; set; }

        /// <summary>
        ///     Gets or sets the update status.
        /// </summary>
        /// <value>The update status.</value>
        public string UpdateStatus { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the Recurrence Rule.
        /// </summary>
        /// <value>The Recurrence Rule.</value>
        public string RRULE { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the RM owner ID.
        /// </summary>
        /// <value>The RM owner ID.</value>
        public int RmOwnerID { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [new event email sent].
        /// </summary>
        /// <value><c>true</c> if [new event email sent]; otherwise, <c>false</c>.</value>
        public bool NewEventEmailSent { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [is private].
        /// </summary>
        /// <value><c>true</c> if [is private]; otherwise, <c>false</c>.</value>
        public bool IsPrivate { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating what the [event timezoneid] is
        /// </summary>
        /// <value>The event timezoneid.</value>
        public string EventTimeZoneId
        {
            get
                {
                    if (string.IsNullOrEmpty(this._eventTimeZoneId))
                    {
                        var modSettings = EventModuleSettings.GetEventModuleSettings(this.ModuleID, null);
                        this._eventTimeZoneId = modSettings.TimeZoneId;
                    }
                    return this._eventTimeZoneId;
                }
            set { this._eventTimeZoneId = value; }
        }

        /// <summary>
        ///     Gets or sets a value indicating what the [other timezoneid] is
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
            set { this._otherTimeZoneId = value; }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether [allow anonymous enrollment].
        /// </summary>
        /// <value><c>true</c> if [allow anonymous enrollment]; otherwise, <c>false</c>.</value>
        public bool AllowAnonEnroll { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating the [content item id].
        /// </summary>
        /// <value>The contentitemid.</value>
        public int ContentItemID { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [event has journal item].
        /// </summary>
        /// <value><c>true</c> if [event has journal item]; otherwise, <c>false</c>.</value>
        public bool JournalItem { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating the [soccialgroup id].
        /// </summary>
        /// <value>The SocialGroupId.</value>
        public int SocialGroupId { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating the [user id].
        /// </summary>
        /// <value>The SocialUserId.</value>
        public int SocialUserId { get; set; }

        /// <summary>
        ///     Gets or sets the summary.
        /// </summary>
        /// <value>The summary.</value>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets a value indicating the [sequence].
        /// </summary>
        /// <value>The Sequence.</value>
        public int Sequence { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating the [recur master sequence].
        /// </summary>
        /// <value>The Recur MasterSequence.</value>
        public int RmSequence { get; set; }

        /// <summary>
        ///     Gets or sets the SocialUserUserName.
        /// </summary>
        /// <value>The SocialUserUserName.</value>
        public string SocialUserUserName { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the SocialUserDisplayName.
        /// </summary>
        /// <value>The SocialUserDisplayName.</value>
        public string SocialUserDisplayName { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the sort expression.
        /// </summary>
        /// <value>The sort expression.</value>
        public static SortFilter SortExpression { get; set; }

        /// <summary>
        ///     Gets or sets the sort direction.
        /// </summary>
        /// <value>The sort direction.</value>
        public static SortDirection SortDirection { get; set; }


        /// <summary>
        ///     Sorting enumeration
        /// </summary>
        public enum SortFilter
        {
            /// <summary>
            ///     By EventID
            /// </summary>
            EventID,

            /// <summary>
            ///     By Date beging
            /// </summary>
            EventDateBegin,

            /// <summary>
            ///     By Date end
            /// </summary>
            EventDateEnd,

            /// <summary>
            ///     Bu Name
            /// </summary>
            EventName,

            /// <summary>
            ///     By duration
            /// </summary>
            Duration,

            /// <summary>
            ///     Bu category name
            /// </summary>
            CategoryName,

            /// <summary>
            ///     By customfield1
            /// </summary>
            CustomField1,

            /// <summary>
            ///     By customfield2
            /// </summary>
            CustomField2,

            /// <summary>
            ///     By description
            /// </summary>
            Description,

            /// <summary>
            ///     By Location name
            /// </summary>
            LocationName
        }

        /// <summary>
        ///     Compares the current instance with another object of the same type and returns an integer that indicates whether
        ///     the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        ///     A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these
        ///     meanings:
        ///     Value
        ///     Meaning
        ///     Less than zero
        ///     This instance is less than <paramref name="obj" />.
        ///     Zero
        ///     This instance is equal to <paramref name="obj" />.
        ///     Greater than zero
        ///     This instance is greater than <paramref name="obj" />.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="obj" /> is not the same type as this instance.
        /// </exception>
        public int CompareTo(object obj)
        {
            var o = (EventInfo) obj;
            var xCompare = this.EventName + Strings.Format(this.EventID, "00000000");
            var yCompare = o.EventName + Strings.Format(o.EventID, "00000000");
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
                xCompare = Convert.ToString(Strings.Format(this.Duration, "000000") +
                                            Strings.Format(this.EventID, "00000000"));
                yCompare = Convert.ToString(
                    Strings.Format(o.Duration, "000000") + Strings.Format(o.EventID, "00000000"));
            }
            else if (SortExpression == SortFilter.EventDateBegin)
            {
                xCompare = Convert.ToString(Strings.Format(this.EventTimeBegin, "yyyyMMddHHmm") +
                                            Strings.Format(this.EventID, "00000000"));
                yCompare = Convert.ToString(Strings.Format(o.EventTimeBegin, "yyyyMMddHHmm") +
                                            Strings.Format(o.EventID, "00000000"));
            }
            else if (SortExpression == SortFilter.EventDateEnd)
            {
                xCompare = Convert.ToString(Strings.Format(this.EventTimeEnd, "yyyyMMddHHmm") +
                                            Strings.Format(this.EventID, "00000000"));
                yCompare = Convert.ToString(Strings.Format(o.EventTimeEnd, "yyyyMMddHHmm") +
                                            Strings.Format(o.EventID, "00000000"));
            }
            else if (SortExpression == SortFilter.LocationName)
            {
                xCompare = this.LocationName + Strings.Format(this.EventID, "00000000");
                yCompare = o.LocationName + Strings.Format(o.EventID, "00000000");
            }
            if (SortDirection == SortDirection.Ascending)
            {
                return string.Compare(xCompare, yCompare, StringComparison.CurrentCulture);
            }
            return string.Compare(yCompare, xCompare, StringComparison.CurrentCulture);
        }

        #endregion
    }

    #endregion

    #region EventMasterInfo Class

    public class EventMasterInfo
    {
        /// <summary>
        ///     Gets or sets the portal ID.
        /// </summary>
        /// <value>The portal ID.</value>
        public int PortalID { get; set; }

        /// <summary>
        ///     Gets or sets the master ID.
        /// </summary>
        /// <value>The master ID.</value>
        public int MasterID { get; set; }

        /// <summary>
        ///     Gets or sets the module ID.
        /// </summary>
        /// <value>The module ID.</value>
        public int ModuleID { get; set; }

        /// <summary>
        ///     Gets or sets the sub event ID.
        /// </summary>
        /// <value>The sub event ID.</value>
        public int SubEventID { get; set; }

        /// <summary>
        ///     Gets or sets the sub event title.
        /// </summary>
        /// <value>The sub event title.</value>
        public string SubEventTitle { get; set; }
    }

    #endregion

    #region EventSignupsInfo Class

    /// <summary>
    ///     Information about the users that have signed up for a particular event
    /// </summary>
    public class EventSignupsInfo : IComparable
    {
        /// <summary>
        ///     Priority of the event
        /// </summary>
        public enum Priority
        {
            /// <summary>
            ///     Hign priority
            /// </summary>
            High = 1,

            /// <summary>
            ///     Medium priority
            /// </summary>
            Medium = 2,

            /// <summary>
            ///     Low priority
            /// </summary>
            Low = 3
        }

        private int _importance;

        // initialization

        // public properties
        /// <summary>
        ///     Gets or sets the event ID.
        /// </summary>
        /// <value>The event ID.</value>
        public int EventID { get; set; }

        /// <summary>
        ///     Gets or sets the module ID.
        /// </summary>
        /// <value>The module ID.</value>
        public int ModuleID { get; set; }

        /// <summary>
        ///     Gets or sets the signup ID.
        /// </summary>
        /// <value>The signup ID.</value>
        public int SignupID { get; set; }

        /// <summary>
        ///     Gets or sets the user ID.
        /// </summary>
        /// <value>The user ID.</value>
        public int UserID { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="EventSignupsInfo" /> is approved.
        /// </summary>
        /// <value><c>true</c> if approved; otherwise, <c>false</c>.</value>
        public bool Approved { get; set; } = false;

        /// <summary>
        ///     Gets or sets the name of the user for the signup
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName { get; set; }

        /// <summary>
        ///     Gets or sets the email of the signup
        /// </summary>
        /// <value>The email.</value>
        public string Email { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether email of the signup is visible.
        /// </summary>
        /// <value><c>true</c> if [email visible]; otherwise, <c>false</c>.</value>
        public bool EmailVisible { get; set; }

        /// <summary>
        ///     Gets or sets the telephone of the signup
        /// </summary>
        /// <value>The telephone.</value>
        public string Telephone { get; set; }

        /// <summary>
        ///     Gets the event date begin.
        /// </summary>
        /// <value>The event date begin.</value>
        public DateTime EventDateBegin => this.EventTimeBegin.Date;

        /// <summary>
        ///     Gets or sets the event time begin.
        /// </summary>
        /// <value>The event time begin.</value>
        public DateTime EventTimeBegin { get; set; } = DateTime.Now;

        /// <summary>
        ///     Gets or sets the event time end.
        /// </summary>
        /// <value>The event time end.</value>
        public DateTime EventTimeEnd { get; set; }

        /// <summary>
        ///     Gets or sets the duration.
        /// </summary>
        /// <value>The duration.</value>
        public int Duration { get; set; }

        /// <summary>
        ///     Gets or sets the name of the event.
        /// </summary>
        /// <value>The name of the event.</value>
        public string EventName { get; set; }

        /// <summary>
        ///     Gets or sets the importance of the event
        /// </summary>
        /// <value>The importance.</value>
        public Priority Importance
        {
            get { return (Priority) this._importance; }
            set { this._importance = (int) value; }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the event is approved.
        /// </summary>
        /// <value><c>true</c> if [event approved]; otherwise, <c>false</c>.</value>
        public bool EventApproved { get; set; } = false;

        /// <summary>
        ///     Gets or sets the max # of enrollments
        /// </summary>
        /// <value>The max # of enrollments.</value>
        public int MaxEnrollment { get; set; } = 0;

        /// <summary>
        ///     Gets or sets the # of enrolled.
        /// </summary>
        /// <value>The # of enrolled.</value>
        public int Enrolled { get; set; }

        /// <summary>
        ///     Gets or sets the PayPal status.
        /// </summary>
        /// <value>The PayPal status.</value>
        public string PayPalStatus { get; set; }

        /// <summary>
        ///     Gets or sets the PayPal result reason
        /// </summary>
        /// <value>The PayPal reason.</value>
        public string PayPalReason { get; set; }

        /// <summary>
        ///     Gets or sets the PayPal transaction ID.
        /// </summary>
        /// <value>The PayPal transaction ID.</value>
        public string PayPalTransID { get; set; }

        /// <summary>
        ///     Gets or sets the PayPal payer ID.
        /// </summary>
        /// <value>The PayPal payer ID.</value>
        public string PayPalPayerID { get; set; }

        /// <summary>
        ///     Gets or sets the PayPal payer status.
        /// </summary>
        /// <value>The pay PayPal status.</value>
        public string PayPalPayerStatus { get; set; }

        /// <summary>
        ///     Gets or sets the PayPal reciever email.
        /// </summary>
        /// <value>The PayPal reciever email.</value>
        public string PayPalRecieverEmail { get; set; }

        /// <summary>
        ///     Gets or sets the PayPal user email.
        /// </summary>
        /// <value>The PayPal user email.</value>
        public string PayPalUserEmail { get; set; }

        /// <summary>
        ///     Gets or sets the PayPal payer email.
        /// </summary>
        /// <value>The PayPal payer email.</value>
        public string PayPalPayerEmail { get; set; }

        /// <summary>
        ///     Gets or sets the first name of the PayPal.
        /// </summary>
        /// <value>The first name of the PayPal.</value>
        public string PayPalFirstName { get; set; }

        /// <summary>
        ///     Gets or sets the last name of the PayPal.
        /// </summary>
        /// <value>The last name of the PayPal.</value>
        public string PayPalLastName { get; set; }

        /// <summary>
        ///     Gets or sets the PayPal address.
        /// </summary>
        /// <value>The PayPal address.</value>
        public string PayPalAddress { get; set; }

        /// <summary>
        ///     Gets or sets the PayPal city.
        /// </summary>
        /// <value>The PayPal city.</value>
        public string PayPalCity { get; set; }

        /// <summary>
        ///     Gets or sets the state of the PayPal.
        /// </summary>
        /// <value>The state of the PayPal.</value>
        public string PayPalState { get; set; }

        /// <summary>
        ///     Gets or sets the PayPal zip.
        /// </summary>
        /// <value>The PayPal zip.</value>
        public string PayPalZip { get; set; }

        /// <summary>
        ///     Gets or sets the PayPal country.
        /// </summary>
        /// <value>The PayPal country.</value>
        public string PayPalCountry { get; set; }

        /// <summary>
        ///     Gets or sets the PayPal currency.
        /// </summary>
        /// <value>The PayPal currency.</value>
        public string PayPalCurrency { get; set; }

        /// <summary>
        ///     Gets or sets the PayPal payment date.
        /// </summary>
        /// <value>The PayPal payment date.</value>
        public DateTime PayPalPaymentDate { get; set; } = DateTime.Now;

        /// <summary>
        ///     Gets or sets the PayPal amount.
        /// </summary>
        /// <value>The PayPal amount.</value>
        public decimal PayPalAmount { get; set; } = 0;

        /// <summary>
        ///     Gets or sets the PayPal fee.
        /// </summary>
        /// <value>The PayPal fee.</value>
        public decimal PayPalFee { get; set; } = 0;

        /// <summary>
        ///     Gets or sets the # of  enrolees.
        /// </summary>
        /// <value>The # of enrolees.</value>
        public int NoEnrolees { get; set; } = 1;

        /// <summary>
        ///     Gets or sets a value indicating what the [event timezoneid] is
        /// </summary>
        /// <value>The event timezoneid.</value>
        public string EventTimeZoneId { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating what the [anonymous email] is
        /// </summary>
        /// <value>The event timezoneid.</value>
        public string AnonEmail { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating what the [anonymous name] is
        /// </summary>
        /// <value>The event timezoneid.</value>
        public string AnonName { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating what the [anonymous telephone] is
        /// </summary>
        /// <value>The event timezoneid.</value>
        public string AnonTelephone { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating what the [anonymous Culture] is
        /// </summary>
        /// <value>The event timezoneid.</value>
        public string AnonCulture { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating what the [anonymous TimeZoneId] is
        /// </summary>
        /// <value>The event timezoneid.</value>
        public string AnonTimeZoneId { get; set; }

        /// <summary>
        ///     Gets or sets the enrollee's dirst name.
        /// </summary>
        /// <value>The enrollee's first name.</value>
        public string FirstName { get; set; }

        /// <summary>
        ///     Gets or sets the enrollee's last name.
        /// </summary>
        /// <value>The enrollee's last name.</value>
        public string LastName { get; set; }

        /// <summary>
        ///     Gets or sets the enrollee's company.
        /// </summary>
        /// <value>The enrollee's company.</value>
        public string Company { get; set; }

        /// <summary>
        ///     Gets or sets the enrollee's job title.
        /// </summary>
        /// <value>The enrollee's job title.</value>
        public string JobTitle { get; set; }

        /// <summary>
        ///     Gets or sets the enrollee's reference number.
        /// </summary>
        /// <value>The enrollee's reference number.</value>
        public string ReferenceNumber { get; set; }

        /// <summary>
        ///     Gets or sets the enrollee's remarks.
        /// </summary>
        /// <value>The enrollee's remarks.</value>
        public string Remarks { get; set; }

        /// <summary>
        ///     Gets or sets the street of the enrollee's address.
        /// </summary>
        /// <value>The street of the enrollee's address.</value>
        public string Street { get; set; }

        /// <summary>
        ///     Gets or sets the postal code of the enrollee's address.
        /// </summary>
        /// <value>The postal code of the enrollee's address.</value>
        public string PostalCode { get; set; }

        /// <summary>
        ///     Gets or sets the city of the enrollee's address.
        /// </summary>
        /// <value>The city of the enrollee's address.</value>
        public string City { get; set; }

        /// <summary>
        ///     Gets or sets the region of the enrollee's address.
        /// </summary>
        /// <value>The region of the enrollee's address.</value>
        public string Region { get; set; }

        /// <summary>
        ///     Gets or sets the country of the enrollee's address.
        /// </summary>
        /// <value>The country of the enrollee's address.</value>
        public string Country { get; set; }

        #region Public Methods

        /// <summary>
        ///     Clones an instance of the eventssignups object
        /// </summary>
        /// <returns>Cloned EventsSignupsinfo object</returns>
        public EventSignupsInfo Clone()
        {
            // create the object
            return (EventSignupsInfo) this.MemberwiseClone();
        }

        #endregion

        #region Sorting

        /// <summary>
        ///     Gets or sets the sort expression.
        /// </summary>
        /// <value>The sort expression.</value>
        public static SortFilter SortExpression { get; set; }

        /// <summary>
        ///     Gets or sets the sort direction.
        /// </summary>
        /// <value>The sort direction.</value>
        public static SortDirection SortDirection { get; set; }


        /// <summary>
        ///     Sorting enumeration
        /// </summary>
        public enum SortFilter
        {
            /// <summary>
            ///     By EventID
            /// </summary>
            EventID,

            /// <summary>
            ///     By Date beging
            /// </summary>
            EventTimeBegin,

            /// <summary>
            ///     By Date end
            /// </summary>
            EventTimeEnd,

            /// <summary>
            ///     Bu Name
            /// </summary>
            EventName,

            /// <summary>
            ///     By duration
            /// </summary>
            Duration,

            /// <summary>
            ///     By approved
            /// </summary>
            Approved
        }

        /// <summary>
        ///     Compares the current instance with another object of the same type and returns an integer that indicates whether
        ///     the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        ///     A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these
        ///     meanings:
        ///     Value
        ///     Meaning
        ///     Less than zero
        ///     This instance is less than <paramref name="obj" />.
        ///     Zero
        ///     This instance is equal to <paramref name="obj" />.
        ///     Greater than zero
        ///     This instance is greater than <paramref name="obj" />.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="obj" /> is not the same type as this instance.
        /// </exception>
        public int CompareTo(object obj)
        {
            var o = (EventSignupsInfo) obj;
            var xCompare = this.EventName + Strings.Format(this.EventID, "00000000");
            var yCompare = o.EventName + Strings.Format(o.EventID, "00000000");
            if (SortExpression == SortFilter.Duration)
            {
                xCompare = Convert.ToString(Strings.Format(this.Duration, "000000") +
                                            Strings.Format(this.EventID, "00000000"));
                yCompare = Convert.ToString(
                    Strings.Format(o.Duration, "000000") + Strings.Format(o.EventID, "00000000"));
            }
            else if (SortExpression == SortFilter.EventTimeBegin)
            {
                xCompare = Convert.ToString(Strings.Format(this.EventTimeBegin, "yyyyMMddHHmm") +
                                            Strings.Format(this.EventID, "00000000"));
                yCompare = Convert.ToString(Strings.Format(o.EventTimeBegin, "yyyyMMddHHmm") +
                                            Strings.Format(o.EventID, "00000000"));
            }
            else if (SortExpression == SortFilter.EventTimeEnd)
            {
                xCompare = Convert.ToString(Strings.Format(this.EventTimeEnd, "yyyyMMddHHmm") +
                                            Strings.Format(this.EventID, "00000000"));
                yCompare = Convert.ToString(Strings.Format(o.EventTimeEnd, "yyyyMMddHHmm") +
                                            Strings.Format(o.EventID, "00000000"));
            }
            else if (SortExpression == SortFilter.Approved)
            {
                xCompare = this.Approved + Strings.Format(this.EventID, "00000000");
                yCompare = o.Approved + Strings.Format(o.EventID, "00000000");
            }
            if (SortDirection == SortDirection.Ascending)
            {
                return string.Compare(xCompare, yCompare, StringComparison.CurrentCulture);
            }
            return string.Compare(yCompare, xCompare, StringComparison.CurrentCulture);
        }

        #endregion
    }

    #endregion

    #region EventPPErrorLogInfo Class

    /// <summary>
    ///     Information  about any infomartion during PayPal payments
    /// </summary>
    public class EventPpErrorLogInfo
    {
        // ReSharper disable ConvertToConstant.Local
        // ReSharper restore ConvertToConstant.Local

        // public properties
        /// <summary>
        ///     Gets the PayPal ID.
        /// </summary>
        /// <value>The PayPal ID.</value>
        public int PayPalID { get; } = 0;

        /// <summary>
        ///     Gets or sets the signup ID.
        /// </summary>
        /// <value>The signup ID.</value>
        public int SignupID { get; set; } = 0;

        /// <summary>
        ///     Gets the create date.
        /// </summary>
        /// <value>The create date.</value>
        public DateTime CreateDate { get; } = DateTime.Now;

        /// <summary>
        ///     Gets or sets the PayPal status.
        /// </summary>
        /// <value>The PayPal status.</value>
        public string PayPalStatus { get; set; }

        /// <summary>
        ///     Gets or sets the PayPal reason.
        /// </summary>
        /// <value>The PayPal reason.</value>
        public string PayPalReason { get; set; }

        /// <summary>
        ///     Gets or sets the PayPal trans ID.
        /// </summary>
        /// <value>The PayPal trans ID.</value>
        public string PayPalTransID { get; set; }

        /// <summary>
        ///     Gets or sets the PayPal payer ID.
        /// </summary>
        /// <value>The PayPal payer ID.</value>
        public string PayPalPayerID { get; set; }

        /// <summary>
        ///     Gets or sets the PayPal payer status.
        /// </summary>
        /// <value>The PayPal payer status.</value>
        public string PayPalPayerStatus { get; set; }

        /// <summary>
        ///     Gets or sets the PayPal reciever email.
        /// </summary>
        /// <value>The PayPal reciever email.</value>
        public string PayPalRecieverEmail { get; set; }

        /// <summary>
        ///     Gets or sets the PayPal user email.
        /// </summary>
        /// <value>The PayPal user email.</value>
        public string PayPalUserEmail { get; set; }

        /// <summary>
        ///     Gets or sets the PayPal payer email.
        /// </summary>
        /// <value>The PayPal payer email.</value>
        public string PayPalPayerEmail { get; set; }

        /// <summary>
        ///     Gets or sets the first name of the PayPal.
        /// </summary>
        /// <value>The first name of the PayPal.</value>
        public string PayPalFirstName { get; set; }

        /// <summary>
        ///     Gets or sets the last name of the PayPal.
        /// </summary>
        /// <value>The last name of the PayPal.</value>
        public string PayPalLastName { get; set; }

        /// <summary>
        ///     Gets or sets the PayPal address.
        /// </summary>
        /// <value>The PayPal address.</value>
        public string PayPalAddress { get; set; }

        /// <summary>
        ///     Gets or sets the PayPal city.
        /// </summary>
        /// <value>The PayPal city.</value>
        public string PayPalCity { get; set; }

        /// <summary>
        ///     Gets or sets the state of the PayPal.
        /// </summary>
        /// <value>The state of the PayPal.</value>
        public string PayPalState { get; set; }

        /// <summary>
        ///     Gets or sets the PayPal zip.
        /// </summary>
        /// <value>The PayPal zip.</value>
        public string PayPalZip { get; set; }

        /// <summary>
        ///     Gets or sets the PayPal country.
        /// </summary>
        /// <value>The PayPal country.</value>
        public string PayPalCountry { get; set; }

        /// <summary>
        ///     Gets or sets the PayPal currency.
        /// </summary>
        /// <value>The PayPal currency.</value>
        public string PayPalCurrency { get; set; }

        /// <summary>
        ///     Gets or sets the PayPal payment date.
        /// </summary>
        /// <value>The PayPal payment date.</value>
        public DateTime PayPalPaymentDate { get; set; } = DateTime.Now;

        /// <summary>
        ///     Gets or sets the PayPal amount.
        /// </summary>
        /// <value>The PayPal amount.</value>
        public double PayPalAmount { get; set; } = 0.0;

        /// <summary>
        ///     Gets or sets the PayPal fee.
        /// </summary>
        /// <value>The PayPal fee.</value>
        public double PayPalFee { get; set; } = 0.0;
    }

    #endregion

    #region EventCategoryInfo Class

    /// <summary>
    ///     Information about the (optional) category of the envent
    /// </summary>
    public class EventCategoryInfo
    {
        // initialization

        // public properties
        /// <summary>
        ///     Gets or sets the portal ID.
        /// </summary>
        /// <value>The portal ID.</value>
        public int PortalID { get; set; }

        /// <summary>
        ///     Gets or sets the category ID.
        /// </summary>
        /// <value>The category ID.</value>
        public int Category { get; set; }

        /// <summary>
        ///     Gets or sets the name of the category.
        /// </summary>
        /// <value>The name of the category.</value>
        public string CategoryName { get; set; }

        /// <summary>
        ///     Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        public string Color { get; set; }

        /// <summary>
        ///     Gets or sets the color of the font.
        /// </summary>
        /// <value>The color of the font.</value>
        public string FontColor { get; set; }
    }

    #endregion

    #region EventLocationInfo Class

    /// <summary>
    ///     Information about the (optional) location of the event
    /// </summary>
    public class EventLocationInfo
    {
        // initialization

        // public properties
        /// <summary>
        ///     Gets or sets the portal ID.
        /// </summary>
        /// <value>The portal ID.</value>
        public int PortalID { get; set; }

        /// <summary>
        ///     Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        public int Location { get; set; }

        /// <summary>
        ///     Gets or sets the name of the location.
        /// </summary>
        /// <value>The name of the location.</value>
        public string LocationName { get; set; }

        /// <summary>
        ///     Gets or sets the map URL.
        /// </summary>
        /// <value>The map URL.</value>
        public string MapURL { get; set; }

        /// <summary>
        ///     Gets or sets the street of the location's address.
        /// </summary>
        /// <value>The street of the location's address.</value>
        public string Street { get; set; }

        /// <summary>
        ///     Gets or sets the postal code of the location's address.
        /// </summary>
        /// <value>The postal code of the location's address.</value>
        public string PostalCode { get; set; }

        /// <summary>
        ///     Gets or sets the city of the location's address.
        /// </summary>
        /// <value>The city of the location's address.</value>
        public string City { get; set; }

        /// <summary>
        ///     Gets or sets the region of the location's address.
        /// </summary>
        /// <value>The region of the location's address.</value>
        public string Region { get; set; }

        /// <summary>
        ///     Gets or sets the country of the location's address.
        /// </summary>
        /// <value>The country of the location's address.</value>
        public string Country { get; set; }
    }

    #endregion

    #region EventNotificationInfo Class

    /// <summary>
    ///     Information for emial notification of events
    /// </summary>
    public class EventNotificationInfo
    {
        // initialization

        // public properties
        /// <summary>
        ///     Gets or sets the event ID.
        /// </summary>
        /// <value>The event ID.</value>
        public int EventID { get; set; }

        /// <summary>
        ///     Gets or sets the portal alias ID.
        /// </summary>
        /// <value>The portal alias ID.</value>
        public int PortalAliasID { get; set; }

        /// <summary>
        ///     Gets or sets the notification ID.
        /// </summary>
        /// <value>The notification ID.</value>
        public int NotificationID { get; set; }

        /// <summary>
        ///     Gets or sets the user email.
        /// </summary>
        /// <value>The user email.</value>
        public string UserEmail { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [notification sent].
        /// </summary>
        /// <value><c>true</c> if [notification sent]; otherwise, <c>false</c>.</value>
        public bool NotificationSent { get; set; } = false;

        /// <summary>
        ///     Gets or sets the notify by date time.
        /// </summary>
        /// <value>The notify by date time.</value>
        public DateTime NotifyByDateTime { get; set; }

        /// <summary>
        ///     Gets or sets the event time begin.
        /// </summary>
        /// <value>The event time begin.</value>
        public DateTime EventTimeBegin { get; set; }

        /// <summary>
        ///     Gets or sets the notify language.
        /// </summary>
        /// <value>The notify language.</value>
        public string NotifyLanguage { get; set; }

        /// <summary>
        ///     Gets or sets the module ID.
        /// </summary>
        /// <value>The module ID.</value>
        public int ModuleID { get; set; }

        /// <summary>
        ///     Gets or sets the tab ID.
        /// </summary>
        /// <value>The tab ID.</value>
        public int TabID { get; set; }
    }

    #endregion

    #region EventRecurMasterInfo Class

    /// <summary>
    ///     Master record for recurring events, holds a set of events together
    /// </summary>
    public class EventRecurMasterInfo
    {
        /// <summary>
        ///     Priority of the (master) events
        /// </summary>
        public enum Priority
        {
            /// <summary>
            ///     High priority
            /// </summary>
            High = 1,

            /// <summary>
            ///     Medium priority
            /// </summary>
            Medium = 2,

            /// <summary>
            ///     Low priority
            /// </summary>
            Low = 3
        }

        private int _enrolled;
        private string _eventTimeZoneId = string.Empty;
        private int _importance;

        /// <summary>
        ///     Gets or sets the recur master ID.
        /// </summary>
        /// <value>The recur master ID.</value>
        public int RecurMasterID { get; set; }

        /// <summary>
        ///     Gets or sets the module ID.
        /// </summary>
        /// <value>The module ID.</value>
        public int ModuleID { get; set; }

        /// <summary>
        ///     Gets or sets the portal ID.
        /// </summary>
        /// <value>The portal ID.</value>
        public int PortalID { get; set; }

        /// <summary>
        ///     Gets or sets the Recurrence rule.
        /// </summary>
        /// <value>The recurrence rule.</value>
        public string RRULE { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the Date start.
        /// </summary>
        /// <value>The start date.</value>
        public DateTime Dtstart { get; set; }

        /// <summary>
        ///     Gets or sets the duration.
        /// </summary>
        /// <value>The duration.</value>
        public string Duration { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the until.
        /// </summary>
        /// <value>The until.</value>
        public DateTime Until { get; set; }

        /// <summary>
        ///     Gets or sets the name of the event.
        /// </summary>
        /// <value>The name of the event.</value>
        public string EventName { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the event description
        /// </summary>
        /// <value>The event description.</value>
        public string EventDesc { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the importance.
        /// </summary>
        /// <value>The importance.</value>
        public Priority Importance
        {
            get { return (Priority) this._importance; }
            set { this._importance = (int) value; }
        }

        /// <summary>
        ///     Gets or sets the notify.
        /// </summary>
        /// <value>The notify.</value>
        public string Notify { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="EventRecurMasterInfo" /> is approved.
        /// </summary>
        /// <value><c>true</c> if approved; otherwise, <c>false</c>.</value>
        public bool Approved { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="EventRecurMasterInfo" /> is signups.
        /// </summary>
        /// <value><c>true</c> if signups; otherwise, <c>false</c>.</value>
        public bool Signups { get; set; }

        /// <summary>
        ///     Gets or sets the max # of enrollment.
        /// </summary>
        /// <value>The max # of enrollment.</value>
        public int MaxEnrollment { get; set; }

        /// <summary>
        ///     Gets or sets the # enrolled.
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
            set { this._enrolled = value; }
        }

        /// <summary>
        ///     Gets or sets the enroll role ID.
        /// </summary>
        /// <value>The enroll role ID.</value>
        public int EnrollRoleID { get; set; } = 0;

        /// <summary>
        ///     Gets or sets the type of the enroll.
        /// </summary>
        /// <value>The type of the enroll.</value>
        public string EnrollType { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the enroll fee.
        /// </summary>
        /// <value>The enroll fee.</value>
        public decimal EnrollFee { get; set; }

        /// <summary>
        ///     Gets or sets the pay pal account.
        /// </summary>
        /// <value>The pay pal account.</value>
        public string PayPalAccount { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets a value indicating whether to display on a detail page.
        /// </summary>
        /// <value><c>true</c> if [detail page]; otherwise, <c>false</c>.</value>
        public bool DetailPage { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to dispay the event in a new page.
        /// </summary>
        /// <value><c>true</c> if [detail new page]; otherwise, <c>false</c>.</value>
        public bool DetailNewWin { get; set; }

        /// <summary>
        ///     Gets or sets the detail URL.
        /// </summary>
        /// <value>The detail URL.</value>
        public string DetailURL { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the image URL.
        /// </summary>
        /// <value>The image URL.</value>
        public string ImageURL { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the type of the image.
        /// </summary>
        /// <value>The type of the image.</value>
        public string ImageType { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the width of the image.
        /// </summary>
        /// <value>The width of the image.</value>
        public int ImageWidth { get; set; }

        /// <summary>
        ///     Gets or sets the height of the image.
        /// </summary>
        /// <value>The height of the image.</value>
        public int ImageHeight { get; set; }

        /// <summary>
        ///     Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        public int Location { get; set; }

        /// <summary>
        ///     Gets or sets the category.
        /// </summary>
        /// <value>The category.</value>
        public int Category { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [image display].
        /// </summary>
        /// <value><c>true</c> if [image display]; otherwise, <c>false</c>.</value>
        public bool ImageDisplay { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [send reminder].
        /// </summary>
        /// <value><c>true</c> if [send reminder]; otherwise, <c>false</c>.</value>
        public bool SendReminder { get; set; }

        /// <summary>
        ///     Gets or sets the reminder time.
        /// </summary>
        /// <value>The reminder time.</value>
        public int ReminderTime { get; set; }

        /// <summary>
        ///     Gets or sets the reminder time measurement.
        /// </summary>
        /// <value>The reminder time measurement.</value>
        public string ReminderTimeMeasurement { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the reminder.
        /// </summary>
        /// <value>The reminder.</value>
        public string Reminder { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the reminder from.
        /// </summary>
        /// <value>The reminder from.</value>
        public string ReminderFrom { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the custom field1.
        /// </summary>
        /// <value>The custom field1.</value>
        public string CustomField1 { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the custom field2.
        /// </summary>
        /// <value>The custom field2.</value>
        public string CustomField2 { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets a value indicating whether [enroll list view].
        /// </summary>
        /// <value><c>true</c> if [enroll list view]; otherwise, <c>false</c>.</value>
        public bool EnrollListView { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [display end date].
        /// </summary>
        /// <value><c>true</c> if [display end date]; otherwise, <c>false</c>.</value>
        public bool DisplayEndDate { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [all day event].
        /// </summary>
        /// <value><c>true</c> if [all day event]; otherwise, <c>false</c>.</value>
        public bool AllDayEvent { get; set; }

        /// <summary>
        ///     Gets or sets the name of the culture.
        /// </summary>
        /// <value>The name of the culture.</value>
        public string CultureName { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the owner ID.
        /// </summary>
        /// <value>The owner ID.</value>
        public int OwnerID { get; set; }

        /// <summary>
        ///     Gets or sets the created by ID.
        /// </summary>
        /// <value>The created by ID.</value>
        public int CreatedByID { get; set; }

        /// <summary>
        ///     Gets or sets the created date.
        /// </summary>
        /// <value>The created date.</value>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        ///     Gets or sets the updated by ID.
        /// </summary>
        /// <value>The updated by ID.</value>
        public int UpdatedByID { get; set; }

        /// <summary>
        ///     Gets or sets the updated date.
        /// </summary>
        /// <value>The updated date.</value>
        public DateTime UpdatedDate { get; set; }

        /// <summary>
        ///     Gets or sets the first event ID.
        /// </summary>
        /// <value>The first event ID.</value>
        public int FirstEventID { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating what the [event timezoneid] is
        /// </summary>
        /// <value>The event timezoneid.</value>
        public string EventTimeZoneId
        {
            get
                {
                    if (string.IsNullOrEmpty(this._eventTimeZoneId))
                    {
                        var modSettings = EventModuleSettings.GetEventModuleSettings(this.ModuleID, null);
                        this._eventTimeZoneId = modSettings.TimeZoneId;
                    }
                    return this._eventTimeZoneId;
                }
            set { this._eventTimeZoneId = value; }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether [allow anonymous enrollment].
        /// </summary>
        /// <value><c>true</c> if [allow anonymous enrollment]; otherwise, <c>false</c>.</value>
        public bool AllowAnonEnroll { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating the [content item id].
        /// </summary>
        /// <value>The contentitemid.</value>
        public int ContentItemID { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [event has journal item].
        /// </summary>
        /// <value><c>true</c> if [event has journal item]; otherwise, <c>false</c>.</value>
        public bool JournalItem { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating the [socialgroup id].
        /// </summary>
        /// <value>The SocialGroupid.</value>
        public int SocialGroupID { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating the [social user id].
        /// </summary>
        /// <value>The SocialUserid.</value>
        public int SocialUserID { get; set; }

        /// <summary>
        ///     Gets or sets the summary.
        /// </summary>
        /// <value>The summary.</value>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets a value indicating the [sequence].
        /// </summary>
        /// <value>The Sequence.</value>
        public int Sequence { get; set; }
    }

    #endregion

    #region EventRRULEInfo Class

    /// <summary>
    ///     Information about the recurrrence rules
    /// </summary>
    public class EventRRULEInfo
    {
        /// <summary>
        ///     Gets or sets the frequency
        /// </summary>
        /// <value>The frequency.</value>
        public string Freq { get; set; }

        /// <summary>
        ///     Gets or sets the interval.
        /// </summary>
        /// <value>The interval.</value>
        public int Interval { get; set; }

        /// <summary>
        ///     Gets or sets the by day.
        /// </summary>
        /// <value>The by day.</value>
        public string ByDay { get; set; }

        /// <summary>
        ///     Gets or sets the by day of the month.
        /// </summary>
        /// <value>The by day of the month.</value>
        public int ByMonthDay { get; set; }

        /// <summary>
        ///     Gets or sets the by month.
        /// </summary>
        /// <value>The by month.</value>
        public int ByMonth { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="EventRRULEInfo" /> is sunday
        /// </summary>
        /// <value><c>true</c> if sunday; otherwise, <c>false</c>.</value>
        public bool Su { get; set; }

        /// <summary>
        ///     Gets or sets the no sunday
        /// </summary>
        /// <value>The no sunday</value>
        public int SuNo { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="EventRRULEInfo" /> is monday
        /// </summary>
        /// <value><c>true</c> if monday; otherwise, <c>false</c>.</value>
        public bool Mo { get; set; }

        /// <summary>
        ///     Gets or sets the no monday
        /// </summary>
        /// <value>The no monday</value>
        public int MoNo { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="EventRRULEInfo" /> is tuesday
        /// </summary>
        /// <value><c>true</c> if tuesday; otherwise, <c>false</c>.</value>
        public bool Tu { get; set; }

        /// <summary>
        ///     Gets or sets the no tuesday
        /// </summary>
        /// <value>The no tuesday.</value>
        public int TuNo { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="EventRRULEInfo" /> is wednessday
        /// </summary>
        /// <value><c>true</c> if wednessday; otherwise, <c>false</c>.</value>
        public bool We { get; set; }

        /// <summary>
        ///     Gets or sets the no wednessday.
        /// </summary>
        /// <value>The no wednessday.</value>
        public int WeNo { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="EventRRULEInfo" /> is thursday.
        /// </summary>
        /// <value><c>true</c> if thursday; otherwise, <c>false</c>.</value>
        public bool Th { get; set; }

        /// <summary>
        ///     Gets or sets the no thursday
        /// </summary>
        /// <value>The no thursday.</value>
        public int ThNo { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="EventRRULEInfo" /> is friday.
        /// </summary>
        /// <value><c>true</c> if friday; otherwise, <c>false</c>.</value>
        public bool Fr { get; set; }

        /// <summary>
        ///     Gets or sets the no friday.
        /// </summary>
        /// <value>The no friday.</value>
        public int FrNo { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="EventRRULEInfo" /> is saturday.
        /// </summary>
        /// <value><c>true</c> if saturday; otherwise, <c>false</c>.</value>
        public bool Sa { get; set; }

        /// <summary>
        ///     Gets or sets the no saturday.
        /// </summary>
        /// <value>The no saturday.</value>
        public int SaNo { get; set; }

        public bool FreqBasic { get; set; }

        public string Wkst { get; set; }
    }

    #endregion

    #region EventEmailInfo Class

    /// <summary>
    ///     Information abotu e-mails related to events
    /// </summary>
    public class EventEmailInfo
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EventEmailInfo" /> class.
        /// </summary>
        public EventEmailInfo()
        {
            var newUserEmails = new ArrayList();
            this.UserEmails = newUserEmails;

            var newUserIDs = new ArrayList();
            this.UserIDs = newUserIDs;

            var newUserLocales = new ArrayList();
            this.UserLocales = newUserLocales;

            var newUserTimeZoneIds = new ArrayList();
            this.UserTimeZoneIds = newUserTimeZoneIds;
        }

        /// <summary>
        ///     Gets or sets the  email subject.
        /// </summary>
        /// <value>The  email subject.</value>
        public string TxtEmailSubject { get; set; }

        /// <summary>
        ///     Gets or sets the  email body.
        /// </summary>
        /// <value>The email body.</value>
        public string TxtEmailBody { get; set; }

        /// <summary>
        ///     Gets or sets the email from.
        /// </summary>
        /// <value>The email from.</value>
        public string TxtEmailFrom { get; set; }

        /// <summary>
        ///     Gets or sets the user I ds.
        /// </summary>
        /// <value>The user I ds.</value>
        public ArrayList UserIDs { get; set; }

        /// <summary>
        ///     Gets or sets the user emails.
        /// </summary>
        /// <value>The user emails.</value>
        public ArrayList UserEmails { get; set; }

        /// <summary>
        ///     Gets or sets the user locales.
        /// </summary>
        /// <value>The user locales.</value>
        public ArrayList UserLocales { get; set; }

        /// <summary>
        ///     Gets or sets the user timezoneids.
        /// </summary>
        /// <value>The user timezoneids.</value>
        public ArrayList UserTimeZoneIds { get; set; }
    }

    #endregion

    #region EventSubscriptionInfo Class

    /// <summary>
    ///     Information about subscription o events
    /// </summary>
    public class EventSubscriptionInfo
    {
        // initialization

        // public properties
        /// <summary>
        ///     Gets or sets the portal ID.
        /// </summary>
        /// <value>The portal ID.</value>
        public int PortalID { get; set; }

        /// <summary>
        ///     Gets or sets the subscription ID.
        /// </summary>
        /// <value>The subscription ID.</value>
        public int SubscriptionID { get; set; }

        /// <summary>
        ///     Gets or sets the module ID.
        /// </summary>
        /// <value>The module ID.</value>
        public int ModuleID { get; set; }

        /// <summary>
        ///     Gets or sets the user ID.
        /// </summary>
        /// <value>The user ID.</value>
        public int UserID { get; set; }
    }

    #endregion

    #region EventListObject Class

    /// <summary>
    ///     Object for listing the events
    /// </summary>
    public class EventListObject : IComparable
    {
        /// <summary>
        ///     Sorting enumeration
        /// </summary>
        public enum SortFilter
        {
            /// <summary>
            ///     By EventID
            /// </summary>
            EventID,

            /// <summary>
            ///     By Date beging
            /// </summary>
            EventDateBegin,

            /// <summary>
            ///     By Date end
            /// </summary>
            EventDateEnd,

            /// <summary>
            ///     Bu Name
            /// </summary>
            EventName,

            /// <summary>
            ///     By duration
            /// </summary>
            Duration,

            /// <summary>
            ///     Bu category name
            /// </summary>
            CategoryName,

            /// <summary>
            ///     By customfield1
            /// </summary>
            CustomField1,

            /// <summary>
            ///     By customfield2
            /// </summary>
            CustomField2,

            /// <summary>
            ///     By description
            /// </summary>
            Description,

            /// <summary>
            ///     By Location name
            /// </summary>
            LocationName
        }


        // public properties

        /// <summary>
        ///     Gets or sets the index id.
        /// </summary>
        /// <value>The index id.</value>
        public int IndexId { get; set; }

        /// <summary>
        ///     Gets or sets the event ID.
        /// </summary>
        /// <value>The event ID.</value>
        public int EventID { get; set; }

        /// <summary>
        ///     Gets or sets the created by ID.
        /// </summary>
        /// <value>The created by ID.</value>
        public int CreatedByID { get; set; }

        /// <summary>
        ///     Gets or sets the owner ID.
        /// </summary>
        /// <value>The owner ID.</value>
        public int OwnerID { get; set; }

        /// <summary>
        ///     Gets or sets the module ID.
        /// </summary>
        /// <value>The module ID.</value>
        public int ModuleID { get; set; }

        /// <summary>
        ///     Gets or sets the event date begin.
        /// </summary>
        /// <value>The event date begin.</value>
        public DateTime EventDateBegin { get; set; }

        /// <summary>
        ///     Gets or sets the event date end.
        /// </summary>
        /// <value>The event date end.</value>
        public DateTime EventDateEnd { get; set; }

        /// <summary>
        ///     Gets or sets the event date end.
        /// </summary>
        /// <value>The event date end.</value>
        public string TxtEventDateEnd { get; set; }

        /// <summary>
        ///     Gets or sets the event time begin.
        /// </summary>
        /// <value>The event time begin.</value>
        public DateTime EventTimeBegin { get; set; }

        /// <summary>
        ///     Gets or sets the event time begin.
        /// </summary>
        /// <value>The event time begin.</value>
        public string TxtEventTimeBegin { get; set; }

        /// <summary>
        ///     Gets or sets the recurrence until.
        /// </summary>
        /// <value>The recurrence until.</value>
        public string RecurUntil { get; set; }

        /// <summary>
        ///     Gets or sets the duration.
        /// </summary>
        /// <value>The duration.</value>
        public int Duration { get; set; }

        /// <summary>
        ///     Gets or sets the name of the event.
        /// </summary>
        /// <value>The name of the event.</value>
        public string EventName { get; set; }

        /// <summary>
        ///     Gets or sets the event description
        /// </summary>
        /// <value>The event description.</value>
        public string EventDesc { get; set; }

        /// <summary>
        ///     Gets or sets the decoded description.
        /// </summary>
        /// <value>The decoded description.</value>
        public string DecodedDesc { get; set; }

        /// <summary>
        ///     Gets or sets the recurrence text.
        /// </summary>
        /// <value>The recurrence text.</value>
        public string RecurText { get; set; }

        /// <summary>
        ///     Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string URL { get; set; }

        /// <summary>
        ///     Gets or sets the target.
        /// </summary>
        /// <value>The target.</value>
        public string Target { get; set; }

        /// <summary>
        ///     Gets or sets the image URL.
        /// </summary>
        /// <value>The image URL.</value>
        public string ImageURL { get; set; }

        /// <summary>
        ///     Gets or sets the name of the category.
        /// </summary>
        /// <value>The name of the category.</value>
        public string CategoryName { get; set; }

        /// <summary>
        ///     Gets or sets the name of the location.
        /// </summary>
        /// <value>The name of the location.</value>
        public string LocationName { get; set; }

        /// <summary>
        ///     Gets or sets the custom field1.
        /// </summary>
        /// <value>The custom field1.</value>
        public string CustomField1 { get; set; }

        /// <summary>
        ///     Gets or sets the custom field2.
        /// </summary>
        /// <value>The custom field2.</value>
        public string CustomField2 { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether visibility is editable.
        /// </summary>
        /// <value><c>true</c> if [edit visibility]; otherwise, <c>false</c>.</value>
        public bool EditVisibility { get; set; }

        /// <summary>
        ///     Gets or sets the color of the category.
        /// </summary>
        /// <value>The color of the category.</value>
        public Color CategoryColor { get; set; }

        /// <summary>
        ///     Gets or sets the color of the category font.
        /// </summary>
        /// <value>The color of the category font.</value>
        public Color CategoryFontColor { get; set; }

        /// <summary>
        ///     Gets or sets the display duration.
        /// </summary>
        /// <value>The display duration.</value>
        public int DisplayDuration { get; set; }

        /// <summary>
        ///     Gets or sets the recur master ID.
        /// </summary>
        /// <value>The recur master ID.</value>
        public int RecurMasterID { get; set; }

        /// <summary>
        ///     Gets or sets the icons.
        /// </summary>
        /// <value>The icons.</value>
        public string Icons { get; set; }

        /// <summary>
        ///     Gets or sets the tooltip.
        /// </summary>
        /// <value>The tooltip.</value>
        public string Tooltip { get; set; }

        /// <summary>
        ///     Gets or sets the sort expression.
        /// </summary>
        /// <value>The sort expression.</value>
        public static SortFilter SortExpression { get; set; }

        /// <summary>
        ///     Gets or sets the sort direction.
        /// </summary>
        /// <value>The sort direction.</value>
        public static SortDirection SortDirection { get; set; }

        /// <summary>
        ///     Compares the current instance with another object of the same type and returns an integer that indicates whether
        ///     the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        ///     A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these
        ///     meanings:
        ///     Value
        ///     Meaning
        ///     Less than zero
        ///     This instance is less than <paramref name="obj" />.
        ///     Zero
        ///     This instance is equal to <paramref name="obj" />.
        ///     Greater than zero
        ///     This instance is greater than <paramref name="obj" />.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="obj" /> is not the same type as this instance.
        /// </exception>
        public int CompareTo(object obj)
        {
            var o = (EventListObject) obj;
            var xCompare = this.EventName + Strings.Format(this.EventID, "00000000");
            var yCompare = o.EventName + Strings.Format(o.EventID, "00000000");
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
                xCompare = Convert.ToString(Strings.Format(this.Duration, "000000") +
                                            Strings.Format(this.EventID, "00000000"));
                yCompare = Convert.ToString(
                    Strings.Format(o.Duration, "000000") + Strings.Format(o.EventID, "00000000"));
            }
            else if (SortExpression == SortFilter.EventDateBegin)
            {
                xCompare = Convert.ToString(Strings.Format(this.EventDateBegin, "yyyyMMddHHmm") +
                                            Strings.Format(this.EventID, "00000000"));
                yCompare = Convert.ToString(Strings.Format(o.EventDateBegin, "yyyyMMddHHmm") +
                                            Strings.Format(o.EventID, "00000000"));
            }
            else if (SortExpression == SortFilter.EventDateEnd)
            {
                xCompare = Convert.ToString(Strings.Format(this.EventDateEnd, "yyyyMMddHHmm") +
                                            Strings.Format(this.EventID, "00000000"));
                yCompare = Convert.ToString(Strings.Format(o.EventDateEnd, "yyyyMMddHHmm") +
                                            Strings.Format(o.EventID, "00000000"));
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
            if (SortDirection == SortDirection.Ascending)
            {
                return string.Compare(xCompare, yCompare, StringComparison.CurrentCulture);
            }
            return string.Compare(yCompare, xCompare, StringComparison.CurrentCulture);
        }
    }

    #endregion

    #region EventUser Class

    /// <summary>
    ///     user related to an event
    /// </summary>
    public class EventUser
    {
        /// <summary>
        ///     Gets or sets the user ID.
        /// </summary>
        /// <value>The user ID.</value>
        public int UserID { get; set; }

        /// <summary>
        ///     Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; set; }

        /// <summary>
        ///     Gets or sets the profile URL.
        /// </summary>
        /// <value>The profile URL.</value>
        public string ProfileURL { get; set; }

        /// <summary>
        ///     Gets or sets the display name URL.
        /// </summary>
        /// <value>The display name URL.</value>
        public string DisplayNameURL { get; set; }
    }

    #endregion

    #region EventEnrollList Class

    /// <summary>
    ///     List of users enrolled into an event
    /// </summary>
    public class EventEnrollList
    {
        /// <summary>
        ///     Gets or sets the signup ID.
        /// </summary>
        /// <value>The signup ID.</value>
        public int SignupID { get; set; }

        /// <summary>
        ///     Gets or sets the name of the enroll user.
        /// </summary>
        /// <value>The name of the enroll user.</value>
        public string EnrollUserName { get; set; }

        /// <summary>
        ///     Gets or sets the display name of the enroll.
        /// </summary>
        /// <value>The display name of the enroll.</value>
        public string EnrollDisplayName { get; set; }

        /// <summary>
        ///     Gets or sets the enroll email.
        /// </summary>
        /// <value>The enroll email.</value>
        public string EnrollEmail { get; set; }

        /// <summary>
        ///     Gets or sets the enroll phone.
        /// </summary>
        /// <value>The enroll phone.</value>
        public string EnrollPhone { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [enroll approved].
        /// </summary>
        /// <value><c>true</c> if [enroll approved]; otherwise, <c>false</c>.</value>
        public bool EnrollApproved { get; set; }

        /// <summary>
        ///     Gets or sets the enroll no.
        /// </summary>
        /// <value>The enroll no.</value>
        public int EnrollNo { get; set; }

        /// <summary>
        ///     Gets or sets the enroll time begin.
        /// </summary>
        /// <value>The enroll time begin.</value>
        public DateTime EnrollTimeBegin { get; set; }
    }

    #endregion
}