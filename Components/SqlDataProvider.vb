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
Imports System.Data
Imports Microsoft.ApplicationBlocks.Data

Namespace DotNetNuke.Modules.Events

    Public Class SqlDataProvider

        Inherits DataProvider

#Region "Private Data"
        Private Const ProviderType As String = "data"
        Private ReadOnly _providerConfiguration As Framework.Providers.ProviderConfiguration = Framework.Providers.ProviderConfiguration.GetProviderConfiguration(ProviderType)
        Private ReadOnly _connectionString As String
        Private ReadOnly _providerPath As String
        Private ReadOnly _objectQualifier As String
        Private ReadOnly _databaseOwner As String
#End Region

#Region "Base Methods"
        Public Sub New()

            ' Read the configuration specific information for this provider
            Dim objProvider As Framework.Providers.Provider = CType(_providerConfiguration.Providers(_providerConfiguration.DefaultProvider), Framework.Providers.Provider)

            ' This code handles getting the connection string from either the connectionString / appsetting section and uses the connectionstring section by default if it exists.  
            ' Get Connection string from web.config
            _connectionString = Common.Utilities.Config.GetConnectionString()

            ' If above funtion does not return anything then connectionString must be set in the dataprovider section.
            If _connectionString = "" Then
                ' Use connection string specified in provider
                _connectionString = objProvider.Attributes("connectionString")
            End If

            _providerPath = objProvider.Attributes("providerPath")

            _objectQualifier = objProvider.Attributes("objectQualifier")
            If _objectQualifier <> "" And _objectQualifier.EndsWith("_") = False Then
                _objectQualifier += "_"
            End If

            _databaseOwner = objProvider.Attributes("databaseOwner")
            If _databaseOwner <> "" And _databaseOwner.EndsWith(".") = False Then
                _databaseOwner += "."
            End If

        End Sub

        Public ReadOnly Property ConnectionString() As String
            Get
                Return _connectionString
            End Get
        End Property

        Public ReadOnly Property ProviderPath() As String
            Get
                Return _providerPath
            End Get
        End Property

        Public Overrides ReadOnly Property ObjectQualifier() As String
            Get
                Return _objectQualifier
            End Get
        End Property

        Public Overrides ReadOnly Property DatabaseOwner() As String
            Get
                Return _databaseOwner
            End Get
        End Property
#End Region

