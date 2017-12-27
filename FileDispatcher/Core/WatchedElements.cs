using System;
using System.ComponentModel;
using System.IO;

namespace FileDispatcher.Core
{
    /// <summary>
    /// Specifies file system element types to watch for. The values are corresponding to <see cref="NotifyFilters"/>.
    /// </summary>
    [Serializable]
    [Flags]
    public enum WatchedElements
    {
        File = NotifyFilters.FileName,
        Directory = NotifyFilters.DirectoryName
    }
}
