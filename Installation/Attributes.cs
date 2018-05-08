using System;

namespace DNNtc
{

    #region Attributes
    /// <summary>
    /// This class is used to indicate which UserControls should be in the install package
    /// </summary>
    public class ModuleControlProperties : Attribute
    {
        /// <summary>
        /// Creates a attribute with the right properties to create a control.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="title">The title.</param>
        /// <param name="userControlType">type of the user control.</param>
        /// <param name="helpUrl">The help URL.</param>
        /// <param name="supportsPartialRendering">if set to <c>true</c> [supports partial rendering].</param>
        /// <param name="supportsPopUps">if set to <c>true</c> [supports pop ups].</param>
        public ModuleControlProperties(string key, string title, ControlType userControlType, string helpUrl, bool supportsPartialRendering = false, bool supportsPopUps = false)
        {
            //Intentially left empty
        }
    }

    /// <summary>
    /// Module permission attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ModulePermission : Attribute
    {
        /// <summary>
        /// Empty Constructor for Intellisense
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="Key"></param>
        /// <param name="Name"></param>
        public ModulePermission(string Code, string Key, string Name)
        {
            //Intentially left empty
        }
    }

    public class BusinessControllerClass : Attribute
    {
        //Intentially left empty
    }

    public class UpgradeEventMessage : Attribute
    {
        public UpgradeEventMessage(string VersionList)
        {
            //Intentially left empty
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ModuleDependencies : Attribute
    {
        public ModuleDependencies(ModuleDependency Type, string Value)
        {
            //Intentially left empty
        }
    }

    #endregion

    #region Helper Enumerations

    /// <summary>
    /// Enum for type of component used.
    /// </summary>
    public enum ComponentType
    {
        Script,
        Assembly,
        Module
    }

    /// <summary>
    /// Enum for the type of controls DotNetNuke is using. 
    /// </summary>
    public enum ControlType
    {
        SkinObject,
        Anonymous,
        View,
        Edit,
        Admin,
        Host
    }

    /// <summary>
    /// The type of dependency for the DotNetNuke Module
    /// </summary>
    public enum ModuleDependency
    {
        CoreVersion,
        Package,
        Permission,
        Type
    }

    #endregion
}