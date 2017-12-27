using System.Collections.Generic;
using System.Windows.Controls;
using FileDispatcher.ViewModels;

namespace FileDispatcher.Core.ViewModels
{
    public class WatcherTriggerViewModel : TriggerViewModel<WatcherTrigger>, IRequiredFieldsInfo
    {
        public WatcherTriggerViewModel() : this(new WatcherTrigger())
        {
        }

        public WatcherTriggerViewModel(WatcherTrigger watcherTrigger) : base(watcherTrigger)
        {
            _path = new FolderPickerViewModel(watcherTrigger.Path);
            _path.PropertyChanged += (s, e) => RaisePropertyChangedAndValidate(nameof(Path));
            _preferenceFiltersViewModel = new PreferenceFiltersViewModel(watcherTrigger.PreferenceFilters);
            _preferenceFiltersViewModel.PropertyChanged += (s, e) => RaisePropertyChangedAndValidate(nameof(PreferenceFiltersViewModel));

            _watchedElements = watcherTrigger.WatchedElements;
            _includeSubdirectories = watcherTrigger.IncludeSubdirectories;
        }

        private bool _includeSubdirectories;

        public bool IncludeSubdirectories
        {
            get => _includeSubdirectories;
            set => SetProperty(ref _includeSubdirectories, value);
        }

        //Potentially expose as IDictionary<KeyValuePair<string, bool>>
        private WatchedElements _watchedElements;

        public WatchedElements WatchedElements
        {
            get => _watchedElements;
            set => SetPropertyAndValidate(ref _watchedElements, value);
        }

        private FolderPickerViewModel _path;
        public FolderPickerViewModel Path => _path;

        private PreferenceFiltersViewModel _preferenceFiltersViewModel;
        public PreferenceFiltersViewModel PreferenceFiltersViewModel => _preferenceFiltersViewModel;

        public bool HasRequiredFieldsFilled => _watchedElements != 0 && _path.HasRequiredFieldsFilled;

        protected override void UpdateModel(WatcherTrigger model)
        {
            model.Path = _path.Item;
            _preferenceFiltersViewModel.UpdateModel();
            model.WatchedElements = _watchedElements;
            model.IncludeSubdirectories = _includeSubdirectories;
            base.UpdateModel(model);
        }

        protected override IEnumerable<ValidationResult> GetValidationResults(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Path):
                    return _path.GetErrors();
                case nameof(PreferenceFiltersViewModel):
                    return _preferenceFiltersViewModel.GetErrors();
                case nameof(WatchedElements):
                    return new ValidationResult[] { new ValidationResult(_watchedElements != 0, "At least one option has to be selected.") };
                default:
                    throw PropertyNotImplementedException(propertyName);
            }
        }
    }
}