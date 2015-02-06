'
' DotNetNuke® - http://www.dnnsoftware.com
' Copyright (c) 2002-2013
' by DNNCorp
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'
Imports System
Imports System.DateTime


Namespace DotNetNuke.Modules.Events

#Region "EventInfo Class "

    Public Class EventInfo
        Implements IComparable
        ''' <summary>
        ''' Priority setting for events, can be used to display appropriate icons in an event
        ''' </summary>
        Public Enum Priority
            ''' <summary>
            ''' High priority
            ''' </summary>
            High = 1
            ''' <summary>
            ''' Medium priority
            ''' </summary>
            Medium = 2
            ''' <summary>
            ''' Low priority
            ''' </summary>
            Low = 3
        End Enum

#Region "Private Members"
        Private _portalID As Integer
        Private _eventID As Integer
        Private _recurMasterID As Integer
        Private _moduleID As Integer
        Private _eventDateEnd As DateTime
        Private _eventTimeBegin As DateTime
        Private _duration As Integer
        Private _eventName As String
        Private _eventDesc As String
        Private _importance As Integer
        Private _createdDate As DateTime
        Private _createdBy As String
        Private _createdByID As Integer
        ' ReSharper disable UnassignedField.Local
        Private _every As Integer
        Private _period As String
        Private _repeatType As String
        ' ReSharper restore UnassignedField.Local
        Private _notify As String
        Private _approved As Boolean
        Private _maxEnrollment As Integer
        Private _signups As Boolean
        Private _enrolled As Integer
        Private _noOfRecurrences As Integer
        Private _lastRecurrence As DateTime
        Private _enrollRoleID As Integer = Nothing
        Private _enrollType As String
        Private _enrollFee As Decimal
        Private _payPalAccount As String
        Private _cancelled As Boolean
        Private _detailPage As Boolean
        Private _detailNewWin As Boolean
        Private _detailURL As String
        Private _imageURL As String
        Private _imageType As String
        Private _imageWidth As Integer
        Private _imageHeight As Integer
        Private _location As Integer
        Private _locationName As String
        Private _mapURL As String
        Private _category As Integer
        Private _categoryName As String
        Private _color As String
        Private _fontColor As String
        Private _imageDisplay As Boolean
        Private _sendReminder As Boolean
        Private _reminderTime As Integer
        Private _reminderTimeMeasurement As String
        Private _reminder As String
        Private _reminderFrom As String
        Private _searchSubmitted As Boolean
        Private _moduleTitle As String
        Private _customField1 As String
        Private _customField2 As String
        Private _enrollListView As Boolean
        Private _displayEndDate As Boolean
        Private _allDayEvent As Boolean
        Private _ownerID As Integer
        Private _ownerName As String
        Private _lastUpdatedAt As DateTime
        Private _lastUpdatedBy As String
        Private _lastUpdatedID As Integer
        Private _originalDateBegin As DateTime
        Private _updateStatus As String
        Private _rrule As String
        Private _rmOwnerID As Integer
        Private _newEventEmailSent As Boolean
        Private _isPrivate As Boolean
        Private _eventTimeZoneId As String
        Private _otherTimeZoneId As String
        Private _allowAnonEnroll As Boolean
        Private _contentItemId As Integer
        Private _journalItem As Boolean
        Private _socialGroupId As Integer
        Private _socialUserId As Integer
        Private _summary As String
        Private _sequence As Integer
        Private _rmSequence As Integer
        Private _socialUserUserName As String
        Private _socialUserDisplayName As String


#End Region

