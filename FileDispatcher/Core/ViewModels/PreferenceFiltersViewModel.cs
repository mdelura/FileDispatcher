using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using FileDispatcher.ViewModels;

namespace FileDispatcher.Core.ViewModels
{
    public class PreferenceFiltersViewModel : BindableDataErrorNotifierModelWrapperBase<PreferenceFilters>
    {

        public PreferenceFiltersViewModel() : this(new PreferenceFilters())
        {
        }

        public PreferenceFiltersViewModel(PreferenceFilters preferenceFilters) : base(preferenceFilters)
        {
            SetFiltersProperty(ref _filters, new ObservableCollection<string>(preferenceFilters.Filters), nameof(Filters));
            SetFiltersProperty(ref _exclusionFilters, new ObservableCollection<string>(preferenceFilters.ExclusionFilters), 
                nameof(ExclusionFilters));
        }

        private ObservableCollection<string> _filters;

        public ObservableCollection<string> Filters
        {
            get => _filters;
            set => SetFiltersProperty(ref _filters, value);
        }

        private ObservableCollection<string> _exclusionFilters;

        public ObservableCollection<string> ExclusionFilters
        {
            get => _exclusionFilters;
            set => SetFiltersProperty(ref _exclusionFilters, value);
        }

        protected override IEnumerable<ValidationResult> GetValidationResults(string propertyName)
        {
            if (propertyName == nameof(Filters) ||
                propertyName == nameof(ExclusionFilters))
            {
                if (_filters != null && _exclusionFilters != null)
                {
                    foreach (var conflict in PreferenceFilters.GetConflictingFilters(_filters, _exclusionFilters))
                    {
                        yield return new ValidationResult(false, $"Conflicting filter: {conflict}");
                    }
                }
            }
            else
            {
                throw PropertyNotImplementedException(propertyName);
            }
        }

        private void SetFiltersProperty(ref ObservableCollection<string> storage, ObservableCollection<string> value, [CallerMemberName] string propertyName = null)
        {
            if (storage != null)
                storage.CollectionChanged -= (s, e) => RaisePropertyChangedAndValidate(propertyName);
            SetPropertyAndValidate(ref storage, value, propertyName);
            storage.CollectionChanged += (s, e) => RaisePropertyChangedAndValidate(propertyName);
        }

        private ObservableCollection<string> GetUnderlyingFiltersField(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Filters):
                    return _filters;
                case nameof(ExclusionFilters):
                    return _exclusionFilters;
                default:
                    throw PropertyNotImplementedException(propertyName);
            }
        }

        protected override void UpdateModel(PreferenceFilters model)
        {
            model.Filters.Clear();
            model.ExclusionFilters.Clear();
            model.Filters.AddRange(_filters);
            model.ExclusionFilters.AddRange(_exclusionFilters);
        }
    }
}
