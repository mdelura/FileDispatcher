using System;
using System.Globalization;

namespace FileDispatcher.Core
{
    [Serializable]
    public class DispatchedEventArgs : EventArgs
    {
        const string logSeparator = "|";

        /// <summary>
        /// Initializes new instance of <see cref="DispatchedEventArgs"/> setting information data about event with provided values
        /// </summary>
        /// <param name="dateTime">When the event occurred</param>
        /// <param name="sourcePath">Source path of Dispatched object</param>
        /// <param name="resultDescription">Description of the type of Dispatch</param>
        public DispatchedEventArgs(DateTime dateTime, string taskName, string sourcePath, string targetPath, string resultDescription)
        {
            TaskName = taskName;
            DateTime = dateTime;
            SourcePath = sourcePath;
            TargetPath = targetPath;
            ResultDescription = resultDescription;
        }

        /// <summary>
        /// Name of the task.
        /// </summary>
        public string TaskName { get; private set; }

        /// <summary>
        /// When the event occurred
        /// </summary>
        public DateTime DateTime { get; private set; }

        /// <summary>
        /// Source path of Dispatched object
        /// </summary>
        public string SourcePath { get; private set; }

        /// <summary>
        /// Target directory of the dispatch
        /// </summary>
        public string TargetPath { get; private set; }

        /// <summary>
        /// Description of the type of Dispatch
        /// </summary>
        public string ResultDescription { get; private set; }

        public static DispatchedEventArgs ParseFromLogEntry(string logEntry)
        {
            string[] logItems = logEntry.Split(new[] { logSeparator }, StringSplitOptions.None);
            int index = 0;
            return new DispatchedEventArgs(
                DateTime.ParseExact(logItems[index++], "F", CultureInfo.CurrentCulture),
                logItems[index++],
                logItems[index++],
                logItems[index++],
                logItems[index++]);
        }

        public override string ToString()
        {
            return string.Join(logSeparator,
                DateTime.ToString("F", CultureInfo.CurrentCulture),
                TaskName,
                SourcePath,
                TargetPath,
                ResultDescription);
        }


    }
}
