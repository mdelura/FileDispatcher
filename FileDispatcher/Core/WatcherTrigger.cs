using System;
using System.IO;
using System.Runtime.Serialization;

namespace FileDispatcher.Core
{
    /// <summary>
    /// Listens to file system notifications and raises <see cref="Ready"/> event when a file or a directory appears to be ready for operations.
    /// </summary>
    [Serializable]
    public class WatcherTrigger : ITrigger
    {
        const string fitsAllFilter = "*";

        [NonSerialized]
        FileSystemWatcher _fileSystemWatcher;

        /// <summary>
        /// Occurs when a file or directory in the specified <see cref="Path"/> is ready to be handled.
        /// </summary>
        public event EventHandler<ReadyEventArgs> Ready;

        public WatcherTrigger()
        {
            Initialize();
        }

        #region Properties
        string _path;
        /// <summary>
        /// Gets or sets the path to be watched.
        /// </summary>
        public string Path
        {
            get => _path;
            set => OnSetPath(value);
        }

        bool _enabled;
        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="WatcherTrigger"/> is enabled to watch.
        /// </summary>
        public bool Enabled
        {
            get => _enabled;
            set => _fileSystemWatcher.EnableRaisingEvents = _enabled = value;
        }

        bool _includeSubdirectories = true;
        /// <summary>
        /// Gets or sets a value indicating whether subdirectories within the specified path should be monitored.
        /// </summary>
        public bool IncludeSubdirectories
        {
            get => _includeSubdirectories;
            set => _fileSystemWatcher.IncludeSubdirectories = _includeSubdirectories = value;
        }

        WatchedElements _watchedElements = WatchedElements.File | WatchedElements.Directory;
        /// <summary>
        /// Gets or sets what elements should be watched for.
        /// </summary>
        public WatchedElements WatchedElements
        {
            get => _watchedElements;
            set => _fileSystemWatcher.NotifyFilter = (NotifyFilters)(_watchedElements = value);
        }

        /// <summary>
        /// Provides file system inclusion and exclusion filters to select watched items.
        /// </summary>
        public PreferenceFilters PreferenceFilters { get; private set; } = new PreferenceFilters(); 
        #endregion

        /// <summary>
        /// Dispose the instance and its private members
        /// </summary>
        public void Dispose()
        {
            _fileSystemWatcher.Dispose();
            _fileSystemWatcher = null;
        }

        protected void OnSetPath(string value)
        {
            _path = value;
            _fileSystemWatcher.Path = _path;
        }

        protected virtual void OnReady(ReadyEventArgs e) => Ready?.Invoke(this, e);

        private void Initialize()
        {
            _fileSystemWatcher = new FileSystemWatcher();

            _fileSystemWatcher.Created += ElementAppeared;
            _fileSystemWatcher.Renamed += ElementAppeared;

            //Only Filters changes need to be monitored
            PreferenceFilters.Filters.CollectionChanged += Filters_CollectionChanged;
        }

        private void RestoreFileSystemWatcher()
        {
            _fileSystemWatcher.Path = _path;
            _fileSystemWatcher.EnableRaisingEvents = _enabled;
            _fileSystemWatcher.IncludeSubdirectories = _includeSubdirectories;
            _fileSystemWatcher.NotifyFilter = (NotifyFilters)_watchedElements;
            _fileSystemWatcher.Filter = GetBaseFilter();
        }

        private void ElementAppeared(object sender, FileSystemEventArgs e)
        {
            //Always check preference filters to avoid misinterpreting
            if (PreferenceFilters.AreMet(System.IO.Path.GetFileName(e.Name)))
                OnReady(new ReadyEventArgs(e));
        }

        private void Filters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _fileSystemWatcher.Filter = GetBaseFilter();
        }

        private string GetBaseFilter()
        {
            //Filters logic:
            //No filters set: Filter should be fitsAllFilter
            //One Filter is present: set the single filter
            //More than one filter is set: Filter is fitsAllFilter again to check everything back in PreferenceFilters
             return PreferenceFilters.Filters.Count == 1 ?
                PreferenceFilters.Filters[0] : fitsAllFilter;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Initialize();
            RestoreFileSystemWatcher();
        }
    }
}
