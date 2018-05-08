using System;
using System.Data;
using Microsoft.VisualBasic;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

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
	
	[CLSCompliant(true)]public enum ScheduleItemType : int
	{
		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Standard item in a schedule
		/// </summary>
		/// -----------------------------------------------------------------------------
		Item = 0,
		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Alternating standard item in a schedule
		/// </summary>
		/// -----------------------------------------------------------------------------
		AlternatingItem,
		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Item in the range header column or row. In the derived ScheduleCalendar,
		/// this is an item in the time header.
		/// </summary>
		/// -----------------------------------------------------------------------------
		RangeHeader,
		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Item in the title header column or row. In the derived ScheduleCalendar,
		/// this is an item in the date header.
		/// </summary>
		/// -----------------------------------------------------------------------------
		TitleHeader,
		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Item in the optional date header column or row.
		/// Only used in the derived ScheduleGeneral control.
		/// </summary>
		/// -----------------------------------------------------------------------------
		DateHeader
	}
	
	public enum LayoutEnum
	{
		/// -----------------------------------------------------------------------------
		/// <summary>
		/// In Horizontal layout, the range values (times in ScheduleCalendar) are shown horizontally
		/// in the first row, and the titles (dates in ScheduleCalendar) are shown vertically in the first column.
		/// </summary>
		/// -----------------------------------------------------------------------------
		Horizontal = 0,
		/// -----------------------------------------------------------------------------
		/// <summary>
		/// In Vertical layout, the range values (times in ScheduleCalendar) are shown vertically
		/// in the first column, and the titles (dates in ScheduleCalendar) are shown horizontally in the first row.
		/// </summary>
		/// -----------------------------------------------------------------------------
		Vertical
	}
	
	/// -----------------------------------------------------------------------------
	/// Project	 : schedule
	/// Class	 : BaseSchedule
	///
	/// -----------------------------------------------------------------------------
	/// <summary>
	/// BaseSchedule is the base class for the ScheduleGeneral and ScheduleCalendar controls
	/// </summary>
	/// -----------------------------------------------------------------------------

		[ParseChildren(true)]public abstract class BaseSchedule : WebControl, INamingContainer, IPostBackEventHandler
		{

			
#region Private and protected properties

			private object _dataSource;

			private ScheduleItemCollection _items;
			private ArrayList _itemsArrayList;
			
			protected Table Table1;
			
			private TableItemStyle _itemStyle;
			private TableItemStyle _alternatingItemStyle;
			private TableItemStyle _RangeHeaderStyle;
			private TableItemStyle _TitleStyle;
			private TableItemStyle _BackgroundStyle;
			
			private string _emptySlotToolTip = "Click here to add data";
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// list with values to be shown in row header
			/// </summary>
			/// -----------------------------------------------------------------------------
			protected ArrayList arrRangeValues
			{
				get
				{
					return ((ArrayList) (ViewState["arrRangeValues"]));
				}
				set
				{
					ViewState["arrRangeValues"] = value;
				}
			}
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// Items is a collection object that contains the ScheduleItem objects
			/// </summary>
			/// -----------------------------------------------------------------------------
			protected virtual ScheduleItemCollection Items
			{
				get
				{
					if (ReferenceEquals(_items, null))
					{
						if (ReferenceEquals(_itemsArrayList, null))
						{
							EnsureChildControls();
						}
						_items = new ScheduleItemCollection(_itemsArrayList);
					}
					return _items;
				}
			}
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// Whether to show the EmptyDataTemplate or not when no data is found
			/// </summary>
			/// <remarks>
			/// Overridden in ScheduleCalendar when FullTimeScale=True
			/// </remarks>
			/// -----------------------------------------------------------------------------
			protected virtual bool ShowEmptyDataTemplate
			{
				get
				{
					return true; // Overridden in ScheduleCalendar
				}
			}
			
#endregion
			
#region Public properties
			[Description("The direction in which the item ranges are shown."), 
			DefaultValue(LayoutEnum.Vertical), Bindable(false), Category("Appearance")]public LayoutEnum Layout
			{
				get
				{
					object o = ViewState["Layout"];
					if (!(ReferenceEquals(o, null)))
					{
						return ((LayoutEnum) o);
					}
					return LayoutEnum.Vertical;
				}
				set
				{
					ViewState["Layout"] = value;
				}
			}
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// If IncludeEndValue is true, the event is shown including the end row or column
			/// </summary>
			/// -----------------------------------------------------------------------------
			[Description("If true, the event is shown including the end row or column"), 
			DefaultValue(false), Bindable(false), Category("Behavior")]public bool IncludeEndValue
			{
				get
				{
					object o = ViewState["IncludeEndValue"];
					if (!(ReferenceEquals(o, null)))
					{
						return System.Convert.ToBoolean(o);
					}
					return false;
				}
				set
				{
					ViewState["IncludeEndValue"] = value;
				}
			}
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// If ShowValueMarks is true, value marks will be shown in the range header column or row.
			/// Applied only when IncludeEndValue is false.
			/// </summary>
			/// -----------------------------------------------------------------------------
			[Description("If true, value marks will be shown in the range header column or row. Applied only when IncludeEndValue is false."), 
			DefaultValue(false), Bindable(false), Category("Behavior")]public bool ShowValueMarks
			{
				get
				{
					object o = ViewState["ShowValueMarks"];
					if (!(ReferenceEquals(o, null)))
					{
						return System.Convert.ToBoolean(o);
					}
					return false;
				}
				set
				{
					ViewState["ShowValueMarks"] = value;
				}
			}
			

			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// The data source that is used to show items in the schedule
			/// </summary>
			/// -----------------------------------------------------------------------------
			[Description("The data source that is used to show items in the schedule"), 
			Bindable(true), DefaultValue(null, ""), 
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), 
			Category("Data")]public dynamic DataSource
			{
				get
				{
					return _dataSource;
				}
				set
				{
					if ((ReferenceEquals(value, null)) || (value is DataTable) || (value is DataSet) || (value is DataView))
					{
						_dataSource = value;
					}
					else
					{
						throw (new HttpException("The DataSource must be a DataTable, DataSet or DataView object"));
					}
				}
			}
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// The table or view used for binding when a DataSet is used as a data source.
			/// </summary>
			/// -----------------------------------------------------------------------------
			[Description("The table or view used for binding when a DataSet is used as a data source."), 
			DefaultValue(""), Category("Data")]public string DataMember
			{
				get
				{
					object o = ViewState["DataMember"];
					if (!(ReferenceEquals(o, null)))
					{
						return Convert.ToString(o);
					}
					return string.Empty;
				}
				set
				{
					ViewState["DataMember"] = value;
				}
			}

			/// -----------------------------------------------------------------------------
			/// <summary>
			/// The database field containing the start of the items.
			/// </summary>
			/// -----------------------------------------------------------------------------
			[Description("The database field containing the start of the items."), 
			Bindable(false), Category("Data")]public string DataRangeStartField
			{
				get
				{
					object o = ViewState["DataRangeStartField"];
					if (!(ReferenceEquals(o, null)))
					{
						return Convert.ToString(o);
					}
					return "";
				}
				set
				{
					ViewState["DataRangeStartField"] = value;
				}
			}
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// The database field containing the end of the items.
			/// </summary>
			/// -----------------------------------------------------------------------------
			[Description("The database field containing the end of the items."), Bindable(false), Category("Data")]public string DataRangeEndField
			{
				get
				{
					object o = ViewState["DataRangeEndField"];
					if (!(ReferenceEquals(o, null)))
					{
						return Convert.ToString(o);
					}
					return "";
				}
				set
				{
					ViewState["DataRangeEndField"] = value;
				}
			}
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// The database field providing the titles.
			/// In Calendar mode this field should be of type Date when TimeFieldsContainDate=false.
			/// </summary>
			/// -----------------------------------------------------------------------------
			[Description("The database field providing the titles. In Calendar mode this field should be of type Date when TimeFieldsContainDate=false."), Bindable(false), Category("Data")]public string TitleField
			{
				get
				{
					object o = ViewState["TitleField"];
					if (!(ReferenceEquals(o, null)))
					{
						return Convert.ToString(o);
					}
					return "";
				}
				set
				{
					ViewState["TitleField"] = value;
				}
			}
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// An optional database field providing the item styles (in the form of a css class name).
			/// If not provided, then the ItemStyle property will be used for all items.
			/// </summary>
			/// -----------------------------------------------------------------------------
			[Description("Optional database field providing the item styles.  If not provided, then the ItemStyle property will be used for all items."), Bindable(false), Category("Data")]public string ItemStyleField
			{
				get
				{
					object o = ViewState["ItemStyleField"];
					if (!(ReferenceEquals(o, null)))
					{
						return Convert.ToString(o);
					}
					return "";
				}
				set
				{
					ViewState["ItemStyleField"] = value;
				}
			}
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// Boolean value indicating if a full time scale should be shown.
			/// If true, show a full time scale.
			/// If false, show only the occurring values in the data source.
			/// </summary>
			/// -----------------------------------------------------------------------------
			[Description("If true, show a full time scale. If false, show only the occurring values in the data source."), DefaultValue(false), Bindable(true), Category("Behavior")]public bool FullTimeScale
			{
				get
				{
					object o = ViewState["FullTimeScale"];
					if (!(ReferenceEquals(o, null)))
					{
						return System.Convert.ToBoolean(o);
					}
					return false;
				}
				set
				{
					ViewState["FullTimeScale"] = value;
				}
			}
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// The number of minutes between each mark on the time scale. Only used when FullTimeScale is true.
			/// </summary>
			/// -----------------------------------------------------------------------------
			[Description("The number of minutes between each mark on the time scale. Only used when FullTimeScale is true."), DefaultValue(60), Bindable(true), Category("Behavior")]public int TimeScaleInterval
			{
				get
				{
					object o = ViewState["TimeScaleInterval"];
					if (!(ReferenceEquals(o, null)))
					{
						return System.Convert.ToInt32(o);
					}
					return 60;
				}
				set
				{
					if (value == 0)
					{
						throw (new HttpException("TimeScaleInterval can not be 0"));
					}
					ViewState["TimeScaleInterval"] = value;
				}
			}
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// The start of the time scale. Only used when FullTimeScale is true.
			/// </summary>
			/// -----------------------------------------------------------------------------
			[Description("The start of the time scale. Only used when FullTimeScale is true."), DefaultValue("8:00:00"), Bindable(true), Category("Behavior")]public TimeSpan StartOfTimeScale
			{
				get
				{
					object o = ViewState["StartOfTimeScale"];
					if (!(ReferenceEquals(o, null)))
					{
						return ((TimeSpan) o);
					}
					if (FullTimeScale)
					{
						return new TimeSpan(0, 0, 0); // = 0:00
					}
					return new TimeSpan(8, 0, 0); // = 8:00 AM
				}
				set
				{
					if (value.Days == 0)
					{
						ViewState["StartOfTimeScale"] = value;
					}
					else
					{
						throw (new HttpException("StartOfTimeScale should be between 0:00:00 and 23:59:00"));
					}
				}
			}
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// The end of the time scale. Only used when FullTimeScale is true.
			/// </summary>
			/// -----------------------------------------------------------------------------
			[Description("The end of the time scale. Only used when FullTimeScale is true."), DefaultValue("17:00:00"), Bindable(true), Category("Behavior")]public TimeSpan EndOfTimeScale
			{
				get
				{
					object o = ViewState["EndOfTimeScale"];
					if (!(ReferenceEquals(o, null)))
					{
						return ((TimeSpan) o);
					}
					if (FullTimeScale)
					{
						return new TimeSpan(1, 0, 0, 0); // = 24:00
					}
					return new TimeSpan(17, 0, 0); // = 5:00 PM
				}
				set
				{
					if (value.Days == 0 || value.Equals(new TimeSpan(1, 0, 0, 0)))
					{
						ViewState["EndOfTimeScale"] = value;
					}
					else
					{
						throw (new HttpException("EndOfTimeScale should be between 0:00:00 and 1.00:00:00 (24h)"));
					}
				}
			}
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// The cell padding of the rendered table.
			/// </summary>
			/// -----------------------------------------------------------------------------
			[Bindable(true), Category("Appearance"), DefaultValue(-1), Description("The cell padding of the rendered table.")]public virtual int CellPadding
			{
				get
				{
					if (ControlStyleCreated == false)
					{
						return -1;
					}
					return ((TableStyle) ControlStyle).CellPadding;
				}
				set
				{
					if (ControlStyle is TableStyle)
					{
						((TableStyle) ControlStyle).CellPadding = value;
					}
				}
			}
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// The cell spacing of the rendered table.
			/// </summary>
			/// -----------------------------------------------------------------------------
			[Bindable(true), Category("Appearance"), DefaultValue(1), Description("The cell spacing of the rendered table.")]public virtual int CellSpacing
			{
				get
				{
					if (ControlStyleCreated == false)
					{
						return 1;
					}
					return ((TableStyle) ControlStyle).CellSpacing;
				}
				set
				{
					if (ControlStyle is TableStyle)
					{
						((TableStyle) ControlStyle).CellSpacing = value;
					}
				}
			}
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// The grid lines to be shown in the rendered table.
			/// </summary>
			/// -----------------------------------------------------------------------------
			[Bindable(true), Category("Appearance"), DefaultValue(System.Web.UI.WebControls.GridLines.None), Description("The grid lines to be shown in the rendered table.")]public virtual GridLines GridLines
			{
				get
				{
					if (ControlStyleCreated == false)
					{
						return GridLines.None;
					}
					return ((TableStyle) ControlStyle).GridLines;
				}
				set
				{
					if (ControlStyle is TableStyle)
					{
						((TableStyle) ControlStyle).GridLines = value;
					}
				}
			}
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// The horizontal alignment of text in the table.
			/// </summary>
			/// -----------------------------------------------------------------------------
			[Bindable(true), Category("Appearance"), DefaultValue(System.Web.UI.WebControls.HorizontalAlign.NotSet), Description("The horizontal alignment of text in the table.")]public virtual HorizontalAlign HorizontalAlign
			{
				get
				{
					if (ControlStyleCreated == false)
					{
						return HorizontalAlign.NotSet;
					}
					return ((TableStyle) ControlStyle).HorizontalAlign;
				}
				set
				{
					if (ControlStyle is TableStyle)
					{
						((TableStyle) ControlStyle).HorizontalAlign = value;
					}
				}
			}
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// When true, clicking on an empty slot in the control will raize the OnEmptySlotClick event.
			/// </summary>
			/// -----------------------------------------------------------------------------
			[Description("When true, clicking on an empty slot in the control will raize the OnEmptySlotClick event."), DefaultValue(false), Bindable(false), Category("Behavior")]public bool EnableEmptySlotClick
			{
				get
				{
					object o = ViewState["EnableEmptySlotClick"];
					if (!(ReferenceEquals(o, null)))
					{
						return System.Convert.ToBoolean(o);
					}
					return false;
				}
				set
				{
					ViewState["EnableEmptySlotClick"] = value;
				}
			}
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// ToolTip on empty slots. Only shown when EmptySlotClickEnabled is true.
			/// </summary>
			/// -----------------------------------------------------------------------------
			[Description("ToolTip on empty slots. Only shown when EmptySlotClickEnabled is true."), DefaultValue("Click here to add data"), Bindable(false), Category("Behavior")]public string EmptySlotToolTip
			{
				get
				{
					return this._emptySlotToolTip;
				}
				set
				{
					this._emptySlotToolTip = value;
				}
			}
			
			protected override System.Web.UI.HtmlTextWriterTag TagKey
			{
				get
				{
					return HtmlTextWriterTag.Div;
				}
			}
			
