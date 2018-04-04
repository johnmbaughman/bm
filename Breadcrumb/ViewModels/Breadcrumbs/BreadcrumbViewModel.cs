namespace Breadcrumb.ViewModels.Breadcrumbs
{
    using Breadcrumb.ViewModels.Interfaces;
    using DirectoryInfoExLib.IO.FileSystemInfoExt;

    /// <summary>
    /// Class implements the viewmodel that manages the complete breadcrump control.
    /// </summary>
    internal class BreadcrumbViewModel : Base.ViewModelBase
    {
      #region fields
        private bool mEnableBreadcrumb;
        private string _suggestedPath;
      #endregion fields

      #region constructors
      /// <summary>
      /// Class constructor
      /// </summary>
      public BreadcrumbViewModel()
      {
        this.BreadcrumbSubTree = new ExTreeNodeViewModel();
        mEnableBreadcrumb = true;
      }
      #endregion constructors

      #region properties
      /// <summary>
      /// Gets a viewmodel that manages the sub-tree brwosing and
      /// selection within the sub-tree component
      /// </summary>
      public ExTreeNodeViewModel BreadcrumbSubTree { get; }

      /// <summary>
      /// Gets/sets a property that determines whether a breadcrumb
      /// switch is turned on or off.
      /// 
      /// On false: A Breadcrumb switch turned off shows the text editable path
      ///  On true: A Breadcrumb switch turned  on shows the BreadcrumbSubTree for browsing
      /// </summary>
      public bool EnableBreadcrumb
      {
        get
        { 
          return mEnableBreadcrumb;
        }

        set
        {
          if (mEnableBreadcrumb != value)
          {
            mEnableBreadcrumb = value;
            NotifyOfPropertyChanged(() => EnableBreadcrumb);
          }
        }
      }

      public string SuggestedPath
      {
        get { return _suggestedPath; }
        set
        {
          _suggestedPath = value;

          NotifyOfPropertyChanged(() => SuggestedPath);
          OnSuggestPathChanged();
        }
      }

        /// <summary>
        /// Contains a list of items that maps into the SuggestBox control.
        /// </summary>
////        public IEnumerable<ISuggestSource> SuggestSources
////        {
////            get
////            {
////                return _suggestSources;
////            }
////            set
////            {
////                _suggestSources = value;
////                NotifyOfPropertyChange(() => SuggestSources);
////            }
////        }
        #endregion properties

        #region methods
        /// <summary>
        /// Method should be called after construction to initialize the viewmodel
        /// to view a default content.
        /// </summary>
        public void InitPath(string initialPath)
        {
            var selector = BreadcrumbSubTree.Selection as ITreeRootSelector<ExTreeNodeViewModel, DirectoryInfoEx>;
            selector.SelectAsync(DirectoryInfoEx.FromString(initialPath) as DirectoryInfoEx);
        }
        
        /// <summary>
        /// Method executes when the text path portion in the
        /// Breadcrumb control has been edit.
        /// </summary>
        private void OnSuggestPathChanged()
        {
          /***
          if (!ShowBreadcrumb)
          {
            Task.Run(async () =>
            {
              foreach (var p in _profiles)
                if (p.MatchPathPattern(SuggestedPath))
                {
                  if (String.IsNullOrEmpty(SuggestedPath) && Entries.AllNonBindable.Count() > 0)
                    SuggestedPath = Entries.AllNonBindable.First().EntryModel.FullPath;

                  var found = await p.ParseThenLookupAsync(SuggestedPath, CancellationToken.None);
                  if (found != null)
                  {
                    _sbox.Dispatcher.BeginInvoke(new System.Action(() => { SelectAsync(found); }));
                    ShowBreadcrumb = true;
                    BroadcastDirectoryChanged(EntryViewModel.FromEntryModel(found));
                  }
                  //else not found
                }
            });//.Start();
          }
           ***/
        }
        #endregion methods
  }
}