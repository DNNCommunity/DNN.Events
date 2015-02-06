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

Namespace DotNetNuke.Modules.Events

    Public MustInherit Class DataProvider

#Region "Shared/Static Methods"
        ' singleton reference to the instantiated object 
        Private Shared _objProvider As DataProvider = Nothing

        ' constructor
        Shared Sub New()
            CreateProvider()
        End Sub

        ' dynamically create provider
        Private Shared Sub CreateProvider()
            _objProvider = CType(Framework.Reflection.CreateObject("data", "DotNetNuke.Modules.Events", ""), DataProvider)
        End Sub

        ' return the provider
        Public Shared Shadows Function Instance() As DataProvider
            Return _objProvider
        End Function

#End Region

        Public MustOverride ReadOnly Property ObjectQualifier() As String
        Public MustOverride ReadOnly Property DatabaseOwner() As String

#Region "Event Functions"
        Public MustOverride Sub EventsDelete(ByVal eventId As Integer, ByVal moduleId As Integer)
        Public MustOverride Function EventsGet(ByVal eventId As Integer, ByVal moduleId As Integer) As IDataReader
        Public MustOverride Function EventsGetByRange(ByVal modules As String, ByVal beginDate As DateTime, ByVal endDate As DateTime, ByVal categoryIDs As String, ByVal locationIDs As String, ByVal socialGroupId As Integer, ByVal socialUserId As Integer) As IDataReader
        Public MustOverride Function EventsSave(ByVal portalId As Integer, ByVal eventId As Integer, ByVal recurMasterId As Integer, ByVal moduleId As Integer, _
                    ByVal eventTimeBegin As DateTime, ByVal duration As Integer, ByVal eventName As String, _
                    ByVal eventDesc As String, ByVal importance As Integer, ByVal createdById As String, _
                    ByVal notify As String, ByVal approved As Boolean, _
                    ByVal signups As Boolean, ByVal maxEnrollment As Integer, ByVal enrollRoleId As Integer, _
                    ByVal enrollFee As Decimal, ByVal enrollType As String, ByVal payPalAccount As String, _
                    ByVal cancelled As Boolean, ByVal detailPage As Boolean, ByVal detailNewWin As Boolean, ByVal detailUrl As String, ByVal imageUrl As String, ByVal imageType As String, ByVal imageWidth As Integer, ByVal imageHeight As Integer, _
                    ByVal imageDisplay As Boolean, ByVal location As Integer, _
                    ByVal category As Integer, _
                    ByVal reminder As String, ByVal sendReminder As Boolean, ByVal reminderTime As Integer, _
                    ByVal reminderTimeMeasurement As String, ByVal reminderFrom As String, ByVal searchSubmitted As Boolean, _
                    ByVal customField1 As String, ByVal customField2 As String, ByVal enrollListView As Boolean, ByVal displayEndDate As Boolean, _
                    ByVal allDayEvent As Boolean, ByVal ownerId As Integer, ByVal lastUpdatedId As Integer, ByVal originalDateBegin As DateTime, _
                    ByVal newEventEmailSent As Boolean, ByVal allowAnonEnroll As Boolean, ByVal contentItemId As Integer, ByVal journalItem As Boolean, _
                    ByVal summary As String, ByVal saveOnly As Boolean) As IDataReader
        Public MustOverride Function EventsModerateEvents(ByVal moduleId As Integer, ByVal socialGroupId As Integer) As IDataReader
        Public MustOverride Function EventsTimeZoneCount(ByVal moduleId As Integer) As Integer
        Public MustOverride Sub EventsUpgrade(ByVal moduleVersion As String)
        Public MustOverride Sub EventsCleanupExpired(ByVal portalId As Integer, ByVal moduleId As Integer)
        Public MustOverride Function EventsGetRecurrences(ByVal recurMasterId As Integer, ByVal moduleId As Integer) As IDataReader
#End Region