#Region "Constructors"

        ' initialization
        ''' <summary>
        ''' Initializes a new instance of the <see cref="EventInfo" /> class.
        ''' </summary>
        Public Sub New()
        End Sub
#End Region

#Region "Public Properties"

        ' public properties
        ''' <summary>
        ''' Gets or sets the portal ID.
        ''' </summary>
        ''' <value>The portal ID.</value>
        Property PortalID() As Integer
            Get
                Return _PortalID
            End Get
            Set(ByVal value As Integer)
                _portalID = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the event ID.
        ''' </summary>
        ''' <value>The event ID.</value>
        Property EventID() As Integer
            Get
                Return _eventID
            End Get
            Set(ByVal value As Integer)
                _eventID = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the recurring master ID.
        ''' </summary>
        ''' <value>The recur master ID.</value>
        Property RecurMasterID() As Integer
            Get
                Return _recurMasterID
            End Get
            Set(ByVal value As Integer)
                _recurMasterID = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the module ID.
        ''' </summary>
        ''' <value>The module ID.</value>
        Property ModuleID() As Integer
            Get
                Return _moduleID
            End Get
            Set(ByVal value As Integer)
                _moduleID = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the event date end.
        ''' </summary>
        ''' <value>The event date end.</value>
        <Obsolete("EventDateEnd is only retained for upgrade from versions prior to 4.1.0")>
        Property EventDateEnd() As Date
            Get
                Return _eventDateEnd
            End Get
            Set(ByVal value As Date)
                _eventDateEnd = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the event time begin.
        ''' </summary>
        ''' <value>The event time begin.</value>
        Property EventTimeBegin() As DateTime
            Get
                Return _eventTimeBegin
            End Get
            Set(ByVal value As DateTime)
                _eventTimeBegin = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets the event time end.
        ''' </summary>
        ''' <value>The event time end.</value>
        ReadOnly Property EventTimeEnd() As DateTime
            Get
                Return EventTimeBegin.AddMinutes(Duration)
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the duration of an event.
        ''' </summary>
        ''' <value>The duration.</value>
        Property Duration() As Integer
            Get
                Return _duration
            End Get
            Set(ByVal value As Integer)
                _duration = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the name of the event.
        ''' </summary>
        ''' <value>The name of the event.</value>
        Property EventName() As String
            Get
                Return _eventName
            End Get
            Set(ByVal value As String)
                _eventName = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the event description
        ''' </summary>
        ''' <value>The event description.</value>
        Property EventDesc() As String
            Get
                Return _eventDesc
            End Get
            Set(ByVal value As String)
                _eventDesc = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the importance of an event
        ''' </summary>
        ''' <value>The importance of an event.</value>
        Property Importance() As Priority
            Get
                Return CType(_importance, Priority)
            End Get
            Set(ByVal value As Priority)
                _importance = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the created date of an event
        ''' </summary>
        ''' <value>The created date of an event.</value>
        Property CreatedDate() As DateTime
            Get
                Return _createdDate
            End Get
            Set(ByVal value As Date)
                _createdDate = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the created by of an event.
        ''' </summary>
        ''' <value>The created by of an event.</value>
        Property CreatedBy() As String
            Get
                Return _createdBy
            End Get
            Set(ByVal value As String)
                _createdBy = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the created by UserID.
        ''' </summary>
        ''' <value>The created by UserID.</value>
        Property CreatedByID() As Integer
            Get
                Return _createdByID
            End Get
            Set(ByVal value As Integer)
                _createdByID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the recurrence unit of an event
        ''' </summary>
        ''' <value>The recurrence unit of an event.</value>
        <Obsolete("Every is only retained for upgrade from versions prior to 4.1.0")>
        ReadOnly Property Every() As Integer
            Get
                Return _every
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the period of a recurrence.
        ''' </summary>
        ''' <value>The period of a recurrnence.</value>
        <Obsolete("Period is only retained for upgrade from versions prior to 4.1.0")>
        ReadOnly Property Period() As String
            Get
                Return _period
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the type of the repeat of a recurrence
        ''' </summary>
        ''' <value>The type of the repeat of a recurrence.</value>
        <Obsolete("RepeatType is only retained for upgrade from versions prior to 4.1.0")>
        ReadOnly Property RepeatType() As String
            Get
                Return _repeatType
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the notification text
        ''' </summary>
        ''' <value>The notification text.</value>
        Property Notify() As String
            Get
                Return _notify
            End Get
            Set(ByVal value As String)
                _notify = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether this <see cref="EventInfo" /> is approved.
        ''' </summary>
        ''' <value><c>true</c> if approved; otherwise, <c>false</c>.</value>
        Property Approved() As Boolean
            Get
                Return _approved
            End Get
            Set(ByVal value As Boolean)
                _approved = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether this <see cref="EventInfo" /> can have signups.
        ''' </summary>
        ''' <value><c>true</c> can have signups; otherwise, <c>false</c>.</value>
        Property Signups() As Boolean
            Get
                Return _signups
            End Get
            Set(ByVal value As Boolean)
                _signups = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the max # of enrollments
        ''' </summary>
        ''' <value>The max # of enrollments</value>
        Property MaxEnrollment() As Integer
            Get
                Return _maxEnrollment
            End Get
            Set(ByVal value As Integer)
                _maxEnrollment = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the # of enrolled people
        ''' </summary>
        ''' <value>The # enrolled of enrolled people.</value>
        Property Enrolled() As Integer
            Get
                If _enrolled < 0 Then
                    Return 0
                End If
                Return _enrolled
            End Get
            Set(ByVal value As Integer)
                _enrolled = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the # f recurrences.
        ''' </summary>
        ''' <value>The # of recurrences.</value>
        Property NoOfRecurrences() As Integer
            Get
                Return _noOfRecurrences
            End Get
            Set(ByVal value As Integer)
                _noOfRecurrences = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the last recurrence date
        ''' </summary>
        ''' <value>The last recurrence date.</value>
        Property LastRecurrence() As Date
            Get
                Return _lastRecurrence
            End Get
            Set(ByVal value As Date)
                _lastRecurrence = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the enroll role ID.
        ''' </summary>
        ''' <value>The enroll role ID.</value>
        Property EnrollRoleID() As Integer
            Get
                Return _enrollRoleID
            End Get
            Set(ByVal value As Integer)
                _enrollRoleID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the type of the enrollment
        ''' </summary>
        ''' <value>The type of the enrollment</value>
        Property EnrollType() As String
            Get
                Return _enrollType
            End Get
            Set(ByVal value As String)
                _enrollType = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the enroll fee.
        ''' </summary>
        ''' <value>The enroll fee.</value>
        Property EnrollFee() As Decimal
            Get
                Return _enrollFee
            End Get
            Set(ByVal value As Decimal)
                _enrollFee = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the PayPal account.
        ''' </summary>
        ''' <value>The PayPal account.</value>
        Property PayPalAccount() As String
            Get
                Return _payPalAccount
            End Get
            Set(ByVal value As String)
                _payPalAccount = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether this <see cref="EventInfo" /> is cancelled.
        ''' </summary>
        ''' <value><c>true</c> if cancelled; otherwise, <c>false</c>.</value>
        Property Cancelled() As Boolean
            Get
                Return _cancelled
            End Get
            Set(ByVal value As Boolean)
                _cancelled = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether a detail page must be openend.
        ''' </summary>
        ''' <value><c>true</c> if detail page must be opened; otherwise, <c>false</c>.</value>
        Property DetailPage() As Boolean
            Get
                Return _detailPage
            End Get
            Set(ByVal value As Boolean)
                _detailPage = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether detail page must appear in a new page.
        ''' </summary>
        ''' <value><c>true</c> if new page; otherwise, <c>false</c>.</value>
        Property DetailNewWin() As Boolean
            Get
                Return _detailNewWin
            End Get
            Set(ByVal value As Boolean)
                _detailNewWin = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the detail URL.
        ''' </summary>
        ''' <value>The detail URL.</value>
        Property DetailURL() As String
            Get
                Return _detailURL
            End Get
            Set(ByVal value As String)
                _detailURL = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the image URL.
        ''' </summary>
        ''' <value>The image URL.</value>
        Property ImageURL() As String
            Get
                Return _imageURL
            End Get
            Set(ByVal value As String)
                _imageURL = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the type of the image.
        ''' </summary>
        ''' <value>The type of the image.</value>
        Property ImageType() As String
            Get
                Return _imageType
            End Get
            Set(ByVal value As String)
                _imageType = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the width of the image.
        ''' </summary>
        ''' <value>The width of the image.</value>
        Property ImageWidth() As Integer
            Get
                Return _imageWidth
            End Get
            Set(ByVal value As Integer)
                _imageWidth = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the height of the image.
        ''' </summary>
        ''' <value>The height of the image.</value>
        Property ImageHeight() As Integer
            Get
                Return _imageHeight
            End Get
            Set(ByVal value As Integer)
                _imageHeight = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the location of an event
        ''' </summary>
        ''' <value>The location of an event.</value>
        Property Location() As Integer
            Get
                Return _location
            End Get
            Set(ByVal value As Integer)
                _location = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the name of the location of an event.
        ''' </summary>
        ''' <value>The name of the location of an event.</value>
        Property LocationName() As String
            Get
                Return _locationName
            End Get
            Set(ByVal value As String)
                _locationName = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the map URL.
        ''' </summary>
        ''' <value>The map URL.</value>
        Property MapURL() As String
            Get
                Return _mapURL
            End Get
            Set(ByVal value As String)
                _mapURL = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the category of an event
        ''' </summary>
        ''' <value>The category of an event.</value>
        Property Category() As Integer
            Get
                Return _category
            End Get
            Set(ByVal value As Integer)
                _category = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the name of the category.
        ''' </summary>
        ''' <value>The name of the category.</value>
        Property CategoryName() As String
            Get
                Return _categoryName
            End Get
            Set(ByVal value As String)
                _categoryName = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the category color to be used for display.
        ''' </summary>
        ''' <value>The color of the category.</value>
        Property Color() As String
            Get
                Return _color
            End Get
            Set(ByVal value As String)
                _color = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the category color of the font (foreground color).
        ''' </summary>
        ''' <value>The color of the font.</value>
        Property FontColor() As String
            Get
                Return _fontColor
            End Get
            Set(ByVal value As String)
                _fontColor = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether an image must be displayed.
        ''' </summary>
        ''' <value><c>true</c> if image must be displayed; otherwise, <c>false</c>.</value>
        Property ImageDisplay() As Boolean
            Get
                Return _imageDisplay
            End Get
            Set(ByVal value As Boolean)
                _imageDisplay = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether to send a reminder.
        ''' </summary>
        ''' <value><c>true</c> if send reminder; otherwise, <c>false</c>.</value>
        Property SendReminder() As Boolean
            Get
                Return _sendReminder
            End Get
            Set(ByVal value As Boolean)
                _sendReminder = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the reminder time.
        ''' </summary>
        ''' <value>The reminder time.</value>
        Property ReminderTime() As Integer
            Get
                Return _reminderTime
            End Get
            Set(ByVal value As Integer)
                _reminderTime = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the reminder time measurement.
        ''' </summary>
        ''' <value>The reminder time measurement.</value>
        Property ReminderTimeMeasurement() As String
            Get
                Return _reminderTimeMeasurement
            End Get
            Set(ByVal value As String)
                _reminderTimeMeasurement = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the reminder.
        ''' </summary>
        ''' <value>The reminder.</value>
        Property Reminder() As String
            Get
                Return _reminder
            End Get
            Set(ByVal value As String)
                _reminder = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the reminder from e-mail address
        ''' </summary>
        ''' <value>The reminder from e-mail address.</value>
        Property ReminderFrom() As String
            Get
                Return _reminderFrom
            End Get
            Set(ByVal value As String)
                _reminderFrom = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether the event is submitted to the search
        ''' </summary>
        ''' <value><c>true</c> if search submitted; otherwise, <c>false</c>.</value>
        Property SearchSubmitted() As Boolean
            Get
                Return _searchSubmitted
            End Get
            Set(ByVal value As Boolean)
                _searchSubmitted = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the module title.
        ''' </summary>
        ''' <value>The module title.</value>
        Property ModuleTitle() As String
            Get
                Return _moduleTitle
            End Get
            Set(ByVal value As String)
                _moduleTitle = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the custom field1.
        ''' </summary>
        ''' <value>The custom field1.</value>
        Property CustomField1() As String
            Get
                Return _customField1
            End Get
            Set(ByVal value As String)
                _customField1 = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the custom field2.
        ''' </summary>
        ''' <value>The custom field2.</value>
        Property CustomField2() As String
            Get
                Return _customField2
            End Get
            Set(ByVal value As String)
                _customField2 = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether enroll list view is to be displayed.
        ''' </summary>
        ''' <value><c>true</c> if [enroll list view]; otherwise, <c>false</c>.</value>
        Property EnrollListView() As Boolean
            Get
                Return _enrollListView
            End Get
            Set(ByVal value As Boolean)
                _enrollListView = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether end date is to be displayed
        ''' </summary>
        ''' <value><c>true</c> if [display end date]; otherwise, <c>false</c>.</value>
        Property DisplayEndDate() As Boolean
            Get
                Return _displayEndDate
            End Get
            Set(ByVal value As Boolean)
                _displayEndDate = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether the event is an all day event.
        ''' </summary>
        ''' <value><c>true</c> if [all day event]; otherwise, <c>false</c>.</value>
        Property AllDayEvent() As Boolean
            Get
                Return _allDayEvent
            End Get
            Set(ByVal value As Boolean)
                _allDayEvent = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the owner ID (userID).
        ''' </summary>
        ''' <value>The owner ID.</value>
        Property OwnerID() As Integer
            Get
                Return _ownerID
            End Get
            Set(ByVal value As Integer)
                _ownerID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the name of the owner of the event
        ''' </summary>
        ''' <value>The name of the owner.</value>
        Property OwnerName() As String
            Get
                Return _ownerName
            End Get
            Set(ByVal value As String)
                _ownerName = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the last updated at.
        ''' </summary>
        ''' <value>The last updated at.</value>
        Property LastUpdatedAt() As DateTime
            Get
                Return _lastUpdatedAt
            End Get
            Set(ByVal value As DateTime)
                _lastUpdatedAt = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the last updated by.
        ''' </summary>
        ''' <value>The last updated by.</value>
        Property LastUpdatedBy() As String
            Get
                Return _lastUpdatedBy
            End Get
            Set(ByVal value As String)
                _lastUpdatedBy = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the last updated userID
        ''' </summary>
        ''' <value>The last updated userID.</value>
        Property LastUpdatedID() As Integer
            Get
                Return _lastUpdatedID
            End Get
            Set(ByVal value As Integer)
                _lastUpdatedID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the original date begin.
        ''' </summary>
        ''' <value>The original date begin.</value>
        Property OriginalDateBegin() As Date
            Get
                Return _originalDateBegin
            End Get
            Set(ByVal value As Date)
                _originalDateBegin = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the update status.
        ''' </summary>
        ''' <value>The update status.</value>
        Property UpdateStatus() As String
            Get
                Return _updateStatus
            End Get
            Set(ByVal value As String)
                _updateStatus = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the Recurrence Rule.
        ''' </summary>
        ''' <value>The Recurrence Rule.</value>
        Property RRULE() As String
            Get
                Return _rrule
            End Get
            Set(ByVal value As String)
                _rrule = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the RM owner ID.
        ''' </summary>
        ''' <value>The RM owner ID.</value>
        Property RmOwnerID() As Integer
            Get
                Return _rmOwnerID
            End Get
            Set(ByVal value As Integer)
                _rmOwnerID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [new event email sent].
        ''' </summary>
        ''' <value><c>true</c> if [new event email sent]; otherwise, <c>false</c>.</value>
        Property NewEventEmailSent() As Boolean
            Get
                Return _newEventEmailSent
            End Get
            Set(ByVal value As Boolean)
                _newEventEmailSent = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [is private].
        ''' </summary>
        ''' <value><c>true</c> if [is private]; otherwise, <c>false</c>.</value>
        Property IsPrivate() As Boolean
            Get
                Return _isPrivate
            End Get
            Set(ByVal value As Boolean)
                _isPrivate = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating what the [event timezoneid] is
        ''' </summary>
        ''' <value>The event timezoneid.</value>
        Property EventTimeZoneId() As String
            Get
                If String.IsNullOrEmpty(_eventTimeZoneId) Then
                    Dim ems As New EventModuleSettings
                    Dim modSettings As EventModuleSettings = ems.GetEventModuleSettings(_moduleID, Nothing)
                    _eventTimeZoneId = modSettings.TimeZoneId
                End If
                Return _eventTimeZoneId
            End Get
            Set(ByVal value As String)
                _eventTimeZoneId = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating what the [other timezoneid] is
        ''' </summary>
        ''' <value>The other timezoneid.</value>
        Property OtherTimeZoneId() As String
            Get
                If _otherTimeZoneId Is Nothing Then
                    _otherTimeZoneId = "UTC"
                End If
                Return _otherTimeZoneId
            End Get
            Set(ByVal value As String)
                _otherTimeZoneId = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [allow anonymous enrollment].
        ''' </summary>
        ''' <value><c>true</c> if [allow anonymous enrollment]; otherwise, <c>false</c>.</value>
        Property AllowAnonEnroll() As Boolean
            Get
                Return _allowAnonEnroll
            End Get
            Set(ByVal value As Boolean)
                _allowAnonEnroll = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating the [content item id].
        ''' </summary>
        ''' <value>The contentitemid.</value>
        Property ContentItemID() As Integer
            Get
                Return _contentItemId
            End Get
            Set(ByVal value As Integer)
                _contentItemId = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [event has journal item].
        ''' </summary>
        ''' <value><c>true</c> if [event has journal item]; otherwise, <c>false</c>.</value>
        Property JournalItem() As Boolean
            Get
                Return _journalItem
            End Get
            Set(ByVal value As Boolean)
                _journalItem = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating the [soccialgroup id].
        ''' </summary>
        ''' <value>The SocialGroupId.</value>
        Property SocialGroupId() As Integer
            Get
                Return _socialGroupId
            End Get
            Set(ByVal value As Integer)
                _socialGroupId = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating the [user id].
        ''' </summary>
        ''' <value>The SocialUserId.</value>
        Property SocialUserId() As Integer
            Get
                Return _socialUserId
            End Get
            Set(ByVal value As Integer)
                _socialUserId = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the summary.
        ''' </summary>
        ''' <value>The summary.</value>
        Property Summary() As String
            Get
                Return _summary
            End Get
            Set(ByVal value As String)
                _summary = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating the [sequence].
        ''' </summary>
        ''' <value>The Sequence.</value>
        Property Sequence() As Integer
            Get
                Return _sequence
            End Get
            Set(ByVal value As Integer)
                _sequence = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating the [recur master sequence].
        ''' </summary>
        ''' <value>The Recur MasterSequence.</value>
        Property RmSequence() As Integer
            Get
                Return _rmSequence
            End Get
            Set(ByVal value As Integer)
                _rmSequence = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the SocialUserUserName.
        ''' </summary>
        ''' <value>The SocialUserUserName.</value>
        Property SocialUserUserName() As String
            Get
                Return _socialUserUserName
            End Get
            Set(ByVal value As String)
                _socialUserUserName = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the SocialUserDisplayName.
        ''' </summary>
        ''' <value>The SocialUserDisplayName.</value>
        Property SocialUserDisplayName() As String
            Get
                Return _socialUserDisplayName
            End Get
            Set(ByVal value As String)
                _socialUserDisplayName = value
            End Set
        End Property

        Private Shared _sortExpression As SortFilter
        ''' <summary>
        ''' Gets or sets the sort expression.
        ''' </summary>
        ''' <value>The sort expression.</value>
        Shared Property SortExpression() As SortFilter
            Get
                Return _sortExpression
            End Get
            Set(ByVal value As SortFilter)
                _sortExpression = value
            End Set
        End Property

        Private Shared _sortDirection As SortDirection
        ''' <summary>
        ''' Gets or sets the sort direction.
        ''' </summary>
        ''' <value>The sort direction.</value>
        Shared Property SortDirection() As SortDirection
            Get
                Return _SortDirection
            End Get
            Set(ByVal value As SortDirection)
                _SortDirection = value
            End Set
        End Property


        ''' <summary>
        ''' Sorting enumeration
        ''' </summary>
        Public Enum SortFilter
            ''' <summary>
            ''' By EventID
            ''' </summary>
            EventID
            ''' <summary>
            ''' By Date beging
            ''' </summary>
            EventDateBegin
            ''' <summary>
            ''' By Date end
            ''' </summary>
            EventDateEnd
            ''' <summary>
            ''' Bu Name
            ''' </summary>
            EventName
            ''' <summary>
            ''' By duration
            ''' </summary>
            Duration
            ''' <summary>
            ''' Bu category name
            ''' </summary>
            CategoryName
            ''' <summary>
            ''' By customfield1
            ''' </summary>
            CustomField1
            ''' <summary>
            ''' By customfield2
            ''' </summary>
            CustomField2
            ''' <summary>
            ''' By description
            ''' </summary>
            Description
            ''' <summary>
            ''' By Location name
            ''' </summary>
            LocationName
        End Enum

        ''' <summary>
        ''' Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        ''' </summary>
        ''' <param name="obj">An object to compare with this instance.</param>
        ''' <returns>
        ''' A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings:
        ''' Value
        ''' Meaning
        ''' Less than zero
        ''' This instance is less than <paramref name="obj" />.
        ''' Zero
        ''' This instance is equal to <paramref name="obj" />.
        ''' Greater than zero
        ''' This instance is greater than <paramref name="obj" />.
        ''' </returns>
        ''' <exception cref="T:System.ArgumentException">
        ''' <paramref name="obj" /> is not the same type as this instance.
        ''' </exception>
        Public Function CompareTo(ByVal obj As Object) As Integer Implements IComparable.CompareTo
            Dim o As EventInfo = CType(obj, EventInfo)
            Dim xCompare As String = EventName + Format(EventID, "00000000")
            Dim yCompare As String = o.EventName + Format(o.EventID, "00000000")
            Select Case SortExpression
                Case SortFilter.CategoryName
                    xCompare = CategoryName + Format(EventID, "00000000")
                    yCompare = o.CategoryName + Format(o.EventID, "00000000")
                Case SortFilter.CustomField1
                    xCompare = CustomField1 + Format(EventID, "00000000")
                    yCompare = o.CustomField1 + Format(o.EventID, "00000000")
                Case SortFilter.CustomField2
                    xCompare = CustomField2 + Format(EventID, "00000000")
                    yCompare = o.CustomField2 + Format(o.EventID, "00000000")
                Case SortFilter.Description
                    xCompare = EventDesc + Format(EventID, "00000000")
                    yCompare = o.EventDesc + Format(o.EventID, "00000000")
                Case SortFilter.Duration
                    xCompare = Format(Duration, "000000") + Format(EventID, "00000000")
                    yCompare = Format(o.Duration, "000000") + Format(o.EventID, "00000000")
                Case SortFilter.EventDateBegin
                    xCompare = Format(EventTimeBegin, "yyyyMMddHHmm") + Format(EventID, "00000000")
                    yCompare = Format(o.EventTimeBegin, "yyyyMMddHHmm") + Format(o.EventID, "00000000")
                Case SortFilter.EventDateEnd
                    xCompare = Format(EventTimeEnd, "yyyyMMddHHmm") + Format(EventID, "00000000")
                    yCompare = Format(o.EventTimeEnd, "yyyyMMddHHmm") + Format(o.EventID, "00000000")
                Case SortFilter.LocationName
                    xCompare = LocationName + Format(EventID, "00000000")
                    yCompare = o.LocationName + Format(o.EventID, "00000000")
            End Select
            If SortDirection = System.Web.UI.WebControls.SortDirection.Ascending Then
                Return System.String.Compare(xCompare, yCompare, StringComparison.CurrentCulture)
            Else
                Return System.String.Compare(yCompare, xCompare, StringComparison.CurrentCulture)
            End If
        End Function


