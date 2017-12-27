using System;
using System.IO;
using FileDispatcher.Core.ExtensionMethods.StringExtensions;

namespace FileDispatcher.Core.Tasks
{
    /// <summary>
    /// Provides methods to delete files on demand.
    /// </summary>
    [Serializable]
    public class DeleteTask : TaskBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DeleteTask"/>.
        /// </summary>
        /// <param name="trigger">Task trigger</param>
        public DeleteTask(ITrigger trigger) : base(trigger)
        {
        }

        /// <summary>
        /// Gets or sets whether the task should perform delete directories, subdirectories and all files in path
        /// Used when the task performs directory delete. 
        /// </summary>
        public bool DeleteRecursive { get; set; }

        protected override DispatchedEventArgs PerformDispatch(string sourcePath)
        {
            string description = null;
            try
            {
                if (sourcePath.IsDirectory())
                {
                    Directory.Delete(sourcePath, DeleteRecursive);
                }
                else
                {
                    File.Delete(sourcePath);
                }
                description = SuccessResult;
            }
            catch (IOException)
            {
                description = $"{sourcePath} cannot be deleted.";
            }
            return new DispatchedEventArgs(DateTime.Now, Name, sourcePath, null, description);
        }
    }
}
