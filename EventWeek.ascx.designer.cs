namespace DotNetNuke.Modules.Events
{
    using DotNetNuke.UI;
    using DotNetNuke.Services.Exceptions;
    using System.Diagnostics;
    using DotNetNuke.Entities.Users;
    using System.Web.UI;
    using System.Drawing;
    using DotNetNuke.Framework;
    using Microsoft.VisualBasic;
    using System.Configuration;
    using System.Web.UI.WebControls;
    using System.Collections;
    using DotNetNuke.Common.Utilities;
    using System.Web;
    using DotNetNuke.Common;
    using System.Web.UI.HtmlControls;
    using DotNetNuke.Services.Localization;
    using System.Data;
    using System;
    using DotNetNuke.Data;
    using DotNetNuke;

    public partial class EventWeek
    {

        ///<summary>
        ///toolTipManager control.
        ///</summary>
        ///<remarks>
        ///Auto-generated field.
        ///To modify move field declaration from designer file to code-behind file.
        ///</remarks>
        protected global::DotNetNuke.Web.UI.WebControls.DnnToolTipManager toolTipManager;

        ///<summary>
        ///pnlDateControls control.
        ///</summary>
        ///<remarks>
        ///Auto-generated field.
        ///To modify move field declaration from designer file to code-behind file.
        ///</remarks>
        protected global::System.Web.UI.WebControls.Panel pnlDateControls;

        ///<summary>
        ///lnkToday control.
        ///</summary>
        ///<remarks>
        ///Auto-generated field.
        ///To modify move field declaration from designer file to code-behind file.
        ///</remarks>
        protected global::System.Web.UI.WebControls.LinkButton lnkToday;

        ///<summary>
        ///dpGoToDate control.
        ///</summary>
        ///<remarks>
        ///Auto-generated field.
        ///To modify move field declaration from designer file to code-behind file.
        ///</remarks>
        protected global::DotNetNuke.Web.UI.WebControls.DnnDatePicker dpGoToDate;

        ///<summary>
        ///SelectCategory control.
        ///</summary>
        ///<remarks>
        ///Auto-generated field.
        ///To modify move field declaration from designer file to code-behind file.
        ///</remarks>
        protected global::DotNetNuke.Modules.Events.SelectCategory SelectCategory;

        ///<summary>
        ///SelectLocation control.
        ///</summary>
        ///<remarks>
        ///Auto-generated field.
        ///To modify move field declaration from designer file to code-behind file.
        ///</remarks>
        protected global::DotNetNuke.Modules.Events.SelectLocation SelectLocation;

        ///<summary>
        ///EventIcons control.
        ///</summary>
        ///<remarks>
        ///Auto-generated field.
        ///To modify move field declaration from designer file to code-behind file.
        ///</remarks>
        protected global::DotNetNuke.Modules.Events.EventIcons EventIcons;

        ///<summary>
        ///lnkPrev control.
        ///</summary>
        ///<remarks>
        ///Auto-generated field.
        ///To modify move field declaration from designer file to code-behind file.
        ///</remarks>
        protected global::System.Web.UI.WebControls.LinkButton lnkPrev;

        ///<summary>
        ///lblWeekOf control.
        ///</summary>
        ///<remarks>
        ///Auto-generated field.
        ///To modify move field declaration from designer file to code-behind file.
        ///</remarks>
        protected global::System.Web.UI.WebControls.Label lblWeekOf;

        ///<summary>
        ///lnkNext control.
        ///</summary>
        ///<remarks>
        ///Auto-generated field.
        ///To modify move field declaration from designer file to code-behind file.
        ///</remarks>
        protected global::System.Web.UI.WebControls.LinkButton lnkNext;

        ///<summary>
        ///schWeek control.
        ///</summary>
        ///<remarks>
        ///Auto-generated field.
        ///To modify move field declaration from designer file to code-behind file.
        ///</remarks>
        protected global::DotNetNuke.Modules.Events.ScheduleControl.ScheduleCalendar schWeek;

        ///<summary>
        ///EventIcons2 control.
        ///</summary>
        ///<remarks>
        ///Auto-generated field.
        ///To modify move field declaration from designer file to code-behind file.
        ///</remarks>
        protected global::DotNetNuke.Modules.Events.EventIcons EventIcons2;
    }
}