#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Clones an instance of the events object
        ''' </summary>
        ''' <returns>Cloned Eventsinfo object</returns>
        Public Function Clone() As EventInfo
            Return DirectCast(MemberwiseClone(), EventInfo)

        End Function
#End Region
    End Class

#End Region

#Region "EventMasterInfo Class "
    Public Class EventMasterInfo

        Private _portalID As Integer
        Private _masterID As Integer
        Private _moduleID As Integer
        Private _subEventID As Integer
        Private _subEventTitle As String

        ''' <summary>
        ''' Gets or sets the portal ID.
        ''' </summary>
        ''' <value>The portal ID.</value>
        Property PortalID() As Integer
            Get
                Return _PortalID
            End Get
            Set(ByVal value As Integer)
                _PortalID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the master ID.
        ''' </summary>
        ''' <value>The master ID.</value>
        Property MasterID() As Integer
            Get
                Return _MasterID
            End Get
            Set(ByVal value As Integer)
                _MasterID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the module ID.
        ''' </summary>
        ''' <value>The module ID.</value>
        Property ModuleID() As Integer
            Get
                Return _ModuleID
            End Get
            Set(ByVal value As Integer)
                _ModuleID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the sub event ID.
        ''' </summary>
        ''' <value>The sub event ID.</value>
        Property SubEventID() As Integer
            Get
                Return _SubEventID
            End Get
            Set(ByVal value As Integer)
                _SubEventID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the sub event title.
        ''' </summary>
        ''' <value>The sub event title.</value>
        Property SubEventTitle() As String
            Get
                Return _SubEventTitle
            End Get
            Set(ByVal value As String)
                _SubEventTitle = value
            End Set
        End Property
    End Class
#End Region

#Region "EventSignupsInfo Class"
    ''' <summary>
    ''' Information about the users that have signed up for a particular event
    ''' </summary>
    Public Class EventSignupsInfo
        Implements IComparable

        ''' <summary>
        ''' Priority of the event
        ''' </summary>
        Public Enum Priority
            ''' <summary>
            ''' Hign priority
            ''' </summary>
            High = 1
            ''' <summary>
            ''' Medium priority
            ''' </summary>
            Medium = 2
            ''' <summary>
            ''' Low priority
            ''' </summary>
            Low = 3
        End Enum

        Private _eventID As Integer
        Private _moduleID As Integer
        Private _signupID As Integer
        Private _userID As Integer
        Private _approved As Boolean = False
        Private _userName As String
        Private _email As String
        Private _emailVisible As Boolean
        Private _telephone As String
        Private _eventTimeBegin As DateTime = Now
        Private _eventTimeEnd As DateTime
        Private _duration As Integer
        Private _eventName As String
        Private _importance As Integer
        Private _eventApproved As Boolean = False
        Private _maxEnrollment As Integer = 0
        Private _enrolled As Integer
        Private _payPalStatus As String
        Private _payPalReason As String
        Private _payPalTransID As String
        Private _payPalPayerID As String
        Private _payPalPayerStatus As String
        Private _payPalRecieverEmail As String
        Private _payPalUserEmail As String
        Private _payPalPayerEmail As String
        Private _payPalFirstName As String
        Private _payPalLastName As String
        Private _payPalAddress As String
        Private _payPalCity As String
        Private _payPalState As String
        Private _payPalZip As String
        Private _payPalCountry As String
        Private _payPalCurrency As String
        Private _payPalPaymentDate As DateTime = Now
        Private _payPalAmount As Decimal = 0
        Private _payPalFee As Decimal = 0
        Private _noEnrolees As Integer = 1
        Private _eventTimeZoneId As String
        Private _anonEmail As String
        Private _anonName As String
        Private _anonTelephone As String
        Private _anonCulture As String
        Private _anonTimeZoneId As String
        Private _firstName As String
        Private _lastName As String
        Private _company As String
        Private _jobTitle As String
        Private _referenceNumber As String
        Private _remarks As String
        Private _street As String
        Private _postalCode As String
        Private _city As String
        Private _region As String
        Private _country As String

        ' initialization
        ''' <summary>
        ''' Initializes a new instance of the <see cref="EventSignupsInfo" /> class.
        ''' </summary>
        Public Sub New()
        End Sub

        ' public properties
        ''' <summary>
        ''' Gets or sets the event ID.
        ''' </summary>
        ''' <value>The event ID.</value>
        Property EventID() As Integer
            Get
                Return _EventID
            End Get
            Set(ByVal value As Integer)
                _EventID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the module ID.
        ''' </summary>
        ''' <value>The module ID.</value>
        Property ModuleID() As Integer
            Get
                Return _ModuleID
            End Get
            Set(ByVal value As Integer)
                _ModuleID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the signup ID.
        ''' </summary>
        ''' <value>The signup ID.</value>
        Property SignupID() As Integer
            Get
                Return _SignupID
            End Get
            Set(ByVal value As Integer)
                _SignupID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the user ID.
        ''' </summary>
        ''' <value>The user ID.</value>
        Property UserID() As Integer
            Get
                Return _UserID
            End Get
            Set(ByVal value As Integer)
                _UserID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether this <see cref="EventSignupsInfo" /> is approved.
        ''' </summary>
        ''' <value><c>true</c> if approved; otherwise, <c>false</c>.</value>
        Property Approved() As Boolean
            Get
                Return _Approved
            End Get
            Set(ByVal value As Boolean)
                _Approved = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the name of the user for the signup
        ''' </summary>
        ''' <value>The name of the user.</value>
        Property UserName() As String
            Get
                Return _UserName
            End Get
            Set(ByVal value As String)
                _UserName = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the email of the signup
        ''' </summary>
        ''' <value>The email.</value>
        Property Email() As String
            Get
                Return _Email
            End Get
            Set(ByVal value As String)
                _Email = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether email of the signup is visible.
        ''' </summary>
        ''' <value><c>true</c> if [email visible]; otherwise, <c>false</c>.</value>
        Property EmailVisible() As Boolean
            Get
                Return _EmailVisible
            End Get
            Set(ByVal value As Boolean)
                _EmailVisible = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the telephone of the signup
        ''' </summary>
        ''' <value>The telephone.</value>
        Property Telephone() As String
            Get
                Return _Telephone
            End Get
            Set(ByVal value As String)
                _Telephone = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the event date begin.
        ''' </summary>
        ''' <value>The event date begin.</value>
        ReadOnly Property EventDateBegin() As Date
            Get
                Return _EventTimeBegin.Date
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the event time begin.
        ''' </summary>
        ''' <value>The event time begin.</value>
        Property EventTimeBegin() As DateTime
            Get
                Return _EventTimeBegin
            End Get
            Set(ByVal value As DateTime)
                _EventTimeBegin = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the event time end.
        ''' </summary>
        ''' <value>The event time end.</value>
        Property EventTimeEnd() As DateTime
            Get
                Return _EventTimeEnd
            End Get
            Set(ByVal value As DateTime)
                _EventTimeEnd = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the duration.
        ''' </summary>
        ''' <value>The duration.</value>
        Property Duration() As Integer
            Get
                Return _Duration
            End Get
            Set(ByVal value As Integer)
                _Duration = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the name of the event.
        ''' </summary>
        ''' <value>The name of the event.</value>
        Property EventName() As String
            Get
                Return _EventName
            End Get
            Set(ByVal value As String)
                _EventName = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the importance of the event
        ''' </summary>
        ''' <value>The importance.</value>
        Property Importance() As Priority
            Get
                Return CType(_Importance, Priority)
            End Get
            Set(ByVal value As Priority)
                _Importance = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether the event is approved.
        ''' </summary>
        ''' <value><c>true</c> if [event approved]; otherwise, <c>false</c>.</value>
        Property EventApproved() As Boolean
            Get
                Return _EventApproved
            End Get
            Set(ByVal value As Boolean)
                _EventApproved = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the max # of enrollments
        ''' </summary>
        ''' <value>The max # of enrollments.</value>
        Property MaxEnrollment() As Integer
            Get
                Return _MaxEnrollment
            End Get
            Set(ByVal value As Integer)
                _MaxEnrollment = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the # of enrolled.
        ''' </summary>
        ''' <value>The # of enrolled.</value>
        Property Enrolled() As Integer
            Get
                Return _Enrolled
            End Get
            Set(ByVal value As Integer)
                _Enrolled = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the PayPal status.
        ''' </summary>
        ''' <value>The PayPal status.</value>
        Property PayPalStatus() As String
            Get
                Return _PayPalStatus
            End Get
            Set(ByVal value As String)
                _PayPalStatus = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the PayPal result reason
        ''' </summary>
        ''' <value>The PayPal reason.</value>
        Property PayPalReason() As String
            Get
                Return _PayPalReason
            End Get
            Set(ByVal value As String)
                _PayPalReason = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the PayPal transaction ID.
        ''' </summary>
        ''' <value>The PayPal transaction ID.</value>
        Property PayPalTransID() As String
            Get
                Return _PayPalTransID
            End Get
            Set(ByVal value As String)
                _PayPalTransID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the PayPal payer ID.
        ''' </summary>
        ''' <value>The PayPal payer ID.</value>
        Property PayPalPayerID() As String
            Get
                Return _PayPalPayerID
            End Get
            Set(ByVal value As String)
                _PayPalPayerID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the PayPal payer status.
        ''' </summary>
        ''' <value>The pay PayPal status.</value>
        Property PayPalPayerStatus() As String
            Get
                Return _PayPalPayerStatus
            End Get
            Set(ByVal value As String)
                _PayPalPayerStatus = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the PayPal reciever email.
        ''' </summary>
        ''' <value>The PayPal reciever email.</value>
        Property PayPalRecieverEmail() As String
            Get
                Return _PayPalRecieverEmail
            End Get
            Set(ByVal value As String)
                _PayPalRecieverEmail = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the PayPal user email.
        ''' </summary>
        ''' <value>The PayPal user email.</value>
        Property PayPalUserEmail() As String
            Get
                Return _PayPalUserEmail
            End Get
            Set(ByVal value As String)
                _PayPalUserEmail = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the PayPal payer email.
        ''' </summary>
        ''' <value>The PayPal payer email.</value>
        Property PayPalPayerEmail() As String
            Get
                Return _PayPalPayerEmail
            End Get
            Set(ByVal value As String)
                _PayPalPayerEmail = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the first name of the PayPal.
        ''' </summary>
        ''' <value>The first name of the PayPal.</value>
        Property PayPalFirstName() As String
            Get
                Return _PayPalFirstName
            End Get
            Set(ByVal value As String)
                _PayPalFirstName = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the last name of the PayPal.
        ''' </summary>
        ''' <value>The last name of the PayPal.</value>
        Property PayPalLastName() As String
            Get
                Return _PayPalLastName
            End Get
            Set(ByVal value As String)
                _PayPalLastName = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the PayPal address.
        ''' </summary>
        ''' <value>The PayPal address.</value>
        Property PayPalAddress() As String
            Get
                Return _PayPalAddress
            End Get
            Set(ByVal value As String)
                _PayPalAddress = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the PayPal city.
        ''' </summary>
        ''' <value>The PayPal city.</value>
        Property PayPalCity() As String
            Get
                Return _PayPalCity
            End Get
            Set(ByVal value As String)
                _PayPalCity = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the state of the PayPal.
        ''' </summary>
        ''' <value>The state of the PayPal.</value>
        Property PayPalState() As String
            Get
                Return _PayPalState
            End Get
            Set(ByVal value As String)
                _PayPalState = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the PayPal zip.
        ''' </summary>
        ''' <value>The PayPal zip.</value>
        Property PayPalZip() As String
            Get
                Return _PayPalZip
            End Get
            Set(ByVal value As String)
                _PayPalZip = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the PayPal country.
        ''' </summary>
        ''' <value>The PayPal country.</value>
        Property PayPalCountry() As String
            Get
                Return _PayPalCountry
            End Get
            Set(ByVal value As String)
                _PayPalCountry = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the PayPal currency.
        ''' </summary>
        ''' <value>The PayPal currency.</value>
        Property PayPalCurrency() As String
            Get
                Return _PayPalCurrency
            End Get
            Set(ByVal value As String)
                _PayPalCurrency = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the PayPal payment date.
        ''' </summary>
        ''' <value>The PayPal payment date.</value>
        Property PayPalPaymentDate() As DateTime
            Get
                Return _PayPalPaymentDate
            End Get
            Set(ByVal value As DateTime)
                _PayPalPaymentDate = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the PayPal amount.
        ''' </summary>
        ''' <value>The PayPal amount.</value>
        Property PayPalAmount() As Decimal
            Get
                Return _PayPalAmount
            End Get
            Set(ByVal value As Decimal)
                _PayPalAmount = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the PayPal fee.
        ''' </summary>
        ''' <value>The PayPal fee.</value>
        Property PayPalFee() As Decimal
            Get
                Return _PayPalFee
            End Get
            Set(ByVal value As Decimal)
                _PayPalFee = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the # of  enrolees.
        ''' </summary>
        ''' <value>The # of enrolees.</value>
        Property NoEnrolees() As Integer
            Get
                Return _NoEnrolees
            End Get
            Set(ByVal value As Integer)
                _NoEnrolees = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating what the [event timezoneid] is
        ''' </summary>
        ''' <value>The event timezoneid.</value>
        Property EventTimeZoneId() As String
            Get
                Return _EventTimeZoneId
            End Get
            Set(ByVal value As String)
                _EventTimeZoneId = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating what the [anonymous email] is
        ''' </summary>
        ''' <value>The event timezoneid.</value>
        Property AnonEmail() As String
            Get
                Return _AnonEmail
            End Get
            Set(ByVal value As String)
                _AnonEmail = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating what the [anonymous name] is
        ''' </summary>
        ''' <value>The event timezoneid.</value>
        Property AnonName() As String
            Get
                Return _AnonName
            End Get
            Set(ByVal value As String)
                _AnonName = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating what the [anonymous telephone] is
        ''' </summary>
        ''' <value>The event timezoneid.</value>
        Property AnonTelephone() As String
            Get
                Return _AnonTelephone
            End Get
            Set(ByVal value As String)
                _AnonTelephone = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating what the [anonymous Culture] is
        ''' </summary>
        ''' <value>The event timezoneid.</value>
        Property AnonCulture() As String
            Get
                Return _AnonCulture
            End Get
            Set(ByVal value As String)
                _AnonCulture = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating what the [anonymous TimeZoneId] is
        ''' </summary>
        ''' <value>The event timezoneid.</value>
        Property AnonTimeZoneId() As String
            Get
                Return _AnonTimeZoneId
            End Get
            Set(ByVal value As String)
                _AnonTimeZoneId = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the enrollee's dirst name.
        ''' </summary>
        ''' <value>The enrollee's first name.</value>
        Property FirstName() As String
            Get
                Return _firstName
            End Get
            Set(ByVal value As String)
                _firstName = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the enrollee's last name.
        ''' </summary>
        ''' <value>The enrollee's last name.</value>
        Property LastName() As String
            Get
                Return _lastName
            End Get
            Set(ByVal value As String)
                _lastName = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the enrollee's company.
        ''' </summary>
        ''' <value>The enrollee's company.</value>
        Property Company() As String
            Get
                Return _company
            End Get
            Set(ByVal value As String)
                _company = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the enrollee's job title.
        ''' </summary>
        ''' <value>The enrollee's job title.</value>
        Property JobTitle() As String
            Get
                Return _jobTitle
            End Get
            Set(ByVal value As String)
                _jobTitle = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the enrollee's reference number.
        ''' </summary>
        ''' <value>The enrollee's reference number.</value>
        Property ReferenceNumber() As String
            Get
                Return _referenceNumber
            End Get
            Set(ByVal value As String)
                _referenceNumber = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the enrollee's remarks.
        ''' </summary>
        ''' <value>The enrollee's remarks.</value>
        Property Remarks() As String
            Get
                Return _remarks
            End Get
            Set(ByVal value As String)
                _remarks = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the street of the enrollee's address.
        ''' </summary>
        ''' <value>The street of the enrollee's address.</value>
        Property Street() As String
            Get
                Return _street
            End Get
            Set(ByVal value As String)
                _street = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the postal code of the enrollee's address.
        ''' </summary>
        ''' <value>The postal code of the enrollee's address.</value>
        Property PostalCode() As String
            Get
                Return _postalCode
            End Get
            Set(ByVal value As String)
                _postalCode = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the city of the enrollee's address.
        ''' </summary>
        ''' <value>The city of the enrollee's address.</value>
        Property City() As String
            Get
                Return _city
            End Get
            Set(ByVal value As String)
                _city = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the region of the enrollee's address.
        ''' </summary>
        ''' <value>The region of the enrollee's address.</value>
        Property Region() As String
            Get
                Return _region
            End Get
            Set(ByVal value As String)
                _region = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the country of the enrollee's address.
        ''' </summary>
        ''' <value>The country of the enrollee's address.</value>
        Property Country() As String
            Get
                Return _country
            End Get
            Set(ByVal value As String)
                _country = value
            End Set
        End Property
