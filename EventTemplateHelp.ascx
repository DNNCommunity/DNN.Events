<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EventTemplateHelp.ascx.cs" Inherits="DotNetNuke.Modules.Events.EventTemplateHelp" %>
<asp:Panel ID="pnlEventsModuleTHelp" runat="server">
<h1>Template help</h1>
<table>
<thead>
<tr>
    <td style="width: 179px">Tokenname</td>
    <td>Description</td>
</tr>
</thead>
<tbody>
<tr>
    <td style="width: 179px">[event:title]</td>
    <td>Event title and the sub calendar modulename when defined in settings</td>
</tr>
<tr>
    <td style="width: 179px">[event:subcalendarname]</td>
    <td>The sub calendar modulename when defined in settings</td>
</tr>
<tr>
    <td style="width: 179px">[event:subcalendarnameclean]</td>
    <td>The sub calendar modulename when defined in settings without brackets</td>
</tr>
<tr>
    <td style="width: 179px">[event:eventimage]<sup>§</sup></td>
    <td>Image when one is associated with the event</td>
</tr>
<tr>
    <td style="width: 179px">[event:eventthumb]<sup>§</sup></td>
    <td>Image thumbnail when an image is associated with the event</td>
</tr>
<tr>
    <td style="width: 179px">[event:imageurl]<sup>§</sup></td>
    <td>URL of the event image when one is associated with the event</td>
</tr>
<tr>
    <td style="width: 179px">[event:startdatelabel]</td>
    <td>Label defined for start date in the resource file</td>
</tr>
<tr>
    <td style="width: 179px">[event:startdate|<i>format</i>]</td>
    <td>Start date formatted (optional) by the format string *</td>
</tr>
<tr>
    <td style="width: 179px">[event:enddatelabel]</td>
    <td>Label defined for end date in the resource file</td>
</tr>
<tr>
    <td style="width: 179px">[event:enddate|<i>format</i>]</td>
    <td>End date formatted (optional) by the format string *</td>
</tr>
<tr>
    <td style="width: 179px">[event:durationdayslabel]</td>
    <td>Label defined for duration days in the resource file</td>
</tr>
<tr>
    <td style="width: 179px">[event:durationdays]</td>
    <td>Duration of the event in whole integer days</td>
</tr>
<tr>
    <td style="width: 179px">[event:timezone]</td>
    <td>Timezone description</td>
</tr>
<tr>
    <td style="width: 179px">[event:importancelabel]<sup>§</sup></td>
    <td>Label defined for importance in the resource file</td>
</tr>
<tr>
    <td style="width: 179px">[event:importance]<sup>§</sup></td>
    <td>Importance with icon (for low and high importance)</td>
</tr>
<tr>
    <td style="width: 179px">
        [event:importanceicon]<sup>§</sup>
    </td>
    <td>
        Importance Icon
    </td>
</tr>
<tr>
    <td style="width: 179px">[event:descriptionlabel]</td>
    <td>Label defined for description in the resource file</td>
</tr>
<tr>
    <td style="width: 179px">[event:description|<i>length</i>]</td>
    <td>Description defined in the event, optionally shortened to specified length</td>
</tr>
<tr>
    <td style="width: 179px">
        [event:summarylabel]
    </td>
    <td>
        Label defined for summary in the resource file
    </td>
</tr>
<tr>
    <td style="width: 179px">
        [event:summary|<i>length</i>]
    </td>
    <td>
        Summary defined in the event, optionally shortened to specified length
    </td>
</tr>
<tr>
    <td style="width: 179px">[event:categorylabel]</td>
    <td>Label defined for category in the resource file</td>
</tr>
<tr>
    <td style="width: 179px">[event:category]</td>
    <td>Category associated with the event</td>
</tr>
<tr>
    <td style="width: 179px">[event:categoryname]</td>
    <td>Name of the Category associated with the event with no colors applied</td>
</tr>
<tr>
    <td style="width: 179px">[event:categoryfontcolor]</td>
    <td>Font color for the event category</td>
</tr>
<tr>
    <td style="width: 179px">[event:categorybackcolor]</td>
    <td>Background color for the event category</td>
