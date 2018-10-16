﻿namespace ShellBrowserLib.SharpShell.Interop.Knownfolders
{
    using ShellBrowserLib.SharpShell.Interop.Dlls;
    using ShellBrowserLib.SharpShell.Interop.Interfaces.Knownfolders;
    using ShellBrowserLib.SharpShell.Interop.Interfaces.KnownFolders;
    using ShellBrowserLib.SharpShell.Interop.ShellFolders;
    using ShellBrowserLib.SharpShell.Pidl;
    using ShellBrowserLib.Shell.Interop.ResourceIds;
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// Store property values for  a known folder.
    /// This class holds the information returned in the KNOWNFOLDER_DEFINITION structure,
    /// and resources referenced by fields in NativeFolderDefinition, such as icon and tool tip.
    ///
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/bb773325(v=vs.85).aspx
    /// </summary>
    internal class KnownFolderProperties : IKnownFolderProperties
    {
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region constructors
        /// <summary>
        /// Class constructor fron internal structure.
        /// </summary>
        /// <param name="knownFolderNative"></param>
        /// <param name="nativeFolderDefinition"></param>
        public KnownFolderProperties(IKnownFolderNative knownFolderNative,
                                     KnownFoldersSafeNativeMethods.NativeFolderDefinition nativeFolderDefinition)
            : this()
        {
            try
            {
                Init(knownFolderNative, nativeFolderDefinition);
            }
            catch(Exception ex)
            {
                logger.Error(ex);
            }
        }

        /// <summary>
        /// Hidden class constructor.
        /// </summary>
        protected KnownFolderProperties()
        {
        }
        #endregion constructors

        public string Name { get; protected set; }

        public FolderCategory Category { get; protected set; }

        public string CanonicalName { get; protected set; }

        public string Description { get; protected set; }

        public Guid ParentId { get; protected set; }

        public string Parent { get; protected set; }

        public string RelativePath { get; protected set; }

        public string ParsingName { get; protected set; }

        public string TooltipResourceId { get; protected set; }

        public string Tooltip { get; protected set; }

        public string LocalizedName { get; protected set; }

        public string LocalizedNameResourceId { get; protected set; }

        public string IconResourceId { get; protected set; }

        ////public BitmapSource Icon { get; protected set; }

        public DefinitionOptions DefinitionOptions { get; protected set; }

        public System.IO.FileAttributes FileAttributes { get; protected set; }

        public Guid FolderTypeId { get; protected set; }

        ////public string folderType { get; protected set; }  Is a manually maintained list of canonical string names in Windows API Pack 1.1

        public Guid FolderId { get; protected set; }

        /// <summary>
        /// Gets the path of this known folder (if any)
        /// 
        /// <seealso cref="IsPathExists"/> and <see cref="IsExistsInFileSystem"/>
        /// </summary>
        public string Path { get; protected set; }

        /// <summary>
        /// Gets whether the known folder object managed in this object is a folder
        /// or file objects that are part of the file system,
        /// that is, its a file, directory, or root directory.
        /// 
        /// A known folder with <see cref="IsExistsInFileSystem"/> = true also
        /// has <see cref="IsPathExists"/> = true but 
        /// <see cref="IsPathExists"/> = true does not require a particular value in
        /// <see cref="IsExistsInFileSystem"/>.
        /// </summary>
        public bool IsExistsInFileSystem { get; private set; }

        /// <summary>
        /// Gets whether the known folder has a path - that is, it has a path
        /// in the Path property but not necessarily a path in the file system.
        /// 
        /// <seealso cref="Path"/> and <see cref="IsExistsInFileSystem"/>
        /// </summary>
        public bool IsPathExists { get; protected set; }

        public RedirectionCapability Redirection { get; protected set; }

        public string Security { get; protected set; }

        /// <summary>
        /// Gets the Pidl Id List in the form of a standard .Net object.
        /// </summary>
        public IdList PidlIdList { get; private set; }

        public override string ToString()
        {
            String ret = "\n";
            ret += string.Format("                   Name: {0}\n", Name);
            ret += string.Format("               Category: {0}\n", Category);
            ret += string.Format("          CanonicalName: {0}\n", CanonicalName);
            ret += string.Format("            Description: {0}\n", Description);
            ret += string.Format("               ParentId: {0}\n", ParentId);
            ret += string.Format("                 Parent: {0}\n", Parent);
            ret += string.Format("           RelativePath: {0}\n", RelativePath);
            ret += string.Format("            ParsingName: {0}\n", ParsingName);
            ret += string.Format("      TooltipResourceId: {0}\n", TooltipResourceId);
            ret += string.Format("                Tooltip: {0}\n", Tooltip);
            ret += string.Format("          LocalizedName: {0}\n", LocalizedName);
            ret += string.Format("LocalizedNameResourceId: {0}\n", LocalizedNameResourceId);
            ret += string.Format("         IconResourceId: {0}\n", IconResourceId);

            ////ret += string.Format(" {0}\n", Icon);

            ret += string.Format("      DefinitionOptions: {0}\n", DefinitionOptions);
            ret += string.Format("         FileAttributes: {0}\n", FileAttributes);
            ret += string.Format("           FolderTypeId: {0}\n", FolderTypeId);

            ////folderType  Is a manually maintained list of canonical string names in Windows API Pack 1.1

            ret += string.Format("               FolderId: {0}\n", FolderId);
            ret += string.Format("                   Path: {0}\n", Path);
            ret += string.Format("           IsPathExists: {0}\n", IsPathExists);
            ret += string.Format("   IsExistsInFileSystem: {0}\n", IsExistsInFileSystem);
            ret += string.Format("            Redirection: {0}\n", Redirection);
            ret += string.Format("               Security: {0}\n", Security);
            ret += string.Format("             PidlIdList: {0}\n", (PidlIdList == null ? "(null)" : PidlIdList.ToString()));

            return ret;
        }

        /// <summary>
        /// Get a string resource given a resource Id
        /// </summary>
        /// <param name="resourceId">The resource Id</param>
        /// <returns>The string resource corresponding to the given resource Id. Returns null if the resource id
        /// is invalid or the string cannot be retrieved for any other reason.</returns>
        private static string GetStringResource(string resourceId)
        {
            string[] parts;
            string library;
            int index;

            if (string.IsNullOrEmpty(resourceId)) { return string.Empty; }

            // Known folder "Recent" has a malformed resource id
            // for its tooltip. This causes the resource id to
            // parse into 3 parts instead of 2 parts if we don't fix.
            resourceId = resourceId.Replace("shell32,dll", "shell32.dll");
            parts = resourceId.Split(new char[] { ',' });

            library = parts[0];
            library = library.Replace(@"@", string.Empty);
            library = Environment.ExpandEnvironmentVariables(library);
            IntPtr handle = CoreNativeMethods.LoadLibrary(library);

            parts[1] = parts[1].Replace("-", string.Empty);
            index = int.Parse(parts[1], CultureInfo.InvariantCulture);

            StringBuilder stringValue = new StringBuilder(255);
            int retval = CoreNativeMethods.LoadString(handle, index, stringValue, 255);

            return retval != 0 ? stringValue.ToString() : null;
        }

        private void Init(IKnownFolderNative knownFolderNative, KnownFoldersSafeNativeMethods.NativeFolderDefinition nativeFolderDefinition)
        {
            this.Name = Marshal.PtrToStringUni(nativeFolderDefinition.name);

            logger.Info("Get KnownFolder Properterties for: " + Name);

            this.Category = nativeFolderDefinition.category;
            this.CanonicalName = Marshal.PtrToStringUni(nativeFolderDefinition.name);
            this.Description = Marshal.PtrToStringUni(nativeFolderDefinition.description);
            this.ParentId = nativeFolderDefinition.parentId;
            this.RelativePath = Marshal.PtrToStringUni(nativeFolderDefinition.relativePath);
            this.ParsingName = Marshal.PtrToStringUni(nativeFolderDefinition.parsingName);
            this.TooltipResourceId = Marshal.PtrToStringUni(nativeFolderDefinition.tooltip);
            this.LocalizedNameResourceId = Marshal.PtrToStringUni(nativeFolderDefinition.localizedName);
            this.IconResourceId = Marshal.PtrToStringUni(nativeFolderDefinition.icon);
            this.Security = Marshal.PtrToStringUni(nativeFolderDefinition.security);
            this.FileAttributes = (System.IO.FileAttributes)nativeFolderDefinition.attributes;
            this.DefinitionOptions = nativeFolderDefinition.definitionOptions;
            this.FolderTypeId = nativeFolderDefinition.folderTypeId;
            ////knownFolderProperties.folderType = FolderTypes.GetFolderType(knownFolderProperties.folderTypeId);

            this.Redirection = knownFolderNative.GetRedirectionCapabilities();

            // Turn tooltip, localized name and icon resource IDs 
            // into the actual resources.
            this.Tooltip = GetStringResource(this.TooltipResourceId);
            this.LocalizedName = GetStringResource(this.LocalizedNameResourceId);

            this.FolderId = knownFolderNative.GetId();

            bool pathExists = false;
            this.IsExistsInFileSystem = false;
            this.IsPathExists = false;
            this.PidlIdList = null;

            using (var kfObj = new KnownFolderNative(knownFolderNative))
            {
                if (kfObj != null)
                {
                    try
                    {
                        bool? isExists = kfObj.IsFileSystem();

                        if (isExists != null)
                        {
                            if (isExists == true)
                                this.IsExistsInFileSystem = true;

                            this.Path = KnownFolderHelper.GetPath(out pathExists, knownFolderNative);
                            this.IsPathExists = pathExists;
                        }
                    }
                    catch { }

                    try
                    {
                        this.PidlIdList = kfObj.KnownFolderToIdList();
                    }
                    catch { throw; }

                    // Load Icon ResourceId from Icon Resource helper (if not already present)
                    if (string.IsNullOrEmpty(IconResourceId) && PidlIdList != null)
                    {
                        IconResourceId = IconHelper.FromPidl(PidlIdList, true, false);
                    }
                }
            }
        }
    }
}
