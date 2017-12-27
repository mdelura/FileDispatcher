using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using FileDispatcher.Core.ViewModels;

namespace FileDispatcher.Core.Tasks.ViewModels
{
    public class ExtractTaskViewModel : TargetableTaskBaseViewModel<ExtractTask>
    {
        public ExtractTaskViewModel(IEnumerable<string> existingTaskNames) : this(new ExtractTask(new WatcherTrigger()), existingTaskNames)
        {
        }

        public ExtractTaskViewModel(ExtractTask task, IEnumerable<string> existingTaskNames) : base(task, existingTaskNames)
        {
            _extractionPreferenceFilters = new PreferenceFiltersViewModel(task.ExtractionPreferenceFilters);
            _extractionPreferenceFilters.PropertyChanged += (s, e) => RaisePropertyChangedAndValidate(nameof(ExtractionPreferenceFilters));
        }

        private PreferenceFiltersViewModel _extractionPreferenceFilters;

        public PreferenceFiltersViewModel ExtractionPreferenceFilters => _extractionPreferenceFilters;

        protected override void UpdateModel(ExtractTask model)
        {
            base.UpdateModel(model);
            _extractionPreferenceFilters.UpdateModel();
        }

        protected override IEnumerable<ValidationResult> GetValidationResults(string propertyName)
        {
            foreach (var result in base.GetValidationResults(propertyName))
            {
                yield return result;
            }

            switch (propertyName)
            {
                case nameof(ExtractionPreferenceFilters):
                    foreach (var result in _extractionPreferenceFilters.GetErrors())
                    {
                        yield return result;
                    }
                    break;
            }
        }
    }
}
