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
		
		public partial class EventIcons
		{
			
			///<summary>
			///lblSubscribe control.
			///</summary>
			///<remarks>
			///Auto-generated field.
			///To modify move field declaration from designer file to code-behind file.
			///</remarks>
			protected global::System.Web.UI.WebControls.Label lblSubscribe;
			
			///<summary>
			///btnSubscribe control.
			///</summary>
			///<remarks>
			///Auto-generated field.
			///To modify move field declaration from designer file to code-behind file.
			///</remarks>
			protected global::System.Web.UI.WebControls.ImageButton btnSubscribe;
			
			///<summary>
			///imgBar control.
			///</summary>
			///<remarks>
			///Auto-generated field.
			///To modify move field declaration from designer file to code-behind file.
			///</remarks>
			protected global::System.Web.UI.WebControls.Image imgBar;
			
			///<summary>
			///btnSettings control.
			///</summary>
			///<remarks>
			///Auto-generated field.
			///To modify move field declaration from designer file to code-behind file.
			///</remarks>
			protected global::System.Web.UI.WebControls.HyperLink btnSettings;
			
			///<summary>
			///btnCategories control.
			///</summary>
			///<remarks>
			///Auto-generated field.
			///To modify move field declaration from designer file to code-behind file.
			///</remarks>
			protected global::System.Web.UI.WebControls.HyperLink btnCategories;
			
			///<summary>
			///btnLocations control.
			///</summary>
			///<remarks>
			///Auto-generated field.
			///To modify move field declaration from designer file to code-behind file.
			///</remarks>
			protected global::System.Web.UI.WebControls.HyperLink btnLocations;
			
			///<summary>
			///btnModerate control.
			///</summary>
			///<remarks>
			///Auto-generated field.
			///To modify move field declaration from designer file to code-behind file.
			///</remarks>
			protected global::System.Web.UI.WebControls.ImageButton btnModerate;
			
			///<summary>
			///btnAdd control.
			///</summary>
			///<remarks>
			///Auto-generated field.
			///To modify move field declaration from designer file to code-behind file.
			///</remarks>
			protected global::System.Web.UI.WebControls.HyperLink btnAdd;
			
			///<summary>
			///btnMonth control.
			///</summary>
			///<remarks>
			///Auto-generated field.
			///To modify move field declaration from designer file to code-behind file.
			///</remarks>
			protected global::System.Web.UI.WebControls.ImageButton btnMonth;
			
			///<summary>
			///btnWeek control.
			///</summary>
			///<remarks>
			///Auto-generated field.
			///To modify move field declaration from designer file to code-behind file.
			///</remarks>
			protected global::System.Web.UI.WebControls.ImageButton btnWeek;
			
			///<summary>
			///btnList control.
			///</summary>
			///<remarks>
			///Auto-generated field.
			///To modify move field declaration from designer file to code-behind file.
			///</remarks>
			protected global::System.Web.UI.WebControls.ImageButton btnList;
			
			///<summary>
			///btnEnroll control.
			///</summary>
			///<remarks>
			///Auto-generated field.
			///To modify move field declaration from designer file to code-behind file.
			///</remarks>
			protected global::System.Web.UI.WebControls.ImageButton btnEnroll;
			
			///<summary>
			///hypiCal control.
			///</summary>
			///<remarks>
			///Auto-generated field.
			///To modify move field declaration from designer file to code-behind file.
			///</remarks>
			protected global::System.Web.UI.WebControls.HyperLink hypiCal;
			
			///<summary>
			///btnRSS control.
			///</summary>
			///<remarks>
			///Auto-generated field.
			///To modify move field declaration from designer file to code-behind file.
			///</remarks>
			protected global::System.Web.UI.WebControls.HyperLink btnRSS;
		}
	}
	

