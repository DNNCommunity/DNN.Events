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

    public partial class EventList
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
        ///gvEvents control.
        ///</summary>
        ///<remarks>
        ///Auto-generated field.
        ///To modify move field declaration from designer file to code-behind file.
        ///</remarks>
        protected global::System.Web.UI.WebControls.GridView gvEvents;

        ///<summary>
        ///divNoEvents control.
        ///</summary>
        ///<remarks>
        ///Auto-generated field.
        ///To modify move field declaration from designer file to code-behind file.
        ///</remarks>
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl divNoEvents;

        ///<summary>
        ///lblNoEvents control.
        ///</summary>
        ///<remarks>
        ///Auto-generated field.
        ///To modify move field declaration from designer file to code-behind file.
        ///</remarks>
        protected global::System.Web.UI.WebControls.Label lblNoEvents;

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

