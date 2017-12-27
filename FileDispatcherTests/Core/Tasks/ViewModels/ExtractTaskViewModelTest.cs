using System.Collections.Generic;
using System.Collections.ObjectModel;
using FileDispatcher.Core;
using FileDispatcher.Core.Tasks;
using FileDispatcher.Core.Tasks.ViewModels;
using FileDispatcher.Core.ViewModels;
using FileDispatcherTests.Core.Tasks.ViewModels.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileDispatcherTests.Core.Tasks.ViewModels
{
    [TestClass]
    public class ExtractTaskViewModelTest : TargetableTaskBaseViewModelTestBase<ExtractTaskViewModel, ExtractTask>
    {
        ObservableCollection<string> _filters = new ObservableCollection<string> { "*.txt", "*.csv" };
        ObservableCollection<string> _exclusionFilters = new ObservableCollection<string> { "data.csv" };

        protected override IEnumerable<string> ExpectedPropertyChangedPropertyNames()
        {
            yield return nameof(ExtractTaskViewModel.ExtractionPreferenceFilters);
            yield return nameof(ExtractTaskViewModel.ExtractionPreferenceFilters);
        }

        protected override IEnumerable<string> ExpectedErrorsChangedPropertyNames()
        {
            yield return nameof(ExtractTaskViewModel.ExtractionPreferenceFilters);
        }

        protected override void SetProperties(ExtractTaskViewModel bindable, bool validValues)
        {
            bindable.ExtractionPreferenceFilters.Filters = _filters;
            bindable.ExtractionPreferenceFilters.ExclusionFilters = validValues ? _exclusionFilters : _filters;
        }

        protected override ExtractTask CreateNonDefaultModel()
        {
            var extractTask = new ExtractTask(new WatcherTrigger());
            extractTask.ExtractionPreferenceFilters.Filters.AddRange(new[] { "*.txt", "*.csv" });
            extractTask.ExtractionPreferenceFilters.ExclusionFilters.AddRange(new[] { "file.txt", "file.csv" });

            return extractTask;
        }
    }
}