</tr>
<tr>
    <td style="width: 179px">[event:locationlabel]</td>
    <td>Label defined for location in the resource file</td>
</tr>
<tr>
    <td style="width: 179px">[event:location]</td>
    <td>Location associated with the event</td>
</tr>
<tr>
    <td style="width: 179px">[event:locationname]</td>
    <td>Name of location associated with the event</td>
</tr>
<tr>
    <td style="width: 179px">[event:locationurl]</td>
    <td>URL of location associated with the event</td>
</tr>
<tr>
    <td style="width: 179px">[event:locationaddresslabel]</td>
    <td>Label defined for address of location in the resource file</td>
</tr>
<tr>
    <td style="width: 179px">[event:locationstreet]</td>
    <td>Street and housenumber of location associated with the event</td>
</tr>
<tr>
    <td style="width: 179px">[event:locationpostalcode]</td>
    <td>Postal code of location associated with the event</td>
</tr>
<tr>
    <td style="width: 179px">[event:locationcity]</td>
    <td>City of location associated with the event</td>
</tr>
<tr>
    <td style="width: 179px">[event:locationregion]</td>
    <td>Region of location associated with the event</td>
</tr>
<tr>
    <td style="width: 179px">[event:locationcountry]</td>
    <td>Country of location associated with the event</td>
</tr>

<tr>
    <td style="width: 179px">[event:recurringlabel]<sup>§</sup></td>
    <td>Label defined for recurrence in the resource file</td>
</tr>
<tr>
    <td style="width: 179px">[event:recurring]<sup>§</sup></td>
    <td>Recurrence defined in the event</td>
</tr>
<tr>
    <td style="width: 179px">
        [event:recurringicon]<sup>§</sup>
    </td>
    <td>
        Recurrence Icon
    </td>
</tr>
<tr>
    <td style="width: 179px">[event:customfield1label]</td>
    <td>Label defined for custom field 1 in the resource file</td>
</tr>
<tr>
    <td style="width: 179px">[event:customfield1]</td>
    <td>Custom field 1 defined in the event</td>
</tr>
<tr>
    <td style="width: 179px">[event:customfield2label]</td>
    <td>Label defined for custom field 2 in the resource file</td>
</tr>
<tr>
    <td style="width: 179px">[event:customfield2]</td>
    <td>Custom field 2 defined in the event</td>
</tr>
<tr>
    <td style="width: 179px">[event:titleurl]<sup>§</sup></td>
    <td>HREF to the event detail page including title</td>
</tr>
<tr>
    <td style="width: 179px">[event:eventurl]<sup>§</sup></td>
    <td>URL to the event detail page</td>
</tr>
<tr>
    <td style="width: 179px">[event:createdbylabel]<sup>§</sup></td>
    <td>Label defined for created by in the resource file</td>
</tr>
<tr>
    <td style="width: 179px">[event:createdby]<sup>§</sup></td>
    <td>Name of user the event was Created By</td>
</tr>
<tr>
    <td style="width: 179px">[event:createdbyid]<sup>§</sup></td>
    <td>Userid of user the event was Created By</td>
</tr>
<tr>
    <td style="width: 179px">[event:createdbyurl]<sup>§</sup></td>
    <td>URL of the profile of user the event was Created By</td>
</tr>
<tr>
    <td style="width: 179px">[event:createdbyprofile]<sup>§</sup></td>
    <td>Link to the profile of user the event was Created By</td>
</tr>
<tr>
    <td style="width: 179px">[event:createddatelabel]</td>
    <td>Label defined for created date in the resource file</td>
</tr>
<tr>
    <td style="width: 179px">[event:createddate|<i>format</i>]</td>
    <td>Date the event was created</td>
</tr>
<tr>
    <td style="width: 179px">[event:ownedbylabel]</td>
    <td>Label defined for owned by in the resource file</td>
</tr>
<tr>
    <td style="width: 179px">[event:ownedby]<sup>§</sup></td>
    <td>Name of user the event is Owned By</td>
</tr>
<tr>
    <td style="width: 179px">[event:ownedbyid]<sup>§</sup></td>
    <td>Userid of user the event is Owned By</td>