#Region "Event Methods"

        Public Overrides Sub EventsDelete(ByVal eventId As Integer, ByVal moduleId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsDelete", eventId, moduleId)
        End Sub

        Public Overrides Function EventsGet(ByVal eventId As Integer, ByVal moduleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsGet", eventId, moduleId), IDataReader)
        End Function

        Public Overrides Function EventsGetByRange(ByVal modules As String, ByVal beginDate As DateTime, ByVal endDate As DateTime, ByVal categoryIDs As String, ByVal locationIDs As String, ByVal socialGroupId As Integer, ByVal socialUserId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsGetByRange", modules, beginDate, endDate, categoryIDs, locationIDs, socialGroupId, socialUserId), IDataReader)
        End Function

        Public Overrides Function EventsSave(ByVal portalId As Integer, ByVal eventId As Integer, ByVal recurMasterId As Integer, ByVal moduleId As Integer, _
            ByVal eventTimeBegin As DateTime, ByVal duration As Integer, ByVal eventName As String, _
            ByVal eventDesc As String, ByVal importance As Integer, ByVal createdById As String, _
            ByVal notify As String, ByVal approved As Boolean, _
            ByVal signups As Boolean, ByVal maxEnrollment As Integer, ByVal enrollRoleId As Integer, _
            ByVal enrollFee As Decimal, ByVal enrollType As String, ByVal payPalAccount As String, _
            ByVal cancelled As Boolean, ByVal detailPage As Boolean, ByVal detailNewWin As Boolean, ByVal detailUrl As String, _
            ByVal imageUrl As String, ByVal imageType As String, ByVal imageWidth As Integer, ByVal imageHeight As Integer, _
            ByVal imageDisplay As Boolean, ByVal location As Integer, _
            ByVal category As Integer, _
            ByVal reminder As String, ByVal sendReminder As Boolean, ByVal reminderTime As Integer, _
            ByVal reminderTimeMeasurement As String, ByVal reminderFrom As String, ByVal searchSubmitted As Boolean, _
            ByVal customField1 As String, ByVal customField2 As String, ByVal enrollListView As Boolean, ByVal displayEndDate As Boolean, _
            ByVal allDayEvent As Boolean, ByVal ownerId As Integer, ByVal lastUpdatedId As Integer, ByVal originalDateBegin As DateTime, _
            ByVal newEventEmailSent As Boolean, ByVal allowAnonEnroll As Boolean, ByVal contentItemId As Integer, ByVal journalItem As Boolean, _
            ByVal summary As String, ByVal saveOnly As Boolean) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsSave", portalId, eventId, recurMasterId, moduleId, _
                 eventTimeBegin, duration, eventName, eventDesc, importance, _
                 createdById, notify, approved, signups, maxEnrollment, enrollRoleId, _
                 enrollFee, enrollType, payPalAccount, cancelled, detailPage, detailNewWin, detailUrl, _
                 imageUrl, imageType, imageWidth, imageHeight, _
                 imageDisplay, location, category, _
                 reminder, sendReminder, reminderTime, reminderTimeMeasurement, reminderFrom, searchSubmitted, _
                 customField1, customField2, enrollListView, displayEndDate, allDayEvent, _
                 ownerId, lastUpdatedId, originalDateBegin, newEventEmailSent, allowAnonEnroll, contentItemId, journalItem, summary, saveOnly), IDataReader)
        End Function

        Public Overrides Function EventsModerateEvents(ByVal moduleId As Integer, ByVal socialGroupId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsModerateEvents", moduleId, socialGroupId), IDataReader)
        End Function

        Public Overrides Function EventsTimeZoneCount(ByVal moduleId As Integer) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsTimeZoneCount", moduleId), Integer)
        End Function

        Public Overrides Sub EventsUpgrade(ByVal moduleVersion As String)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsUpgrade", moduleVersion)
        End Sub

        Public Overrides Sub EventsCleanupExpired(ByVal portalId As Integer, ByVal moduleId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsCleanupExpired", portalId, moduleId)
        End Sub

        Public Overrides Function EventsGetRecurrences(ByVal recurMasterId As Integer, ByVal moduleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsGetRecurrences", recurMasterId, moduleId), IDataReader)
        End Function


#End Region

#Region "Master Events Methods"

        Public Overrides Sub EventsMasterDelete(ByVal masterId As Integer, ByVal moduleId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsMasterDelete", MasterID, ModuleID)
        End Sub

        Public Overrides Function EventsMasterAssignedModules(ByVal moduleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsMasterAssignedModules", ModuleID), IDataReader)
        End Function

        Public Overrides Function EventsMasterAvailableModules(ByVal portalId As Integer, ByVal moduleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsMasterAvailableModules", PortalID, ModuleID), IDataReader)
        End Function

        Public Overrides Function EventsMasterGet(ByVal moduleId As Integer, ByVal subEventId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsMasterGet", moduleId, subEventId), IDataReader)
        End Function

        Public Overrides Function EventsMasterSave(ByVal masterId As Integer, ByVal moduleId As Integer, ByVal subEventId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsMasterSave", masterId, moduleId, subEventId), IDataReader)
        End Function

#End Region

#Region "Events Signup Methods"
        Public Overrides Sub EventsSignupsDelete(ByVal signupId As Integer, ByVal moduleId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsSignupsDelete", SignupID, ModuleID)
        End Sub

        Public Overrides Function EventsSignupsGet(ByVal signupId As Integer, ByVal moduleId As Integer, ByVal ppipn As Boolean) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsSignupsGet", signupId, moduleId, PPIPN), IDataReader)
        End Function

        Public Overrides Function EventsSignupsGetEvent(ByVal eventId As Integer, ByVal moduleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsSignupsGetEvent", EventID, ModuleID), IDataReader)
        End Function

        Public Overrides Function EventsSignupsGetEventRecurMaster(ByVal recurMasterId As Integer, ByVal moduleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsSignupsGetEventRecurMaster", RecurMasterID, ModuleID), IDataReader)
        End Function

        Public Overrides Function EventsSignupsGetUser(ByVal eventId As Integer, ByVal userId As Integer, ByVal moduleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsSignupsGetUser", EventID, UserID, ModuleID), IDataReader)
        End Function

        Public Overrides Function EventsSignupsGetAnonUser(ByVal eventId As Integer, ByVal anonEmail As String, ByVal moduleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsSignupsGetAnonUser", eventId, AnonEmail, ModuleID), IDataReader)
        End Function

        Public Overrides Function EventsSignupsMyEnrollments(ByVal moduleId As Integer, ByVal userId As Integer, ByVal socialGroupId As Integer, ByVal categoryIDs As String, ByVal beginDate As DateTime, ByVal endDate As DateTime) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsSignupsMyEnrollments", moduleId, userId, socialGroupId, categoryIDs, beginDate, EndDate), IDataReader)
        End Function

        Public Overrides Function EventsSignupsSave(ByVal eventId As Integer, ByVal signupId As Integer, ByVal moduleId As Integer, ByVal userId As Integer, ByVal approved As Boolean, _
            ByVal payPalStatus As String, ByVal payPalReason As String, ByVal payPalTransId As String, ByVal payPalPayerId As String, _
            ByVal payPalPayerStatus As String, ByVal payPalRecieverEmail As String, ByVal payPalUserEmail As String, _
            ByVal payPalPayerEmail As String, ByVal payPalFirstName As String, ByVal payPalLastName As String, ByVal payPalAddress As String, _
            ByVal payPalCity As String, ByVal payPalState As String, ByVal payPalZip As String, ByVal payPalCountry As String, _
            ByVal payPalCurrency As String, ByVal payPalPaymentDate As DateTime, ByVal payPalAmount As Decimal, ByVal payPalFee As Decimal, _
            ByVal noEnrolees As Integer, ByVal anonEmail As String, ByVal anonName As String, ByVal anonTelephone As String, _
            ByVal anonCulture As String, ByVal anonTimeZoneId As String, ByVal firstName As String, ByVal lastName As String, _
            ByVal company As String, ByVal jobTitle As String, ByVal referenceNumber As String, _
            ByVal street As String, ByVal postalCode As String, ByVal city As String, ByVal region As String, ByVal country As String _
            ) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsSignupsSave", _
            eventId, signupId, moduleId, userId, approved, _
            payPalStatus, payPalReason, payPalTransId, payPalPayerId, _
            payPalPayerStatus, payPalRecieverEmail, payPalUserEmail, _
            payPalPayerEmail, payPalFirstName, payPalLastName, payPalAddress, _
            payPalCity, payPalState, payPalZip, payPalCountry, _
            payPalCurrency, payPalPaymentDate, payPalAmount, payPalFee, noEnrolees, anonEmail, anonName, anonTelephone, _
            anonCulture, anonTimeZoneId, firstName, lastName, company, jobTitle, referenceNumber, _
            street, postalCode, city, region, country), IDataReader)
        End Function

        Public Overrides Function EventsModerateSignups(ByVal moduleId As Integer, ByVal socialGroupId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsModerateSignups", ModuleID, SocialGroupId), IDataReader)
        End Function


#End Region

#Region "Events Category and Location Methods"
        Public Overrides Sub EventsCategoryDelete(ByVal category As Integer, ByVal portalId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsCategoryDelete", Category, PortalID)
        End Sub

        Public Overrides Function EventsCategoryGet(ByVal category As Integer, ByVal portalId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsCategoryGet", Category, PortalID), IDataReader)
        End Function

        Public Overrides Function EventsCategoryGetByName(ByVal categoryName As String, ByVal portalId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsCategoryGetByName", CategoryName, PortalID), IDataReader)
        End Function

        Public Overrides Function EventsCategoryList(ByVal portalId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsCategoryList", PortalId), IDataReader)
        End Function

        Public Overrides Function EventsCategorySave(ByVal portalId As Integer, ByVal category As Integer, _
                    ByVal categoryName As String, ByVal color As String, ByVal fontColor As String) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsCategorySave", PortalID, Category, _
                CategoryName, Color, FontColor), IDataReader)
        End Function

        Public Overrides Sub EventsLocationDelete(ByVal location As Integer, ByVal portalId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsLocationDelete", Location, PortalID)
        End Sub

        Public Overrides Function EventsLocationGet(ByVal location As Integer, ByVal portalId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsLocationGet", Location, PortalID), IDataReader)
        End Function

        Public Overrides Function EventsLocationGetByName(ByVal locationName As String, ByVal portalId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsLocationGetByName", locationName, portalId), IDataReader)
        End Function

        Public Overrides Function EventsLocationList(ByVal portalId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsLocationList", PortalId), IDataReader)
        End Function

        Public Overrides Function EventsLocationSave(ByVal portalId As Integer, ByVal location As Integer, _
                    ByVal locationName As String, ByVal mapUrl As String, _
                    ByVal street As String, ByVal postalCode As String, ByVal city As String, _
                    ByVal region As String, ByVal country As String) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsLocationSave", portalId, location, _
                locationName, mapUrl, street, postalCode, city, region, country), IDataReader)
        End Function
#End Region

#Region "PPErrorLog Methods"

        Public Overrides Function EventsPpErrorLogAdd(ByVal signupId As Integer, _
            ByVal payPalStatus As String, ByVal payPalReason As String, ByVal payPalTransId As String, ByVal payPalPayerId As String, _
            ByVal payPalPayerStatus As String, ByVal payPalRecieverEmail As String, ByVal payPalUserEmail As String, _
            ByVal payPalPayerEmail As String, ByVal payPalFirstName As String, ByVal payPalLastName As String, ByVal payPalAddress As String, _
            ByVal payPalCity As String, ByVal payPalState As String, ByVal payPalZip As String, ByVal payPalCountry As String, _
            ByVal payPalCurrency As String, ByVal payPalPaymentDate As DateTime, ByVal payPalAmount As Double, ByVal payPalFee As Double) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsPPErrorLogAdd", _
            signupId, _
            payPalStatus, payPalReason, payPalTransId, payPalPayerId, _
            payPalPayerStatus, payPalRecieverEmail, payPalUserEmail, _
            payPalPayerEmail, payPalFirstName, payPalLastName, payPalAddress, _
            payPalCity, payPalState, payPalZip, payPalCountry, _
            payPalCurrency, payPalPaymentDate, payPalAmount, PayPalFee _
            ), IDataReader)
        End Function

#End Region

#Region "Events Notification Methods"
        Public Overrides Sub EventsNotificationTimeChange(ByVal eventId As Integer, ByVal eventTimeBegin As DateTime, ByVal moduleId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsNotificationTimeChange", EventID, EventTimeBegin, ModuleID)
        End Sub

        Public Overrides Sub EventsNotificationDelete(ByVal deleteDate As DateTime)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsNotificationDelete", DeleteDate)
        End Sub

        Public Overrides Function EventsNotificationGet(ByVal eventId As Integer, ByVal userEmail As String, ByVal moduleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsNotificationGet", eventId, UserEmail, ModuleID), IDataReader)
        End Function

        Public Overrides Function EventsNotificationsToSend(ByVal notifyTime As DateTime) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsNotificationsToSend", NotifyTime), IDataReader)
        End Function

        Public Overrides Function EventsNotificationSave(ByVal notificationId As Integer, ByVal eventId As Integer, ByVal portalAliasId As Integer, ByVal userEmail As String, ByVal notificationSent As Boolean, ByVal notifyByDateTime As DateTime, ByVal eventTimeBegin As DateTime, ByVal notifyLanguage As String, ByVal moduleId As Integer, ByVal tabId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsNotificationSave", notificationId, eventId, portalAliasId, userEmail, notificationSent, notifyByDateTime, eventTimeBegin, notifyLanguage, ModuleID, TabID), IDataReader)
        End Function

#End Region

#Region "Event Recur Master Methods"
        Public Overrides Sub EventsRecurMasterDelete(ByVal recurMasterId As Integer, ByVal moduleId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsRecurMasterDelete", RecurMasterID, ModuleID)
        End Sub

        Public Overrides Function EventsRecurMasterGet(ByVal recurMasterId As Integer, ByVal moduleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsRecurMasterGet", RecurMasterID, ModuleID), IDataReader)
        End Function

        Public Overrides Function EventsRecurMasterSave(ByVal recurMasterId As Integer, ByVal moduleId As Integer, ByVal portalId As Integer, ByVal rrule As String, _
            ByVal dtstart As DateTime, ByVal duration As String, ByVal until As DateTime, _
            ByVal eventName As String, ByVal eventDesc As String, ByVal importance As Integer, ByVal notify As String, ByVal approved As Boolean, _
            ByVal signups As Boolean, ByVal maxEnrollment As Integer, ByVal enrollRoleId As Integer, _
            ByVal enrollFee As Decimal, ByVal enrollType As String, ByVal payPalAccount As String, _
            ByVal detailPage As Boolean, ByVal detailNewWin As Boolean, ByVal detailUrl As String, ByVal imageUrl As String, _
            ByVal imageType As String, ByVal imageWidth As Integer, ByVal imageHeight As Integer, ByVal imageDisplay As Boolean, ByVal location As Integer, _
            ByVal category As Integer, ByVal reminder As String, ByVal sendReminder As Boolean, ByVal reminderTime As Integer, _
            ByVal reminderTimeMeasurement As String, ByVal reminderFrom As String, ByVal customField1 As String, ByVal customField2 As String, _
            ByVal enrollListView As Boolean, ByVal displayEndDate As Boolean, ByVal allDayEvent As Boolean, ByVal cultureName As String, _
            ByVal ownerId As Integer, ByVal createdById As Integer, ByVal updatedById As Integer, ByVal eventTimeZoneId As String, ByVal allowAnonEnroll As Boolean, _
            ByVal contentItemId As Integer, ByVal socialGroupId As Integer, ByVal socialUserId As Integer, ByVal summary As String) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsRecurMasterSave", recurMasterId, moduleId, portalId, rrule, _
                 dtstart, duration, until, eventName, eventDesc, importance, notify, approved, _
                 signups, maxEnrollment, enrollRoleId, _
                 enrollFee, enrollType, payPalAccount, detailPage, detailNewWin, detailUrl, imageUrl, imageType, imageWidth, imageHeight, _
                 imageDisplay, location, category, reminder, sendReminder, reminderTime, _
                 reminderTimeMeasurement, reminderFrom, customField1, customField2, enrollListView, displayEndDate, allDayEvent, cultureName, ownerId, _
                 createdById, updatedById, eventTimeZoneId, allowAnonEnroll, ContentItemId, SocialGroupId, SocialUserId, Summary), IDataReader)
        End Function

        Public Overrides Function EventsRecurMasterModerate(ByVal moduleId As Integer, ByVal socialGroupId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsRecurMasterModerate", ModuleID, SocialGroupId), IDataReader)
        End Function


#End Region

#Region "Event Subscription Methods"
        Public Overrides Sub EventsSubscriptionDeleteUser(ByVal userId As Integer, ByVal moduleId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsSubscriptionDeleteUser", UserID, ModuleID)
        End Sub

        Public Overrides Function EventsSubscriptionGetUser(ByVal userId As Integer, ByVal moduleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsSubscriptionGetUser", UserID, ModuleID), IDataReader)
        End Function

        Public Overrides Function EventsSubscriptionGetModule(ByVal moduleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsSubscriptionGetModule", ModuleID), IDataReader)
        End Function

        Public Overrides Function EventsSubscriptionGetSubModule(ByVal moduleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsSubscriptionGetSubModule", ModuleID), IDataReader)
        End Function

        Public Overrides Function EventsSubscriptionSave(ByVal subscriptionId As Integer, ByVal moduleId As Integer, ByVal portalId As Integer, ByVal userId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "EventsSubscriptionSave", subscriptionId, moduleId, PortalID, UserID), IDataReader)
        End Function

#End Region
    End Class

End Namespace