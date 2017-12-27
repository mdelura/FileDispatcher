using System;
using System.IO;
using FileDispatcher.Core.ExtensionMethods.StringExtensions;

namespace FileDispatcher.Core.Tasks
{
    /// <summary>
    /// Provides methods to copy files on demand.
    /// </summary>
    [Serializable]
    public class CopyTask : TargetableTaskBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CopyTask"/>.
        /// </summary>
        /// <param name="trigger">Task trigger</param>
        public CopyTask(ITrigger trigger) : base(trigger)
        {
        }

        /// <summary>
        /// Gets or sets whether copying directory should include subdirectories
        /// </summary>
        public bool CopySubdirectories { get; set; }

        protected override DispatchedEventArgs PerformDispatch(string sourcePath)
        {
            string description = null;
            string targetPath = TargetRouter.GetTargetPath(sourcePath);

            if (!HandleIfExists(ref targetPath, ref description) ||
                TargetExistsBehaviour != TargetExistsBehaviour.DoNothing)
            {
                try
                {
                    if (sourcePath.IsDirectory())
                    {
                        CopyDirectory(sourcePath, targetPath, CopySubdirectories, Overwrite());
                    }
                    else
                    {
                        File.Copy(sourcePath, targetPath, Overwrite());
                    }
                }
                catch (IOException e)
                {
                    description = e.Message;
                }
            }
            return new DispatchedEventArgs(DateTime.Now, Name, sourcePath, targetPath, description);
        }

        private static void CopyDirectory(string sourcePath, string targetPath, bool copySubdirectories, bool overwrite)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo sourceDirInfo = new DirectoryInfo(sourcePath);

            if (!sourceDirInfo.Exists)
            {
                throw new DirectoryNotFoundException($"Source directory does not exist or could not be found: {sourcePath}");
            }

            DirectoryInfo[] sourceSubdirectories = sourceDirInfo.GetDirectories();
            // If the target directory doesn't exist, create it.
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = sourceDirInfo.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(targetPath, file.Name);
                file.CopyTo(tempPath, overwrite);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubdirectories)
            {
                foreach (DirectoryInfo subdir in sourceSubdirectories)
                {
                    string temppath = Path.Combine(targetPath, subdir.Name);
                    CopyDirectory(subdir.FullName, temppath, copySubdirectories, overwrite);
                }
            }
        }
    }
}
