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
    using System.ComponentModel;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    /// -----------------------------------------------------------------------------
    /// Project	 : schedule
    /// Class	 : ScheduleItem
    /// 
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     The ScheduleItem class represents an item in the Schedule control.
    ///     It can either be an item in the schedule itself, or a header item.
    /// </summary>
    /// -----------------------------------------------------------------------------
    [CLSCompliant(true)]
    [ToolboxItem(false)]
    public class ScheduleItem : WebControl, INamingContainer
    {
        private object _dataItem;
#if NET_2_0
		private DataKey _dataKey;
#endif

        public dynamic DataItem
        {
            get { return this._dataItem; }
            set { this._dataItem = value; }
        }

#if NET_2_0
		public DataKey DataKey
		{
			get
			{
				return _dataKey;
			}
			set
			{
				_dataKey = value;
			}
		}
#endif

        public virtual int DataSetIndex { get; } = -1;

        public virtual ScheduleItemType ItemType { get; }

        public ScheduleItem(int dataSetIndex1, ScheduleItemType itemType1)
        {
            this.DataSetIndex = dataSetIndex1;
            this.ItemType = itemType1;
        }

        protected override bool OnBubbleEvent(object source, EventArgs e)
        {
            // pass any command events to the Schedule control itself
            if (e is CommandEventArgs)
            {
                var args = new ScheduleCommandEventArgs(this, source, (CommandEventArgs) e);
                this.RaiseBubbleEvent(this, args);
                return true;
            }
            return false;
        } //OnBubbleEvent
    }
}