#Region "Sorting"
        Private Shared _sortExpression As SortFilter
        ''' <summary>
        ''' Gets or sets the sort expression.
        ''' </summary>
        ''' <value>The sort expression.</value>
        Shared Property SortExpression() As SortFilter
            Get
                Return _SortExpression
            End Get
            Set(ByVal value As SortFilter)
                _SortExpression = value
            End Set
        End Property

        Private Shared _sortDirection As SortDirection
        ''' <summary>
        ''' Gets or sets the sort direction.
        ''' </summary>
        ''' <value>The sort direction.</value>
        Shared Property SortDirection() As SortDirection
            Get
                Return _SortDirection
            End Get
            Set(ByVal value As SortDirection)
                _SortDirection = value
            End Set
        End Property


        ''' <summary>
        ''' Sorting enumeration
        ''' </summary>
        Public Enum SortFilter
            ''' <summary>
            ''' By EventID
            ''' </summary>
            EventID
            ''' <summary>
            ''' By Date beging
            ''' </summary>
            EventTimeBegin
            ''' <summary>
            ''' By Date end
            ''' </summary>
            EventTimeEnd
            ''' <summary>
            ''' Bu Name
            ''' </summary>
            EventName
            ''' <summary>
            ''' By duration
            ''' </summary>
            Duration
            ''' <summary>
            ''' By approved
            ''' </summary>
            Approved
        End Enum

        ''' <summary>
        ''' Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        ''' </summary>
        ''' <param name="obj">An object to compare with this instance.</param>
        ''' <returns>
        ''' A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings:
        ''' Value
        ''' Meaning
        ''' Less than zero
        ''' This instance is less than <paramref name="obj" />.
        ''' Zero
        ''' This instance is equal to <paramref name="obj" />.
        ''' Greater than zero
        ''' This instance is greater than <paramref name="obj" />.
        ''' </returns>
        ''' <exception cref="T:System.ArgumentException">
        ''' <paramref name="obj" /> is not the same type as this instance.
        ''' </exception>
        Public Function CompareTo(ByVal obj As Object) As Integer Implements IComparable.CompareTo
            Dim o As EventSignupsInfo = CType(obj, EventSignupsInfo)
            Dim xCompare As String = EventName + Format(EventID, "00000000")
            Dim yCompare As String = o.EventName + Format(o.EventID, "00000000")
            Select Case SortExpression
                Case SortFilter.Duration
                    xCompare = Format(Duration, "000000") + Format(EventID, "00000000")
                    yCompare = Format(o.Duration, "000000") + Format(o.EventID, "00000000")
                Case SortFilter.EventTimeBegin
                    xCompare = Format(EventTimeBegin, "yyyyMMddHHmm") + Format(EventID, "00000000")
                    yCompare = Format(o.EventTimeBegin, "yyyyMMddHHmm") + Format(o.EventID, "00000000")
                Case SortFilter.EventTimeEnd
                    xCompare = Format(EventTimeEnd, "yyyyMMddHHmm") + Format(EventID, "00000000")
                    yCompare = Format(o.EventTimeEnd, "yyyyMMddHHmm") + Format(o.EventID, "00000000")
                Case SortFilter.Approved
                    xCompare = Approved.ToString + Format(EventID, "00000000")
                    yCompare = o.Approved.ToString + Format(o.EventID, "00000000")
            End Select
            If SortDirection = System.Web.UI.WebControls.SortDirection.Ascending Then
                Return System.String.Compare(xCompare, yCompare, StringComparison.CurrentCulture)
            Else
                Return System.String.Compare(yCompare, xCompare, StringComparison.CurrentCulture)
            End If
        End Function

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Clones an instance of the eventssignups object
        ''' </summary>
        ''' <returns>Cloned EventsSignupsinfo object</returns>
        Public Function Clone() As EventSignupsInfo
            ' create the object
            Return DirectCast(MemberwiseClone(), EventSignupsInfo)
        End Function
#End Region

    End Class
#End Region

#Region "EventPPErrorLogInfo Class"
    ''' <summary>
    ''' Information  about any infomartion during PayPal payments
    ''' </summary>
    Public Class EventPpErrorLogInfo
        ' ReSharper disable ConvertToConstant.Local
        Private ReadOnly _payPalID As Integer = 0
        ' ReSharper restore ConvertToConstant.Local
        Private _signupID As Integer = 0
        Private ReadOnly _createDate As DateTime = Now
        Private _payPalStatus As String
        Private _payPalReason As String
        Private _payPalTransID As String
        Private _payPalPayerID As String
        Private _payPalPayerStatus As String
        Private _payPalRecieverEmail As String
        Private _payPalUserEmail As String
        Private _payPalPayerEmail As String
        Private _payPalFirstName As String
        Private _payPalLastName As String
        Private _payPalAddress As String
        Private _payPalCity As String
        Private _payPalState As String
        Private _payPalZip As String
        Private _payPalCountry As String
        Private _payPalCurrency As String
        Private _payPalPaymentDate As DateTime = Now
        Private _payPalAmount As Double = 0.0
        Private _payPalFee As Double = 0.0

        ' public properties
        ''' <summary>
        ''' Gets the PayPal ID.
        ''' </summary>
        ''' <value>The PayPal ID.</value>
        Public ReadOnly Property PayPalID() As Integer
            Get
                Return _PayPalID
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the signup ID.
        ''' </summary>
        ''' <value>The signup ID.</value>
        Property SignupID() As Integer
            Get
                Return _SignupID
            End Get
            Set(ByVal value As Integer)
                _SignupID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the create date.
        ''' </summary>
        ''' <value>The create date.</value>
        Public ReadOnly Property CreateDate() As DateTime
            Get
                Return _CreateDate
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the PayPal status.
        ''' </summary>
        ''' <value>The PayPal status.</value>
        Property PayPalStatus() As String
            Get
                Return _PayPalStatus
            End Get
            Set(ByVal value As String)
                _PayPalStatus = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the PayPal reason.
        ''' </summary>
        ''' <value>The PayPal reason.</value>
        Property PayPalReason() As String
            Get
                Return _PayPalReason
            End Get
            Set(ByVal value As String)
                _PayPalReason = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the PayPal trans ID.
        ''' </summary>
        ''' <value>The PayPal trans ID.</value>
        Property PayPalTransID() As String
            Get
                Return _PayPalTransID
            End Get
            Set(ByVal value As String)
                _PayPalTransID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the PayPal payer ID.
        ''' </summary>
        ''' <value>The PayPal payer ID.</value>
        Property PayPalPayerID() As String
            Get
                Return _PayPalPayerID
            End Get
            Set(ByVal value As String)
                _PayPalPayerID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the PayPal payer status.
        ''' </summary>
        ''' <value>The PayPal payer status.</value>
        Property PayPalPayerStatus() As String
            Get
                Return _PayPalPayerStatus
            End Get
            Set(ByVal value As String)
                _PayPalPayerStatus = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the PayPal reciever email.
        ''' </summary>
        ''' <value>The PayPal reciever email.</value>
        Property PayPalRecieverEmail() As String
            Get
                Return _PayPalRecieverEmail
            End Get
            Set(ByVal value As String)
                _PayPalRecieverEmail = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the PayPal user email.
        ''' </summary>
        ''' <value>The PayPal user email.</value>
        Property PayPalUserEmail() As String
            Get
                Return _PayPalUserEmail
            End Get
            Set(ByVal value As String)
                _PayPalUserEmail = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the PayPal payer email.
        ''' </summary>
        ''' <value>The PayPal payer email.</value>
        Property PayPalPayerEmail() As String
            Get
                Return _PayPalPayerEmail
            End Get
            Set(ByVal value As String)
                _PayPalPayerEmail = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the first name of the PayPal.
        ''' </summary>
        ''' <value>The first name of the PayPal.</value>
        Property PayPalFirstName() As String
            Get
                Return _PayPalFirstName
            End Get
            Set(ByVal value As String)
                _PayPalFirstName = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the last name of the PayPal.
        ''' </summary>
        ''' <value>The last name of the PayPal.</value>
        Property PayPalLastName() As String
            Get
                Return _PayPalLastName
            End Get
            Set(ByVal value As String)
                _PayPalLastName = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the PayPal address.
        ''' </summary>
        ''' <value>The PayPal address.</value>
        Property PayPalAddress() As String
            Get
                Return _PayPalAddress
            End Get
            Set(ByVal value As String)
                _PayPalAddress = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the PayPal city.
        ''' </summary>
        ''' <value>The PayPal city.</value>
        Property PayPalCity() As String
            Get
                Return _PayPalCity
            End Get
            Set(ByVal value As String)
                _PayPalCity = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the state of the PayPal.
        ''' </summary>
        ''' <value>The state of the PayPal.</value>
        Property PayPalState() As String
            Get
                Return _PayPalState
            End Get
            Set(ByVal value As String)
                _PayPalState = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the PayPal zip.
        ''' </summary>
        ''' <value>The PayPal zip.</value>
        Property PayPalZip() As String
            Get
                Return _PayPalZip
            End Get
            Set(ByVal value As String)
                _PayPalZip = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the PayPal country.
        ''' </summary>
        ''' <value>The PayPal country.</value>
        Property PayPalCountry() As String
            Get
                Return _PayPalCountry
            End Get
            Set(ByVal value As String)
                _PayPalCountry = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the PayPal currency.
        ''' </summary>
        ''' <value>The PayPal currency.</value>
        Property PayPalCurrency() As String
            Get
                Return _PayPalCurrency
            End Get
            Set(ByVal value As String)
                _PayPalCurrency = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the PayPal payment date.
        ''' </summary>
        ''' <value>The PayPal payment date.</value>
        Property PayPalPaymentDate() As DateTime
            Get
                Return _PayPalPaymentDate
            End Get
            Set(ByVal value As DateTime)
                _PayPalPaymentDate = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the PayPal amount.
        ''' </summary>
        ''' <value>The PayPal amount.</value>
        Property PayPalAmount() As Double
            Get
                Return _PayPalAmount
            End Get
            Set(ByVal value As Double)
                _payPalAmount = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the PayPal fee.
        ''' </summary>
        ''' <value>The PayPal fee.</value>
        Property PayPalFee() As Double
            Get
                Return _payPalFee
            End Get
            Set(ByVal value As Double)
                _payPalFee = Value
            End Set
        End Property

    End Class
