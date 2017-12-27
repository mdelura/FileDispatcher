using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using FileDispatcher.Core;
using Prism.Mvvm;

namespace FileDispatcher.ViewModels
{
    class LogWindowViewModel : BindableBase
    {
        const string logArchivePath = @".\LogArchive\";


        public LogWindowViewModel()
        {
            //Get Current log + archived logs
            var logs = (new KeyValuePair<string, ObservableCollection<DispatchedEventArgs>>[]
            {
                new KeyValuePair<string, ObservableCollection<DispatchedEventArgs>>("Current", DispatchManager.Log)
            })
            .Concat(GetArchivedLogs())
            .ToDictionary(kv => kv.Key, kv => kv.Value);

            Logs = new ReadOnlyDictionary<string, ObservableCollection<DispatchedEventArgs>>(logs);
        }

        private static IEnumerable<KeyValuePair<string, ObservableCollection<DispatchedEventArgs>>> GetArchivedLogs()
        {
            if (!Directory.Exists(logArchivePath))
                return new KeyValuePair<string, ObservableCollection<DispatchedEventArgs>>[] { };

            return Directory.GetFiles(logArchivePath)
                            .Select(f => new KeyValuePair<string, ObservableCollection<DispatchedEventArgs>>(
                                FormatLogName(f), new ObservableCollection<DispatchedEventArgs>(File.ReadAllLines(f)
                                    .Select(l => DispatchedEventArgs.ParseFromLogEntry(l)))));
        }

        private static string FormatLogName(string archiveFile)
        {
            string fileName = Path.GetFileName(archiveFile);
            const string logPrefix = "DispatchLog_";
            int prefixLength = logPrefix.Length;

            return DateTime.Parse($"{fileName.Substring(prefixLength, 4)}/{fileName.Substring(prefixLength + 4, 2)}/{fileName.Substring(prefixLength + 6, 2)}")
                .ToString("dd-MMM");
        }

        public ReadOnlyDictionary<string, ObservableCollection<DispatchedEventArgs>> Logs { get; private set; }

        private ObservableCollection<DispatchedEventArgs> _selectedLog;
        public ObservableCollection<DispatchedEventArgs> SelectedLog
        {
            get => _selectedLog;
            set => SetProperty(ref _selectedLog, value);
        }

    }
}