#Region "Master Event Functions"
        Public MustOverride Sub EventsMasterDelete(ByVal masterId As Integer, ByVal moduleId As Integer)
        Public MustOverride Function EventsMasterGet(ByVal moduleId As Integer, ByVal subEventId As Integer) As IDataReader
        Public MustOverride Function EventsMasterAssignedModules(ByVal moduleId As Integer) As IDataReader
        Public MustOverride Function EventsMasterAvailableModules(ByVal portalId As Integer, ByVal moduleId As Integer) As IDataReader
        Public MustOverride Function EventsMasterSave(ByVal masterId As Integer, ByVal moduleId As Integer, ByVal subEventId As Integer) As IDataReader
#End Region

#Region "Event Categories and Locations Functions"
        Public MustOverride Sub EventsCategoryDelete(ByVal category As Integer, ByVal portalId As Integer)
        Public MustOverride Function EventsCategoryGetByName(ByVal categoryName As String, ByVal portalId As Integer) As IDataReader
        Public MustOverride Function EventsCategoryGet(ByVal category As Integer, ByVal portalId As Integer) As IDataReader
        Public MustOverride Function EventsCategoryList(ByVal portalId As Integer) As IDataReader
        Public MustOverride Function EventsCategorySave(ByVal portalId As Integer, ByVal category As Integer, _
                    ByVal categoryName As String, ByVal color As String, ByVal fontColor As String) As IDataReader

        Public MustOverride Sub EventsLocationDelete(ByVal location As Integer, ByVal portalId As Integer)
        Public MustOverride Function EventsLocationGetByName(ByVal locationName As String, ByVal portalId As Integer) As IDataReader
        Public MustOverride Function EventsLocationGet(ByVal location As Integer, ByVal portalId As Integer) As IDataReader
        Public MustOverride Function EventsLocationList(ByVal portalId As Integer) As IDataReader
        Public MustOverride Function EventsLocationSave(ByVal portalId As Integer, ByVal location As Integer, _
                    ByVal locationName As String, ByVal mapUrl As String, _
                    ByVal street As String, ByVal postalCode As String, ByVal city As String, _
                    ByVal region As String, ByVal country As String) As IDataReader
#End Region

