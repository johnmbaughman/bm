﻿namespace SuggestLib
{
    using Interfaces;
    using SuggestLib.SuggestSource;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Uses ISuggestSource and HierarchyHelper to suggest automatically.
    /// </summary>
    public class SuggestBox : SuggestBoxBase
    {
        protected static readonly new log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region fields
        /// <summary>
        /// Implements the backing property of the <see cref="RootItem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RootItemProperty =
            DependencyProperty.Register("RootItem", typeof(object),
                typeof(SuggestBox), new PropertyMetadata(null));

        public static readonly DependencyProperty HierarchyHelperProperty =
            DependencyProperty.Register("HierarchyHelper", typeof(IHierarchyHelper),
            typeof(SuggestBox),
            new UIPropertyMetadata(new PathHierarchyHelper("Parent", "Value", "SubEntries")));

        public static readonly DependencyProperty SuggestSourcesProperty = DependencyProperty.Register(
            "SuggestSources",
            typeof(IEnumerable<ISuggestSource>),
            typeof(SuggestBox),
            new PropertyMetadata(new List<ISuggestSource>(new[] { new AutoSuggestSource() })));
        #endregion fields

        #region Constructor
        /// <summary>
        /// Static class constructor.
        /// </summary>
        static SuggestBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SuggestBox),
                new FrameworkPropertyMetadata(typeof(SuggestBox)));
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public SuggestBox()
        {
        }
        #endregion

        #region Public Properties
        public IHierarchyHelper HierarchyHelper
        {
            get { return (IHierarchyHelper)GetValue(HierarchyHelperProperty); }
            set { SetValue(HierarchyHelperProperty, value); }
        }

        public IEnumerable<ISuggestSource> SuggestSources
        {
            get { return (IEnumerable<ISuggestSource>)GetValue(SuggestSourcesProperty); }
            set { SetValue(SuggestSourcesProperty, value); }
        }

        /// <summary>
        /// Gets/sets a dependency property that holds an object that represents the current
        /// location. This location object is also handed down to the SuggestedSources
        /// object to make finding the next list of suggestions a simple matter of retrieving
        /// the children of the current the rootitem.
        /// 
        /// This property should be assigned by the client application (eg. Breadcrumb) and be
        /// updated throughout the browsing with the suggestions.
        /// </summary>
        public object RootItem
        {
            get { return GetValue(RootItemProperty); }
            set { SetValue(RootItemProperty, value); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Updates the TextBox.Text Binding expression (if any) and
        /// raises the <see cref="ValueChangedEvent"/> event to notify
        /// subscribers of the changed text value.
        /// </summary>
        protected override void updateSource()
        {
            logger.DebugFormat("{0}", (string.IsNullOrEmpty(Text) ? "" : Text));

            try
            {
                var txtBindingExpr = this.GetBindingExpression(TextBox.TextProperty);
                if (txtBindingExpr == null)
                    return;

                bool valid = true;
                if (HierarchyHelper != null)
                    valid = HierarchyHelper.GetItem(RootItem, Text) != null;

                if (valid)
                {
                    if (txtBindingExpr != null)
                        txtBindingExpr.UpdateSource();

                    RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
                }
                else
                    Validation.MarkInvalid(txtBindingExpr,
                                           new ValidationError(new PathExistsValidationRule(),
                                                               txtBindingExpr,
                                                               "Path does not exists.", null));
            }
            catch (Exception exp)
            {
                logger.Error(exp);
            }
        }

        /// <summary>
        /// Method executes when new text is entered in the textbox portion of the control.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            logger.DebugFormat("{0}", (string.IsNullOrEmpty(Text) ? "" : Text));

            base.OnTextChanged(e);

            try
            {
                var suggestSources = SuggestSources;
                var hierarchyHelper = HierarchyHelper;
                string input = Text;
                object location = RootItem;
                IsHintVisible = String.IsNullOrEmpty(input);

                if (IsEnabled == true)
                {
                    _PopUpIsCancelled = false;
                    _suggestionIsConsumed = false;
                }

                if (IsEnabled && suggestSources != null)
                {
                    Task.Run(async () =>
                    {
                        List<Task<IList<object>>> tasks = new List<Task<IList<object>>>();
                        foreach (var item in suggestSources)
                        {
                            // Query suggestion source for suggestions on this input
                            tasks.Add(item.SuggestAsync(location, input, hierarchyHelper));
                        }

                        //// Not sure why but this LINQ statement generates 2 queries - doubling the effort
                        ////var tasks = from s in suggestSources
                        ////            select s.SuggestAsync(data, text, hierarchyHelper);

                        await Task.WhenAll(tasks);

                        return tasks.SelectMany(tsk => tsk.Result).Distinct().ToList();
                    }
                    ).ContinueWith(
                        (pTask) =>
                        {
                            if (!pTask.IsFaulted)
                                this.SetValue(SuggestionsProperty, pTask.Result);

                        }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            catch (Exception exp)
            {
                logger.Error(exp);
            }
        }
        #endregion
    }
}