</tr>
<tr>
    <td style="width: 179px">[event:ownedbyurl]<sup>§</sup></td>
    <td>URL of the profile of user the event is Owned By</td>
</tr>
<tr>
    <td style="width: 179px">[event:ownedbyprofile]<sup>§</sup></td>
    <td>Link to the profile of user the event is Owned By</td>
</tr>
<tr>
    <td style="width: 179px">[event:lastupdatedbylabel]<sup>§</sup></td>
    <td>Label defined for last updated by in the resource file</td>
</tr>
<tr>
    <td style="width: 179px">[event:lastupdatedby]<sup>§</sup></td>
    <td>Name of user the event was Last Updated By</td>
</tr>
<tr>
    <td style="width: 179px">[event:lastupdatedbyid]<sup>§</sup></td>
    <td>Userid of user the event was Last Updated By</td>
</tr>
<tr>
    <td style="width: 179px">[event:lastupdatedbyurl]<sup>§</sup></td>
    <td>URL of the profile of user the event was Last Updated By</td>
</tr>
<tr>
    <td style="width: 179px">[event:lastupdatedbyprofile]<sup>§</sup></td>
    <td>Link to the profile of user the event was Last Updated By</td>
</tr>
<tr>
    <td style="width: 179px">[event:lastupdateddatelabel]</td>
    <td>Label defined for last updated date in the resource file</td>
</tr>
<tr>
    <td style="width: 179px">[event:lastupdateddate|<i>format</i>]</td>
    <td>Date the event was last updated</td>
</tr>
<tr>
    <td style="width: 179px">[event:alldayeventtext]</td>
    <td>Label used for an All Day Event - e.g. &#39;(All Day)&#39;</td>
</tr>
<tr>
    <td style="width: 179px">[event:maxenrollmentslabel]</td>
    <td>Label of Max Enrolments</td>
</tr>
<tr>
    <td style="width: 179px">[event:maxenrollments]</td>
    <td>Maximum number of enrolments for the event</td>
</tr>
<tr>
    <td style="width: 179px">[event:noenrollmentslabel]</td>
    <td>Label of No. Enrolments</td>
</tr>
<tr>
    <td style="width: 179px">[event:noenrollments]</td>
    <td>Number of people currently enroled for the event</td>
</tr>
<tr>
    <td style="width: 179px">[event:novacancieslabel]</td>
    <td>Label of No. Vacancies</td>
</tr>
<tr>
    <td style="width: 179px">[event:novacancies]</td>
    <td>Number of vacancies currently available for the event</td>
</tr>
<tr>
    <td style="width: 179px">[event:enrollfeelabel]<sup>§</sup></td>
    <td>Label of Enrolment Fee</td>
</tr>
<tr>
    <td style="width: 179px">[event:enrollfee]<sup>§</sup></td>
    <td>Enrolment fee including currency</td>
</tr>
<tr>
    <td style="width: 179px">
        [event:enrollicon]<sup>§</sup>
    </td>
    <td>
        Enrolment Icon
    </td>
</tr>
<tr>
    <td style="width: 179px">
        [event:reminderlabel]<sup>§</sup>
    </td>
    <td>
        Label of Notification
    </td>
</tr>
<tr>
    <td style="width: 179px">
        [event:reminder]<sup>§</sup>
    </td>
    <td>
        Notification information
    </td>
</tr>
<tr>
    <td style="width: 179px">
        [event:remindericon]<sup>§</sup>
    </td>
    <td>
        Notification Icon
    </td>
</tr>
<tr>
    <td style="width: 179px">
        [event:viewicon]<sup>§</sup>
    </td>
    <td>
        Shows the View Icon for events with a custom detail page, when the user is the
        event editor
    </td>
</tr>
<tr>
    <td style="width: 179px">
        [event:editbutton]
    </td>
    <td>
        Adds a button using the standard edit button image that links to the event
        edit page. The button is only visible to event editors, and only in standard
        display views (Month/Week/Day/List/Repeater) and tooltips.
    </td>
</tr>
<tr>
    <td style="width: 179px">[event:eventid]</td>
    <td>ID of the event as recorded in the Events table</td>
