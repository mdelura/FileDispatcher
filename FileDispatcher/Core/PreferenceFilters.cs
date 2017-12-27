using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using FileDispatcher.Core.ExtensionMethods.StringExtensions;

namespace FileDispatcher.Core
{
    /// <summary>
    /// Provides combination of file system filters and exclusion filters to select preferred items.
    /// </summary>
    [Serializable]
    public class PreferenceFilters
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PreferenceFilters"/>
        /// </summary>
        public PreferenceFilters()
        {
            Initialize();
        }

        /// <summary>
        /// Provides 'include' filters
        /// </summary>
        public ObservableCollection<string> Filters { get; private set; } = new ObservableCollection<string>();

        /// <summary>
        /// Provides 'exclude' filters
        /// </summary>
        public ObservableCollection<string> ExclusionFilters { get; private set; } = new ObservableCollection<string>();

        /// <summary>
        /// Get filters from two sets that are in conflict (one is excluding the other or has the same items)
        /// </summary>
        /// <param name="filters">Collection of filters to be checked for conflicts</param>
        /// <param name="otherFilters">Other collection of filters to be checked for conflicts</param>
        /// <returns>Collection of conflicting filters if found any</returns>
        public static IEnumerable<string> GetConflictingFilters(IEnumerable<string> filters, IEnumerable<string> otherFilters)
        {
            return filters.Intersect(otherFilters);
        }

        /// <summary>
        /// Check if defined preferences are met by the <paramref name="name"/>
        /// </summary>
        /// <param name="name">Name to be checked</param>
        /// <returns><see langword="true"/> if file name doesn't match any of <see cref="ExclusionFilters"/> and matches at least
        /// one <see cref="Filters"/>, otherwise <see langword="false"/></returns>
        public bool AreMet(string name)
        {
            //Check if fits any exclusion filter. If so then return false, otherwise check Filters for match
            return !ExclusionFilters.Any(ef => name.FitsFilter(ef)) ?
                Filters.Any(f => name.FitsFilter(f)) : false;
        }

        private void Initialize()
        {
            Filters.CollectionChanged += (s, e) => VerifyFiltersAreNotConflicting(e, ExclusionFilters);
            ExclusionFilters.CollectionChanged += (s, e) => VerifyFiltersAreNotConflicting(e, Filters);
        }

        private void VerifyFiltersAreNotConflicting(NotifyCollectionChangedEventArgs e, IEnumerable<string> otherFilters)
        {
            //Verify only Add and Replace actions for conflicting filters
            if (e.Action == NotifyCollectionChangedAction.Add ||
                e.Action == NotifyCollectionChangedAction.Replace)
            {
                var conflicts = GetConflictingFilters(e.NewItems.Cast<string>(), otherFilters);
                if (conflicts.Any())
                    throw new ArgumentException($"Filter conflicting with existing opposite filter: {string.Join(", ", conflicts)}");
            }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Initialize();
        }
    }
}