#End Region

#Region "EventCategoryInfo Class"
    ''' <summary>
    ''' Information about the (optional) category of the envent
    ''' </summary>
    Public Class EventCategoryInfo

        Private _portalID As Integer
        Private _category As Integer
        Private _categoryName As String
        Private _color As String
        Private _fontColor As String

        ' initialization
        ''' <summary>
        ''' Initializes a new instance of the <see cref="EventCategoryInfo" /> class.
        ''' </summary>
        Public Sub New()
        End Sub

        ' public properties
        ''' <summary>
        ''' Gets or sets the portal ID.
        ''' </summary>
        ''' <value>The portal ID.</value>
        Property PortalID() As Integer
            Get
                Return _PortalID
            End Get
            Set(ByVal value As Integer)
                _PortalID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the category ID.
        ''' </summary>
        ''' <value>The category ID.</value>
        Property Category() As Integer
            Get
                Return _Category
            End Get
            Set(ByVal value As Integer)
                _Category = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the name of the category.
        ''' </summary>
        ''' <value>The name of the category.</value>
        Property CategoryName() As String
            Get
                Return _CategoryName
            End Get
            Set(ByVal value As String)
                _CategoryName = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the color.
        ''' </summary>
        ''' <value>The color.</value>
        Property Color() As String
            Get
                Return _Color
            End Get
            Set(ByVal value As String)
                _Color = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the color of the font.
        ''' </summary>
        ''' <value>The color of the font.</value>
        Property FontColor() As String
            Get
                Return _FontColor
            End Get
            Set(ByVal value As String)
                _FontColor = value
            End Set
        End Property
    End Class
#End Region

#Region "EventLocationInfo Class"
    ''' <summary>
    ''' Information about the (optional) location of the event
    ''' </summary>
    Public Class EventLocationInfo

        Private _portalID As Integer
        Private _location As Integer
        Private _locationName As String
        Private _mapURL As String
        Private _street As String
        Private _postalCode As String
        Private _city As String
        Private _region As String
        Private _country As String

        ' initialization
        ''' <summary>
        ''' Initializes a new instance of the <see cref="EventLocationInfo" /> class.
        ''' </summary>
        Public Sub New()
        End Sub

        ' public properties
        ''' <summary>
        ''' Gets or sets the portal ID.
        ''' </summary>
        ''' <value>The portal ID.</value>
        Property PortalID() As Integer
            Get
                Return _PortalID
            End Get
            Set(ByVal value As Integer)
                _PortalID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the location.
        ''' </summary>
        ''' <value>The location.</value>
        Property Location() As Integer
            Get
                Return _Location
            End Get
            Set(ByVal value As Integer)
                _Location = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the name of the location.
        ''' </summary>
        ''' <value>The name of the location.</value>
        Property LocationName() As String
            Get
                Return _LocationName
            End Get
            Set(ByVal value As String)
                _LocationName = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the map URL.
        ''' </summary>
        ''' <value>The map URL.</value>
        Property MapURL() As String
            Get
                Return _MapURL
            End Get
            Set(ByVal value As String)
                _MapURL = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the street of the location's address.
        ''' </summary>
        ''' <value>The street of the location's address.</value>
        Property Street() As String
            Get
                Return _street
            End Get
            Set(ByVal value As String)
                _street = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the postal code of the location's address.
        ''' </summary>
        ''' <value>The postal code of the location's address.</value>
        Property PostalCode() As String
            Get
                Return _postalCode
            End Get
            Set(ByVal value As String)
                _postalCode = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the city of the location's address.
        ''' </summary>
        ''' <value>The city of the location's address.</value>
        Property City() As String
            Get
                Return _city
            End Get
            Set(ByVal value As String)
                _city = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the region of the location's address.
        ''' </summary>
        ''' <value>The region of the location's address.</value>
        Property Region() As String
            Get
                Return _region
            End Get
            Set(ByVal value As String)
                _region = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the country of the location's address.
        ''' </summary>
        ''' <value>The country of the location's address.</value>
        Property Country() As String
            Get
                Return _country
            End Get
            Set(ByVal value As String)
                _country = value
            End Set
        End Property
    End Class
#End Region

#Region "EventNotificationInfo Class"
    ''' <summary>
    ''' Information for emial notification of events
    ''' </summary>
    Public Class EventNotificationInfo

        Private _eventID As Integer
        Private _portalAliasID As Integer
        Private _notificationID As Integer
        Private _userEmail As String
        Private _notificationSent As Boolean = False
        Private _notifyByDateTime As DateTime
        Private _eventTimeBegin As DateTime
        Private _notifyLanguage As String
        Private _moduleID As Integer
        Private _tabID As Integer

        ' initialization
        ''' <summary>
        ''' Initializes a new instance of the <see cref="EventNotificationInfo" /> class.
        ''' </summary>
        Public Sub New()
        End Sub

        ' public properties
        ''' <summary>
        ''' Gets or sets the event ID.
        ''' </summary>
        ''' <value>The event ID.</value>
        Property EventID() As Integer
            Get
                Return _EventID
            End Get
            Set(ByVal value As Integer)
                _EventID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the portal alias ID.
        ''' </summary>
        ''' <value>The portal alias ID.</value>
        Property PortalAliasID() As Integer
            Get
                Return _PortalAliasID
            End Get
            Set(ByVal value As Integer)
                _PortalAliasID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the notification ID.
        ''' </summary>
        ''' <value>The notification ID.</value>
        Property NotificationID() As Integer
            Get
                Return _NotificationID
            End Get
            Set(ByVal value As Integer)
                _NotificationID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the user email.
        ''' </summary>
        ''' <value>The user email.</value>
        Property UserEmail() As String
            Get
                Return _UserEmail
            End Get
            Set(ByVal value As String)
                _UserEmail = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [notification sent].
        ''' </summary>
        ''' <value><c>true</c> if [notification sent]; otherwise, <c>false</c>.</value>
        Property NotificationSent() As Boolean
            Get
                Return _NotificationSent
            End Get
            Set(ByVal value As Boolean)
                _NotificationSent = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the notify by date time.
        ''' </summary>
        ''' <value>The notify by date time.</value>
        Property NotifyByDateTime() As DateTime
            Get
                Return _NotifyByDateTime
            End Get
            Set(ByVal value As DateTime)
                _NotifyByDateTime = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the event time begin.
        ''' </summary>
        ''' <value>The event time begin.</value>
        Property EventTimeBegin() As DateTime
            Get
                Return _EventTimeBegin
            End Get
            Set(ByVal value As DateTime)
                _EventTimeBegin = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the notify language.
        ''' </summary>
        ''' <value>The notify language.</value>
        Property NotifyLanguage() As String
            Get
                Return _NotifyLanguage
            End Get
            Set(ByVal value As String)
                _NotifyLanguage = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the module ID.
        ''' </summary>
        ''' <value>The module ID.</value>
        Property ModuleID() As Integer
            Get
                Return _ModuleID
            End Get
            Set(ByVal value As Integer)
                _ModuleID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the tab ID.
        ''' </summary>
        ''' <value>The tab ID.</value>
        Property TabID() As Integer
            Get
                Return _TabID
            End Get
            Set(ByVal value As Integer)
                _TabID = value
            End Set
        End Property

    End Class
#End Region

