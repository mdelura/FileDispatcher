using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDispatcher.Core
{
    /// <summary>
    /// Provides data about ready file or directory.
    /// </summary>
    [Serializable]
    public class ReadyEventArgs
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ReadyEventArgs"/>
        /// </summary>
        /// <param name="fileSystemEventArgs">Base <see cref="FileSystemEventArgs"/> for this <see cref="ReadyEventArgs"/></param>
        public ReadyEventArgs(FileSystemEventArgs fileSystemEventArgs)
        {
            FullPath = fileSystemEventArgs.FullPath;
            Name = fileSystemEventArgs.Name;
        }

        /// <summary>
        /// Full path of the ready file or directory.
        /// </summary>
        public string FullPath { get; }

        /// <summary>
        /// Name of file the ready file or directory.
        /// </summary>
        public string Name { get; }
    }
}
