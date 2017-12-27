using System;
using System.IO;
using FileDispatcher.Core.ExtensionMethods.StringExtensions;

namespace FileDispatcher.Core.Tasks
{
    /// <summary>
    /// Provides methods to move files to another directory on demand.
    /// </summary>
    [Serializable]
    public class MoveTask : TargetableTaskBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MoveTask"/>.
        /// </summary>
        /// <param name="trigger">Task trigger</param>
        public MoveTask(ITrigger trigger) : base(trigger)
        {
        }

        protected override DispatchedEventArgs PerformDispatch(string sourcePath)
        {
            string targetPath = TargetRouter.GetTargetPath(sourcePath);
            string resultDescription = null;

            if (!HandleIfExists(ref targetPath, ref resultDescription) ||
                TargetExistsBehaviour != TargetExistsBehaviour.DoNothing ||
                Overwrite())
            {
                try
                {
                    if (sourcePath.IsDirectory())
                    {
                        Directory.Move(sourcePath, targetPath);
                    }
                    else
                    {
                        File.Move(sourcePath, targetPath);
                    }
                }
                catch (IOException e)
                {
                    resultDescription = e.Message;
                }
            }
            return new DispatchedEventArgs(DateTime.Now, Name, sourcePath, targetPath, resultDescription);
        }
    }
}