#Region "EventRecurMasterInfo Class "
    ''' <summary>
    ''' Master record for recurring events, holds a set of events together
    ''' </summary>
    Public Class EventRecurMasterInfo

        ''' <summary>
        ''' Priority of the (master) events
        ''' </summary>
        Public Enum Priority
            ''' <summary>
            ''' High priority
            ''' </summary>
            High = 1
            ''' <summary>
            ''' Medium priority
            ''' </summary>
            Medium = 2
            ''' <summary>
            ''' Low priority
            ''' </summary>
            Low = 3
        End Enum

        Private _recurMasterID As Integer
        Private _moduleID As Integer
        Private _portalID As Integer
        Private _rrule As String
        Private _dtstart As DateTime
        Private _duration As String
        Private _until As DateTime
        Private _eventName As String
        Private _eventDesc As String
        Private _importance As Integer
        Private _notify As String
        Private _approved As Boolean
        Private _maxEnrollment As Integer
        Private _signups As Boolean
        Private _enrolled As Integer
        Private _enrollRoleID As Integer = Nothing
        Private _enrollType As String
        Private _enrollFee As Decimal
        Private _payPalAccount As String
        Private _detailPage As Boolean
        Private _detailNewWin As Boolean
        Private _detailURL As String
        Private _imageURL As String
        Private _imageType As String
        Private _imageWidth As Integer
        Private _imageHeight As Integer
        Private _location As Integer
        Private _category As Integer
        Private _imageDisplay As Boolean
        Private _sendReminder As Boolean
        Private _reminderTime As Integer
        Private _reminderTimeMeasurement As String
        Private _reminder As String
        Private _reminderFrom As String
        Private _customField1 As String
        Private _customField2 As String
        Private _enrollListView As Boolean
        Private _displayEndDate As Boolean
        Private _allDayEvent As Boolean
        Private _cultureName As String
        Private _ownerID As Integer
        Private _createdByID As Integer
        Private _createdDate As DateTime
        Private _updatedByID As Integer
        Private _updatedDate As DateTime
        Private _firstEventID As Integer
        Private _eventTimeZoneId As String
        Private _allowAnonEnroll As Boolean
        Private _contentItemId As Integer
        Private _journalItem As Boolean
        Private _socialGroupId As Integer
        Private _socialUserId As Integer
        Private _summary As String
        Private _sequence As Integer

        ''' <summary>
        ''' Gets or sets the recur master ID.
        ''' </summary>
        ''' <value>The recur master ID.</value>
        Property RecurMasterID() As Integer
            Get
                Return _RecurMasterID
            End Get
            Set(ByVal value As Integer)
                _RecurMasterID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the module ID.
        ''' </summary>
        ''' <value>The module ID.</value>
        Property ModuleID() As Integer
            Get
                Return _ModuleID
            End Get
            Set(ByVal value As Integer)
                _ModuleID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the portal ID.
        ''' </summary>
        ''' <value>The portal ID.</value>
        Property PortalID() As Integer
            Get
                Return _PortalID
            End Get
            Set(ByVal value As Integer)
                _PortalID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the Recurrence rule.
        ''' </summary>
        ''' <value>The recurrence rule.</value>
        Property RRULE() As String
            Get
                Return _RRULE
            End Get
            Set(ByVal value As String)
                _RRULE = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the Date start.
        ''' </summary>
        ''' <value>The start date.</value>
        Property Dtstart() As DateTime
            Get
                Return _dtstart
            End Get
            Set(ByVal value As DateTime)
                _dtstart = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the duration.
        ''' </summary>
        ''' <value>The duration.</value>
        Property Duration() As String
            Get
                Return _Duration
            End Get
            Set(ByVal value As String)
                _Duration = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the until.
        ''' </summary>
        ''' <value>The until.</value>
        Property Until() As DateTime
            Get
                Return _Until
            End Get
            Set(ByVal value As DateTime)
                _Until = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the name of the event.
        ''' </summary>
        ''' <value>The name of the event.</value>
        Property EventName() As String
            Get
                Return _EventName
            End Get
            Set(ByVal value As String)
                _EventName = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the event description
        ''' </summary>
        ''' <value>The event description.</value>
        Property EventDesc() As String
            Get
                Return _EventDesc
            End Get
            Set(ByVal value As String)
                _EventDesc = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the importance.
        ''' </summary>
        ''' <value>The importance.</value>
        Property Importance() As Priority
            Get
                Return CType(_Importance, Priority)
            End Get
            Set(ByVal value As Priority)
                _Importance = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the notify.
        ''' </summary>
        ''' <value>The notify.</value>
        Property Notify() As String
            Get
                Return _Notify
            End Get
            Set(ByVal value As String)
                _Notify = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether this <see cref="EventRecurMasterInfo" /> is approved.
        ''' </summary>
        ''' <value><c>true</c> if approved; otherwise, <c>false</c>.</value>
        Property Approved() As Boolean
            Get
                Return _Approved
            End Get
            Set(ByVal value As Boolean)
                _Approved = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether this <see cref="EventRecurMasterInfo" /> is signups.
        ''' </summary>
        ''' <value><c>true</c> if signups; otherwise, <c>false</c>.</value>
        Property Signups() As Boolean
            Get
                Return _Signups
            End Get
            Set(ByVal value As Boolean)
                _Signups = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the max # of enrollment.
        ''' </summary>
        ''' <value>The max # of enrollment.</value>
        Property MaxEnrollment() As Integer
            Get
                Return _MaxEnrollment
            End Get
            Set(ByVal value As Integer)
                _MaxEnrollment = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the # enrolled.
        ''' </summary>
        ''' <value>The #enrolled.</value>
        Property Enrolled() As Integer
            Get
                If _Enrolled < 0 Then
                    Return 0
                End If
                Return _Enrolled
            End Get
            Set(ByVal value As Integer)
                _Enrolled = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the enroll role ID.
        ''' </summary>
        ''' <value>The enroll role ID.</value>
        Property EnrollRoleID() As Integer
            Get
                Return _EnrollRoleID
            End Get
            Set(ByVal value As Integer)
                _EnrollRoleID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the type of the enroll.
        ''' </summary>
        ''' <value>The type of the enroll.</value>
        Property EnrollType() As String
            Get
                Return _EnrollType
            End Get
            Set(ByVal value As String)
                _EnrollType = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the enroll fee.
        ''' </summary>
        ''' <value>The enroll fee.</value>
        Property EnrollFee() As Decimal
            Get
                Return _EnrollFee
            End Get
            Set(ByVal value As Decimal)
                _EnrollFee = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the pay pal account.
        ''' </summary>
        ''' <value>The pay pal account.</value>
        Property PayPalAccount() As String
            Get
                Return _PayPalAccount
            End Get
            Set(ByVal value As String)
                _PayPalAccount = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether to display on a detail page.
        ''' </summary>
        ''' <value><c>true</c> if [detail page]; otherwise, <c>false</c>.</value>
        Property DetailPage() As Boolean
            Get
                Return _DetailPage
            End Get
            Set(ByVal value As Boolean)
                _DetailPage = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets a value indicating whether to dispay the event in a new page.
        ''' </summary>
        ''' <value><c>true</c> if [detail new page]; otherwise, <c>false</c>.</value>
        Property DetailNewWin() As Boolean
            Get
                Return _DetailNewWin
            End Get
            Set(ByVal value As Boolean)
                _DetailNewWin = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the detail URL.
        ''' </summary>
        ''' <value>The detail URL.</value>
        Property DetailURL() As String
            Get
                Return _DetailURL
            End Get
            Set(ByVal value As String)
                _DetailURL = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the image URL.
        ''' </summary>
        ''' <value>The image URL.</value>
        Property ImageURL() As String
            Get
                Return _ImageURL
            End Get
            Set(ByVal value As String)
                _ImageURL = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the type of the image.
        ''' </summary>
        ''' <value>The type of the image.</value>
        Property ImageType() As String
            Get
                Return _ImageType
            End Get
            Set(ByVal value As String)
                _ImageType = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the width of the image.
        ''' </summary>
        ''' <value>The width of the image.</value>
        Property ImageWidth() As Integer
            Get
                Return _ImageWidth
            End Get
            Set(ByVal value As Integer)
                _ImageWidth = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the height of the image.
        ''' </summary>
        ''' <value>The height of the image.</value>
        Property ImageHeight() As Integer
            Get
                Return _ImageHeight
            End Get
            Set(ByVal value As Integer)
                _ImageHeight = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the location.
        ''' </summary>
        ''' <value>The location.</value>
        Property Location() As Integer
            Get
                Return _Location
            End Get
            Set(ByVal value As Integer)
                _Location = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the category.
        ''' </summary>
        ''' <value>The category.</value>
        Property Category() As Integer
            Get
                Return _Category
            End Get
            Set(ByVal value As Integer)
                _Category = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [image display].
        ''' </summary>
        ''' <value><c>true</c> if [image display]; otherwise, <c>false</c>.</value>
        Property ImageDisplay() As Boolean
            Get
                Return _ImageDisplay
            End Get
            Set(ByVal value As Boolean)
                _ImageDisplay = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [send reminder].
        ''' </summary>
        ''' <value><c>true</c> if [send reminder]; otherwise, <c>false</c>.</value>
        Property SendReminder() As Boolean
            Get
                Return _SendReminder
            End Get
            Set(ByVal value As Boolean)
                _SendReminder = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the reminder time.
        ''' </summary>
        ''' <value>The reminder time.</value>
        Property ReminderTime() As Integer
            Get
                Return _ReminderTime
            End Get
            Set(ByVal value As Integer)
                _ReminderTime = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the reminder time measurement.
        ''' </summary>
        ''' <value>The reminder time measurement.</value>
        Property ReminderTimeMeasurement() As String
            Get
                Return _ReminderTimeMeasurement
            End Get
            Set(ByVal value As String)
                _ReminderTimeMeasurement = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the reminder.
        ''' </summary>
        ''' <value>The reminder.</value>
        Property Reminder() As String
            Get
                Return _Reminder
            End Get
            Set(ByVal value As String)
                _Reminder = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the reminder from.
        ''' </summary>
        ''' <value>The reminder from.</value>
        Property ReminderFrom() As String
            Get
                Return _ReminderFrom
            End Get
            Set(ByVal value As String)
                _ReminderFrom = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the custom field1.
        ''' </summary>
        ''' <value>The custom field1.</value>
        Property CustomField1() As String
            Get
                Return _CustomField1
            End Get
            Set(ByVal value As String)
                _CustomField1 = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the custom field2.
        ''' </summary>
        ''' <value>The custom field2.</value>
        Property CustomField2() As String
            Get
                Return _CustomField2
            End Get
            Set(ByVal value As String)
                _CustomField2 = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [enroll list view].
        ''' </summary>
        ''' <value><c>true</c> if [enroll list view]; otherwise, <c>false</c>.</value>
        Property EnrollListView() As Boolean
            Get
                Return _EnrollListView
            End Get
            Set(ByVal value As Boolean)
                _EnrollListView = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [display end date].
        ''' </summary>
        ''' <value><c>true</c> if [display end date]; otherwise, <c>false</c>.</value>
        Property DisplayEndDate() As Boolean
            Get
                Return _DisplayEndDate
            End Get
            Set(ByVal value As Boolean)
                _DisplayEndDate = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [all day event].
        ''' </summary>
        ''' <value><c>true</c> if [all day event]; otherwise, <c>false</c>.</value>
        Property AllDayEvent() As Boolean
            Get
                Return _AllDayEvent
            End Get
            Set(ByVal value As Boolean)
                _AllDayEvent = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the name of the culture.
        ''' </summary>
        ''' <value>The name of the culture.</value>
        Property CultureName() As String
            Get
                Return _CultureName
            End Get
            Set(ByVal value As String)
                _CultureName = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the owner ID.
        ''' </summary>
        ''' <value>The owner ID.</value>
        Property OwnerID() As Integer
            Get
                Return _OwnerID
            End Get
            Set(ByVal value As Integer)
                _OwnerID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the created by ID.
        ''' </summary>
        ''' <value>The created by ID.</value>
        Property CreatedByID() As Integer
            Get
                Return _CreatedByID
            End Get
            Set(ByVal value As Integer)
                _CreatedByID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the created date.
        ''' </summary>
        ''' <value>The created date.</value>
        Property CreatedDate() As DateTime
            Get
                Return _CreatedDate
            End Get
            Set(ByVal value As Date)
                _CreatedDate = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the updated by ID.
        ''' </summary>
        ''' <value>The updated by ID.</value>
        Property UpdatedByID() As Integer
            Get
                Return _UpdatedByID
            End Get
            Set(ByVal value As Integer)
                _UpdatedByID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the updated date.
        ''' </summary>
        ''' <value>The updated date.</value>
        Property UpdatedDate() As DateTime
            Get
                Return _UpdatedDate
            End Get
            Set(ByVal value As DateTime)
                _UpdatedDate = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the first event ID.
        ''' </summary>
        ''' <value>The first event ID.</value>
        Property FirstEventID() As Integer
            Get
                Return _FirstEventID
            End Get
            Set(ByVal value As Integer)
                _FirstEventID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating what the [event timezoneid] is
        ''' </summary>
        ''' <value>The event timezoneid.</value>
        Property EventTimeZoneId() As String
            Get
                If String.IsNullOrEmpty(_EventTimeZoneId) Then
                    Dim ems As New EventModuleSettings
                    Dim modSettings As EventModuleSettings = ems.GetEventModuleSettings(_moduleID, Nothing)
                    _eventTimeZoneId = modSettings.TimeZoneId
                End If
                Return _EventTimeZoneId
            End Get
            Set(ByVal value As String)
                _EventTimeZoneId = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [allow anonymous enrollment].
        ''' </summary>
        ''' <value><c>true</c> if [allow anonymous enrollment]; otherwise, <c>false</c>.</value>
        Property AllowAnonEnroll() As Boolean
            Get
                Return _AllowAnonEnroll
            End Get
            Set(ByVal value As Boolean)
                _AllowAnonEnroll = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating the [content item id].
        ''' </summary>
        ''' <value>The contentitemid.</value>
        Property ContentItemID() As Integer
            Get
                Return _ContentItemId
            End Get
            Set(ByVal value As Integer)
                _ContentItemId = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [event has journal item].
        ''' </summary>
        ''' <value><c>true</c> if [event has journal item]; otherwise, <c>false</c>.</value>
        Property JournalItem() As Boolean
            Get
                Return _JournalItem
            End Get
            Set(ByVal value As Boolean)
                _JournalItem = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating the [socialgroup id].
        ''' </summary>
        ''' <value>The SocialGroupid.</value>
        Property SocialGroupID() As Integer
            Get
                Return _SocialGroupId
            End Get
            Set(ByVal value As Integer)
                _SocialGroupId = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating the [social user id].
        ''' </summary>
        ''' <value>The SocialUserid.</value>
        Property SocialUserID() As Integer
            Get
                Return _SocialUserId
            End Get
            Set(ByVal value As Integer)
                _SocialUserId = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the summary.
        ''' </summary>
        ''' <value>The summary.</value>
        Property Summary() As String
            Get
                Return _Summary
            End Get
            Set(ByVal value As String)
                _Summary = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating the [sequence].
        ''' </summary>
        ''' <value>The Sequence.</value>
        Property Sequence() As Integer
            Get
                Return _Sequence
            End Get
            Set(ByVal value As Integer)
                _Sequence = value
            End Set
        End Property

    End Class
#End Region

#Region "EventRRULEInfo Class "
    ''' <summary>
    ''' Information about the recurrrence rules
    ''' </summary>
    Public Class EventRRULEInfo

        Private _freq As String
        Private _interval As Integer
        Private _byDay As String
        Private _byMonthDay As Integer
        Private _byMonth As Integer
        Private _su, _mo, _tu, _we, _th, _fr, _sa As Boolean
        Private _suNo, _moNo, _tuNo, _weNo, _thNo, _frNo, _saNo As Integer
        Private _freqBasic As Boolean
        Private _wkst As String

        ''' <summary>
        ''' Gets or sets the frequency
        ''' </summary>
        ''' <value>The frequency.</value>
        Property Freq() As String
            Get
                Return _Freq
            End Get
            Set(ByVal value As String)
                _Freq = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the interval.
        ''' </summary>
        ''' <value>The interval.</value>
        Property Interval() As Integer
            Get
                Return _Interval
            End Get
            Set(ByVal value As Integer)
                _Interval = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the by day.
        ''' </summary>
        ''' <value>The by day.</value>
        Property ByDay() As String
            Get
                Return _ByDay
            End Get
            Set(ByVal value As String)
                _ByDay = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the by day of the month.
        ''' </summary>
        ''' <value>The by day of the month.</value>
        Property ByMonthDay() As Integer
            Get
                Return _ByMonthDay
            End Get
            Set(ByVal value As Integer)
                _ByMonthDay = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the by month.
        ''' </summary>
        ''' <value>The by month.</value>
        Property ByMonth() As Integer
            Get
                Return _ByMonth
            End Get
            Set(ByVal value As Integer)
                _ByMonth = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether this <see cref="EventRRULEInfo" /> is sunday
        ''' </summary>
        ''' <value><c>true</c> if sunday; otherwise, <c>false</c>.</value>
        Property Su() As Boolean
            Get
                Return _Su
            End Get
            Set(ByVal value As Boolean)
                _Su = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the no sunday
        ''' </summary>
        ''' <value>The no sunday</value>
        Property SuNo() As Integer
            Get
                Return _SuNo
            End Get
            Set(ByVal value As Integer)
                _SuNo = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether this <see cref="EventRRULEInfo" /> is monday
        ''' </summary>
        ''' <value><c>true</c> if monday; otherwise, <c>false</c>.</value>
        Property Mo() As Boolean
            Get
                Return _Mo
            End Get
            Set(ByVal value As Boolean)
                _Mo = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the no monday
        ''' </summary>
        ''' <value>The no monday</value>
        Property MoNo() As Integer
            Get
                Return _MoNo
            End Get
            Set(ByVal value As Integer)
                _MoNo = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether this <see cref="EventRRULEInfo" /> is tuesday
        ''' </summary>
        ''' <value><c>true</c> if tuesday; otherwise, <c>false</c>.</value>
        Property Tu() As Boolean
            Get
                Return _Tu
            End Get
            Set(ByVal value As Boolean)
                _Tu = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the no tuesday
        ''' </summary>
        ''' <value>The no tuesday.</value>
        Property TuNo() As Integer
            Get
                Return _TuNo
            End Get
            Set(ByVal value As Integer)
                _TuNo = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether this <see cref="EventRRULEInfo" /> is wednessday
        ''' </summary>
        ''' <value><c>true</c> if wednessday; otherwise, <c>false</c>.</value>
        Property We() As Boolean
            Get
                Return _We
            End Get
            Set(ByVal value As Boolean)
                _We = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the no wednessday.
        ''' </summary>
        ''' <value>The no wednessday.</value>
        Property WeNo() As Integer
            Get
                Return _WeNo
            End Get
            Set(ByVal value As Integer)
                _WeNo = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether this <see cref="EventRRULEInfo" /> is thursday.
        ''' </summary>
        ''' <value><c>true</c> if thursday; otherwise, <c>false</c>.</value>
        Property Th() As Boolean
            Get
                Return _Th
            End Get
            Set(ByVal value As Boolean)
                _Th = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the no thursday
        ''' </summary>
        ''' <value>The no thursday.</value>
        Property ThNo() As Integer
            Get
                Return _ThNo
            End Get
            Set(ByVal value As Integer)
                _ThNo = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether this <see cref="EventRRULEInfo" /> is friday.
        ''' </summary>
        ''' <value><c>true</c> if friday; otherwise, <c>false</c>.</value>
        Property Fr() As Boolean
            Get
                Return _Fr
            End Get
            Set(ByVal value As Boolean)
                _Fr = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the no friday.
        ''' </summary>
        ''' <value>The no friday.</value>
        Property FrNo() As Integer
            Get
                Return _FrNo
            End Get
            Set(ByVal value As Integer)
                _FrNo = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether this <see cref="EventRRULEInfo" /> is saturday.
        ''' </summary>
        ''' <value><c>true</c> if saturday; otherwise, <c>false</c>.</value>
        Property Sa() As Boolean
            Get
                Return _Sa
            End Get
            Set(ByVal value As Boolean)
                _Sa = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the no saturday.
        ''' </summary>
        ''' <value>The no saturday.</value>
        Property SaNo() As Integer
            Get
                Return _SaNo
            End Get
            Set(ByVal value As Integer)
                _SaNo = value
            End Set
        End Property

        Property FreqBasic() As Boolean
            Get
                Return _FreqBasic
            End Get
            Set(ByVal value As Boolean)
                _FreqBasic = value
            End Set
        End Property

        Property Wkst() As String
            Get
                Return _wkst
            End Get
            Set(ByVal value As String)
                _wkst = value
            End Set
        End Property

    End Class
