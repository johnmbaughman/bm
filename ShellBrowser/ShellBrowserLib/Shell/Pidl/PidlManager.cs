﻿namespace ShellBrowserLib.SharpShell.Pidl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using ShellBrowserLib.IDs;
    using ShellBrowserLib.SharpShell.Interop;
    using ShellBrowserLib.SharpShell.Interop.Dlls;
    using ShellBrowserLib.SharpShell.Interop.Knownfolders;
    using ShellBrowserLib.SharpShell.Interop.KnownFolders;
    using ShellBrowserLib.SharpShell.Interop.ShellFolders;
    using ShellBrowserLib.Shell.Enums;

    /// <summary>
    /// Defines a type of path to be retrieved from the WIndows Shell API.
    /// </summary>
    public enum TypOfPath
    {
        /// <summary>
        /// Can be empty if a knownfolder does not have a physical storage representation
        /// in a directory.
        /// </summary>
        PhysicalStoragePath = 0, // 'e.g.: C:\Documents ...'

        /// <summary>
        /// Should always be available for knownfolders and normal folders.
        ///     KnownFolder: '::{FDD39AD0-238F-46AF-ADB4-6C85480369C7}' for 'Documents'
        /// Non-KnownFolder: 'c:\'                                      for 'c:\'
        /// </summary>
        LogicalPath = 1  // 
    }

    /// <summary>
    /// The PidlManager is a class that offers a set of functions for 
    /// working with PIDLs.
    /// </summary>
    /// <remarks>
    /// For more information on PIDLs, please see:
    ///     http://msdn.microsoft.com/en-us/library/windows/desktop/cc144090.aspx
    /// Notes:
    ///     http://msdn.microsoft.com/en-us/library/windows/desktop/cc144093.aspx
    /// </remarks>
    public static class PidlManager
    {
        /// <summary>
        /// Method converts an <see cref="IntPtr"/> based PIDL into a List of
        /// <see cref="ShellId"/>s.
        /// </summary>
        /// <returns>List of <see cref="ShellId"/>s for pidl object</returns>
        public static List<ShellId> Decode(IntPtr pidl)
        {
            //  Pidl is a pointer to an idlist, an idlist is a set of shitemid
            //  structures that have length indicator of two bytes, then the id data.
            //  The whole thing ends with two null bytes.

            //  Storage for the decoded pidl.
            var idList = new List<byte[]>();

            //  Start reading memory, shitemid at at time.
            if (pidl != default(IntPtr))
            {
                int bytesRead = 0;
                ushort idLength = 0;
                while ((idLength = (ushort)Marshal.ReadInt16(pidl, bytesRead)) != 0)
                {
                    //  Read the data.
                    var id = new byte[idLength - 2];
                    Marshal.Copy(pidl + bytesRead + 2, id, 0, idLength - 2);
                    idList.Add(id);
                    bytesRead += idLength;
                }
            }

            return idList.Select(id => new ShellId(id)).ToList();
        }

        /// <summary>
        /// Gets the List of <see cref="ShellId"/>s for the desktop known folder.
        /// </summary>
        /// <returns>List of <see cref="ShellId"/>s for the desktop object</returns>
        public static IdList GetDesktop()
        {
            IntPtr pidl = default(IntPtr);
            try
            {
                NativeMethods.SHGetKnownFolderIDList(KnownFolderGuids.FOLDERID_Desktop,
                                                     KNOWN_FOLDER_FLAG.KF_NO_FLAGS, IntPtr.Zero, out pidl);

                if (pidl != default(IntPtr))
                {
                    var idlist = IdList.Create(Decode(pidl));
                    return idlist;
                }
            }
            finally
            {
                if (pidl != default(IntPtr))
                    NativeMethods.ILFree(pidl);
            }

            return null;
        }

        /// <summary>
        /// Converts a Win32 PIDL to a <see cref="PidlManager"/>.<see cref="IdList"/>.
        /// 
        /// The PIDL is not freed by the PIDL manager, if it has been allocated by the
        /// shell it is the caller's responsibility to manage it.
        /// </summary>
        /// <param name="pidl">The pidl.</param>
        /// <returns>An <see cref="IdList"/> that corresponds to the PIDL.</returns>
        public static IdList PidlToIdlist(IntPtr pidl)
        {
            if(pidl == IntPtr.Zero)
                throw new Exception("Cannot create an ID list from a null pidl.");

            //  Create the raw ID list.
            List<ShellId> ids = Decode(pidl);

            //  Return a new idlist from the pidl.
            return IdList.Create(ids);
        }

        /// <summary>
        /// Method converts an <see cref="IntPtr"/> based PIDL
        /// into an Array of <see cref="ShellId"/>s.
        /// </summary>
        /// <returns>List of <see cref="ShellId"/>s for pidl object</returns>
        public static IdList[] APidlToIdListArray(IntPtr apidl, int count)
        {
            var pidls = new IntPtr[count];
            
            Marshal.Copy(apidl, pidls, 0, count);

            return pidls.Select(PidlToIdlist).ToArray();
        }

        /// <summary>
        /// Method converts a List of <see cref="ShellId"/>s into
        /// an <see cref="IntPtr"/> based PIDL.
        /// </summary>
        /// <returns><see cref="IntPtr"/> based PIDL object</returns>
        public static IntPtr IdListToPidl(IdList idList)
        {
            //  Turn the ID list into a set of raw bytes.
            var rawBytes = new List<byte>();

            //  Each item starts with it's length, then the data. The length includes
            //  two bytes, as it counts the length as a short.
            foreach (var id in idList.Ids)
            {
                //  Add the size and data.
                short length = (short)(id.Length + 2);
                rawBytes.AddRange(BitConverter.GetBytes(length));
                rawBytes.AddRange(id.RawId);
            }

            //  Write the null termination.
            rawBytes.Add(0);
            rawBytes.Add(0);

            //  Allocate COM memory for the pidl.
            var ptr = Marshal.AllocCoTaskMem(rawBytes.Count);

            //  Copy the raw bytes.
            for (var i = 0; i < rawBytes.Count; i++)
            {
                Marshal.WriteByte(ptr, i, rawBytes[i]);
            }

            //  We've allocated the pidl, copied it and are ready to rock.
            return ptr;
        }

        /// <summary>
        /// Combines 2 lists of <see cref="ShellId"/>s and returns a new
        /// list of <see cref="ShellId"/>s. 
        /// </summary>
        /// <returns><see cref="IntPtr"/> based PIDL object</returns>
        public static IdList Combine(IdList folderIdList, IdList folderItemIdList)
        {
            var combined = new List<ShellId>(folderIdList.Ids);
            combined.AddRange(folderItemIdList.Ids);

            return IdList.Create(combined);
        }

        /// <summary>
        /// Combine two IdLists - avoids processing the first IdList if it appears
        /// to be null when parentIdList and relativeChild Ids are merged and
        /// parentIdList is the desktop.
        /// </summary>
        /// <param name="folderIdList"></param>
        /// <param name="folderItemIdList"></param>
        /// <returns></returns>
        public static IdList CombineParentChild(IdList folderIdList, IdList folderItemIdList)
        {
            List<ShellId> combined = null;

            bool bCombine = false;
            if (folderIdList != null)
            {
                if (folderIdList.Size > 0)
                    bCombine = true;
            }

            if (bCombine == true)
            {
                combined = new List<ShellId>(folderIdList.Ids);
                combined.AddRange(folderItemIdList.Ids);
            }
            else
                combined = new List<ShellId>(folderItemIdList.Ids);

            return IdList.Create(combined);
        }

        /// <summary>
        /// Memory management helper function to remove a given <see cref="IntPtr"/>
        /// from allocated memory.
        /// </summary>
        /// <returns>The default value for <see cref="IntPtr"/>
        /// which should be used to reset the parameter passed in.</returns>
        public static IntPtr FreeCoTaskMem(IntPtr pidl)
        {
          if (pidl != default(IntPtr))
             Marshal.FreeCoTaskMem(pidl);
           
          return default(IntPtr);
        }

        /// <summary>
        /// Memory management helper function to remove a given <see cref="IntPtr"/>
        /// formated ItemIdList from allocated memory.
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/cc144089(v=vs.85).aspx#unknown_38608
        /// </summary>
        /// <returns>The default value for <see cref="IntPtr"/>
        /// which should be used to reset the parameter passed in.</returns>
        public static IntPtr ILFree(IntPtr pidl)
        {
          if (pidl != default(IntPtr))
            NativeMethods.ILFree(pidl);
           
          return default(IntPtr);
        }
        
        /// <summary>
        /// Returns one <see cref="IntPtr"/> based PIDL merged from the
        /// given array of PIDLs.
        /// </summary>
        /// <param name="pidls"></param>
        /// <returns></returns>
        public static IntPtr PidlsToAPidl(IntPtr[] pidls)
        {
            var buffer = Marshal.AllocCoTaskMem(pidls.Length*IntPtr.Size);
            Marshal.Copy(pidls, 0, buffer, pidls.Length);

            return buffer;
        }

        /// <summary>
        /// Gets the display name for a given <see cref="IntPtr"/> based PIDL.
        /// </summary>
        /// <returns>the display name as string</returns>
        public static string GetPidlDisplayName(IntPtr pidl)
        {
            SHFILEINFO fileInfo = new SHFILEINFO();
            NativeMethods.SHGetFileInfo(pidl, 0, out fileInfo,
                                      (uint)Marshal.SizeOf(fileInfo),
                                      SHGFI.SHGFI_PIDL | SHGFI.SHGFI_DISPLAYNAME);

            string ret = fileInfo.szDisplayName;

            return ret;
        }

        /// <summary>
        /// Converts a path representation 'C:\' into an
        /// <see cref="IntPtr"/> formated PIDL representation.
        /// 
        /// The memory of the PIDL returned must be freed with
        /// <see cref="Marshal.FreeCoTaskMem"/> by the caller.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IntPtr GetPIDLFromPath(string path)
        {
            if (string.IsNullOrEmpty(path) == true)
                return IntPtr.Zero;

            IntPtr pidlPtr = default(IntPtr);

            // Handle Special Folder path notation
            if (ShellHelpers.IsSpecialPath(path) == ShellHelpers.SpecialPath.IsSpecialPath)
            {
                // Get the KnownFolderId Guid for this special folder
                var kf_guid = new Guid(path.Substring(KF_IID.IID_Prefix.Length));

                using (var kf = KnownFolderHelper.FromPath(path))
                {
                    if (kf != null)
                    {
                        kf.Obj.GetIDList(0, out pidlPtr);
                        return pidlPtr;
                    }
                }
            }

            using (var desktopFolder = new ShellFolderDesktop())
            {
                SFGAO pdwAttributes = 0;
                uint pchEaten = 0;

                if (desktopFolder.Obj.ParseDisplayName(IntPtr.Zero, IntPtr.Zero,
                                                       path, ref pchEaten, out pidlPtr,
                                                       ref pdwAttributes) == (uint)HRESULT.S_OK)
                    return pidlPtr;
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// Converts a <see cref="IntPtr"/> formated PIDL representation
        /// into a path representation 'C:\'.
        /// 
        /// The memory of the PIDL passed in must be freed
        /// (<see cref="Marshal.FreeCoTaskMem"/>) by the caller.
        /// </summary>
        /// <param name="pidl"></param>
        /// <param name="pathType"></param>
        /// <returns></returns>
        public static string GetPathFromPIDL(IntPtr pidl,
                                             TypOfPath pathType = TypOfPath.LogicalPath)
        {
            if (pidl != default(IntPtr))
            {
                using (var parentPath = KnownFolderHelper.FromPIDL(pidl))
                {
                    if (pathType == TypOfPath.LogicalPath)
                    {
                        if (parentPath != null)
                        {
                            var props = KnownFolderHelper.GetFolderProperties(parentPath.Obj);

                            if (props != null)
                                return string.Format("{0}{1}{2}{3}", KF_IID.IID_Prefix,
                                                                     "{", props.FolderId, "}");
                        }
                    }
                }

                var sb = new StringBuilder(NativeMethods.MAX_PATH);

                if (NativeMethods.SHGetPathFromIDList(pidl, sb) == true)
                    return sb.ToString();
            }

            return null;
        }

        /// <summary>
        /// Gets the parent IdList (parent PIDL) if available and a
        /// relative child idList (relative child PIDL).
        /// </summary>
        /// <param name="originalPath"></param>
        /// <param name="parentList"></param>
        /// <param name="relativeChild"></param>
        /// <returns></returns>
        public static bool GetParentIdListFromPath(string originalPath
                                                 , out IdList parentList
                                                 , out IdList relativeChild)
        {
            parentList = null;
            relativeChild = null;

            if (string.IsNullOrEmpty(originalPath))
                return false;

            // Desktop is the root of the shell and does not have a parent
            if (originalPath.Equals(KF_IID.ID_FOLDERID_Desktop, StringComparison.InvariantCultureIgnoreCase))
                return true;

            IntPtr apidl = default(IntPtr);
            try
            {
                // Decode the pidl for this path into a list of ShellIds
                apidl = PidlManager.GetPIDLFromPath(originalPath);

                if (apidl == default(IntPtr)) // Cannot resolve this PIDL
                    return false;

                // Convert PIDL into list of shellids and remove last id
                var ashellListId = PidlManager.Decode(apidl);

                return GetKFParentChildIdList(IdList.Create(ashellListId), out parentList, out relativeChild);
            }
            finally
            {
                apidl = PidlManager.ILFree(apidl);
            }
        }

        /// <summary>
        /// Method attempts to find that parent of a known folder by looking at the
        /// ParentId field in the associated <see cref="KnownFolderProperties"/> object
        /// and returns the new parent and child <see cref="IdList"/>s on success.
        /// </summary>
        /// <param name="ashellListId"></param>
        /// <param name="parentList"></param>
        /// <param name="relativeChild"></param>
        /// <returns>True if known folder parent was available in ParentId field,
        /// otherwise false.</returns>
        public static bool GetKFParentChildIdList(IdList ashellListId
                                                , out IdList parentList
                                                , out IdList relativeChild)
        {
            parentList = null;
            relativeChild = null;

            relativeChild = ashellListId.GetRelativeChildId();
            parentList = ashellListId.GetParentId();

            // Get Parent Id which is always the first part minus last id in the sequence
            if (parentList != null)
                return true;
            else
            {   // Try to find parent through known folder information lookup
                using (KnownFolderNative kf = KnownFolderHelper.FromPIDL(ashellListId))
                {
                    if (kf != null)
                    {
                        var props = KnownFolderHelper.GetFolderProperties(kf.Obj);

                        if (props.Parent != null)
                        {
                            if ((parentList = IdList.FromKnownFolderGuid(props.ParentId)) != null)
                                return true;
                        }
                        else
                        {
                            // Just return the desktop as parent if the givem item has no more parents
                            using (var desktop = KnownFolderHelper.FromKnownFolderGuid(new Guid(KF_ID.ID_FOLDERID_Desktop)))
                            {
                                IntPtr desktopPtr = default(IntPtr);
                                try
                                {
                                    desktopPtr = desktop.KnownFolderToPIDL();

                                    if (desktopPtr != default(IntPtr))
                                    {
                                        parentList = IdList.Create(PidlManager.Decode(desktopPtr));
                                        return true;
                                    }
                                }
                                finally
                                {
                                    if (desktopPtr != default(IntPtr))
                                        desktopPtr = PidlManager.ILFree(desktopPtr);
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}
