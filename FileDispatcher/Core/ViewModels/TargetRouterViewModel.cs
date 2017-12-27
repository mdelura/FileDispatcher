using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using FileDispatcher.ViewModels;

namespace FileDispatcher.Core.ViewModels
{
    public class TargetRouterViewModel : BindableDataErrorNotifierModelWrapperBase<TargetRouter>, IRequiredFieldsInfo
    {
        const string baseTargetPathItemPropertyName = nameof(FolderPickerViewModel.Item);

        public TargetRouterViewModel() : this(new TargetRouter())
        {
        }

        public TargetRouterViewModel(TargetRouter targetRouter) : base(targetRouter)
        {
            _baseTargetPath = new FolderPickerViewModel(targetRouter.BaseTargetPath);
            _baseTargetPath.PropertyChanged += (s, e) => RaisePropertyChangedAndValidate(nameof(BaseTargetPath));
            _matchSubdirectory = targetRouter.MatchSubdirectory;
            _matchToSimilarFile = targetRouter.MatchToSimilarFile;
        }


        private FolderPickerViewModel _baseTargetPath;
        public FolderPickerViewModel BaseTargetPath => _baseTargetPath;

        private Match _matchSubdirectory;
        public Match MatchSubdirectory
        {
            get => _matchSubdirectory;
            set => SetProperty(ref _matchSubdirectory, value);
        }

        private Match _matchToSimilarFile;
        public Match MatchToSimilarFile
        {
            get => _matchToSimilarFile;
            set => SetProperty(ref _matchToSimilarFile, value);
        }

        public bool HasRequiredFieldsFilled => BaseTargetPath.HasRequiredFieldsFilled;

        protected override void UpdateModel(TargetRouter model)
        {
            model.BaseTargetPath = _baseTargetPath.Item;
            model.MatchSubdirectory = _matchSubdirectory;
            model.MatchToSimilarFile = _matchToSimilarFile;
        }

        protected override IEnumerable<ValidationResult> GetValidationResults(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(BaseTargetPath):
                    return _baseTargetPath.GetErrors();
                default:
                    throw PropertyNotImplementedException(propertyName);
            }
        }
    }
}