#End Region

#Region "EventEmailInfo Class "
    ''' <summary>
    ''' Information abotu e-mails related to events
    ''' </summary>
    Public Class EventEmailInfo
        Private _txtEmailSubject As String
        Private _txtEmailBody As String
        Private _txtEmailFrom As String
        Private _userIDs As ArrayList
        Private _userEmails As ArrayList
        Private _userLocales As ArrayList
        Private _userTimeZoneIds As ArrayList

        ''' <summary>
        ''' Gets or sets the  email subject.
        ''' </summary>
        ''' <value>The  email subject.</value>
        Property TxtEmailSubject() As String
            Get
                Return _txtEmailSubject
            End Get
            Set(ByVal value As String)
                _txtEmailSubject = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the  email body.
        ''' </summary>
        ''' <value>The email body.</value>
        Property TxtEmailBody() As String
            Get
                Return _txtEmailBody
            End Get
            Set(ByVal value As String)
                _txtEmailBody = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the email from.
        ''' </summary>
        ''' <value>The email from.</value>
        Property TxtEmailFrom() As String
            Get
                Return _txtEmailFrom
            End Get
            Set(ByVal value As String)
                _txtEmailFrom = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the user I ds.
        ''' </summary>
        ''' <value>The user I ds.</value>
        Property UserIDs() As ArrayList
            Get
                Return _UserIDs
            End Get
            Set(ByVal value As ArrayList)
                _userIDs = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the user emails.
        ''' </summary>
        ''' <value>The user emails.</value>
        Property UserEmails() As ArrayList
            Get
                Return _userEmails
            End Get
            Set(ByVal value As ArrayList)
                _userEmails = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the user locales.
        ''' </summary>
        ''' <value>The user locales.</value>
        Property UserLocales() As ArrayList
            Get
                Return _userLocales
            End Get
            Set(ByVal value As ArrayList)
                _userLocales = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the user timezoneids.
        ''' </summary>
        ''' <value>The user timezoneids.</value>
        Property UserTimeZoneIds() As ArrayList
            Get
                Return _userTimeZoneIds
            End Get
            Set(ByVal value As ArrayList)
                _userTimeZoneIds = Value
            End Set
        End Property

        ''' <summary>
        ''' Initializes a new instance of the <see cref="EventEmailInfo" /> class.
        ''' </summary>
        Public Sub New()
            Dim newUserEmails As New ArrayList()
            UserEmails = newUserEmails

            Dim newUserIDs As New ArrayList()
            UserIDs = newUserIDs

            Dim newUserLocales As New ArrayList()
            UserLocales = newUserLocales

            Dim newUserTimeZoneIds As New ArrayList()
            UserTimeZoneIds = newUserTimeZoneIds

        End Sub
    End Class
#End Region

#Region "EventSubscriptionInfo Class"
    ''' <summary>
    ''' Information about subscription o events
    ''' </summary>
    Public Class EventSubscriptionInfo

        Private _portalID As Integer
        Private _moduleID As Integer
        Private _userID As Integer
        Private _subscriptionID As Integer

        ' initialization
        ''' <summary>
        ''' Initializes a new instance of the <see cref="EventSubscriptionInfo" /> class.
        ''' </summary>
        Public Sub New()
        End Sub

        ' public properties
        ''' <summary>
        ''' Gets or sets the portal ID.
        ''' </summary>
        ''' <value>The portal ID.</value>
        Property PortalID() As Integer
            Get
                Return _PortalID
            End Get
            Set(ByVal value As Integer)
                _PortalID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the subscription ID.
        ''' </summary>
        ''' <value>The subscription ID.</value>
        Property SubscriptionID() As Integer
            Get
                Return _SubscriptionID
            End Get
            Set(ByVal value As Integer)
                _SubscriptionID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the module ID.
        ''' </summary>
        ''' <value>The module ID.</value>
        Property ModuleID() As Integer
            Get
                Return _ModuleID
            End Get
            Set(ByVal value As Integer)
                _ModuleID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the user ID.
        ''' </summary>
        ''' <value>The user ID.</value>
        Property UserID() As Integer
            Get
                Return _UserID
            End Get
            Set(ByVal value As Integer)
                _UserID = value
            End Set
        End Property

    End Class
#End Region

