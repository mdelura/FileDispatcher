using System;
using System.IO;
using System.Reflection;

namespace FileDispatcher.Core.Tasks
{
    [Serializable]
    /// <summary>
    /// When derived provides methods to perform dispatch task
    /// </summary>
    public abstract class TargetableTaskBase : TaskBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="TargetableTaskBase"/>
        /// </summary>
        /// <param name="trigger">Task trigger</param>
        public TargetableTaskBase(ITrigger trigger) : base(trigger)
        {
        }

        /// <summary>
        /// Gets or sets behaviour in case target object already exists
        /// True by default.
        /// </summary>
        public virtual TargetExistsBehaviour TargetExistsBehaviour { get; set; } = TargetExistsBehaviour.DoNothing;

        /// <summary>
        /// Gets or sets <see cref="ITargetRouter"/> to provide final target route.
        /// </summary>
        public TargetRouter TargetRouter { get; set; } = new TargetRouter();

        /// <summary>
        /// Based on <see cref="Tasks.TargetExistsBehaviour"/> value decides if can overwrite existing object
        /// </summary>
        /// <returns></returns>
        protected virtual bool Overwrite() => TargetExistsBehaviour == TargetExistsBehaviour.Overwrite ? true : false;

        /// <summary>
        /// Provide <paramref name="targetPath"/> and <paramref name="description"/> as an output according to value of <see cref="TargetExistsBehaviour"/> 
        /// </summary>
        /// <param name="targetPath"></param>
        /// <param name="description"></param>
        protected bool HandleIfExists(ref string targetPath, ref string description)
        {
            bool exists = File.Exists(targetPath) || Directory.Exists(targetPath);

            if (exists)
            {
                switch (TargetExistsBehaviour)
                {
                    case TargetExistsBehaviour.DoNothing:
                        description = $"{targetPath} already exists, skipped.";
                        break;
                    case TargetExistsBehaviour.Rename:
                        description = $"{targetPath} already exists, renamed to ";
                        targetPath = GetNewFileName(targetPath);
                        description += targetPath;
                        break;
                    case TargetExistsBehaviour.Overwrite:
                        description = $"{targetPath} overwritten.";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(
                            $"{nameof(TargetExistsBehaviour)}.{TargetExistsBehaviour} is not supported by {MethodBase.GetCurrentMethod().GetType()}");                }
            }
            else
            {
                description = SuccessResult;
            }
            return exists;
        }

        /// <summary>
        /// Gets new file name if present already exists
        /// </summary>
        /// <param name="targetPath">File path to be renamed</param>
        /// <returns>Target path with added counter that is non existing file name.</returns>
        private string GetNewFileName(string targetPath)
        {
            int count = 0;
            string renamedPath;
            do
            {
                count++;
                renamedPath = Path.Combine(Path.GetDirectoryName(targetPath),
                    $"{Path.GetFileNameWithoutExtension(targetPath)}_{count.ToString("0000")}.{Path.GetExtension(targetPath)}");
            } while (File.Exists(renamedPath));
            return renamedPath;
        }
    }
}