using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using FileDispatcher.Core;
using FileDispatcher.Core.ViewModels;
using FileDispatcherTests.ViewModels.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static FileDispatcherTests.TestUtil;

namespace FileDispatcherTests.ViewModels
{
    [TestClass]
    public class WatcherTriggerViewModelTest : BindableDataErrorNotifierModelWrapperTestBase<WatcherTriggerViewModel, WatcherTrigger> 
    {
        ObservableCollection<string> _filters = new ObservableCollection<string> { "*.txt", "*.csv" };
        ObservableCollection<string> _exclusionFilters = new ObservableCollection<string> { "data.csv" };

        protected override void AssertModelPropertyValuesAreAsExpected(WatcherTriggerViewModel bindable, WatcherTrigger model)
        {
            Assert.AreEqual(bindable.Path.Item, model.Path);
            Assert.AreEqual(bindable.Enabled, model.Enabled);
            Assert.AreEqual(bindable.IncludeSubdirectories, model.IncludeSubdirectories);
            Assert.AreEqual(bindable.WatchedElements, model.WatchedElements);
            AssertCollectionsAreEqual(_filters, model.PreferenceFilters.Filters);
            AssertCollectionsAreEqual(_exclusionFilters, model.PreferenceFilters.ExclusionFilters);
        }

        protected override WatcherTrigger CreateNonDefaultModel()
        {
            var watcherTrigger = new WatcherTrigger();
            watcherTrigger.Path = CreateTestDirectory();
            watcherTrigger.WatchedElements = WatchedElements.Directory;
            watcherTrigger.IncludeSubdirectories = !watcherTrigger.IncludeSubdirectories;
            watcherTrigger.PreferenceFilters.Filters.AddRange(_filters);
            watcherTrigger.PreferenceFilters.ExclusionFilters.AddRange(_exclusionFilters);
            watcherTrigger.Enabled = !watcherTrigger.Enabled;

            return watcherTrigger;
        }

        protected override IEnumerable<string> ExpectedErrorsChangedPropertyNames()
        {
            yield return nameof(WatcherTriggerViewModel.Path);
            yield return nameof(WatcherTriggerViewModel.PreferenceFiltersViewModel);
        }

        protected override IEnumerable<string> ExpectedPropertyChangedPropertyNames()
        {
            return new string[]
            {
                nameof(WatcherTriggerViewModel.Path),
                nameof(WatcherTriggerViewModel.Enabled),
                nameof(WatcherTriggerViewModel.IncludeSubdirectories),
                nameof(WatcherTriggerViewModel.WatchedElements),
                nameof(WatcherTriggerViewModel.PreferenceFiltersViewModel),
                nameof(WatcherTriggerViewModel.PreferenceFiltersViewModel)
            };
        }

        protected override void SetProperties(WatcherTriggerViewModel bindable, bool validValues)
        {
            bindable.Path.Item = validValues ? CreateTestDirectory() : GetNonExistingPath();
            bindable.Enabled = !bindable.Enabled;
            bindable.IncludeSubdirectories = !bindable.IncludeSubdirectories;
            bindable.WatchedElements = WatchedElements.Directory;
            bindable.PreferenceFiltersViewModel.Filters = _filters;
            bindable.PreferenceFiltersViewModel.ExclusionFilters = validValues ? _exclusionFilters : _filters;
        }
    }
}