#Region "EventListObject Class"
    ''' <summary>
    ''' Object for listing the events
    ''' </summary>
    Public Class EventListObject
        Implements IComparable

        ' public properties
        Private _indexId As Integer
        ''' <summary>
        ''' Gets or sets the index id.
        ''' </summary>
        ''' <value>The index id.</value>
        Property IndexId() As Integer
            Get
                Return _IndexId
            End Get
            Set(ByVal value As Integer)
                _IndexId = value
            End Set
        End Property
        Private _eventID As Integer
        ''' <summary>
        ''' Gets or sets the event ID.
        ''' </summary>
        ''' <value>The event ID.</value>
        Property EventID() As Integer
            Get
                Return _EventID
            End Get
            Set(ByVal value As Integer)
                _EventID = value
            End Set
        End Property

        Private _createdByID As Integer
        ''' <summary>
        ''' Gets or sets the created by ID.
        ''' </summary>
        ''' <value>The created by ID.</value>
        Property CreatedByID() As Integer
            Get
                Return _CreatedByID
            End Get
            Set(ByVal value As Integer)
                _CreatedByID = value
            End Set
        End Property

        Private _ownerID As Integer
        ''' <summary>
        ''' Gets or sets the owner ID.
        ''' </summary>
        ''' <value>The owner ID.</value>
        Property OwnerID() As Integer
            Get
                Return _OwnerID
            End Get
            Set(ByVal value As Integer)
                _OwnerID = value
            End Set
        End Property

        Private _moduleID As Integer
        ''' <summary>
        ''' Gets or sets the module ID.
        ''' </summary>
        ''' <value>The module ID.</value>
        Property ModuleID() As Integer
            Get
                Return _ModuleID
            End Get
            Set(ByVal value As Integer)
                _ModuleID = value
            End Set
        End Property

        Private _eventDateBegin As Date
        ''' <summary>
        ''' Gets or sets the event date begin.
        ''' </summary>
        ''' <value>The event date begin.</value>
        Property EventDateBegin() As Date
            Get
                Return _EventDateBegin
            End Get
            Set(ByVal value As Date)
                _EventDateBegin = Value
            End Set
        End Property

        Private _eventDateEnd As Date
        ''' <summary>
        ''' Gets or sets the event date end.
        ''' </summary>
        ''' <value>The event date end.</value>
        Property EventDateEnd() As Date
            Get
                Return _EventDateEnd
            End Get
            Set(ByVal value As Date)
                _EventDateEnd = Value
            End Set
        End Property

        Private _txtEventDateEnd As String
        ''' <summary>
        ''' Gets or sets the event date end.
        ''' </summary>
        ''' <value>The event date end.</value>
        Property TxtEventDateEnd() As String
            Get
                Return _txtEventDateEnd
            End Get
            Set(ByVal value As String)
                _txtEventDateEnd = value
            End Set
        End Property

        Private _eventTimeBegin As DateTime
        ''' <summary>
        ''' Gets or sets the event time begin.
        ''' </summary>
        ''' <value>The event time begin.</value>
        Property EventTimeBegin() As DateTime
            Get
                Return _EventTimeBegin
            End Get
            Set(ByVal value As DateTime)
                _EventTimeBegin = Value
            End Set
        End Property

        Private _txtEventTimeBegin As String
        ''' <summary>
        ''' Gets or sets the event time begin.
        ''' </summary>
        ''' <value>The event time begin.</value>
        Property TxtEventTimeBegin() As String
            Get
                Return _txtEventTimeBegin
            End Get
            Set(ByVal value As String)
                _txtEventTimeBegin = value
            End Set
        End Property

        Private _recurUntil As String
        ''' <summary>
        ''' Gets or sets the recurrence until.
        ''' </summary>
        ''' <value>The recurrence until.</value>
        Property RecurUntil() As String
            Get
                Return _RecurUntil
            End Get
            Set(ByVal value As String)
                _RecurUntil = value
            End Set
        End Property

        Private _duration As Integer
        ''' <summary>
        ''' Gets or sets the duration.
        ''' </summary>
        ''' <value>The duration.</value>
        Property Duration() As Integer
            Get
                Return _Duration
            End Get
            Set(ByVal value As Integer)
                _Duration = value
            End Set
        End Property

        Private _eventName As String
        ''' <summary>
        ''' Gets or sets the name of the event.
        ''' </summary>
        ''' <value>The name of the event.</value>
        Property EventName() As String
            Get
                Return _EventName
            End Get
            Set(ByVal value As String)
                _EventName = value
            End Set
        End Property

        Private _eventDesc As String
        ''' <summary>
        ''' Gets or sets the event description
        ''' </summary>
        ''' <value>The event description.</value>
        Property EventDesc() As String
            Get
                Return _EventDesc
            End Get
            Set(ByVal value As String)
                _EventDesc = value
            End Set
        End Property

        Private _decodedDesc As String
        ''' <summary>
        ''' Gets or sets the decoded description.
        ''' </summary>
        ''' <value>The decoded description.</value>
        Property DecodedDesc() As String
            Get
                Return _DecodedDesc
            End Get
            Set(ByVal value As String)
                _DecodedDesc = value
            End Set
        End Property

        Private _recurText As String
        ''' <summary>
        ''' Gets or sets the recurrence text.
        ''' </summary>
        ''' <value>The recurrence text.</value>
        Property RecurText() As String
            Get
                Return _RecurText
            End Get
            Set(ByVal value As String)
                _RecurText = value
            End Set
        End Property

        Private _url As String
        ''' <summary>
        ''' Gets or sets the URL.
        ''' </summary>
        ''' <value>The URL.</value>
        Property URL() As String
            Get
                Return _URL
            End Get
            Set(ByVal value As String)
                _URL = value
            End Set
        End Property

        Private _target As String
        ''' <summary>
        ''' Gets or sets the target.
        ''' </summary>
        ''' <value>The target.</value>
        Property Target() As String
            Get
                Return _Target
            End Get
            Set(ByVal value As String)
                _Target = value
            End Set
        End Property

        Private _imageURL As String
        ''' <summary>
        ''' Gets or sets the image URL.
        ''' </summary>
        ''' <value>The image URL.</value>
        Property ImageURL() As String
            Get
                Return _ImageURL
            End Get
            Set(ByVal value As String)
                _ImageURL = value
            End Set
        End Property


        Private _categoryName As String
        ''' <summary>
        ''' Gets or sets the name of the category.
        ''' </summary>
        ''' <value>The name of the category.</value>
        Property CategoryName() As String
            Get
                Return _CategoryName
            End Get
            Set(ByVal value As String)
                _CategoryName = value
            End Set
        End Property

        Private _locationName As String
        ''' <summary>
        ''' Gets or sets the name of the location.
        ''' </summary>
        ''' <value>The name of the location.</value>
        Property LocationName() As String
            Get
                Return _LocationName
            End Get
            Set(ByVal value As String)
                _LocationName = value
            End Set
        End Property

        Private _customField1 As String
        ''' <summary>
        ''' Gets or sets the custom field1.
        ''' </summary>
        ''' <value>The custom field1.</value>
        Property CustomField1() As String
            Get
                Return _CustomField1
            End Get
            Set(ByVal value As String)
                _CustomField1 = value
            End Set
        End Property

        Private _customField2 As String
        ''' <summary>
        ''' Gets or sets the custom field2.
        ''' </summary>
        ''' <value>The custom field2.</value>
        Property CustomField2() As String
            Get
                Return _CustomField2
            End Get
            Set(ByVal value As String)
                _CustomField2 = value
            End Set
        End Property

        Private _editVisibility As Boolean
        ''' <summary>
        ''' Gets or sets a value indicating whether visibility is editable.
        ''' </summary>
        ''' <value><c>true</c> if [edit visibility]; otherwise, <c>false</c>.</value>
        Property EditVisibility() As Boolean
            Get
                Return _EditVisibility
            End Get
            Set(ByVal value As Boolean)
                _EditVisibility = value
            End Set
        End Property

        Private _categoryColor As Color
        ''' <summary>
        ''' Gets or sets the color of the category.
        ''' </summary>
        ''' <value>The color of the category.</value>
        Property CategoryColor() As Color
            Get
                Return _categoryColor
            End Get
            Set(ByVal value As Color)
                _categoryColor = value
            End Set
        End Property

        Private _categoryFontColor As Color
        ''' <summary>
        ''' Gets or sets the color of the category font.
        ''' </summary>
        ''' <value>The color of the category font.</value>
        Property CategoryFontColor() As Color
            Get
                Return _categoryFontColor
            End Get
            Set(ByVal value As Color)
                _categoryFontColor = value
            End Set
        End Property

        Private _displayDuration As Integer
        ''' <summary>
        ''' Gets or sets the display duration.
        ''' </summary>
        ''' <value>The display duration.</value>
        Property DisplayDuration() As Integer
            Get
                Return _DisplayDuration
            End Get
            Set(ByVal value As Integer)
                _DisplayDuration = value
            End Set
        End Property

        Private _recurMasterID As Integer
        ''' <summary>
        ''' Gets or sets the recur master ID.
        ''' </summary>
        ''' <value>The recur master ID.</value>
        Property RecurMasterID() As Integer
            Get
                Return _RecurMasterID
            End Get
            Set(ByVal value As Integer)
                _RecurMasterID = value
            End Set
        End Property

        Private _icons As String
        ''' <summary>
        ''' Gets or sets the icons.
        ''' </summary>
        ''' <value>The icons.</value>
        Property Icons() As String
            Get
                Return _Icons
            End Get
            Set(ByVal value As String)
                _Icons = value
            End Set
        End Property

        Private _tooltip As String
        ''' <summary>
        ''' Gets or sets the tooltip.
        ''' </summary>
        ''' <value>The tooltip.</value>
        Property Tooltip() As String
            Get
                Return _Tooltip
            End Get
            Set(ByVal value As String)
                _Tooltip = value
            End Set
        End Property

        Private Shared _sortExpression As SortFilter
        ''' <summary>
        ''' Gets or sets the sort expression.
        ''' </summary>
        ''' <value>The sort expression.</value>
        Shared Property SortExpression() As SortFilter
            Get
                Return _SortExpression
            End Get
            Set(ByVal value As SortFilter)
                _SortExpression = value
            End Set
        End Property

        Private Shared _sortDirection As SortDirection
        ''' <summary>
        ''' Gets or sets the sort direction.
        ''' </summary>
        ''' <value>The sort direction.</value>
        Shared Property SortDirection() As SortDirection
            Get
                Return _SortDirection
            End Get
            Set(ByVal value As SortDirection)
                _SortDirection = value
            End Set
        End Property


        ''' <summary>
        ''' Sorting enumeration
        ''' </summary>
        Public Enum SortFilter
            ''' <summary>
            ''' By EventID
            ''' </summary>
            EventID
            ''' <summary>
            ''' By Date beging
            ''' </summary>
            EventDateBegin
            ''' <summary>
            ''' By Date end
            ''' </summary>
            EventDateEnd
            ''' <summary>
            ''' Bu Name
            ''' </summary>
            EventName
            ''' <summary>
            ''' By duration
            ''' </summary>
            Duration
            ''' <summary>
            ''' Bu category name
            ''' </summary>
            CategoryName
            ''' <summary>
            ''' By customfield1
            ''' </summary>
            CustomField1
            ''' <summary>
            ''' By customfield2
            ''' </summary>
            CustomField2
            ''' <summary>
            ''' By description
            ''' </summary>
            Description
            ''' <summary>
            ''' By Location name
            ''' </summary>
            LocationName
        End Enum

        ''' <summary>
        ''' Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        ''' </summary>
        ''' <param name="obj">An object to compare with this instance.</param>
        ''' <returns>
        ''' A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings:
        ''' Value
        ''' Meaning
        ''' Less than zero
        ''' This instance is less than <paramref name="obj" />.
        ''' Zero
        ''' This instance is equal to <paramref name="obj" />.
        ''' Greater than zero
        ''' This instance is greater than <paramref name="obj" />.
        ''' </returns>
        ''' <exception cref="T:System.ArgumentException">
        ''' <paramref name="obj" /> is not the same type as this instance.
        ''' </exception>
        Public Function CompareTo(ByVal obj As Object) As Integer Implements IComparable.CompareTo
            Dim o As EventListObject = CType(obj, EventListObject)
            Dim xCompare As String = EventName + Format(EventID, "00000000")
            Dim yCompare As String = o.EventName + Format(o.EventID, "00000000")
            Select Case SortExpression
                Case SortFilter.CategoryName
                    xCompare = CategoryName + Format(EventID, "00000000")
                    yCompare = o.CategoryName + Format(o.EventID, "00000000")
                Case SortFilter.CustomField1
                    xCompare = CustomField1 + Format(EventID, "00000000")
                    yCompare = o.CustomField1 + Format(o.EventID, "00000000")
                Case SortFilter.CustomField2
                    xCompare = CustomField2 + Format(EventID, "00000000")
                    yCompare = o.CustomField2 + Format(o.EventID, "00000000")
                Case SortFilter.Description
                    xCompare = EventDesc + Format(EventID, "00000000")
                    yCompare = o.EventDesc + Format(o.EventID, "00000000")
                Case SortFilter.Duration
                    xCompare = Format(Duration, "000000") + Format(EventID, "00000000")
                    yCompare = Format(o.Duration, "000000") + Format(o.EventID, "00000000")
                Case SortFilter.EventDateBegin
                    xCompare = Format(EventDateBegin, "yyyyMMddHHmm") + Format(EventID, "00000000")
                    yCompare = Format(o.EventDateBegin, "yyyyMMddHHmm") + Format(o.EventID, "00000000")
                Case SortFilter.EventDateEnd
                    xCompare = Format(EventDateEnd, "yyyyMMddHHmm") + Format(EventID, "00000000")
                    yCompare = Format(o.EventDateEnd, "yyyyMMddHHmm") + Format(o.EventID, "00000000")
                Case SortFilter.LocationName
                    xCompare = LocationName + Format(EventID, "00000000")
                    yCompare = o.LocationName + Format(o.EventID, "00000000")
                Case SortFilter.EventID
                    xCompare = Format(EventID, "00000000")
                    yCompare = Format(o.EventID, "00000000")
            End Select
            If SortDirection = System.Web.UI.WebControls.SortDirection.Ascending Then
                Return System.String.Compare(xCompare, yCompare, StringComparison.CurrentCulture)
            Else
                Return System.String.Compare(yCompare, xCompare, StringComparison.CurrentCulture)
            End If
        End Function
    End Class
#End Region
#Region "EventUser Class"
    ''' <summary>
    ''' user related to an event
    ''' </summary>
    Public Class EventUser
        Private _userID As Integer
        Private _displayName As String
        Private _profileURL As String
        Private _displayNameURL As String

        ''' <summary>
        ''' Gets or sets the user ID.
        ''' </summary>
        ''' <value>The user ID.</value>
        Property UserID() As Integer
            Get
                Return _UserID
            End Get
            Set(ByVal value As Integer)
                _UserID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the display name.
        ''' </summary>
        ''' <value>The display name.</value>
        Property DisplayName() As String
            Get
                Return _DisplayName
            End Get
            Set(ByVal value As String)
                _DisplayName = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the profile URL.
        ''' </summary>
        ''' <value>The profile URL.</value>
        Property ProfileURL() As String
            Get
                Return _ProfileURL
            End Get
            Set(ByVal value As String)
                _ProfileURL = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the display name URL.
        ''' </summary>
        ''' <value>The display name URL.</value>
        Property DisplayNameURL() As String
            Get
                Return _DisplayNameURL
            End Get
            Set(ByVal value As String)
                _DisplayNameURL = value
            End Set
        End Property

    End Class
#End Region

#Region "EventEnrollList Class "
    ''' <summary>
    ''' List of users enrolled into an event
    ''' </summary>
    Public Class EventEnrollList
        Private _signupID As Int32
        Private _enrollUserName As String
        Private _enrollDisplayName As String
        Private _enrollEmail As String
        Private _enrollPhone As String
        Private _enrollApproved As Boolean
        Private _enrollNo As Integer
        Private _enrollTimeBegin As DateTime

        ''' <summary>
        ''' Gets or sets the signup ID.
        ''' </summary>
        ''' <value>The signup ID.</value>
        Property SignupID() As Int32
            Get
                Return _signupID
            End Get
            Set(ByVal value As Int32)
                _signupID = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the name of the enroll user.
        ''' </summary>
        ''' <value>The name of the enroll user.</value>
        Property EnrollUserName() As String
            Get
                Return _enrollUserName
            End Get
            Set(ByVal value As String)
                _enrollUserName = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the display name of the enroll.
        ''' </summary>
        ''' <value>The display name of the enroll.</value>
        Property EnrollDisplayName() As String
            Get
                Return _enrollDisplayName
            End Get
            Set(ByVal value As String)
                _enrollDisplayName = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the enroll email.
        ''' </summary>
        ''' <value>The enroll email.</value>
        Property EnrollEmail() As String
            Get
                Return _enrollEmail
            End Get
            Set(ByVal value As String)
                _enrollEmail = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the enroll phone.
        ''' </summary>
        ''' <value>The enroll phone.</value>
        Property EnrollPhone() As String
            Get
                Return _enrollPhone
            End Get
            Set(ByVal value As String)
                _enrollPhone = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether [enroll approved].
        ''' </summary>
        ''' <value><c>true</c> if [enroll approved]; otherwise, <c>false</c>.</value>
        Property EnrollApproved() As Boolean
            Get
                Return _enrollApproved
            End Get
            Set(ByVal value As Boolean)
                _enrollApproved = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the enroll no.
        ''' </summary>
        ''' <value>The enroll no.</value>
        Property EnrollNo() As Integer
            Get
                Return _enrollNo
            End Get
            Set(ByVal value As Integer)
                _enrollNo = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the enroll time begin.
        ''' </summary>
        ''' <value>The enroll time begin.</value>
        Property EnrollTimeBegin() As DateTime
            Get
                Return _enrollTimeBegin
            End Get
            Set(ByVal value As DateTime)
                _enrollTimeBegin = Value
            End Set
        End Property
    End Class
#End Region

End Namespace