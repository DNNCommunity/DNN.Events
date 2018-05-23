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

namespace DNNtc
{
    using System;

    #region Attributes

    /// <summary>
    ///     This class is used to indicate which UserControls should be in the install package
    /// </summary>
    public class ModuleControlProperties : Attribute
    {
        /// <summary>
        ///     Creates a attribute with the right properties to create a control.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="title">The title.</param>
        /// <param name="userControlType">type of the user control.</param>
        /// <param name="helpUrl">The help URL.</param>
        /// <param name="supportsPartialRendering">if set to <c>true</c> [supports partial rendering].</param>
        /// <param name="supportsPopUps">if set to <c>true</c> [supports pop ups].</param>
        public ModuleControlProperties(string key, string title, ControlType userControlType, string helpUrl,
                                       bool supportsPartialRendering = false, bool supportsPopUps = false)
        {
            //Intentially left empty
        }
    }

    /// <summary>
    ///     Module permission attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ModulePermission : Attribute
    {
        /// <summary>
        ///     Empty Constructor for Intellisense
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
    ///     Enum for type of component used.
    /// </summary>
    public enum ComponentType
    {
        Script,
        Assembly,
        Module
    }

    /// <summary>
    ///     Enum for the type of controls DotNetNuke is using.
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
    ///     The type of dependency for the DotNetNuke Module
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