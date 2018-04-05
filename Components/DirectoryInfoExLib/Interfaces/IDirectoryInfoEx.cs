﻿namespace DirectoryInfoExLib.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using System.Threading;
    using DirectoryInfoExLib.Tools;
    using DirectoryInfoExLib.IO.Header.KnownFolder;
    using DirectoryInfoExLib.IO.Header.KnownFolder.Enums;
    using System.Drawing;
    using DirectoryInfoExLib.Enums;

    public enum DirectoryTypeEnum { dtDesktop, dtSpecial, dtDrive, dtFolder, dtRoot }

    /// <summary>
    /// Represents a directory in PIDL system.
    /// </summary>
    public interface IDirectoryInfoEx : IDisposable, ISerializable, ICloneable
    {
        #region properties
        string Name { get; }
        string FullName { get; }
        string Label { get; }

        IDirectoryInfoEx Parent { get; }

        IDirectoryInfoEx Root { get; }

        bool IsBrowsable { get; set; }

        bool IsFileSystem { get; set; }

        bool HasSubFolder { get; set; }

        DirectoryTypeEnum DirectoryType { get; }

        Environment.SpecialFolder? ShellFolderType { get; }

        KnownFolder KnownFolderType { get; }

        KnownFolderIds? KnownFolderId { get; }
        #endregion properties

        #region methods
        bool Equals(IDirectoryInfoEx other);

        IEnumerable<IDirectoryInfoEx> EnumerateDirectories(String searchPattern, SearchOption searchOption, CancelDelegate cancel);

        IEnumerable<IDirectoryInfoEx> EnumerateDirectories(String searchPattern, SearchOption searchOption);
        IEnumerable<IDirectoryInfoEx> EnumerateDirectories(String searchPattern);
        IEnumerable<IDirectoryInfoEx> EnumerateDirectories();

        #region GetXXX
        /// <summary>
        /// Return a list of sub directories
        /// </summary>
        IDirectoryInfoEx[] GetDirectories(String searchPattern, SearchOption searchOption);

        /// <summary>
        /// Return a list of sub directories
        /// </summary>
        IDirectoryInfoEx[] GetDirectories(String searchPattern);

        /// <summary>
        /// Return a list of sub directories
        /// </summary>
        IDirectoryInfoEx[] GetDirectories();

        Task<IDirectoryInfoEx[]> GetDirectoriesAsync(String searchPattern,
                                                     SearchOption searchOption,
                                                     CancellationToken ct);
        #endregion

        Bitmap GetIconInner(IconSize size);
        Bitmap GetBitmap(IconSize size, IntPtr ptr, bool isDirectory, bool forceLoad);
        #endregion methods
    }
}
