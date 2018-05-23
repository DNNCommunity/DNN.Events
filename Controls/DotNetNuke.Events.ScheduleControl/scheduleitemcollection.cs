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
    using System.Collections;

    /// -----------------------------------------------------------------------------
    /// Project	 : schedule
    /// Class	 : ScheduleItemCollection
    /// 
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     The ScheduleItemCollection class represents a collection of ScheduleItem objects.
    /// </summary>
    /// -----------------------------------------------------------------------------
    public class ScheduleItemCollection : ICollection, IEnumerable
    {
        private readonly ArrayList items;

        public ScheduleItemCollection(ArrayList items)
        {
            this.items = items;
        }

        public bool IsReadOnly => false;

        public ScheduleItem this[int index] => (ScheduleItem) this.items[index];

        public int Count => this.items.Count;

        public bool IsSynchronized => false;

        public dynamic SyncRoot => this;

        public void CopyTo(Array array, int index)
        {
            var current = default(ScheduleItem);
            foreach (ScheduleItem tempLoopVar_current in this)
            {
                current = tempLoopVar_current;
                array.SetValue(current, index);
                index++;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return this.items.GetEnumerator();
        }
    } //ScheduleItemCollection
}