#endregion
			
#region Styles
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// The style applied to schedule items.
			/// </summary>
			
			[Description("The style applied to schedule items. "), Bindable(false), Category("Style"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]public virtual TableItemStyle ItemStyle
			{
				get
				{
					if (ReferenceEquals(_itemStyle, null))
					{
						_itemStyle = new TableItemStyle();
						if (IsTrackingViewState)
						{
							((IStateManager) _itemStyle).TrackViewState();
						}
					}
					return _itemStyle;
				}
			}
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// The style applied to alternating schedule items.
			/// </summary>
			/// -----------------------------------------------------------------------------
			[Description("The style applied to alternating schedule items. "), Bindable(false), Category("Style"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]public virtual TableItemStyle AlternatingItemStyle
			{
				get
				{
					if (ReferenceEquals(_alternatingItemStyle, null))
					{
						_alternatingItemStyle = new TableItemStyle();
						if (IsTrackingViewState)
						{
							((IStateManager) _alternatingItemStyle).TrackViewState();
						}
					}
					return _alternatingItemStyle;
				}
			}
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// The style applied to range header items.
			/// </summary>
			/// -----------------------------------------------------------------------------
			[Description("The style applied to range header items. "), Bindable(false), Category("Style"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]public virtual TableItemStyle RangeHeaderStyle
			{
				get
				{
					if (ReferenceEquals(_RangeHeaderStyle, null))
					{
						_RangeHeaderStyle = new TableItemStyle();
						if (IsTrackingViewState)
						{
							((IStateManager) _RangeHeaderStyle).TrackViewState();
						}
					}
					return _RangeHeaderStyle;
				}
			}
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// The style applied to title items.
			/// </summary>
			/// -----------------------------------------------------------------------------
			[Description("The style applied to title items. "), Bindable(false), Category("Style"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]public virtual TableItemStyle TitleStyle
			{
				get
				{
					if (ReferenceEquals(_TitleStyle, null))
					{
						_TitleStyle = new TableItemStyle();
						if (IsTrackingViewState)
						{
							((IStateManager) _TitleStyle).TrackViewState();
						}
					}
					return _TitleStyle;
				}
			}
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// The style applied to the background cells.
			/// </summary>
			/// -----------------------------------------------------------------------------
			[Description("The style applied to the background cells. "), Bindable(false), Category("Style"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]public virtual TableItemStyle BackgroundStyle
			{
				get
				{
					if (ReferenceEquals(_BackgroundStyle, null))
					{
						_BackgroundStyle = new TableItemStyle();
						if (IsTrackingViewState)
						{
							((IStateManager) _BackgroundStyle).TrackViewState();
						}
					}
					return _BackgroundStyle;
				}
			}
			
#endregion
			
#region Templates
			
			private ITemplate _itemTemplate = null;
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// Main template providing content for each regular item in the body of the schedule.
			/// </summary>
			/// -----------------------------------------------------------------------------
			[TemplateContainer(typeof(ScheduleItem)), Browsable(false), Description("The content to be shown in each regular item."), PersistenceMode(PersistenceMode.InnerProperty)]public ITemplate ItemTemplate
			{
				get
				{
					return _itemTemplate;
				}
				set
				{
					_itemTemplate = value;

				}
			}
			
			private ITemplate _emptyDataTemplate = null;
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// Optional template providing content to be shown when the data source is empty.
			/// This template is not used by the ScheduleCalendar control when FullTimeScale=True.
			/// </summary>
			/// -----------------------------------------------------------------------------
			[TemplateContainer(typeof(ScheduleItem)), Browsable(false), Description("The content to be shown when the data source is empty. This template is not used by the ScheduleCalendar control when FullTimeScale=True."), PersistenceMode(PersistenceMode.InnerProperty)]public ITemplate EmptyDataTemplate
			{
				get
				{
					return _emptyDataTemplate;
				}
				set
				{
					_emptyDataTemplate = value;

				}
			}
#endregion
			
#region Events
			
			protected virtual void OnItemCommand(ScheduleCommandEventArgs e)
			{
				if (ItemCommandEvent != null)
					ItemCommandEvent(this, e);
			}
			
			protected virtual void OnItemCreated(ScheduleItemEventArgs e)
			{
				if (ItemCreatedEvent != null)
					ItemCreatedEvent(this, e);
			}
			
			protected virtual void OnItemDataBound(ScheduleItemEventArgs e)
			{
				if (ItemDataBoundEvent != null)
					ItemDataBoundEvent(this, e);
			}
			
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// Raised when EnableEmptySlotClick is true and the user clicks on an empty slot.
			/// </summary>
			/// <param name="e">Event argument with information about the cell that was clicked</param>
			/// -----------------------------------------------------------------------------
			protected virtual void OnEmptySlotClick(ClickableTableCellEventArgs e)
			{
				if (EmptySlotClickEvent != null)
					EmptySlotClickEvent(this, e);
			}
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// Raised when a CommandEvent occurs within an item.
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			/// -----------------------------------------------------------------------------
			[Category("Action"), Description("Raised when a CommandEvent occurs within an item.")]private ScheduleCommandEventHandler ItemCommandEvent;
			public event ScheduleCommandEventHandler ItemCommand
			{
				add
				{
					ItemCommandEvent = (ScheduleCommandEventHandler) System.Delegate.Combine(ItemCommandEvent, value);
				}
				remove
				{
					ItemCommandEvent = (ScheduleCommandEventHandler) System.Delegate.Remove(ItemCommandEvent, value);
				}
			}
			
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// Raised when an item is created and is ready for customization.
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			/// -----------------------------------------------------------------------------
			[Category("Behavior"), Description("Raised when an item is created and is ready for customization.")]private ScheduleItemEventHandler ItemCreatedEvent;
			public event ScheduleItemEventHandler ItemCreated
			{
				add
				{
					ItemCreatedEvent = (ScheduleItemEventHandler) System.Delegate.Combine(ItemCreatedEvent, value);
				}
				remove
				{
					ItemCreatedEvent = (ScheduleItemEventHandler) System.Delegate.Remove(ItemCreatedEvent, value);
				}
			}
			
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// Raised when an item is data-bound.
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			/// -----------------------------------------------------------------------------
			[Category("Behavior"), Description("Raised when an item is data-bound.")]private ScheduleItemEventHandler ItemDataBoundEvent;
			public event ScheduleItemEventHandler ItemDataBound
			{
				add
				{
					ItemDataBoundEvent = (ScheduleItemEventHandler) System.Delegate.Combine(ItemDataBoundEvent, value);
				}
				remove
				{
					ItemDataBoundEvent = (ScheduleItemEventHandler) System.Delegate.Remove(ItemDataBoundEvent, value);
				}
			}
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// Occurs when the user clicks on an empty slot.
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			/// -----------------------------------------------------------------------------
			[Category("Action"), Description("Occurs when the user clicks on an empty slot.")]private ClickableTableCellEventHandler EmptySlotClickEvent;
			public event ClickableTableCellEventHandler EmptySlotClick
			{
				add
				{
					EmptySlotClickEvent = (ClickableTableCellEventHandler) System.Delegate.Combine(EmptySlotClickEvent, value);
				}
				remove
				{
					EmptySlotClickEvent = (ClickableTableCellEventHandler) System.Delegate.Remove(EmptySlotClickEvent, value);
				}
			}
			
			
#endregion
			
#region Methods and Implementation
			
			protected BaseSchedule() // hide constructor in this abstract class
			{
			}
			

			
			protected override void CreateChildControls()
			{
				CheckVersion();
				Controls.Clear();
				if ((!ReferenceEquals(ViewState["RowCount"], null)) || (!ReferenceEquals(ViewState["Empty"], null)))
				{
					// Create the control hierarchy using the view state, not the data source.
					CreateControlHierarchy(false);
				}
				else
				{
					_itemsArrayList = new ArrayList();
					ClearChildViewState();
				}
			}
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// Checks if the application is running on the proper version of ASP.NET.
			/// If not, an exception is thrown.
			/// </summary>
			/// -----------------------------------------------------------------------------
			public void CheckVersion()
			{
			}
			

			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// Overrides Control.Databind.
			/// </summary>
			/// -----------------------------------------------------------------------------
			public override void DataBind()
			{
				OnDataBinding(EventArgs.Empty); // See BaseDatalist control in mono
			}
			
			/// -----------------------------------------------------------------------------
			/// <summary>
			/// Overrides Control.OnDatabinding.
			/// </summary>
			/// -----------------------------------------------------------------------------
			protected override void OnDataBinding(EventArgs e)
			{
				base.OnDataBinding(e);
				if (!ReferenceEquals(DataSource, null))
				{
					Controls.Clear(); // clear any existing child controls
					
					// clear any previous viewstate for existing child controls
					ClearChildViewState();
					CreateControlHierarchy(true);
					
					// prevent child controls from being created again
					ChildControlsCreated = true;
					TrackViewState();
				}
			}
				public void CreateControlHierarchy(bool useDataSource)
				{
					if (!(ReferenceEquals(_itemsArrayList, null)))
					{
						_itemsArrayList.Clear();
						_items = null;
					}
					else
					{
						_itemsArrayList = new ArrayList();
					}
					

						if (useDataSource)
						{
							string strCheckConfiguration = CheckConfiguration();
							// check if all the necessary properties are set
							if (!string.IsNullOrEmpty(strCheckConfiguration))
							{
								throw (new HttpException(strCheckConfiguration));
							}

							DataView dv = GetDataView();

							Controls.Clear();
							if ((ReferenceEquals(dv, null) || dv.Count == 0) && ShowEmptyDataTemplate)
							{
								RenderEmptyDataTemplate();
								return ;
							}
							// clear any existing child controls
							Table1 = new Table();
							Controls.Add(Table1);
							Table1.CopyBaseAttributes(this);
							if (ControlStyleCreated)
							{
								Table1.ApplyStyle(ControlStyle);
							}
							
							PreprocessData(ref dv);
							
							FillRangeValueArray(ref dv);
							FillTitleValueArray(ref dv);
							
							CreateEmptyTable();
							
							AddRangeHeaderData();
							AddTitleHeaderData();
							
							AddData(dv);
							
							if (!IncludeEndValue && ShowValueMarks)
							{
								AddRangeValueMarks();
							}
							
							AddDateHeaderData();
							
							// Save information for use in round trips (enough to re-create the control tree).
							if (!ReferenceEquals(HttpContext.Current, null))
							{
								SaveControlTree();
							}
						}
						else // Not useDataSource
						{
							LoadControlTree(); // Recreate the control tree from viewstate
						}
					}
					
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// Check if all properties are set to make the control work
					/// Override this function in derived versions.
					/// </summary>
					/// <returns></returns>
					/// -----------------------------------------------------------------------------
					abstract public string CheckConfiguration();
					
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// Create the list with all range header values (Start or End)
					/// </summary>
					/// <param name="dv"></param>
					/// -----------------------------------------------------------------------------
					abstract public void FillRangeValueArray(ref DataView dv);
					
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// Remove any doubles from the ArrayList and
					/// </summary>
					/// <param name="arr">The ArrayList object</param>
					/// -----------------------------------------------------------------------------
					public static void RemoveDoubles(ArrayList arr)
					{
						int count = arr.Count;
						int i = 0;
						for (i = count - 1; i >= 1; i--)
						{
							if (arr[i].ToString() == arr[i - 1].ToString())
							{
								arr.RemoveAt(i);
							}
						}
					}
					
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// Pre-process the data in the dataview
					/// Currently only used in ScheduleCalendar to split the items that span midnight
					/// </summary>
					/// <param name="dv">The DataView object containing the data</param>
					/// -----------------------------------------------------------------------------
					virtual public void PreprocessData(ref DataView dv)
					{
						// override to add usefull processing if any
					}
					
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// Create the list with all the title values
					/// </summary>
					/// <param name="dvView">The DataView object containing the data</param>
					/// -----------------------------------------------------------------------------
					virtual public void FillTitleValueArray(ref DataView dvView)
					{
						// Override in ScheduleGeneral to fill title array
					}
					
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// Get the number of times the control should repeat the schedule
					/// (every time with new headers)
					/// </summary>
					/// <returns>An integer value indicating the repetition count</returns>
					/// <remarks>
					/// Overridden in ScheduleCalendar to return NumberOfRepetitions (usually number of weeks)
					/// </remarks>
					/// -----------------------------------------------------------------------------
					virtual public int GetRepetitionCount()
					{
						return 1;
					}
					
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// Create a Table control with the right number of rows and columns.
					/// The actual content is added later.
					/// </summary>
					/// -----------------------------------------------------------------------------
					public void CreateEmptyTable()
					{
						int col = 0; // counters
						int row = 0;
						int iWeek = 0;
						
						int nTitles = GetTitleCount();
						
						if (Layout == LayoutEnum.Vertical)
						{
							for (iWeek = 1; iWeek <= GetRepetitionCount(); iWeek++)
							{
								// Add a row of title header cells
								Table1.Rows.Add(new TableRow());
								TableRow tr0 = Table1.Rows[Table1.Rows.Count - 1];
								for (col = 1; col <= nTitles + 1; col++)
								{
									tr0.Cells.Add(new TableHeaderCell());
									tr0.Cells[tr0.Cells.Count - 1].ApplyStyle(TitleStyle);
									tr0.Cells[tr0.Cells.Count - 1].Text = "&nbsp;";
								}
								
								int nRows = 0;
								if (!IncludeEndValue && ShowValueMarks)
								{
									// Create 2 rows for each value allowing for a value mark to be added in between
									nRows = arrRangeValues.Count * 2;
								}
								else
								{
									nRows = arrRangeValues.Count;
								}
								
								for (row = 1; row <= nRows; row++)
								{
									// add a cell for the range header
									Table1.Rows.Add(new TableRow());
									TableRow tr = Table1.Rows[Table1.Rows.Count - 1];
									// add a cell for the header column
									tr.Cells.Add(new TableHeaderCell());
									TableCell cell0 = tr.Cells[tr.Cells.Count - 1];
									cell0.ApplyStyle(RangeHeaderStyle);
									cell0.Text = "&nbsp;";
									
									// If no value marks are needed add cells to all the rows
									// Else show only the even rows and the first row
									
									if (IncludeEndValue || !ShowValueMarks || row % 2 == 0 | row == 1)
									{
										
										// add a cell for each normal column
										for (col = 1; col <= nTitles; col++)
										{
											if (EnableEmptySlotClick && (!(ShowValueMarks && !IncludeEndValue && (row == 1 | row == nRows))))
											{
												tr.Cells.Add(new ClickableTableCell(Table1.Rows.Count - 1, col));
											}
											else
											{
												tr.Cells.Add(new TableCell());
											}
											TableCell cell = tr.Cells[tr.Cells.Count - 1];
											cell.ApplyStyle(BackgroundStyle);
											
											if (!IncludeEndValue && ShowValueMarks)
											{
												if (row > 1 & row < nRows)
												{
													// the first and last row only have a span of 1
													cell.Text = "&nbsp;";
													cell.RowSpan = 2;
												}
											}
											else
											{
												cell.Text = "&nbsp;";
											}
										}
									}
								}
							}
						}
						else // Horizontal
						{
							int nColumnsForRangeHeaders = arrRangeValues.Count;
							if (!IncludeEndValue && ShowValueMarks)
							{
								// Create 2 columns for each value allowing for a value mark to be added in between
								nColumnsForRangeHeaders = arrRangeValues.Count * 2;
							}
							
							// In Horizontal layout, ignore repetition count: show 1 week only
							// Add range header cell
							Table1.Rows.Add(new TableRow());
							TableRow tr0 = Table1.Rows[0];
							tr0.Cells.Add(new TableHeaderCell());
							tr0.Cells[0].ApplyStyle(TitleStyle);
							tr0.Cells[0].Text = "&nbsp;";
							for (col = 1; col <= nColumnsForRangeHeaders; col++)
							{
								tr0.Cells.Add(new TableHeaderCell());
								TableCell cell = tr0.Cells[col];
								cell.ApplyStyle(RangeHeaderStyle);
								cell.Text = "&nbsp;";
							}
							
							int nColumns = arrRangeValues.Count;
							if (!IncludeEndValue && ShowValueMarks)
							{
								// Extra column to allow the range headers to sit on the separation
								// When for example there are 4 values, we make 5 columns
								// 1 startcolumn, 3 columns sitting each between 2 values, and an end column
								nColumns++;
							}
							
							for (row = 1; row <= nTitles; row++)
							{
								Table1.Rows.Add(new TableRow());
								TableRow tr = Table1.Rows[row];
								// add a cell for the title header
								tr.Cells.Add(new TableHeaderCell());
								tr.Cells[0].ApplyStyle(TitleStyle);
								tr.Cells[0].Text = "&nbsp;";
								
								// add a cell for each column
								for (col = 1; col <= nColumns; col++)
								{
									if (EnableEmptySlotClick && (!(!IncludeEndValue && ShowValueMarks && (col == 1 | col == nColumns))))
									{
										tr.Cells.Add(new ClickableTableCell(row, col));
									}
									else
									{
										tr.Cells.Add(new TableCell());
									}
									TableCell cell = tr.Cells[col];
									cell.ApplyStyle(BackgroundStyle);
									cell.Text = "&nbsp;";
									
									if (!IncludeEndValue && ShowValueMarks && col > 1 & col < nColumns)
									{
										// the first and last column only have a span of 1
										cell.ColumnSpan = 2;
									}
								}
							}
						}
						
					}
					
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// Add date headers to the table when SeparateDateHeader=True
					/// Overridden only in ScheduleGeneral to add date header data
					/// </summary>
					/// -----------------------------------------------------------------------------
					virtual public void AddDateHeaderData()
					{
						// Override in ScheduleGeneral to add date header data
					}
					
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// Iterate the arrRangeValues array, creating a new header item for each data item
					/// </summary>
					/// -----------------------------------------------------------------------------
					public void AddRangeHeaderData()
					{
						int cellsPerWeek = 1 + arrRangeValues.Count;
						if (!IncludeEndValue && ShowValueMarks)
						{
							cellsPerWeek = 1 + arrRangeValues.Count * 2;
						}
						
						int j = 0;
						for (j = 0; j <= GetRepetitionCount() - 1; j++)
						{
							int i = 0;
							for (i = 0; i <= arrRangeValues.Count - 1; i++)
							{
								object obj = arrRangeValues[i];
								
								int rangeValueIndex = i + 1 + j * cellsPerWeek;
								if (!IncludeEndValue && ShowValueMarks)
								{
									rangeValueIndex = i * 2 + 1 + j * cellsPerWeek;
								}
								
								CreateItem(rangeValueIndex, 0, ScheduleItemType.RangeHeader, true, obj, -1);
							}
						}
					}
					
					abstract public void AddTitleHeaderData();
					
					abstract public string GetSortOrder();
					
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// Add the actual items from the data source to the body of the table
					/// </summary>
					/// <param name="dvView">The data view that contains the data items</param>
					/// -----------------------------------------------------------------------------
					public void AddData(DataView dvView)
					{
						
						if (ReferenceEquals(ItemTemplate, null))
						{
							throw (new HttpException("The ItemTemplate is missing."));
						}
						
						if (ReferenceEquals(dvView, null))
						{
							return ;
						}
						
						string strSortOrder = GetSortOrder();
						if (!string.IsNullOrEmpty(strSortOrder))
						{
							dvView.Sort = strSortOrder;
						}
						
						// iterate DataSource creating a new item for each data item
						int prevTitleIndex = -1;
						int prevStartCellIndex = -1;
						
						int i = 0;
						// make sure the data is processed in the right order: from bottom right up to top left.
						for (i = dvView.Count - 1; i >= 0; i--)
						{
							
							DataRowView drv = dvView[i];
							// this dataRowView of data will be one entry in the schedule
							
							// Let's find out where it should be displayed
							// Instead of column or row numbers, we use titleIndex and valueIndex
							// These can be used in the same way, whether layout is horizontal or vertical
							// titleIndex is the row number in Horizontal mode and the column number in vertical mode
							// valueIndex is the row number in Vertical mode and the column number in Horizontal mode
							// Both start at 0
							object objTitleField = drv[GetTitleField()];
							int titleIndex = CalculateTitleIndex(objTitleField);
							if (titleIndex < 1)
							{
								break; // since titleIndex is descending, and this one is too low already, skip all the rest too
							}
							
							object objStartValue = drv[DataRangeStartField];
							object objEndValue = drv[DataRangeEndField];
							int startCellIndex = CalculateRangeCellIndex(objStartValue, objTitleField, false);
							int endCellIndex = CalculateRangeCellIndex(objEndValue, objTitleField, true);
							if (startCellIndex > -1 & endCellIndex > -1) // if not out of range
							{
								if (!IncludeEndValue)
								{
									endCellIndex--;
								}
								int Span = endCellIndex - startCellIndex + 1;
								if (Span == 0)
								{
									Span = 1;
								}
								
								int maxStartCellIndex = 0;
								if (Layout == LayoutEnum.Vertical)
								{
									maxStartCellIndex = Table1.Rows.Count - 1;
								}
								else // Horizontal
								{
									if (titleIndex >= Table1.Rows.Count)
									{
										// make sure nothing is added in this case
										maxStartCellIndex = -2;
									}
									else
									{
										maxStartCellIndex = System.Convert.ToInt32(Table1.Rows[titleIndex].Cells.Count - 1);
									}
								}
								if (startCellIndex > 0 && startCellIndex <= maxStartCellIndex)
								{
									
									int cellIndex = startCellIndex;
									if (titleIndex == prevTitleIndex && startCellIndex + Span > prevStartCellIndex)
									{
										// this cell is overlapping with the previous one (the one below or to the right)
										// prevStartValue is the starting cell index of the previous item
										// (in vertical layout the one below and in horizontal layout the one to the right)
										// this index is higher than this cell's index
										// (because the previous starting cell is under or to the right of this starting cell)
										// that's because we work from bottom right to top left
										// split the column or row that's overlapping so that we can show both contents
										// the last value to split is the end value of this item
										SplitTitle(titleIndex, startCellIndex, endCellIndex);
									}
									// create new content
									ScheduleItemType type = ScheduleItemType.Item;
									if (i % 2 == 1)
									{
										type = ScheduleItemType.AlternatingItem;
									}
									// use the index in _itemsArrayList as dataSetIndex
									if (Layout == LayoutEnum.Vertical || cellIndex <= System.Convert.ToInt32(Table1.Rows[titleIndex].Cells.Count - 1))
									{
										ScheduleItem Item = CreateItem(cellIndex, titleIndex, type, true, drv, _itemsArrayList.Count);
										MergeCells(cellIndex, titleIndex, Span);
										_itemsArrayList.Add(Item);
									}
									// save location for next item to compare with
									prevTitleIndex = titleIndex;
									prevStartCellIndex = startCellIndex;
								}
							}
						}
						
					}
					
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// Add range value marks to indicate the range values
					/// </summary>
					/// -----------------------------------------------------------------------------
					protected void AddRangeValueMarks()
					{
						if (Layout == LayoutEnum.Vertical)
						{
							// Insert an extra column for range marks after the first column
							
							int rowsPerWeek = 1 + arrRangeValues.Count * 2;
							int j = 0;
							for (j = 0; j <= GetRepetitionCount() - 1; j++)
							{
								// First insert a cell in the title row
								int rangeValueIndex = j * rowsPerWeek;
								Table1.Rows[rangeValueIndex].Cells.AddAt(1, new TableHeaderCell());
								TableCell tc1 = Table1.Rows[rangeValueIndex].Cells[1];
								tc1.Text = "&nbsp;";
								tc1.ApplyStyle(TitleStyle);
								
								int i = 0;
								for (i = 1; i <= rowsPerWeek - 1; i++)
								{
									rangeValueIndex = i + j * rowsPerWeek;
									
									Table1.Rows[rangeValueIndex].Cells.AddAt(1, new TableHeaderCell());
									TableCell tc = Table1.Rows[rangeValueIndex].Cells[1];
									tc.Text = "&nbsp;";
									TableCell tc0 = Table1.Rows[rangeValueIndex].Cells[0];
								}
								for (i = 1; i <= rowsPerWeek - 1; i += 2)
								{
									// each rangeheader spans 2 rows (over the rangeheader mark)
									// merge these cells over 2 rows in the first column
									rangeValueIndex = i + j * rowsPerWeek;
									MergeCells(rangeValueIndex, 0, 2);
								}
							}
						}
						else // Horizontal
						{
							TableRow tr0 = Table1.Rows[0];
							// Add new row for rangevalue marks
							Table1.Rows.AddAt(1, new TableRow());
							TableRow tr1 = Table1.Rows[1];
							
							// title column
							tr1.Cells.Add(new TableHeaderCell());
							TableCell tc1 = tr1.Cells[0];
							tc1.Text = "&nbsp;";
							tc1.ApplyStyle(TitleStyle);
							
							int nColumns = arrRangeValues.Count * 2 + 1;
							int i = 0;
							for (i = 1; i <= nColumns - 1; i++)
							{
								TableCell tc0 = tr0.Cells[i];
								tr1.Cells.Add(new TableHeaderCell());
								TableCell tc = tr1.Cells[i];
								tc.Text = "&nbsp;";
							}
							// iterate the cells in reverse order for merging
							for (i = nColumns - 2; i >= 1; i -= 2)
							{
								// each rangeheader spans 2 rows (over the rangeheader mark)
								// merge these cells over 2 columns in the first column
								MergeCells(i, 0, 2);
							}
						}
					}
					
					protected virtual string GetTitleField()
					{
						// overridden in ScheduleCalendar
						return TitleField;
					}
					
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// Calculate the range cell index in the table, given the objRangeValue and the objTitleValue
					/// The values refer to the real cell index in the table, taking into account whether cells are
					/// spanning over value marks (in horizontal mode)
					/// In vertical layout, the result is the real row index in the table
					/// In horizontal layout, the result is the real cell index in the row (before any merging
					/// of cells due to value spanning)
					/// </summary>
					/// <param name="objRangeValue">The range value from the data source</param>
					/// <param name="objTitleValue">The title value from the data source</param>
					/// <param name="isEndValue">False if we're calculating the range value index for the
					/// start of the item, True if it's the end</param>
					/// <returns>The range cell index</returns>
					/// -----------------------------------------------------------------------------
					protected abstract int CalculateRangeCellIndex(object objRangeValue, object objTitleValue, bool isEndValue);
					
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// Calculate the TitleIndex in the table, given the objTitleValue
					/// </summary>
					/// <param name="objTitleValue">The title value from the data source</param>
					/// <returns>The title index</returns>
					/// -----------------------------------------------------------------------------
					protected abstract int CalculateTitleIndex(object objTitleValue);
					
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// Merge cells starting at startCellIndex
					/// </summary>
					/// <param name="startCellIndex">Index of the first cell to merge.</param>
					/// <param name="titleIndex">Row or column containing the adjacent cells.</param>
					/// <param name="Span">Number of columns or rows that the newly merged cell should span.</param>
					/// <returns>The newly merged cell if successful. Nothing when merging fails.</returns>
					/// <remarks>
					/// In horizontal layout, merging may start at 0
					/// </remarks>
					/// -----------------------------------------------------------------------------
					public TableCell MergeCells(int startCellIndex, int titleIndex, int span)
					{
						if (titleIndex > GetTitleCount())
						{
							return null;
						}
						
						int minValueIndex = System.Convert.ToInt32(System.Convert.ToInt32(Layout == LayoutEnum.Horizontal ? 0 : 1));
						if (startCellIndex < minValueIndex)
						{
							return null;
						}
						if (span < 2)
						{
							return null;
						}
						
						int maxValueIndex = 0;
						if (Layout == LayoutEnum.Horizontal)
						{
							maxValueIndex = System.Convert.ToInt32(Table1.Rows[0].Cells.Count - 1);
						}
						else // Vertical
						{
							maxValueIndex = Table1.Rows.Count - 1;
						}
						
						if (startCellIndex > maxValueIndex)
						{
							return null;
						}
						
						if (startCellIndex + span - 1 > maxValueIndex)
						{
							span = maxValueIndex - startCellIndex + 1;
						}
						
						try
						{
							if (!IncludeEndValue && ShowValueMarks && Layout == LayoutEnum.Horizontal & titleIndex > 0)
							{
								// in this case every item spans 2 columns
								span = span * 2;
							}
							
							if (span <= GetValueSpan(startCellIndex, titleIndex))
							{
								return null;
							}
							RemoveCells(startCellIndex, startCellIndex + span, titleIndex);
							// change span property to extend the cell
							SetValueSpan(startCellIndex, titleIndex, span);
							return GetCell(startCellIndex, titleIndex);
						}
						catch (Exception ex)
						{
							if (!ReferenceEquals(HttpContext.Current, null))
							{
								Context.Trace.Warn(ex.Message);
							}
						}
						return null;
					}
					
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// Removes all the cells from startValueIndex to endValueIndex, except the first cell
					/// </summary>
					/// <param name="startCellIndex">Index of the first cell to remove.</param>
					/// <param name="endCellIndex">Index of the last cell to remove.</param>
					/// <param name="titleIndex">Row or column containing the adjacent cells.</param>
					/// -----------------------------------------------------------------------------
					private void RemoveCells(int startCellIndex, int endCellIndex, int titleIndex)
					{
						// When cells are merged in HTML, it is done by increasing the span of the first cell,
						// and removing the other cells.
						int prevSpan = GetValueSpan(startCellIndex, titleIndex);
						
						int valueCellIndex = startCellIndex + prevSpan;
						while (valueCellIndex < endCellIndex)
						{
							int nextCellIndex = 0;
							if (Layout == LayoutEnum.Vertical)
							{
								nextCellIndex = valueCellIndex;
							}
							else
							{
								nextCellIndex = startCellIndex + 1; // use +1, not +prevSpan
							}
							prevSpan = GetValueSpan(nextCellIndex, titleIndex);
							RemoveCell(nextCellIndex, titleIndex);
							valueCellIndex += prevSpan;
						}
					}
					
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// Removes the cell at cellValueIndex
					/// </summary>
					/// <param name="cellValueIndex">Cell value index of the cell to be removed.</param>
					/// <param name="titleIndex">Title index of the cell to be removed.</param>
					/// -----------------------------------------------------------------------------
					private void RemoveCell(int cellValueIndex, int titleIndex)
					{
						if (Layout == LayoutEnum.Vertical)
						{
							Table1.Rows[cellValueIndex].Cells.RemoveAt(titleIndex);
						}
						else // Horizontal
						{
							Table1.Rows[titleIndex].Cells.RemoveAt(cellValueIndex);
						}
					}
					
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// in vertical layout, get the column span. In horizontal layout, get the row span
					/// </summary>
					/// <param name="valueIndex">Value index of the cell</param>
					/// <param name="titleIndex">Title index of the cell</param>
					/// <returns>Integer value indicating the span width</returns>
					/// -----------------------------------------------------------------------------
					private int GetSplitSpan(int valueIndex, int titleIndex)
					{
						int returnValue = 0;
						if (Layout == LayoutEnum.Vertical)
						{
							returnValue = Table1.Rows[valueIndex].Cells[titleIndex].ColumnSpan;
						}
						else
						{
							returnValue = Table1.Rows[titleIndex].Cells[valueIndex].RowSpan;
						}
						if (returnValue == 0)
						{
							returnValue = 1;
						}
						return returnValue;
					}
					
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// Check if the cells at titleIndex are already spanning over multiple rows or columns
					/// for the given range. The span must be the same over that range.
					/// </summary>
					/// <param name="titleIndex">Title index of the range</param>
					/// <param name="firstCellIndex"></param>
					/// <param name="lastCellIndex"></param>
					/// <returns>True or false</returns>
					/// -----------------------------------------------------------------------------
					private bool IsRangeAlreadySpanning(int titleIndex, int firstCellIndex, int lastCellIndex)
					{
						int SplitSpan = GetSplitSpan(firstCellIndex, titleIndex);
						if (SplitSpan == 1)
						{
							return false;
						}
						if (Layout == LayoutEnum.Vertical) // v1.6.0.4 bugfix
						{
							int row = firstCellIndex + GetValueSpan(firstCellIndex, titleIndex);
							while (row <= lastCellIndex)
							{
								if (GetSplitSpan(row, titleIndex) != SplitSpan)
								{
									return false;
								}
								row = row + GetValueSpan(row, titleIndex);
							}
							// the last cell of the range must also end at startOfRange
							// if not, we can't split the range without adding a new column or row
							if (row > lastCellIndex + 1)
							{
								return false;
							}
						}
						else // Horizontal
						{
							int cellIndex = firstCellIndex + 1; // v1.6.0.3 bugfix
							for (cellIndex = firstCellIndex + 1; cellIndex <= lastCellIndex; cellIndex++)
							{
								if (GetSplitSpan(cellIndex, titleIndex) != SplitSpan)
								{
									return false;
								}
							}
						}
						return true;
					}
					
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// Split a column or row in 2 in order to allow the display of overlapping items.
					/// </summary>
					/// <param name="titleIndex">Title index of the cells to be split</param>
					/// <param name="firstCellIndexToSplit">Index of the first cell in the range to split</param>
					/// <param name="lastCellIndexToSplit">Index of the last cell in the range to split</param>
					/// -----------------------------------------------------------------------------
					private void SplitTitle(int titleIndex, int firstCellIndexToSplit, int lastCellIndexToSplit)
					{
						// To show overlapping items, we simply split the entire column or row in 2,
						// except when it was already split before.
						// Only the overlapping items are actually shown in 2 columns or rows, the other items
						// are shown sitting on both columns or rows.
						// This works well only when there are not yet any split columns or rows to the left of or above this one
						// If we work from right to left and bottom to top, this should be OK
						//
						// Because of the way tables work in HTML, the approach is different for each layout.
						// In vertical layout, we'll split the column by adding cells in several rows.
						// In horizontal layout, we'll add a new row, and add several cells to it.
						int cellsPerWeek = 1 + arrRangeValues.Count; // including one cell for title headers
						
						if (!IncludeEndValue && ShowValueMarks)
						{
							if (Layout == LayoutEnum.Horizontal)
							{
								// Extra column to allow the range headers to sit on the separation
								// When for example there are 4 values, we make 6 columns
								// 1 title column, 1 start column, 3 columns (each sitting between 2 values),
								// and an end column
								cellsPerWeek = 2 + arrRangeValues.Count;
							}
							else
							{
								// Each item takes 2 rows
								cellsPerWeek = 1 + arrRangeValues.Count * 2;
							}
						}
						
						int firstCellIndexInThisWeek = System.Convert.ToInt32((firstCellIndexToSplit / cellsPerWeek) * cellsPerWeek);
						int lastCellIndexInThisWeek = firstCellIndexInThisWeek + cellsPerWeek - 1;
						
						bool SplitEntireColumnOrRow = !IsRangeAlreadySpanning(titleIndex, firstCellIndexToSplit, lastCellIndexToSplit);
						
						TableRow newRow = null; // only used in horizontal layout if SplitEntireColumnOrRow=True
						if (SplitEntireColumnOrRow && Layout == LayoutEnum.Horizontal)
						{
							// insert a new row
							Table1.Rows.AddAt(titleIndex, new TableRow());
							newRow = Table1.Rows[titleIndex];
							titleIndex++;
						}
						
						if (Layout == LayoutEnum.Vertical)
						{
							// in vertical layout, realCellIndex is the same as valueIndex
							int row = firstCellIndexInThisWeek;
							while (row <= lastCellIndexInThisWeek)
							{
								// first, get the span of the original cell
								int originalCellSpan = GetValueSpan(row, titleIndex);
								
								if (row >= firstCellIndexToSplit && row <= lastCellIndexToSplit)
								{
									// split the cell in this row by inserting a new cell
									ClickableTableCell ctc0 = new ClickableTableCell(row, titleIndex);
									Table1.Rows[row].Cells.AddAt(titleIndex, ctc0);
									TableCell tc = Table1.Rows[row].Cells[titleIndex];
									tc.RowSpan = originalCellSpan;
									if (!SplitEntireColumnOrRow)
									{
										// no extra column was added, only an extra cell
										// therefore: reduce the columnspan of the original cell
										TableCell tcOriginal = Table1.Rows[row].Cells[titleIndex + 1];
										tc.ColumnSpan = tcOriginal.ColumnSpan - 1;
										tcOriginal.ColumnSpan = 1;
									}
									else // SplitEntireColumnOrRow
									{
										if (row + originalCellSpan - 1 > lastCellIndexToSplit)
										{
											// previous item went below this one, so split the remainder too
											// this is done by inserting another empty cell
											tc.RowSpan = lastCellIndexToSplit - row + 1;
											// split the remainder as well
											ClickableTableCell ctc = new ClickableTableCell(lastCellIndexToSplit + 1, titleIndex);
											Table1.Rows[lastCellIndexToSplit + 1].Cells.AddAt(titleIndex, ctc);
											TableCell tc2 = Table1.Rows[lastCellIndexToSplit + 1].Cells[titleIndex];
											tc2.ApplyStyle(BackgroundStyle);
											tc2.Text = "&nbsp;";
											tc2.RowSpan = row + originalCellSpan - 1 - lastCellIndexToSplit;
										}
									}
								}
								else // this cell should not be split
								{
									if (SplitEntireColumnOrRow)
									{
										// we have added an extra column, but this cell should not be split,
										// it should only be spread over the extra column
										TableCell tc = Table1.Rows[row].Cells[titleIndex];
										int ColumnSpan = tc.ColumnSpan;
										if (ColumnSpan == 0)
										{
											ColumnSpan = 1;
										}
										tc.ColumnSpan = ColumnSpan + 1;
									}
								}
								row = row + originalCellSpan; // skip as many rows as this span
							}
						}
						else // Horizontal
						{
							// in horizontal layout, valueIndex is still the index referring to the list of values
							//   realCellIndex indicates the index (offset) of the cell in the TableRow
							//   this is not the same, when a cell has a horizontal span of 2, the next cell will have
							//   a realCellIndex that is 1 higher, but a valueIndex that is 2 higher.
							int valueIndex = firstCellIndexInThisWeek;
							int realCellIndex = valueIndex;
							while (valueIndex <= lastCellIndexInThisWeek)
							{
								// first, get the span of the original cell
								int originalCellSpan = GetValueSpan(realCellIndex, titleIndex);
								int originalValueSpan = originalCellSpan;
								if (!IncludeEndValue && ShowValueMarks)
								{
									// in this case a normal item already has a span of 2, but it's only 1 cell
									originalValueSpan = originalCellSpan / 2;
									if (originalValueSpan == 0)
									{
										originalValueSpan = 1;
									}
								}
								
								if (valueIndex >= firstCellIndexToSplit && valueIndex <= lastCellIndexToSplit)
								{
									if (!SplitEntireColumnOrRow)
									{
										// the current cell is already spanning several rows
										// there is no need to add another row, just split the current cell
										// over the rows that it's already occupying.
										TableRow thisRow = Table1.Rows[titleIndex];
										int SplitSpan = thisRow.Cells[valueIndex].RowSpan;
										TableRow lastRowInSpan = Table1.Rows[titleIndex + SplitSpan - 1];
										// move the current cell down to the last row of the rows that are being spanned
										// it should become the first cell in that row
										lastRowInSpan.Cells.AddAt(0, thisRow.Cells[realCellIndex]);
										lastRowInSpan.Cells[0].RowSpan = 1;
										// add a new cell in this row
										ClickableTableCell ctc = new ClickableTableCell(titleIndex, valueIndex);
										thisRow.Cells.AddAt(realCellIndex, ctc);
										thisRow.Cells[realCellIndex].ColumnSpan = originalCellSpan;
										thisRow.Cells[realCellIndex].RowSpan = SplitSpan - 1;
									}
									else // SplitEntireColumnOrRow
									{
										// split the cell in this column by inserting a new cell
										ClickableTableCell ctc0 = new ClickableTableCell(titleIndex - 1, valueIndex);
										newRow.Cells.Add(ctc0);
										TableCell tc = newRow.Cells[newRow.Cells.Count - 1];
										tc.ColumnSpan = originalCellSpan;
										if (valueIndex + originalValueSpan - 1 > lastCellIndexToSplit)
										{
											// previous item ended further to the right than this one, so split the remainder too
											// this is done by inserting another empty cell
											int newValueSpan = lastCellIndexToSplit - valueIndex + 1;
											if (!IncludeEndValue && ShowValueMarks)
											{
												// in this case a normal item has a span of 2
												newValueSpan *= 2;
											}
											
											tc.ColumnSpan = newValueSpan;
											// split the remainder as well
											ClickableTableCell ctc = new ClickableTableCell(titleIndex - 1, valueIndex + originalValueSpan - 1);
											newRow.Cells.Add(ctc);
											TableCell tc2 = newRow.Cells[newRow.Cells.Count - 1];
											int newValueSpan2 = valueIndex + originalValueSpan - 1 - lastCellIndexToSplit;
											if (!IncludeEndValue && ShowValueMarks)
											{
												// in this case a normal item has a span of 2
												newValueSpan2 *= 2;
											}
											tc2.ColumnSpan = newValueSpan2;
											tc2.ApplyStyle(BackgroundStyle);
											tc2.Text = "&nbsp;";
										}
									}
								}
								else // this cell should not be split
								{
									if (SplitEntireColumnOrRow)
									{
										// we have added an extra row, but this cell should not be split,
										// it should only be spread over the extra row
										// move this cell up one row to the new row, and spread it over more rows
										newRow.Cells.Add(Table1.Rows[titleIndex].Cells[realCellIndex]);
										realCellIndex--; // decrease cellindex, because one cell has been removed
										// the cell was moved, and it is currently the last cell of newRow
										TableCell tc2 = newRow.Cells[newRow.Cells.Count - 1];
										int RowSpan = tc2.RowSpan; // get its span
										if (RowSpan == 0)
										{
											RowSpan = 1;
										}
										tc2.RowSpan = RowSpan + 1; // increase it
									}
								}
								valueIndex = valueIndex + originalValueSpan; // for the index, skip as many rows/columns as this span
								realCellIndex++; // if horizontal, just take the next cell
							}
						}
						
					}
					
				
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// Get the DataView from the DataSource and DataMember properties
					/// </summary>
					/// <returns></returns>
					/// -----------------------------------------------------------------------------
					public DataView GetDataView()
					{
						DataView returnValue = default(DataView);
						returnValue = null;
						
						if (!ReferenceEquals(this.DataSource, null))
						{
							// determine if the datasource is a DataSet, DataTable or DataView
							if (this.DataSource is DataView)
							{
								returnValue = (DataView) this.DataSource;
							}
							else if (this.DataSource is DataTable)
							{
								returnValue = new DataView((DataTable) this.DataSource);
							}
							else if (this.DataSource is DataSet)
							{
								DataSet ds = (DataSet) this.DataSource;
								if (ReferenceEquals(this.DataMember, null) || this.DataMember == "")
								{
									// if data member isn't supplied, default to the first table
									returnValue = new DataView(ds.Tables[0]);
								}
								else
								{
									returnValue = new DataView(ds.Tables[this.DataMember]); // if data member is supplied, use it
								}
							}
						}
						// throw an exception if there is a problem with the data source
						if (ReferenceEquals(returnValue, null))
						{
							throw (new HttpException("Error finding the DataSource. " + "Please check the DataSource and DataMember properties."));
						}
						return returnValue;
					}
					
					// Override to get the corresponding templates
					protected abstract ITemplate GetTemplate(ScheduleItemType type);
					
					protected virtual TableItemStyle GetStyle(ScheduleItemType type)
					{
						TableItemStyle returnValue = default(TableItemStyle);
						// Override to add additional styles (such as the style for DateHeaders in ScheduleGeneral)
						returnValue = null;
						switch (type)
						{
							case ScheduleItemType.RangeHeader:
								return RangeHeaderStyle;
							case ScheduleItemType.TitleHeader:
								return TitleStyle;
							case ScheduleItemType.Item:
								return ItemStyle;
							case ScheduleItemType.AlternatingItem:
								returnValue = new TableItemStyle();
								if (!ReferenceEquals(_itemStyle, null))
								{
									returnValue.MergeWith(ItemStyle);
								}
								if (!ReferenceEquals(_alternatingItemStyle, null))
								{
									returnValue.CopyFrom(AlternatingItemStyle);
								}
								break;
						}
						return returnValue;
					}
					
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// Instantiate item using the corresponding template (if any)
					/// </summary>
					/// <param name="item">ScheduleItem to instantiate</param>
					/// <param name="cell">corresponding table cell</param>
					/// -----------------------------------------------------------------------------
					private void InstantiateItem(ScheduleItem item, TableCell cell)
					{
						ITemplate template = GetTemplate(item.ItemType);
						if (!ReferenceEquals(template, null))
						{
							template.InstantiateIn(item); // initialize item from template
						}
						else
						{
							if (item.ItemType == ScheduleItemType.Item | item.ItemType == ScheduleItemType.AlternatingItem)
							{
								// this exception should never fire: another should already have fired in AddData()
								throw (new HttpException("The ItemTemplate is missing"));
							}
							// no template provided, just show data item
							object value = item.DataItem; // for header items, DataItem is just a String or a Date
							Literal lit = new Literal();
							if (!ReferenceEquals(value, null))
							{
								lit.Text = FormatDataValue(value, item.ItemType);
							}
							else
							{
								// On postback, viewstate should keep the contents, but it fails to do so ***
								// The headers stay blank on postback if no template is provided.
								// For now, to make it work on postback too, make sure you provide all the templates
								lit.Text = "&nbsp;";
							}
							item.Controls.Add(lit);
						}
						TableItemStyle myStyle = GetStyle(item.ItemType);
						if (!ReferenceEquals(myStyle, null))
						{
							cell.ApplyStyle(myStyle);
						}
						
					}
					
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// when there's no template, try to present the data in a reasonable format
					/// </summary>
					/// <param name="value">Value of the item</param>
					/// <param name="type">Type of the item</param>
					/// <returns>A formatted string</returns>
					/// -----------------------------------------------------------------------------
					protected abstract string FormatDataValue(object value, ScheduleItemType type);
					
					/// <summary>
					/// Create the item
					/// </summary>
					/// <param name="cellValueIndex">Cell value index of the item</param>
					/// <param name="titleIndex">Title index of the item</param>
					/// <param name="itemType">Type of the item</param>
					/// <param name="dataBind">Whether databinding should be performed</param>
					/// <param name="dataItem">Data item of the item</param>
					/// <param name="dataSetIndex">Index of the item in the dataset</param>
					/// <returns>The newly created ScheduleItem</returns>
					/// -----------------------------------------------------------------------------
					protected ScheduleItem CreateItem(int cellValueIndex, int titleIndex, ScheduleItemType itemType, bool dataBind, object dataItem, int dataSetIndex)
					{
						
						ScheduleItem item = new ScheduleItem(dataSetIndex, itemType);
						if (dataBind)
						{
							item.DataItem = dataItem;
						}
						TableCell cell = GetCell(cellValueIndex, titleIndex);
						InstantiateItem(item, cell);
						
						if (item.ItemType == ScheduleItemType.Item | item.ItemType == ScheduleItemType.AlternatingItem)
						{
							if (ItemStyleField != "" && !ReferenceEquals(dataItem, null))
							{
								DataRowView drv = (DataRowView) dataItem;
								object objItemStyle = drv[ItemStyleField];
								if (!Information.IsDBNull(objItemStyle))
								{
									TableItemStyle style = new TableItemStyle();
									style.CssClass = Convert.ToString(objItemStyle);
									cell.ApplyStyle(style);
								}
							}
						}
						
						ScheduleItemEventArgs e = new ScheduleItemEventArgs(item);
						OnItemCreated(e);
						GetCell(cellValueIndex, titleIndex).Controls.Add(item);
						
						if (dataBind)
						{
							item.DataBind();
							OnItemDataBound(e);
							//item.DataItem = Nothing
						}
						return item;
					}
					
					protected abstract int GetTitleCount();
					
					virtual public int GetRangeHeaderIndex()
					{
						// overridden in ScheduleGeneral when using Date Headers
						return 0;
					}
					
					private int GetMinTitleIndex()
					{
						if (!IncludeEndValue && ShowValueMarks)
						{
							return GetRangeHeaderIndex() + 2;
						}
						else
						{
							return GetRangeHeaderIndex() + 1;
						}
					}
					
					private int GetMaxTitleIndex()
					{
						return GetMinTitleIndex() + GetTitleCount() - 1;
					}
					
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// In vertical layout, get the row span. In horizontal layout, get the column span
					/// </summary>
					/// <param name="cellValueIndex">Cell value index of the item</param>
					/// <param name="titleIndex">Title index of the item</param>
					/// <returns>The integer value of the span</returns>
					/// -----------------------------------------------------------------------------
					protected int GetValueSpan(int cellValueIndex, int titleIndex)
					{
						int returnValue = 0;
						if (Layout == LayoutEnum.Vertical)
						{
							returnValue = Table1.Rows[cellValueIndex].Cells[titleIndex].RowSpan;
						}
						else
						{
							returnValue = Table1.Rows[titleIndex].Cells[cellValueIndex].ColumnSpan;
						}
						if (returnValue == 0)
						{
							returnValue = 1;
						}
						return returnValue;
					}
					
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// in vertical layout, set the row span. In horizontal layout, set the column span
					/// </summary>
					/// <param name="cellValueIndex">Cell value index of the item</param>
					/// <param name="titleIndex">Title index of the item</param>
					/// <param name="span">The new span value</param>
					/// -----------------------------------------------------------------------------
					private void SetValueSpan(int cellValueIndex, int titleIndex, int span)
					{
						if (Layout == LayoutEnum.Vertical)
						{
							Table1.Rows[cellValueIndex].Cells[titleIndex].RowSpan = span;
						}
						else
						{
							Table1.Rows[titleIndex].Cells[cellValueIndex].ColumnSpan = span;
						}
					}
					
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// Get the cell at the given indexes.
					/// </summary>
					/// <param name="cellValueIndex">Cell value index of the item</param>
					/// <param name="titleIndex">Title index of the item</param>
					/// <returns>The TableCell object of the corresponding cell</returns>
					/// -----------------------------------------------------------------------------
					private TableCell GetCell(int cellValueIndex, int titleIndex)
					{
						TableCell returnValue = default(TableCell);
						if (Layout == LayoutEnum.Vertical)
						{
							returnValue = Table1.Rows[cellValueIndex].Cells[titleIndex];
						}
						else
						{
							returnValue = Table1.Rows[titleIndex].Cells[cellValueIndex];
						}
						return returnValue;
					}
					
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// Show the EmptyDataTemplate when the data source is empty
					/// </summary>
					/// -----------------------------------------------------------------------------
					private void RenderEmptyDataTemplate()
					{
						if (!ReferenceEquals(EmptyDataTemplate, null))
						{
							PlaceHolder plh = new PlaceHolder();
							EmptyDataTemplate.InstantiateIn(plh); // initialize from template
							Controls.Add(plh);
							ViewState["Empty"] = true; // raize a flag
						}
					}
					
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// Handle events raised by children by overriding OnBubbleEvent.
					/// </summary>
					/// <param name="source">the source of the event</param>
					/// <param name="e">event data</param>
					/// <returns>true if the event has been handled</returns>
					/// -----------------------------------------------------------------------------
					protected override bool OnBubbleEvent(object source, EventArgs e)
					{
						if (e is ScheduleCommandEventArgs)
						{
							ScheduleCommandEventArgs ce = (ScheduleCommandEventArgs) e;
							OnItemCommand(ce);
							string cmdName = ce.CommandName.ToLower();

						}
						return true;
					}
					
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// RaisePostBackEvent is called when the user clicks on an empty slot
					/// (see clickabletablecell.vb)
					/// </summary>
					/// <param name="eventArgument">String with the event argument.
					/// Contains something like "3-7", where 3 is the row and 7 the column </param>
					/// -----------------------------------------------------------------------------
					public void RaisePostBackEvent(string eventArgument)
					{
						// get column and row number from eventArgument
						string[] args = eventArgument.Split('-');
						int row = int.Parse(args[0]);
						int column = int.Parse(args[1]);
						object Title = null;
						object RangeStartValue = null;
						object RangeEndValue = null;
						int RangeStartValueIndex = CalculateRangeValueIndex(row, column);
						if (Layout == LayoutEnum.Horizontal)
						{
							Title = CalculateTitle(row, column);
						}
						else
						{
							Title = CalculateTitle(column, row);
						}
						RangeStartValue = arrRangeValues[RangeStartValueIndex]; //CalculateRangeValue(row - 1)
						int RangeEndValueIndex = RangeStartValueIndex + 1;
						if (RangeEndValueIndex >= arrRangeValues.Count)
						{
							RangeEndValueIndex = arrRangeValues.Count - 1;
						}
						RangeEndValue = arrRangeValues[RangeEndValueIndex]; //CalculateRangeValue(endRow)

						ClickableTableCellEventArgs ctcea = new ClickableTableCellEventArgs(Title, RangeStartValue, RangeEndValue);
						OnEmptySlotClick(ctcea);
					}
					
					///' -----------------------------------------------------------------------------
					///' <summary>
					///' Calculate the title value given the cell index
					///' </summary>
					///' <param name="titleIndex">Title index of the cell</param>
					///' <returns>Object containing the title</returns>
					///' -----------------------------------------------------------------------------
					public abstract dynamic CalculateTitle(int titleIndex, int cellIndex);
					
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// Calculate the range value index given the cell index
					/// </summary>
					/// <param name="row">Row of the cell</param>
					/// <param name="column">Column of the cell</param>
					/// <returns>Integer containing the range value index</returns>
					/// -----------------------------------------------------------------------------
					public int CalculateRangeValueIndex(int row, int column)
					{
						if (Layout == LayoutEnum.Horizontal)
						{
							return column - 1 - System.Convert.ToInt32(!IncludeEndValue && ShowValueMarks ? 1 : 0);
						}
						else
						{
							int rowsPerWeek = CalculateRowsPerRepetition();
							int RangeValueIndex = 0;
							if (!IncludeEndValue && ShowValueMarks)
							{
								RangeValueIndex = System.Convert.ToInt32((row % rowsPerWeek) / 2 - 1);
							}
							else
							{
								RangeValueIndex = System.Convert.ToInt32((row % rowsPerWeek) - 1);
							}
							if (RangeValueIndex < 0)
							{
								RangeValueIndex = 0;
							}
							return RangeValueIndex;
						}
					}
					
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// Calculate the number of rows per repetition
					/// </summary>
					/// <returns>Integer containing the number of rows</returns>
					/// -----------------------------------------------------------------------------
					public int CalculateRowsPerRepetition()
					{
						int returnValue = 0;
						if (!IncludeEndValue && ShowValueMarks)
						{
							returnValue = arrRangeValues.Count * 2 + 1;
						}
						else
						{
							returnValue = arrRangeValues.Count + 1;
						}
						return returnValue;
					}
					
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// Save information for use in round trips (enough to re-create the control tree).
					/// </summary>
					/// -----------------------------------------------------------------------------
					private void SaveControlTree()
					{
						if (Items.Count == 0)
						{
							return ;
						}
						int[] arrCellCount = new int[Table1.Rows.Count + 1]; // number of cells in each row
						int[] arrHeaderCount = new int[Table1.Rows.Count + 1]; // number of header cells in each row
						int[] arrItemCols = new int[Items.Count + 1];
						int[] arrItemRows = new int[Items.Count + 1];

						int nCells = Table1.Rows.Count * Table1.Rows[0].Cells.Count;
						if (Table1.Rows.Count > 0 && Table1.Rows[1].Cells.Count > Table1.Rows[0].Cells.Count)
						{
							nCells = Table1.Rows.Count * Table1.Rows[1].Cells.Count;
						}
						int[] arrClickRows = new int[nCells + 1]; // row of each clickable cell
						int[] arrClickColumns = new int[nCells + 1]; // column of each clickable cell
						
						int row = 0;
						int col = 0;
						int ClickableCellCount = 0;
						for (row = 0; row <= Table1.Rows.Count - 1; row++)
						{
							int cellsInThisRow = Table1.Rows[row].Cells.Count;
							arrCellCount[row] = cellsInThisRow;
							for (col = 0; col <= cellsInThisRow - 1; col++)
							{
								if (Table1.Rows[row].Cells[col] is TableHeaderCell)
								{
									arrHeaderCount[row] = col + 1; // will continue to increase until normal cells start
								}
								if (EnableEmptySlotClick && Table1.Rows[row].Cells[col] is ClickableTableCell)
								{
									ClickableTableCell ccell = (ClickableTableCell) (Table1.Rows[row].Cells[col]);
									arrClickColumns[ClickableCellCount] = ccell.Column;
									arrClickRows[ClickableCellCount] = ccell.Row;
									ClickableCellCount++;
								}
								TableCell cell = Table1.Rows[row].Cells[col];
								if (cell.HasControls())
								{
									if (cell.Controls[0] is ScheduleItem)
									{
										ScheduleItem item = (ScheduleItem) (cell.Controls[0]);
										int dataSetIndex = item.DataSetIndex;
										if (dataSetIndex >= 0) // body item
										{
											arrItemRows[dataSetIndex] = row;
											arrItemCols[dataSetIndex] = col;
										}
									}
								}
							}
						}
						ViewState["RowCount"] = Table1.Rows.Count; // number of rows
						ViewState["ItemCount"] = Items.Count; // number of items in datasource
						ViewState["arrCellCount"] = arrCellCount; // number of cells in each row
						ViewState["arrHeaderCount"] = arrHeaderCount; // number of header cells in each row
						ViewState["arrItemCols"] = arrItemCols; // column index of each item
						ViewState["arrItemRows"] = arrItemRows; // row index of each item

						if (EnableEmptySlotClick)
						{
							Array.Resize(ref arrClickRows, ClickableCellCount + 1); // reduce viewstate
							ViewState["arrClickRows"] = arrClickRows; // row of each clickable cell
							Array.Resize(ref arrClickColumns, ClickableCellCount + 1);
							ViewState["arrClickColumns"] = arrClickColumns; // column of each clickable cell
						}
					}
					
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// Recreate the control tree from viewstate
					/// </summary>
					/// -----------------------------------------------------------------------------
					private void LoadControlTree()
					{
						Controls.Clear();
						if (System.Convert.ToBoolean(ViewState["Empty"]) == true)
						{
							// empty control: use EmptyDataTemplate
							PlaceHolder plh = new PlaceHolder();
							EmptyDataTemplate.InstantiateIn(plh);
							Controls.Add(plh);
							return ;
						}
						// When the control is empty, and EmptyDataTemplate does not exist:
						if (ReferenceEquals(ViewState["RowCount"], null))
						{
							return ;
						}
						int RowCount = System.Convert.ToInt32(ViewState["RowCount"]);
						int ItemCount = System.Convert.ToInt32(ViewState["ItemCount"]);
						int[] arrCellCount = (int[]) (ViewState["arrCellCount"]); // number of cells in each row
						int[] arrHeaderCount = (int[]) (ViewState["arrHeaderCount"]); // number of row header cells in each row
						int[] arrItemCols = (int[]) (ViewState["arrItemCols"]); // column index of each data item
						int[] arrItemRows = (int[]) (ViewState["arrItemRows"]); // row index of each data item
						int[] arrClickRows = null;
						int[] arrClickColumns = null;
						if (EnableEmptySlotClick)
						{
							arrClickRows = (int[]) (ViewState["arrClickRows"]); // row of each clickable cell
							arrClickColumns = (int[]) (ViewState["arrClickColumns"]); // column of each clickable cell
						}
						// clear any existing child controls
						Table1 = new Table();
						Controls.Add(Table1);
						
						int week = 0;
						int row = 0;
						int col = 0;
						int ClickableCellCount = 0;
						int rowsPerRepetition = RowCount / GetRepetitionCount(); // Only used in vertical layout
						
						// first recreate the table
						for (row = 0; row <= RowCount - 1; row++)
						{
							Table1.Rows.Add(new TableRow());
							TableRow tr = Table1.Rows[Table1.Rows.Count - 1];
							for (col = 0; col <= arrCellCount[row] - 1; col++)
							{
								if (col < arrHeaderCount[row])
								{
									// create row header cells for this week
									tr.Cells.Add(new TableHeaderCell());
								}
								else
								{
									if (EnableEmptySlotClick)
									{
										bool isClickableCell = true;
										if (ShowValueMarks && !IncludeEndValue)
										{
											if (Layout == LayoutEnum.Horizontal & arrHeaderCount[row] > 0)
											{
												// first and last columns should not be clickable
												// when the title is split over several rows, only check the first row
												if (col == arrHeaderCount[row])
												{
													isClickableCell = false;
												}
												if (col == arrCellCount[row] - 1)
												{
													isClickableCell = false;
												}
											}
											else // Vertical
											{
												int rowInThisRepetition = row % rowsPerRepetition;
												if (rowInThisRepetition == 1)
												{
													isClickableCell = false;
												}
												if (rowInThisRepetition == rowsPerRepetition - 1)
												{
													isClickableCell = false;
												}
											}
										}
										if (isClickableCell)
										{
											// col may differ from the real column because of cells
											// spanning several rows
											// We need the real column here for the EmptySlotClick to work
											// It's too complicated to calculate, therefore, we use ViewState
											int fixedColumn = arrClickColumns[ClickableCellCount];
											int fixedRow = arrClickRows[ClickableCellCount];
											tr.Cells.Add(new ClickableTableCell(fixedRow, fixedColumn));
											ClickableCellCount++;
										}
										else
										{
											tr.Cells.Add(new TableCell());
										}
									}
									else
									{
										tr.Cells.Add(new TableCell());
									}
								}
							}
						}
						
						// now add the items
						// it's imperative that we do it in the same order as before the postback
						// 1) RangeHeader items
						// 2) TitleHeader items
						// 3) Normal Items
						// 4) DateHeader items (if any)
						//
						// add RangeHeader items
						if (Layout == LayoutEnum.Horizontal)
						{
							// create range header items in the first or second row
							int rangeHeaderRow = GetRangeHeaderIndex();
							for (col = 1; col <= arrCellCount[rangeHeaderRow] - 1; col++)
							{
								CreateItem(col, rangeHeaderRow, ScheduleItemType.RangeHeader, false, null, -1);
							}
						}
						else // Layout = LayoutEnum.Vertical
						{
							for (week = 0; week <= GetRepetitionCount() - 1; week++)
							{
								// create range header items of this repetition in the first or second column
								int minRangeHeaderRow = week * rowsPerRepetition + 1;
								int maxRangeHeaderRow = week * rowsPerRepetition + rowsPerRepetition - 1;
								int iStep = System.Convert.ToInt32(!IncludeEndValue && ShowValueMarks ? 2 : 1);
								
								for (row = minRangeHeaderRow; row <= maxRangeHeaderRow; row += iStep)
								{
									col = arrHeaderCount[row] - 1; // the range header column is the right-most column of the header columns
									if (!IncludeEndValue && ShowValueMarks)
									{
										// in this case, the range header column is the 2nd to the right of the header columns
										col--;
									}
									
									if (col >= 0)
									{
										CreateItem(row, col, ScheduleItemType.RangeHeader, false, null, -1);
									}
								}
							}
						}
						
						// add Title Header items
						if (Layout == LayoutEnum.Vertical)
						{
							for (col = GetMinTitleIndex(); col <= arrCellCount[0] - 1; col++)
							{
								for (week = 0; week <= GetRepetitionCount() - 1; week++)
								{
									// create title header items in the first row of the week
									int titleHeaderRow = week * rowsPerRepetition;
									CreateItem(titleHeaderRow, col, ScheduleItemType.TitleHeader, false, null, -1);
								}
							}
						}
						else // Layout = LayoutEnum.Horizontal
						{
							// create title header items in the first column
							for (row = GetMinTitleIndex(); row <= RowCount - 1; row++)
							{
								// titles may be merged over rows. Only the first row of a title contains the item
								// So check if there is a title in this row
								if (arrHeaderCount[row] == 1)
								{
									CreateItem(0, row, ScheduleItemType.TitleHeader, false, null, -1);
								}
							}
							
						}
						
						// add the (non-header) items
						int i = 0;
						for (i = 0; i <= ItemCount - 1; i++)
						{
							// reconstruct the items
							row = arrItemRows[i];
							col = arrItemCols[i];
							ScheduleItem item = default(ScheduleItem);
							if (i % 2 == 1)
							{
								if (Layout == LayoutEnum.Vertical)
								{
									item = CreateItem(row, col, ScheduleItemType.AlternatingItem, false, null, -1);
								}
								else
								{
									item = CreateItem(col, row, ScheduleItemType.AlternatingItem, false, null, -1);
								}
							}
							else
							{
								if (Layout == LayoutEnum.Vertical)
								{
									item = CreateItem(row, col, ScheduleItemType.Item, false, null, -1);
								}
								else
								{
									item = CreateItem(col, row, ScheduleItemType.Item, false, null, -1);
								}
							}
						}
						
						if (GetRangeHeaderIndex() == 1)
						{
							// add DateHeader items
							if (Layout == LayoutEnum.Horizontal)
							{
								// first row is the date header row
								for (col = 1; col <= arrCellCount[0] - 1; col++)
								{
									CreateItem(col, 0, ScheduleItemType.RangeHeader, false, null, -1);
								}
							}
							else // Layout = LayoutEnum.Vertical
							{
								for (week = 0; week <= GetRepetitionCount() - 1; week++)
								{
									// create date header items in the first column
									int minDateHeaderRow = week * rowsPerRepetition + 1;
									int maxDateHeaderRow = week * rowsPerRepetition + rowsPerRepetition - 1;
									for (row = minDateHeaderRow; row <= maxDateHeaderRow; row++)
									{
										// date headers may be merged over columns.
										// Only the first column of a date header contains the item
										// So check if there is a title in this column
										int headersInDateHeaderRow = 2;
										if (!IncludeEndValue && ShowValueMarks)
										{
											headersInDateHeaderRow = 3;
										}
										
										if (arrHeaderCount[row] == headersInDateHeaderRow)
										{
											// first column contains a date header
											CreateItem(row, 0, ScheduleItemType.RangeHeader, false, null, -1);
										}
									}
								}
							}
						}
						
					}
					
					protected override Style CreateControlStyle()
					{
						// Since the Schedule control renders an HTML table,
						// an instance of the TableStyle class is used as the control style.
						TableStyle mystyle = new TableStyle(ViewState);
						// Set up default initial state.
						//mystyle.CellSpacing = 0
						mystyle.CellPadding = -1;
						mystyle.GridLines = System.Web.UI.WebControls.GridLines.None;
						return mystyle;
					}
					

					/// -----------------------------------------------------------------------------
					/// <summary>
					/// Customized state management to handle saving state of contained objects.
					/// </summary>
					/// <param name="savedState"></param>
					/// -----------------------------------------------------------------------------
					protected override void LoadViewState(object savedState)
					{
						if (!(ReferenceEquals(savedState, null)))
						{
							object[] myState = (object[]) savedState;
							
							if (!(ReferenceEquals(myState[0], null)))
							{
								base.LoadViewState(myState[0]);
							}
							if (!(ReferenceEquals(myState[1], null)))
							{
								((IStateManager) ItemStyle).LoadViewState(myState[1]);
							}
							if (!(ReferenceEquals(myState[2], null)))
							{
								((IStateManager) AlternatingItemStyle).LoadViewState(myState[2]);
							}
							if (!(ReferenceEquals(myState[3], null)))
							{
								((IStateManager) RangeHeaderStyle).LoadViewState(myState[3]);
							}
							if (!(ReferenceEquals(myState[4], null)))
							{
								((IStateManager) TitleStyle).LoadViewState(myState[4]);
							}
							if (!(ReferenceEquals(myState[5], null)))
							{
								((IStateManager) BackgroundStyle).LoadViewState(myState[5]);
							}
						}
					}
					
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// Customize state management to handle saving state of contained objects such as styles.
					/// </summary>
					/// <returns></returns>
					/// -----------------------------------------------------------------------------
					protected override dynamic SaveViewState()
					{
						object baseState = base.SaveViewState();
						object itemStyleState = null;
						object alternatingItemStyleState = null;
						object RangeHeaderStyleState = null;
						object TitleStyleState = null;
						object BackgroundStyleState = null;
						
						if (!(ReferenceEquals(_itemStyle, null)))
						{
							itemStyleState = ((IStateManager) _itemStyle).SaveViewState();
						}
						else
						{
							itemStyleState = null;
						}
						if (!(ReferenceEquals(_alternatingItemStyle, null)))
						{
							alternatingItemStyleState = ((IStateManager) _alternatingItemStyle).SaveViewState();
						}
						else
						{
							alternatingItemStyleState = null;
						}
						if (!(ReferenceEquals(_RangeHeaderStyle, null)))
						{
							RangeHeaderStyleState = ((IStateManager) _RangeHeaderStyle).SaveViewState();
						}
						else
						{
							RangeHeaderStyleState = null;
						}
						if (!(ReferenceEquals(_TitleStyle, null)))
						{
							TitleStyleState = ((IStateManager) _TitleStyle).SaveViewState();
						}
						else
						{
							TitleStyleState = null;
						}
						if (!(ReferenceEquals(_BackgroundStyle, null)))
						{
							BackgroundStyleState = ((IStateManager) _BackgroundStyle).SaveViewState();
						}
						else
						{
							BackgroundStyleState = null;
						}
						
						object[] myState = new object[7];
						myState[0] = baseState;
						myState[1] = itemStyleState;
						myState[2] = alternatingItemStyleState;
						myState[3] = RangeHeaderStyleState;
						myState[4] = TitleStyleState;
						myState[5] = BackgroundStyleState;
						return myState;
					}
					
					/// -----------------------------------------------------------------------------
					/// <summary>
					/// Customize state management to handle saving state of contained objects such as styles.
					/// </summary>
					/// -----------------------------------------------------------------------------
					protected override void TrackViewState()
					{
						base.TrackViewState();
						
						if (!(ReferenceEquals(_itemStyle, null)))
						{
							((IStateManager) _itemStyle).TrackViewState();
						}
						if (!(ReferenceEquals(_alternatingItemStyle, null)))
						{
							((IStateManager) _alternatingItemStyle).TrackViewState();
						}
						if (!(ReferenceEquals(_RangeHeaderStyle, null)))
						{
							((IStateManager) _RangeHeaderStyle).TrackViewState();
						}
						if (!(ReferenceEquals(_TitleStyle, null)))
						{
							((IStateManager) _TitleStyle).TrackViewState();
						}
						if (!(ReferenceEquals(_BackgroundStyle, null)))
						{
							((IStateManager) _BackgroundStyle).TrackViewState();
						}
					}
					
#endregion
					
				}
			}
