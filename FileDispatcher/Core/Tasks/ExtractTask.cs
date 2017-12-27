using System;
using System.IO;
using System.IO.Compression;

namespace FileDispatcher.Core.Tasks
{
    /// <summary>
    /// Provides methods to perform files extraction from zip archive.
    /// </summary>
    [Serializable]
    public class ExtractTask : TargetableTaskBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ExtractTask"/>.
        /// </summary>
        /// <param name="trigger">Task trigger</param>
        public ExtractTask(ITrigger trigger) : base(trigger)
        {
        }

        public PreferenceFilters ExtractionPreferenceFilters { get; private set; } = new PreferenceFilters();

        /// <summary>
        /// Perform extraction from <paramref name="sourcePath"/> file
        /// </summary>
        /// <param name="sourcePath">Full path <see cref="string"/> of the archive file</param>
        protected override DispatchedEventArgs PerformDispatch(string sourcePath)
        {
            using (ZipArchive archive = ZipFile.OpenRead(sourcePath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (ExtractionPreferenceFilters.AreMet(entry.Name))
                    {
                        string description = null;
                        string targetPath = TargetRouter.GetTargetPath(entry.Name);

                        //Perform extraction
                        if (!HandleIfExists(ref targetPath, ref description) ||
                            TargetExistsBehaviour != TargetExistsBehaviour.DoNothing)
                        {
                            try
                            {
                                entry.ExtractToFile(targetPath, Overwrite());
                            }
                            catch (IOException e)
                            {
                                description = e.Message;
                            }
                        }
                        //Raise Dispatched event for each file that matched the filter
                        OnDispatched(new DispatchedEventArgs(DateTime.Now, Name, sourcePath, targetPath, description));
                    }
                }
            }
            //OnDispatched called for each extracted file separately (if any matched filters) returning null 
            return null;
        }
    }
}