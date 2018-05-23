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
    /// Class	 : ScheduleItemEventArgs
    /// 
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     The ScheduleItemEventArgs class can be used with the OnItemCommand event of the Schedule controls
    /// </summary>
    [CLSCompliant(true)]
    public sealed class ScheduleCommandEventArgs : CommandEventArgs
    {
        private readonly object _commandSource;

        public ScheduleCommandEventArgs(ScheduleItem item, object commandSource,
                                        CommandEventArgs originalArgs) : base(originalArgs)
        {
            this.Item = item;
            this._commandSource = commandSource;
        }

        public ScheduleItem Item { get; }

        public dynamic CommandSource => this._commandSource;
    }

    public delegate void ScheduleCommandEventHandler(object sender, ScheduleCommandEventArgs e);
}