</tr>
<tr>
    <td style="width: 179px">[event:eventmoduleid]</td>
    <td>ID of the module instance the event was created in</td>
</tr>
<tr>
    <td style="width: 179px">[event:enrollmentdefaulturl]</td>
    <td>Default URL of the custom/external enrollment page</td>
</tr>
<tr>
    <td style="width: 179px">
        &nbsp;
    </td>
    <td>
        &nbsp;
    </td>
</tr>
<tr>
    <td colspan="2" style="font-weight: 700">Only available in enrolment emails</td>
</tr>
<tr>
    <td style="width: 179px">[event:signupuserid</td>
    <td>Userid of user enrolled to an event</td>
</tr>
<tr>
    <td style="width: 179px">[event:signupusername]</td>
    <td>Full name of user enrolled to an event</td>
</tr>
<tr>
    <td style="width: 179px">[event:signupuserfirstname]</td>
    <td>First name of user enrolled to an event</td>
</tr>
<tr>
    <td style="width: 179px">[event:signupuserlastname]</td>
    <td>Last name of user enrolled to an event</td>
</tr>
<tr>
    <td style="width: 179px">[event:signupuseremail]</td>
    <td>Email of user enrolled to an event</td>
</tr>
<tr>
    <td style="width: 179px">[event:signupuserstreet]</td>
    <td>Street of user enrolled to an event</td>
</tr>
<tr>
    <td style="width: 179px">[event:signupuserpostalcode]</td>
    <td>Postal code of user enrolled to an event</td>
</tr>
<tr>
    <td style="width: 179px">[event:signupusercity]</td>
    <td>City of user enrolled to an event</td>
</tr>
<tr>
    <td style="width: 179px">[event:signupuserregion]</td>
    <td>Region of user enrolled to an event</td>
</tr>
<tr>
    <td style="width: 179px">[event:signupusercountry]</td>
    <td>Country of user enrolled to an event</td>
</tr>
<tr>
    <td style="width: 179px">[event:signupusercompany]</td>
    <td>Company of user enrolled to an event</td>
</tr>
<tr>
    <td style="width: 179px">[event:signupuserjobtitle]</td>
    <td>Job title of user enrolled to an event</td>
</tr>
<tr>
    <td style="width: 179px">[event:signupuserrefnumber]</td>
    <td>Reference number of user enrolled to an event</td>
</tr>
<tr>
    <td style="width: 179px">
        [event:signupdatelabel]
    </td>
    <td>
        Label defined for start date in the resource file
    </td>
</tr>
<tr>
    <td style="width: 179px">
        [event:signupdate|<i>format</i>]
    </td>
    <td>
        Start date formatted (optional) by the format string *
    </td>
</tr>
<tr>
    <td style="width: 179px">
        [event:noenroleeslabel]
    </td>
    <td>
        Label of number of people enroled by this user
    </td>
</tr>
<tr>
    <td style="width: 179px">
        [event:moderateurl]
    </td>
    <td>
        URL of the moderation page
    </td>
</tr>
<tr>
    <td style="width: 179px">
        &nbsp;
    </td>
    <td>
        &nbsp;
    </td>
</tr>
<tr>
    <td colspan="2" style="font-weight: 700">
        Only available in List View
        Repeater&nbsp;Templates
    </td>
</tr>
<tr>
    <td style="width: 179px">
        [event:repeaterheadertext]
    </td>
    <td>
        Localizable header text (TokenListRptHeader) - Header template only
    </td>
</tr>
<tr>
    <td style="width: 179px">
        [event:repeaterzeroeventstext]
    </td>
    <td>
        Localizable text for text (TokenListRptHeaderZeroEvents) to displayed when there are no events to be listed - Header template only
    </td>
</tr>
<tr>
    <td style="vertical-align: top; width: 179px;">
        [event:repeatertooltip]
    </td>
    <td>
        Event tooltip when not using table layout - advanced users only.
        <br/>
        E.g. <i>&lt;li [event:repeatertooltip]&gt;[event:title]&lt;/li&gt; </i>
        <br/>
        Body template only.
    </td>
</tr>
<tr>
    <td style="width: 179px">
        [event:repeaterfootertext]
    </td>
    <td>
        Localizable footer text (TokenListRptFooter) - Footer template only
    </td>
</tr>
<tr>
    <td style="width: 179px">
        IFZEROEVENTS
    </td>
    <td>
        Used in format [IFZEROEVENTS][event:repeaterzeroeventstext[/IFZEROEVENTS] to
        include the text when there are no events - Header template only.
    </td>
</tr>
<tr>
    <td style="width: 179px">
        &nbsp;
    </td>
    <td>
        &nbsp;
    </td>
</tr>
<tr>
    <td colspan="2" style="font-weight: 700">Only available in Social Groups</td>
</tr>
<tr>
    <td style="width: 179px">
        [event:socialgrouprolenamelabel]
    </td>
    <td>
        Localizable token for the group name label
    </td>
</tr>
<tr>
    <td style="width: 179px">
        [event:socialgrouprolename]
    </td>
    <td>
        The name of the group
    </td>
</tr>
<tr>
    <td style="width: 179px">
        [event:socialgrouproleid]
    </td>
    <td>
        The ID of the group
    </td>
</tr>
<tr>
    <td style="width: 179px">
        &nbsp;
    </td>
    <td>
        &nbsp;
    </td>
</tr>
<tr>
    <td colspan="2" style="font-weight: 700">Only available in User Profiles</td>
</tr>
<tr>
    <td style="width: 179px">
        [event:socialuserusernamelabel]
    </td>
    <td>
        Localizable token for the user name label
    </td>
</tr>
<tr>
    <td style="width: 179px">
        [event:socialuserusername]
    </td>
    <td>
        The user&#39;s username
    </td>
</tr>
<tr>
    <td style="width: 179px">
        [event:socialuserdisplaynamelabel]
    </td>
    <td>
        Localizable token for the display name label
    </td>
</tr>
<tr>
    <td style="width: 179px">
        [event:socialuserdisplayname]
    </td>
    <td>
        The user&#39;s displayname
    </td>
</tr>
<tr>
    <td style="width: 179px">
        &nbsp;
    </td>
    <td>
        &nbsp;
    </td>
</tr>
<tr>
    <td colspan="2">
        § - These tokens are not available in Event Reminders when running in Medium
        Trust
    </td>
</tr>
<tr>
    <td colspan="2">
        * - See section below for date formatting advice
    </td>
</tr>
</tbody>
</table>
<h2>Other Tokens</h2>
<p>
    The following tokens can be used to alter what is displayed based on options stored in the database:-
</p>
<table>
<thead>
<tr>
    <td>Token</td>
    <td>Description</td>
    <td>Example</td>
</tr>
</thead>
<tbody>
<tr>
    <td>ALLDAYEVENT</td>
    <td>Includes the text specified within the token if the event is an All Day Event.</td>
    <td>[ALLDAYEVENT][event:startdate|D][/ALLDAYEVENT]</td>
</tr>
<tr>
    <td>NOTALLDAYEVENT</td>
    <td>
        Includes the text specified within the token if the event is not an All Day
        Event.
    </td>
    <td>[NOTALLDAYEVENT][event:startdate][/NOTALLDAYEVENT]</td>
</tr>
<tr>
    <td>DISPLAYENDDATE</td>
    <td>Includes the text specified within the token if Display End Date is selected.</td>
    <td>[DISPLAYENDDATE][event:enddate][/DISPLAYENDDATE]</td>
</tr>
<tr>
    <td>DISPLAYCUSTOMFIELD1</td>
    <td>Includes the text specified within the token if Custom Field 1 is enabled.</td>
    <td>[DISPLAYCUSTOMFIELD1][event:customfield1][/DISPLAYCUSTOMFIELD1]</td>
</tr>
<tr>
    <td>DISPLAYCUSTOMFIELD2</td>
    <td>Includes the text specified within the token if Custom Field 2 is enabled.</td>
    <td>[DISPLAYCUSTOMFIELD2][event:customfield1][/DISPLAYCUSTOMFIELD2]</td>
</tr>
<tr>
    <td>IFHASIMAGE</td>
    <td>
        Includes the text specified within the token if the event has been assigned an
        image.
    </td>
    <td>[IFHASIMAGE]&lt;a href=&quot;[event:imageurl]&quot;&gt;[event:eventimage]&lt;/a&gt;[/IFHASIMAGE]</td>
</tr>
<tr>
    <td>IFNOTHASIMAGE</td>
    <td>
        Includes the text specified within the token if the event hasn't been assigned an
        image.
    </td>
    <td>[IFNOTHASIMAGE]No image[/IFNOTHASIMAGE]</td>
</tr>
<tr>
    <td>IFHASCATEGORY</td>
    <td>
        Includes the text specified within the token if the event has been assigned to a
        Category.
    </td>
    <td>[IFHASCATEGORY][event:category][/IFHASCATEGORY]</td>
</tr>
<tr>
    <td>IFHASLOCATION</td>
    <td>
        Includes the text specified within the token if the event has been assigned to a
        Location.
    </td>
    <td>[IFHASLOCATION][event:location][/IFHASLOCATION]</td>
</tr>
<tr>
    <td>IFHASLOCATIONURL</td>
    <td>
        Includes the text specified within the token if the event has been assigned to a
        Location which has a url defined.
    </td>
    <td>[IFHASLOCATIONURL][event:locationurl][/IFHASLOCATIONURL]</td>
</tr>
<tr>
    <td>IFNOTHASLOCATIONURL</td>
    <td>
        Includes the text specified within the token if the event has been assigned to a
        Location which does not have a url defined.
    </td>
    <td>[IFNOTHASLOCATIONURL]No link[/IFNOTHASLOCATIONURL]</td>
</tr>
<tr>
    <td>IFALLOWSENROLLMENTS</td>
    <td>Includes the text specified within the token if the event allows enrollments</td>
    <td>[IFALLOWSENROLLMENTS][event:maxenrollments][/IFALLOWSENROLLMENTS]</td>
</tr>
<tr>
    <td>ONEDAYEVENT</td>
    <td>
        Includes the text specified within the token if the event is a one day event
        (i.e. all in one day, as opposed to an All Day Event)
    </td>
    <td>[ONEDAYEVENT][event:startdate] - [event:enddate|hh:mm][/ONEDAYEVENT]</td>
</tr>
<tr>
    <td>NOTONEDAYEVENT</td>
    <td>
        Includes the text specified within the token if the event spans more than one
        day
    </td>
    <td>[NOTONEDAYEVENT][event:startdate] - [event:enddate][/NOTONEDAYEVENT]</td>
</tr>
<tr>
    <td>RECURRINGEVENT</td>
    <td>Includes the text specified within the token if the event is a recurring event</td>
    <td>[RECURRINGEVENT]Recurring - [event:recurring][/RECURRINGEVENT]</td>
</tr>
<tr>
    <td>NOTPRIVATE</td>
    <td>
        Includes the text specified within the token if the event is not private (or you
        are the event owner or moderator). Used when a Private Message is set in the
        module settings.
    </td>
    <td>[NOTPRIVATE][event:titleurl][/NOTPRIVATE]</td>
</tr>
<tr>
    <td>PRIVATE</td>
    <td>
        Includes the text specified within the token if the event is private and your
        are not the owner/moderator.
    </td>
    <td>[PRIVATE]Private Event[/PRIVATE]</td>
</tr>
<tr>
    <td>NOTRECURRINGEVENT</td>
    <td>Includes the text specified within the token if the event is a one off event.</td>
    <td>[NOTRECURRINGEVENT]One day event[/NOTRECURRINGEVENT]</td>
</tr>
<tr>
    <td>
        IFTIMEZONEDISPLAYED
    </td>
    <td>
        If the Display Timezone settings is enabled, the text specified within the token
        is included.
    </td>
    <td>
        [IFTIMEZONEDISPLAY]([event:timezone])[/IFTIMEZONEDISPLAY]
    </td>
</tr>
<tr>
    <td>
        CUSTOMDETAILPAGE
    </td>
    <td>
        If the event has a custom detail page and custom detail pages are enabled, the
        text specified within the token is included.
    </td>
    <td>
        [CUSTOMDETAILPAGE][event:titleurl][/CUSTOMDETAILPAGE]
    </td>
</tr>
<tr>
    <td>
        IFMULTIDAY
    </td>
    <td>
        If the event is longer than 1440 minutes, the text specified within the token is included.
    </td>
    <td>
        [IFMULTIDAY][event:durationdays] [event:durationdayslabel][/IFMULTIDAY]
    </td>
</tr>
<tr>
    <td>
        IFNOTMULTIDAY
    </td>
    <td>
        If the event is not longer than 1440 minutes, the text specified within the token is included.
    </td>
    <td>
        [IFNOTMULTIDAY]1 day[/IFNOTMULTIDAY]
    </td>
</tr>
<tr>
    <td>
        HASROLE
    </td>
    <td>
        If the current user is in the specified role, the text specified within the
        token is included.
    </td>
    <td>
        [HASROLE_Registered Users][event:customfield2][/HASROLE_Registered Users]
    </td>
</tr>
<tr>
    <td>
        HASNOTROLE
    </td>
    <td>
        If the current user is not in the specified role, the text specified within the
        token is included.
    </td>
    <td>
        [HASNOTROLE_Registered Users]Please login.[/HASNOTROLE_Registered Users]
    </td>
</tr>
<tr>
    <td>
        IFHASSUMMARY
    </td>
    <td>
        Includes the text specified within the token if the event has a
        Summary.
    </td>
    <td>
        [IFHASSUMMARY][event:summary|50][/IFHASSUMMARY]
    </td>
</tr>
<tr>
    <td>
        IFNOTHASSUMMARY
    </td>
    <td>
        Includes the text specified within the token if the event does not have a Summary.
    </td>
    <td>
        [IFNOTHASSUMMARY]No Summary[/IFNOTHASSUMMARY]
    </td>
</tr>
<tr>
    <td>
        IFENROLED
    </td>
    <td>
        Includes the text specified within the token if the logged on user is enroled to
        the event.
    </td>
    <td>
        [IFENROLED]You are enroled for this event.[/IFENROLED]
    </td>
</tr>
<tr>
    <td>
        IFNOTENROLED
    </td>
    <td>
        Includes the text specified within the token if the logged on user is not
        enroled to the event.
    </td>
    <td>
        [IFNOTENROLED]Login to Enrol[/IFNOTENROLED]
    </td>
</tr>
<tr>
    <td>
        IFISSOCIALGROUP
    </td>
    <td>
        Includes the text specified within the token if event is in a Social Group.
    </td>
    <td>
        [IFISSOCIALGROUP]([event:socialgrouprolename])[/IFISSOCIALGROUP]
    </td>
</tr>
<tr>
    <td>
        IFISSOCIALUSER
    </td>
    <td>
        Includes the text specified within the token if event is in a User Profile.
    </td>
    <td>
        [IFISSOCIALUSER]([event:socialuserdisplayname])[/IFISSOCIALUSER]
    </td>
</tr>
<tr>
    <td>IFISFULL</td>
    <td>
        Includes the text specified within the token if the event is fully booked
        (no vacancies left).
    </td>
    <td>[IFISFULL]You cannot enroll for this event as it is fully booked.[/IFISFULL]</td>
</tr>
<tr>
    <td>IFNOTISFULL</td>
    <td>
        Includes the text specified within the token if the event is not fully booked
        (still vacancies left).
    </td>
    <td>[IFNOTISFULL]You can still enroll for this event as it is not yet fully booked.[/IFISFULL]</td>
</tr>
</tbody>
</table>
<h2>Limited Use Tokens</h2>
<p>
    The following tokens have specific limited uses:-
</p>
<table>
    <thead>
    <tr>
        <td style="width: 80px">Token</td>
        <td>Description</td>
    </tr>
    </thead>
    <tbody>
    <tr>
        <td>BREAK&nbsp; </td>
        <td>
            The token [BREAK] is used within the Detail View to denote where the template
            should break to display the following elements which are not tokenised:-<br/>
            <ol style="padding-left: 20px">
                <li>Reminder controls</li>
                <li>Enrollment controls</li>
                <li>List of Enrollees</li>
            </ol>
            This token can only be used three times. Any text placed before the first [BREAK] appears above all three elements.
            Any text after the third appears below the list of Enrollees, and above the
            control buttons.
        </td>
    </tr>
    </tbody>
</table>
<h2>Datetime formatting</h2>
<p>
    Format string that defines the text representation of a datetime.<br/>
    Examples:<br/>
    <b>MM/dd/yyyy HH:mm:ss</b> : 12/31/2008 23:59:59<br/>
    <b>dddd d MMMM yyyy hh:mm:ss tt </b>&nbsp;: Wednesday 31 December 2008 11:59:59
    PM<br/>
</p>
<table>
    <thead>
    <tr>
        <td>Format specifier</td>
        <td>Description</td>
        <td>Example</td>
    </tr>
    </thead>
    <tbody>
    <tr>
        <td>d</td>
        <td>Day of the month as a number from 1 to 31</td>
        <td>1</td>
    </tr>
    <tr>
        <td>dd</td>
        <td>Day of the month as a number from 01 to 31</td>
        <td>01</td>
    </tr>
    <tr>
        <td>ddd</td>
        <td>Abbreviated name of the day</td>
        <td>Mon</td>
    </tr>
    <tr>
        <td>dddd</td>
        <td>Full name of the day</td>
        <td>Monday</td>
    </tr>
    <tr>
        <td>M</td>
        <td>Month as a number from 1 to 12</td>
        <td>1</td>
    </tr>
    <tr>
        <td>MM</td>
        <td>Month as a number from 01 to 12</td>
        <td>01</td>
    </tr>
    <tr>
        <td>MMM</td>
        <td>Abbreviated name of the month</td>
        <td>Jan</td>
    </tr>
    <tr>
        <td>MMMM</td>
        <td>Full name of the month</td>
        <td>January</td>
    </tr>
    <tr>
        <td>yy</td>
        <td>Year as a two-digit number</td>
        <td>01</td>
    </tr>
    <tr>
        <td>yyy</td>
        <td>Year as a four-digit number</td>
        <td>2001</td>
    </tr>
    <tr>
        <td>t</td>
        <td>Represents the first charactor of the AM/PM designator</td>
        <td>A</td>
    </tr>
    <tr>
        <td>tt</td>
        <td>Represents the AM/PM designator</td>
        <td>AM</td>
    </tr>
    <tr>
        <td>h</td>
        <td>Hour as a number from 1 to 11</td>
        <td>1</td>
    </tr>
    <tr>
        <td>hh</td>
        <td>Hour as a number from 01 to 11</td>
        <td>01</td>
    </tr>
    <tr>
        <td>H</td>
        <td>Hour as a number from 1 to 23</td>
        <td>1</td>
    </tr>
    <tr>
        <td>HH</td>
        <td>Hour as a number from 01 to 23</td>
        <td>01</td>
    </tr>
    <tr>
        <td>m</td>
        <td>Represents the minute as a number from 0 to 59</td>
        <td>5</td>
    </tr>
    <tr>
        <td>mm</td>
        <td>Represents the minute as a number from 00 to 59</td>
        <td>05</td>
    </tr>
    <tr>
        <td>s</td>
        <td>Represents the second as a number from 0 to 59</td>
        <td>3</td>
    </tr>
    <tr>
        <td>ss</td>
        <td>Represents the second as a number from 00 to 59</td>
        <td>03</td>
    </tr>
    <tr>
        <td>f</td>
        <td>Full date time pattern - Long date, short time</td>
        <td>Thursday, April 10, 2008 6:30 AM </td>
    </tr>
    <tr>
        <td>F</td>
        <td>Full date time pattern - Long date, long time</td>
        <td>Thursday, April 10, 2008 6:30:00 AM </td>
    </tr>
    <tr>
        <td>g</td>
        <td>General date time pattern - Short date, short time</td>
        <td>4/10/2008 6:30 AM </td>
    </tr>
    <tr>
        <td>G</td>
        <td>General date time pattern - Short date, long time</td>
        <td>4/10/2008 6:30:00 AM </td>
    </tr>
    </tbody>
</table>
<i>Reference:<a href="http://msdn.microsoft.com/en-us/library/97x6twsz.aspx">Date and Time Format Strings (msdn.microsoft.com)</a></i>
</asp:Panel>