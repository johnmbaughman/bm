﻿namespace ShellBrowserLib.Shell.Interop.ResourceIds
{
    using ShellBrowserLib.SharpShell.Interop;
    using ShellBrowserLib.SharpShell.Interop.Dlls;
    using ShellBrowserLib.SharpShell.Pidl;
    using ShellBrowserLib.Shell.Enums;
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Gets the ResourceId (libararyName, index) of a shell icon to support
    /// IconExtaction and display in UI layer.
    /// 
    /// Based on SystemImageList created By LYCJ (2014), released under MIT license
    /// +-> Based on http://vbaccelerator.com/home/net/code/libraries/Shell_Projects/SysImageList/article.asp
    /// </summary>
    internal static class IconHelper
    {
        /// <summary>
        /// Gets the ResourceId (libararyName, index) of a shell icon
        /// based on an <see cref="IdList"/> object
        /// to support IconExtaction and display in UI layer.
        /// </summary>
        /// <param name="ilPidl"></param>
        /// <param name="isDirectory"></param>
        /// <param name="forceLoadFromDisk"></param>
        /// <param name="size"></param>
        /// <param name="iconState"></param>
        /// <returns></returns>
        public static string FromPidl(IdList ilPidl,
                                      bool isDirectory,
                                      bool forceLoadFromDisk,
                                      IconSize size = IconSize.large,
                                      ShellIconStateConstants iconState = ShellIconStateConstants.ShellIconStateNormal)
        {
            IntPtr ptrPidl = default(IntPtr);
            try
            {
                if ((ptrPidl = PidlManager.IdListToPidl(ilPidl)) != default(IntPtr))
                {
                    return IconHelper.FromPidl(ptrPidl, isDirectory, forceLoadFromDisk,
                                                        size, iconState);
                }
            }
            finally
            {
                if (ptrPidl != default(IntPtr))
                    ptrPidl = PidlManager.ILFree(ptrPidl);
            }

            return null;
        }

        /// <summary>
        /// Gets the ResourceId (libararyName, index) of a shell icon
        /// based on an <see cref="IntPtr"/> formated pidl id-list.
        /// to support IconExtaction and display in UI layer.
        /// 
        /// The caller is responsible for freeing the pidl in <paramref name="ptrPidl"/>.
        /// </summary>
        /// <param name="forceLoadFromDisk"></param>
        /// <param name="iconState"></param>
        /// <param name="isDirectory"></param>
        /// <param name="ptrPidl"></param>
        /// <param name="size"></param>
        public static string FromPidl(IntPtr ptrPidl,
                                      bool isDirectory,
                                      bool forceLoadFromDisk,
                                      IconSize size = IconSize.large,
                                      ShellIconStateConstants iconState = ShellIconStateConstants.ShellIconStateNormal)
        {
            if (!isXpOrAbove())
                throw new NotSupportedException("Windows XP or above required.");

            // There is no thumbnail mode in shell.
            var _size = size == IconSize.thumbnail ? IconSize.jumbo : size;

            // XP does not have extra large or jumbo icon size.
            if (!isVistaUp() && (_size == IconSize.jumbo || _size == IconSize.extraLarge))
                _size = IconSize.large;

            return getIconIndex(ptrPidl, isDirectory, forceLoadFromDisk, size, iconState);
        }

        /// <summary>
        /// Determines if we run on Windows XP or later version of Windows. 
        /// </summary>
        /// <returns>true if current verion is Windows XP or later, false otherwise.</returns>
        internal static bool isXpOrAbove()
        {
            bool ret = false;
            if (Environment.OSVersion.Version.Major > 5)
            {
                ret = true;
            }
            else if ((Environment.OSVersion.Version.Major == 5) &&
                    (Environment.OSVersion.Version.Minor >= 1))
            {
                ret = true;
            }

            return ret;
        }

        /// <summary>
        /// Determines if we run on Windows Vista or later version of Windows. 
        /// </summary>
        /// <returns>true if current verion is Windows Vista or later, false otherwise.</returns>
        internal static bool isVistaUp()
        {
            return (Environment.OSVersion.Version.Major >= 6);
        }

        private static string getIconIndex(IntPtr pidlPtr,
                                           bool isDirectory,
                                           bool forceLoadFromDisk,
                                           IconSize size,
                                           ShellIconStateConstants iconState)
        {
            SHGFI dwFlags;
            FileAttribute dwAttr;

            getAttributes(isDirectory, forceLoadFromDisk, size, out dwAttr, out dwFlags);
            dwFlags = SHGFI.SHGFI_PIDL | SHGFI.SHGFI_ICONLOCATION;

            SHFILEINFO shfi = new SHFILEINFO();
            uint shfiSize = (uint)Marshal.SizeOf(shfi.GetType());
            IntPtr retVal = SharpShell.Interop.Dlls.NativeMethods.SHGetFileInfo(pidlPtr, (UInt32)dwAttr, ref shfi, shfiSize, ((uint)(dwFlags) | (uint)iconState));
            try
            {
                if (retVal.Equals(IntPtr.Zero))
                {
                    System.Diagnostics.Debug.Assert((!retVal.Equals(IntPtr.Zero)), "Failed to get icon index");
                    return null;
                }
                else    // Return final "library, index" formated string to extract actual icon
                   return string.Format("{0}, {1}", shfi.szDisplayName, shfi.iIcon);
            }
            finally
            {
                if (retVal.Equals(IntPtr.Zero))
                {
                    if (shfi.hIcon != default(IntPtr))
                        NativeMethods.DestroyIcon(shfi.hIcon);
                }
            }
        }

        private static void getAttributes(bool isDirectory,
                                   bool forceLoadFromDisk,
                                   IconSize _size,
                                   out FileAttribute dwAttr, out SHGFI dwFlags)
        {
            dwFlags = SHGFI.SHGFI_SYSICONINDEX;
            dwAttr = 0;

            if (_size == IconSize.small)
                dwFlags |= SHGFI.SHGFI_SMALLICON;

            if (isDirectory)
            {
                dwAttr = FileAttribute.FILE_ATTRIBUTE_DIRECTORY;
            }
            else
                if (!forceLoadFromDisk)
            {
                dwFlags |= SHGFI.SHGFI_USEFILEATTRIBUTES;
                dwAttr = FileAttribute.FILE_ATTRIBUTE_NORMAL;
            }
        }
    }
}
