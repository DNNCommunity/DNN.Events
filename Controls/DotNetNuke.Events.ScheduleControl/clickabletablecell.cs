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


namespace DotNetNuke.Modules.Events.ScheduleControl
{
    using System;
    using System.Web.UI.WebControls;

    /// -----------------------------------------------------------------------------
    /// Project	 : schedule
    /// Class	 : ClickableTableCell
    /// 
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     The ClickableTableCell class is used for causing postback when the user
    ///     clicks on empty slots
    /// </summary>
    public class ClickableTableCell : TableCell
    {
        public ClickableTableCell(int newRow, int newColumn)
        {
            this.Row = newRow;
            this.Column = newColumn;
        }

        public int Row { get; set; }

        public int Column { get; set; }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (this.Controls.Count > 0)
            {
                return; // don't allow clicking on cells that contain existing items
            }
            this.Style.Add("cursor", "hand"); // change the cursor: only works in Internet Explorer
            var scheduleControl = (BaseSchedule) this.Parent.Parent.Parent;
            // get tooltip from parent schedule control
            this.ToolTip = scheduleControl.EmptySlotToolTip;
            var eventArgument = this.Row + "-" + Convert.ToString(this.Column);
            this.Attributes["onclick"] =
                this.Page.ClientScript.GetPostBackClientHyperlink(scheduleControl, eventArgument);
        }
    }
}