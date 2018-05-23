using DotNetNuke.Services.Exceptions;
using System.Diagnostics;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using System.Collections;
using System;
using DotNetNuke.Security.Permissions;

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
    using global::Components;

    [DNNtc.ModuleControlProperties("Settings", "Event Settings", DNNtc.ControlType.Admin, "https://dnnevents.codeplex.com/documentation", true, true)]
    public partial class Settings : Entities.Modules.ModuleSettingsBase
    {

        #region  Web Form Designer Generated Code
        //This call is required by the Web Form Designer.
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
        }

        private void Page_Init(System.Object sender, EventArgs e)
        {
            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            InitializeComponent();
        }
        #endregion

        #region Private Data
        #endregion

        #region Help Methods
        /// <summary>
        /// Load current settings into the controls from the modulesettings
        /// </summary>
        /// <remarks></remarks>
        public override void LoadSettings()
        {
            // Force full PostBack since these pass off to aspx page
            if (AJAX.IsInstalled())
            {
                AJAX.RegisterPostBackControl(cmdUpgrade);
            }

            EventModuleSettings emSettings = EventModuleSettings.GetEventModuleSettings(this.ModuleId, this.LocalResourceFile);

            string dummyRmid = emSettings.RecurDummy;
            divUpgrade.Visible = false;
            divRetry.Visible = false;
            if (!ReferenceEquals(dummyRmid, null) &&
                dummyRmid != "99999")
            {
                divUpgrade.Visible = true;
            }


        }

        /// <summary>
        /// Take all settings and write them back to the database
        /// </summary>
        /// <remarks></remarks>
        public override void UpdateSettings()
        {
            try
            {

                MakeModerator_Editor();
                UpdateSubscriptions();
                string emCacheKey = "EventsModuleTitle" + ModuleId.ToString();
                Common.Utilities.DataCache.ClearCache(emCacheKey);

            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void MakeModerator_Editor()
        {
            try
            {
                bool blEditor;
                ArrayList arrRoles = new ArrayList();
                ArrayList arrUsers = new ArrayList();

                DotNetNuke.Security.Permissions.ModulePermissionInfo objPermission = default(DotNetNuke.Security.Permissions.ModulePermissionInfo);
                DotNetNuke.Security.Permissions.PermissionController objPermissionController = new DotNetNuke.Security.Permissions.PermissionController();

                Entities.Modules.ModuleController objModules = new Entities.Modules.ModuleController();
                // Get existing module permissions
                Entities.Modules.ModuleInfo objModule = objModules.GetModule(ModuleId, TabId);

                DotNetNuke.Security.Permissions.ModulePermissionCollection objModulePermissions2 = new DotNetNuke.Security.Permissions.ModulePermissionCollection();
                foreach (ModulePermissionInfo perm in objModule.ModulePermissions)
                {
                    if ((perm.PermissionKey == "EVENTSMOD" && perm.AllowAccess) || (perm.PermissionKey == "EDIT" && perm.AllowAccess))
                    {
                        blEditor = false;
                        foreach (ModulePermissionInfo perm2 in objModule.ModulePermissions)
                        {
                            if (perm2.PermissionKey == "EVENTSEDT" && ((perm.RoleID == perm2.RoleID & perm.RoleID >= 0) || (perm.UserID == perm2.UserID & perm.UserID >= 0)))
                            {
                                if (perm2.AllowAccess)
                                {
                                    blEditor = true;
                                }
                                else
                                {
                                    objModulePermissions2.Add(perm2);
                                }
                            }
                        }
                        if (blEditor == false)
                        {
                            if (perm.UserID >= 0)
                            {
                                arrUsers.Add(perm.UserID);
                            }
                            else
                            {
                                arrRoles.Add(perm.RoleID);
                            }
                        }
                    }
                }

                // Remove negative edit permissions where user is moderator
                foreach (ModulePermissionInfo perm in objModulePermissions2)
                {
                    objModule.ModulePermissions.Remove(perm);
                }

                ArrayList objEditPermissions = objPermissionController.GetPermissionByCodeAndKey("EVENTS_MODULE", "EVENTSEDT");
                PermissionInfo objEditPermission = (PermissionInfo)(objEditPermissions[0]);

                foreach (int iRoleID in arrRoles)
                {
                    // Add Edit Permission for Moderator Role
                    objPermission = new DotNetNuke.Security.Permissions.ModulePermissionInfo();
                    objPermission.RoleID = iRoleID;
                    objPermission.ModuleID = ModuleId;
                    objPermission.PermissionKey = objEditPermission.PermissionKey;
                    objPermission.PermissionName = objEditPermission.PermissionName;
                    objPermission.PermissionCode = objEditPermission.PermissionCode;
                    objPermission.PermissionID = objEditPermission.PermissionID;
                    objPermission.AllowAccess = true;
                    objModule.ModulePermissions.Add(objPermission);
                }
                foreach (int iUserID in arrUsers)
                {
                    objPermission = new DotNetNuke.Security.Permissions.ModulePermissionInfo();
                    objPermission.UserID = iUserID;
                    objPermission.ModuleID = ModuleId;
                    objPermission.PermissionKey = objEditPermission.PermissionKey;
                    objPermission.PermissionName = objEditPermission.PermissionName;
                    objPermission.PermissionCode = objEditPermission.PermissionCode;
                    objPermission.PermissionID = objEditPermission.PermissionID;
                    objPermission.AllowAccess = true;
                    objModule.ModulePermissions.Add(objPermission);
                }
                ModulePermissionController.SaveModulePermissions(objModule);

            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void UpdateSubscriptions()
        {

            EventSubscriptionController objCtlEventSubscriptions = new EventSubscriptionController();
            ArrayList lstEventSubscriptions = default(ArrayList);
            lstEventSubscriptions = objCtlEventSubscriptions.EventsSubscriptionGetModule(ModuleId);
            if (lstEventSubscriptions.Count == 0)
            {
                return;
            }

            EventInfoHelper objEventInfo = new EventInfoHelper(ModuleId, TabId, PortalId, null);
            ArrayList lstusers = objEventInfo.GetEventModuleViewers();

            EventSubscriptionInfo objEventSubscription = default(EventSubscriptionInfo);
            foreach (EventSubscriptionInfo tempLoopVar_objEventSubscription in lstEventSubscriptions)
            {
                objEventSubscription = tempLoopVar_objEventSubscription;
                if (!lstusers.Contains(objEventSubscription.UserID))
                {
                    UserController objCtlUser = new UserController();
                    UserInfo objUser = objCtlUser.GetUser(PortalId, objEventSubscription.UserID);

                    if (ReferenceEquals(objUser, null) || !objUser.IsSuperUser)
                    {
                        objCtlEventSubscriptions.EventsSubscriptionDeleteUser(objEventSubscription.UserID, ModuleId);
                    }
                }
            }
        }

        #endregion

        #region Links, Buttons and Events
        protected void cmdUpgrade_Click(object sender, EventArgs e)
        {
            EventModuleSettings emSettings = EventModuleSettings.GetEventModuleSettings(ModuleId, LocalResourceFile);

            string dummyRmid = emSettings.RecurDummy;
            divUpgrade.Visible = false;
            divRetry.Visible = false;
            if (!ReferenceEquals(dummyRmid, null) &&
                dummyRmid != "99999")
            {
                EventController objEventController = new EventController();
                bool upgradeOk = objEventController.UpgradeRecurringEventModule(ModuleId, int.Parse(dummyRmid), emSettings.Maxrecurrences, LocalResourceFile);
                EventController objEventCtl = new EventController();
                objEventCtl.EventsUpgrade("04.01.00");
                if (!upgradeOk)
                {
                    divUpgrade.Visible = true;
                    divRetry.Visible = true;
                }
            }
        }

        #endregion

    }

}


