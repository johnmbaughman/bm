﻿namespace BreadcrumbTestLib.ViewModels.Breadcrumbs
{
    using BreadcrumbTestLib.SystemIO;
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using DirectoryInfoExLib.Interfaces;
    using BmLib.Enums;

    /// <summary>
    /// Implements a <see cref="IDirectoryBrowser"/> based <seealso ref="ICompareHierarchy"> object
    /// to compute the relationship of two <see cref="IDirectoryBrowser"/> based paths to each other.
    /// </summary>
    public class ExHierarchyComparer : ICompareHierarchy<IDirectoryBrowser>
    {
        #region fields
        /// <summary>
        /// Log4net logger facility for this class.
        /// </summary>
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private PathComparer _pathComparer = new PathComparer();
        #endregion fields

        #region constructors
        /// <summary>
        /// Class cosntructor
        /// </summary>
        public ExHierarchyComparer()
        {
        }
        #endregion constructors

        #region methods
        public HierarchicalResult CompareHierarchyInner(IDirectoryBrowser a, IDirectoryBrowser b)
        {
            Logger.InfoFormat("IDirectoryBrowser a '{0}' IDirectoryBrowser b '{1}'", a.FullName, b.FullName);

            if (a == null || b == null)
                return HierarchicalResult.Unrelated;

            if (!a.FullName.Contains("::") && !b.FullName.Contains("::"))
                return this._pathComparer.CompareHierarchy(a.FullName, b.FullName);

            if (a.FullName.Equals(b.FullName))
                return HierarchicalResult.Current;

            string key = string.Format("{0}-compare-{1}", a.FullName, b.FullName);

            if (a.FullName == b.FullName)
                return HierarchicalResult.Current;
            else if (DirectoryInfoExLib.Factory.HasParent(b, a.FullName))
                return HierarchicalResult.Child;
            else if (DirectoryInfoExLib.Factory.HasParent(a, b.FullName))
                return HierarchicalResult.Parent;
            else return HierarchicalResult.Unrelated;
        }

        public HierarchicalResult CompareHierarchy(IDirectoryBrowser a, IDirectoryBrowser b)
        {
            Logger.InfoFormat("IDirectoryBrowser a '{0}' IDirectoryBrowser b '{1}'", a.FullName, b.FullName);

            HierarchicalResult retVal = this.CompareHierarchyInner(a, b);
            ////Debug.WriteLine(String.Format("{2} {0},{1}", a.FullPath, b.FullPath, retVal));

            return retVal;
        }

////    private bool HasParent(FileSystemInfoEx child, DirectoryInfoEx parent)
////    {
////      DirectoryInfoEx current = child.Parent;
////      while (current != null)
////      {
////        if (current.Equals(parent))
////          return true;
////        current = current.Parent;
////      }
////      return false;
////    }
        #endregion methods
    }
}
