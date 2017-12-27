using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using FileDispatcher.Core.ExtensionMethods.SerializationExtensions;
using FileDispatcher.Core.Tasks;
using NLog;

namespace FileDispatcher.Core
{
    /// <summary>
    /// Manages dispatch tasks. Allows adding, removing and saving dispatch tasks.
    /// </summary>
    class DispatchManager
    {
        const string savedTasksFileName = "Tasks.bin";
        const string currentLogFileName = "CurrentLog.log";

        static Logger dispatchLogger = LogManager.GetLogger("DispatchLog");

        static List<TaskBase> tasks;

        static bool initialized;

        static DispatchManager()
        {
            tasks = new List<TaskBase>();
            Log = new ObservableCollection<DispatchedEventArgs>();
        }

        public static ObservableCollection<DispatchedEventArgs> Log { get; private set; }

        public static IEnumerable<TaskBase> Tasks => tasks.AsEnumerable();

        /// <summary>
        /// Initializes <see cref="DispatchManager"/> by loading serialized tasks and current log.
        /// </summary>
        public static void Initialize()
        {
            if (initialized)
                throw new InvalidOperationException($"{nameof(DispatchManager)} has already been initialized.");

            //Load saved tasks if the data file exists
            if (File.Exists(savedTasksFileName))
            {
                tasks = DeserializeTasks(savedTasksFileName);
            }
            //Load current log entries if the file exists
            if (File.Exists(currentLogFileName))
            {
                Log.AddRange(File.ReadAllLines(currentLogFileName).Select(l => DispatchedEventArgs.ParseFromLogEntry(l)));
            }

            initialized = true;
        }

        public static void AddTask(TaskBase task)
        {
            tasks.Add(task);
            task.Dispatched += OnDispatched;
            SaveTasks();
        }

        public static void RemoveTask(TaskBase task)
        {
            tasks.Remove(task);
            task.Dispose();
            SaveTasks();
        }

        private static List<TaskBase> DeserializeTasks(string fileName) => fileName.DeserializeFromFile<List<TaskBase>>();

        public static void SaveTasks() => tasks.SerializeToFile(savedTasksFileName);

        private static void OnDispatched(object sender, DispatchedEventArgs e)
        {
            dispatchLogger.Info(e);
            App.Current.Dispatcher.Invoke(() => Log.Add(e));
        }
    }
}