#Region "Event Enrollment and Moderation Functions"
        Public MustOverride Sub EventsSignupsDelete(ByVal signupId As Integer, ByVal moduleId As Integer)
        Public MustOverride Function EventsModerateSignups(ByVal moduleId As Integer, ByVal socialGroupId As Integer) As IDataReader
        Public MustOverride Function EventsSignupsGet(ByVal signupId As Integer, ByVal moduleId As Integer, ByVal ppipn As Boolean) As IDataReader
        Public MustOverride Function EventsSignupsGetEvent(ByVal eventId As Integer, ByVal moduleId As Integer) As IDataReader
        Public MustOverride Function EventsSignupsGetEventRecurMaster(ByVal recurMasterId As Integer, ByVal moduleId As Integer) As IDataReader
        Public MustOverride Function EventsSignupsGetUser(ByVal eventId As Integer, ByVal userId As Integer, ByVal moduleId As Integer) As IDataReader
        Public MustOverride Function EventsSignupsGetAnonUser(ByVal eventId As Integer, ByVal anonEmail As String, ByVal moduleId As Integer) As IDataReader
        Public MustOverride Function EventsSignupsMyEnrollments(ByVal moduleId As Integer, ByVal userId As Integer, ByVal socialGroupId As Integer, ByVal categoryIDs As String, ByVal beginDate As DateTime, ByVal endDate As DateTime) As IDataReader
        Public MustOverride Function EventsSignupsSave(ByVal eventId As Integer, ByVal signupId As Integer, _
            ByVal moduleId As Integer, ByVal userId As Integer, ByVal approved As Boolean, _
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
#End Region

#Region "Event PayPal Functions"
        Public MustOverride Function EventsPpErrorLogAdd(ByVal signupId As Integer, _
            ByVal payPalStatus As String, ByVal payPalReason As String, ByVal payPalTransId As String, ByVal payPalPayerId As String, _
            ByVal payPalPayerStatus As String, ByVal payPalRecieverEmail As String, ByVal payPalUserEmail As String, _
            ByVal payPalPayerEmail As String, ByVal payPalFirstName As String, ByVal payPalLastName As String, ByVal payPalAddress As String, _
            ByVal payPalCity As String, ByVal payPalState As String, ByVal payPalZip As String, ByVal payPalCountry As String, _
            ByVal payPalCurrency As String, ByVal payPalPaymentDate As DateTime, ByVal payPalAmount As Double, ByVal payPalFee As Double) As IDataReader
#End Region

#Region "Event Notification Functions"
        Public MustOverride Sub EventsNotificationTimeChange(ByVal eventId As Integer, ByVal eventTimeBegin As DateTime, ByVal moduleId As Integer)
        Public MustOverride Sub EventsNotificationDelete(ByVal deleteDate As DateTime)
        Public MustOverride Function EventsNotificationGet(ByVal eventId As Integer, ByVal userEmail As String, ByVal moduleId As Integer) As IDataReader
        Public MustOverride Function EventsNotificationsToSend(ByVal notifyTime As DateTime) As IDataReader
        Public MustOverride Function EventsNotificationSave(ByVal notificationId As Integer, ByVal eventId As Integer, ByVal portalAliasId As Integer, ByVal userEmail As String, _
            ByVal notificationSent As Boolean, ByVal notifyByDateTime As DateTime, ByVal eventTimeBegin As DateTime, ByVal notifyLanguage As String, ByVal moduleId As Integer, ByVal tabId As Integer) As IDataReader
#End Region

#Region "Event Recur Master Functions"
        Public MustOverride Sub EventsRecurMasterDelete(ByVal recurMasterId As Integer, ByVal moduleId As Integer)
        Public MustOverride Function EventsRecurMasterGet(ByVal recurMasterId As Integer, ByVal moduleId As Integer) As IDataReader
        Public MustOverride Function EventsRecurMasterSave(ByVal recurMasterId As Integer, ByVal moduleId As Integer, ByVal portalId As Integer, ByVal rrule As String, _
            ByVal dtstart As DateTime, ByVal duration As String, ByVal until As DateTime, _
            ByVal eventName As String, ByVal eventDesc As String, ByVal importance As Integer, ByVal notify As String, ByVal approved As Boolean, _
            ByVal signups As Boolean, ByVal maxEnrollment As Integer, ByVal enrollRoleId As Integer, _
            ByVal enrollFee As Decimal, ByVal enrollType As String, ByVal payPalAccount As String, _
            ByVal detailPage As Boolean, ByVal detailNewWin As Boolean, ByVal detailUrl As String, ByVal imageUrl As String, _
            ByVal imageType As String, ByVal imageWidth As Integer, ByVal imageHeight As Integer, ByVal imageDisplay As Boolean, ByVal location As Integer, _
            ByVal category As Integer, ByVal reminder As String, ByVal sendReminder As Boolean, ByVal reminderTime As Integer, _
            ByVal reminderTimeMeasurement As String, ByVal reminderFrom As String, ByVal customField1 As String, ByVal customField2 As String, _
            ByVal enrollListView As Boolean, ByVal displayEndDate As Boolean, ByVal allDayEvent As Boolean, ByVal cultureName As String, ByVal ownerId As Integer, ByVal createdById As Integer, _
            ByVal updatedById As Integer, ByVal eventTimeZoneId As String, ByVal allowAnonEnroll As Boolean, ByVal contentItemId As Integer, ByVal socialGroupId As Integer, ByVal socialUserId As Integer, ByVal summary As String) As IDataReader
        Public MustOverride Function EventsRecurMasterModerate(ByVal moduleId As Integer, ByVal socialGroupId As Integer) As IDataReader
#End Region

#Region "Event Subscription Functions"
        Public MustOverride Sub EventsSubscriptionDeleteUser(ByVal userId As Integer, ByVal moduleId As Integer)
        Public MustOverride Function EventsSubscriptionGetUser(ByVal userId As Integer, ByVal moduleId As Integer) As IDataReader
        Public MustOverride Function EventsSubscriptionGetModule(ByVal moduleId As Integer) As IDataReader
        Public MustOverride Function EventsSubscriptionGetSubModule(ByVal moduleId As Integer) As IDataReader
        Public MustOverride Function EventsSubscriptionSave(ByVal subscriptionId As Integer, ByVal moduleId As Integer, ByVal portalId As Integer, ByVal userId As Integer) As IDataReader
#End Region

    End Class

End Namespace
