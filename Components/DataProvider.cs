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
    using System.Data;
    using DotNetNuke.Framework;

    public abstract class DataProvider
    {
        public abstract string ObjectQualifier { get; }
        public abstract string DatabaseOwner { get; }

        #region Event PayPal Functions

        public abstract IDataReader EventsPpErrorLogAdd(int signupId, string
                                                            payPalStatus, string payPalReason, string payPalTransId,
                                                        string payPalPayerId, string payPalPayerStatus,
                                                        string payPalRecieverEmail, string payPalUserEmail,
                                                        string payPalPayerEmail,
                                                        string payPalFirstName, string payPalLastName,
                                                        string payPalAddress, string payPalCity,
                                                        string payPalState, string payPalZip, string payPalCountry,
                                                        string payPalCurrency,
                                                        DateTime payPalPaymentDate, double payPalAmount,
                                                        double payPalFee);

        #endregion

        #region Shared/Static Methods

        // singleton reference to the instantiated object
        private static DataProvider _objProvider;

        // constructor
        static DataProvider()
        {
            CreateProvider();
        }

        // dynamically create provider
        private static void CreateProvider()
        {
            _objProvider = (DataProvider) Reflection.CreateObject("data", "DotNetNuke.Modules.Events", "");
        }

        // return the provider
        public new static DataProvider Instance()
        {
            return _objProvider;
        }

        #endregion

        #region Event Functions

        public abstract void EventsDelete(int eventId, int moduleId);
        public abstract IDataReader EventsGet(int eventId, int moduleId);

        public abstract IDataReader EventsGetByRange(string modules, DateTime beginDate, DateTime endDate,
                                                     string categoryIDs, string locationIDs, int socialGroupId,
                                                     int socialUserId);

        public abstract IDataReader EventsSave(int portalId, int eventId, int recurMasterId, int moduleId,
                                               DateTime eventTimeBegin, int duration, string eventName,
                                               string eventDesc, int importance, string createdById, string notify,
                                               bool approved, bool signups, int maxEnrollment, int enrollRoleId,
                                               decimal enrollFee, string enrollType, string payPalAccount,
                                               bool cancelled, bool detailPage, bool detailNewWin, string detailUrl,
                                               string imageUrl, string imageType, int imageWidth, int imageHeight,
                                               bool imageDisplay, int location, int category, string reminder,
                                               bool sendReminder, int reminderTime, string reminderTimeMeasurement,
                                               string reminderFrom, bool searchSubmitted, string customField1,
                                               string customField2, bool enrollListView, bool displayEndDate,
                                               bool allDayEvent, int ownerId, int lastUpdatedId,
                                               DateTime originalDateBegin, bool newEventEmailSent, bool allowAnonEnroll,
                                               int contentItemId, bool journalItem, string summary, bool saveOnly);

        public abstract IDataReader EventsModerateEvents(int moduleId, int socialGroupId);
        public abstract int EventsTimeZoneCount(int moduleId);
        public abstract void EventsUpgrade(string moduleVersion);
        public abstract void EventsCleanupExpired(int portalId, int moduleId);
        public abstract IDataReader EventsGetRecurrences(int recurMasterId, int moduleId);

        #endregion

        #region Master Event Functions

        public abstract void EventsMasterDelete(int masterId, int moduleId);
        public abstract IDataReader EventsMasterGet(int moduleId, int subEventId);
        public abstract IDataReader EventsMasterAssignedModules(int moduleId);
        public abstract IDataReader EventsMasterAvailableModules(int portalId, int moduleId);
        public abstract IDataReader EventsMasterSave(int masterId, int moduleId, int subEventId);

        #endregion

        #region Event Categories and Locations Functions

        public abstract void EventsCategoryDelete(int category, int portalId);
        public abstract IDataReader EventsCategoryGetByName(string categoryName, int portalId);
        public abstract IDataReader EventsCategoryGet(int category, int portalId);
        public abstract IDataReader EventsCategoryList(int portalId);

        public abstract IDataReader EventsCategorySave(int portalId, int category, string
                                                           categoryName, string color, string fontColor);

        public abstract void EventsLocationDelete(int location, int portalId);
        public abstract IDataReader EventsLocationGetByName(string locationName, int portalId);
        public abstract IDataReader EventsLocationGet(int location, int portalId);
        public abstract IDataReader EventsLocationList(int portalId);

        public abstract IDataReader EventsLocationSave(int portalId, int location, string
                                                           locationName, string mapUrl, string street,
                                                       string postalCode, string city, string region,
                                                       string country);

        #endregion

        #region Event Enrollment and Moderation Functions

        public abstract void EventsSignupsDelete(int signupId, int moduleId);
        public abstract IDataReader EventsModerateSignups(int moduleId, int socialGroupId);
        public abstract IDataReader EventsSignupsGet(int signupId, int moduleId, bool ppipn);
        public abstract IDataReader EventsSignupsGetEvent(int eventId, int moduleId);
        public abstract IDataReader EventsSignupsGetEventRecurMaster(int recurMasterId, int moduleId);
        public abstract IDataReader EventsSignupsGetUser(int eventId, int userId, int moduleId);
        public abstract IDataReader EventsSignupsGetAnonUser(int eventId, string anonEmail, int moduleId);

        public abstract IDataReader EventsSignupsMyEnrollments(int moduleId, int userId, int socialGroupId,
                                                               string categoryIDs, DateTime beginDate,
                                                               DateTime endDate);

        public abstract IDataReader EventsSignupsSave(int eventId, int signupId, int moduleId,
                                                      int userId, bool approved, string
                                                          payPalStatus, string payPalReason, string payPalTransId,
                                                      string payPalPayerId, string payPalPayerStatus,
                                                      string payPalRecieverEmail, string payPalUserEmail,
                                                      string payPalPayerEmail,
                                                      string payPalFirstName, string payPalLastName,
                                                      string payPalAddress, string payPalCity,
                                                      string payPalState, string payPalZip, string payPalCountry,
                                                      string payPalCurrency,
                                                      DateTime payPalPaymentDate, decimal payPalAmount,
                                                      decimal payPalFee,
                                                      int noEnrolees, string anonEmail, string anonName,
                                                      string anonTelephone, string anonCulture,
                                                      string anonTimeZoneId, string firstName, string lastName,
                                                      string company,
                                                      string jobTitle, string referenceNumber, string street,
                                                      string postalCode, string city, string region, string country);

        #endregion

        #region Event Notification Functions

        public abstract void EventsNotificationTimeChange(int eventId, DateTime eventTimeBegin, int moduleId);
        public abstract void EventsNotificationDelete(DateTime deleteDate);
        public abstract IDataReader EventsNotificationGet(int eventId, string userEmail, int moduleId);
        public abstract IDataReader EventsNotificationsToSend(DateTime notifyTime);

        public abstract IDataReader EventsNotificationSave(int notificationId, int eventId, int portalAliasId,
                                                           string userEmail, bool notificationSent,
                                                           DateTime notifyByDateTime, DateTime eventTimeBegin,
                                                           string notifyLanguage, int
                                                               moduleId, int tabId);

        #endregion

        #region Event Recur Master Functions

        public abstract void EventsRecurMasterDelete(int recurMasterId, int moduleId);
        public abstract IDataReader EventsRecurMasterGet(int recurMasterId, int moduleId);

        public abstract IDataReader EventsRecurMasterSave(int recurMasterId, int moduleId, int portalId, string rrule,
                                                          DateTime dtstart, string duration, DateTime Until,
                                                          string eventName, string eventDesc, int importance,
                                                          string notify, bool approved, bool signups, int maxEnrollment,
                                                          int enrollRoleId, decimal enrollFee, string enrollType,
                                                          string payPalAccount, bool detailPage, bool detailNewWin,
                                                          string detailUrl, string imageUrl, string imageType,
                                                          int imageWidth, int imageHeight, bool imageDisplay,
                                                          int location, int category, string reminder,
                                                          bool sendReminder, int reminderTime,
                                                          string reminderTimeMeasurement, string reminderFrom,
                                                          string customField1, string customField2, bool enrollListView,
                                                          bool displayEndDate, bool allDayEvent, string cultureName,
                                                          int ownerId, int createdById, int updatedById,
                                                          string eventTimeZoneId, bool allowAnonEnroll,
                                                          int contentItemId, int socialGroupId, int socialUserId,
                                                          string summary);

        public abstract IDataReader EventsRecurMasterModerate(int moduleId, int socialGroupId);

        #endregion

        #region Event Subscription Functions

        public abstract void EventsSubscriptionDeleteUser(int userId, int moduleId);
        public abstract IDataReader EventsSubscriptionGetUser(int userId, int moduleId);
        public abstract IDataReader EventsSubscriptionGetModule(int moduleId);
        public abstract IDataReader EventsSubscriptionGetSubModule(int moduleId);
        public abstract IDataReader EventsSubscriptionSave(int subscriptionId, int moduleId, int portalId, int userId);

        #endregion
    }
}