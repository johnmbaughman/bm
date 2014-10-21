﻿namespace Breadcrumb.ViewModels.Helpers
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using System.Threading.Tasks;
	using Breadcrumb.Defines;
	using Breadcrumb.ViewModels.Interfaces;
	using Breadcrumb.ViewModels.TreeLookupProcessors;
	using Breadcrumb.ViewModels.TreeSelectors;

	public class TreeRootSelectorViewModel<VM, T> : TreeSelectorViewModel<VM, T>, ITreeRootSelector<VM, T>
	{
		#region fields
		private T _selectedValue = default(T);
		private ITreeSelector<VM, T> _selectedSelector;
		private Stack<ITreeSelector<VM, T>> _prevPath = null;
		private IEnumerable<ICompareHierarchy<T>> _comparers;
		private ObservableCollection<VM> _rootItems = null;
		#endregion fields

		#region constructors
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="entryHelper"></param>
		/// <param name="compareFunc"></param>
		/// <param name="rootLevel">Level of TreeItem to consider as root, root items should shown in expander 
		/// (e.g. in OverflowedAndRootItems) and have caption and expander hidden when the path is longer than it.</param>
		public TreeRootSelectorViewModel(IEntriesHelper<VM> entryHelper) ////int rootLevel = 0,
			////params Func<T, T, HierarchicalResult>[] compareFuncs)
			: base(entryHelper)
		{
			////_rootLevel = rootLevel;
			////_compareFuncs = compareFuncs;
			////Comparers = new [] { PathComparer.LocalDefault };
		}

		#endregion constructors

		#region events
		public event EventHandler SelectionChanged;
		#endregion events

		#region properties
		public ObservableCollection<VM> OverflowedAndRootItems
		{
			get
			{
				if (this._rootItems == null)
					this.updateRootItems();

				return this._rootItems;
			}

			set
			{
				this._rootItems = value;
				this.NotifyOfPropertyChanged(() => this.OverflowedAndRootItems);
			}
		}

		public ITreeSelector<VM, T> SelectedSelector
		{
			get { return this._selectedSelector; }
		}

		public VM SelectedViewModel
		{
			get
			{
				return (this.SelectedSelector == null ? default(VM) : this.SelectedSelector.ViewModel);
			}
		}

		public T SelectedValue
		{
			get { return this._selectedValue; }
			set { this.SelectAsync(value); }
		}

		////public int RootLevel { get { return _rootLevel; } set { _rootLevel = value; } }

		public IEnumerable<ICompareHierarchy<T>> Comparers
		{
			get
			{
				return this._comparers;
			}
			
			set
			{
				this._comparers = value;
			}
		}
		#endregion properties

		#region methods
		public override void ReportChildSelected(Stack<ITreeSelector<VM, T>> path)
		{
			ITreeSelector<VM, T> prevSelector = this._selectedSelector;

			T prevSelectedValue = this._selectedValue;
			this._prevPath = path;

			this._selectedSelector = path.Last();
			this._selectedValue = path.Last().Value;

			if (prevSelectedValue != null && !prevSelectedValue.Equals(path.Last().Value))
			{
				prevSelector.IsSelected = false;
			}

			this.NotifyOfPropertyChanged(() => this.SelectedValue);
			this.NotifyOfPropertyChanged(() => this.SelectedViewModel);

			if (this.SelectionChanged != null)
				this.SelectionChanged(this, EventArgs.Empty);

			this.updateRootItems(path);
		}

		public override void ReportChildDeselected(Stack<ITreeSelector<VM, T>> path)
		{
		}

		public async Task SelectAsync(T value)
		{
			if (this._selectedValue == null ||
			    this.CompareHierarchy(this._selectedValue, value) != HierarchicalResult.Current)
			{
				await this.LookupAsync(value, RecrusiveSearch<VM, T>.LoadSubentriesIfNotLoaded,
								SetSelected<VM, T>.WhenSelected, SetChildSelected<VM, T>.ToSelectedChild);
			}
		}

		public HierarchicalResult CompareHierarchy(T value1, T value2)
		{
			foreach (var c in this.Comparers)
			{
				var retVal = c.CompareHierarchy(value1, value2);
				if (retVal != HierarchicalResult.Unrelated)
					return retVal;
			}
			return HierarchicalResult.Unrelated;
		}

		private async Task updateRootItemsAsync(ITreeSelector<VM, T> selector, ObservableCollection<VM> rootItems, int level)
		{
			if (level == 0)
				return;

			List<ITreeSelector<VM, T>> rootTreeSelectors = new List<ITreeSelector<VM, T>>();

			// Perform a lookup and for all directories in next level of current directory (load asynchronously if not loaded), 
			// add directories's Selector to rootTreeSelectors.
			await selector.LookupAsync(default(T),
							BroadcastNextLevel<VM, T>.LoadSubentriesIfNotLoaded,
							new TreeLookupProcessor<VM, T>(HierarchicalResult.All, (hr, p, c) =>
											{
												rootTreeSelectors.Add(c);
												return true;
											}));

			// Then foreach rootTreeSelectors, add to rootItems and preferm updateRootItemAsync.
			foreach (var c in rootTreeSelectors)
			{
				rootItems.Add(c.ViewModel);
				c.IsRoot = true;
				
				await this.updateRootItemsAsync(c, rootItems, level - 1);
			}
		}

		private void updateRootItems(Stack<ITreeSelector<VM, T>> path = null)
		{
			////if (_rootItems == null)
			this._rootItems = new ObservableCollection<VM>();
			////else _rootItems.Clear();

			if (path != null && path.Count() > 0)
			{
				foreach (var p in path.Reverse())
				{
					if (!(this.EntryHelper.AllNonBindable.Contains(p.ViewModel)))
						this._rootItems.Add(p.ViewModel);
				}

				this._rootItems.Add(default(VM)); // Separator
			}

			this.updateRootItemsAsync(this, this._rootItems, 1);
		}
		#endregion
	}
}
