using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using FileDispatcher.Core.ViewModels;

namespace FileDispatcher.Core.Tasks.ViewModels
{
    public abstract class TargetableTaskBaseViewModel<TTargetableTask> : TaskBaseViewModel<TTargetableTask>
        where TTargetableTask : TargetableTaskBase
    {
        public TargetableTaskBaseViewModel(TTargetableTask task, IEnumerable<string> existingTaskNames) : base(task, existingTaskNames)
        {
            _targetRouterViewModel = new TargetRouterViewModel(task.TargetRouter);
            _targetRouterViewModel.ErrorsChanged += (s, e) => ValidateTriggerAndTargetPaths();
            _targetRouterViewModel.PropertyChanged += (s, e) => ValidateTriggerAndTargetPaths(nameof(TargetRouterViewModel));

            TriggerViewModel.ErrorsChanged += (s, e) => ValidateTriggerAndTargetPaths();
            TriggerViewModel.PropertyChanged += (s, e) => ValidateTriggerAndTargetPaths();

            _targetExistsBehaviour = task.TargetExistsBehaviour;
        }

        private TargetExistsBehaviour _targetExistsBehaviour;
        public TargetExistsBehaviour TargetExistsBehaviour
        {
            get => _targetExistsBehaviour;
            set => SetProperty(ref _targetExistsBehaviour, value);
        }

        private TargetRouterViewModel _targetRouterViewModel;
        public TargetRouterViewModel TargetRouterViewModel => _targetRouterViewModel;

        public override bool HasRequiredFieldsFilled => base.HasRequiredFieldsFilled && _targetRouterViewModel.HasRequiredFieldsFilled;

        protected override void UpdateModel(TTargetableTask model)
        {
            base.UpdateModel(model);
            model.TargetExistsBehaviour = _targetExistsBehaviour;
            _targetRouterViewModel.UpdateModel();
        }

        protected override IEnumerable<ValidationResult> GetValidationResults(string propertyName)
        {
            //Get base results
            foreach (var result in base.GetValidationResults(propertyName))
            {
                yield return result;
            }

            //Get this results
            switch (propertyName)
            {
                case (nameof(TargetRouterViewModel)):
                    foreach (var result in _targetRouterViewModel.GetErrors())
                    {
                        yield return result;
                    }
                    break;
            }
        }

        private void ValidateTriggerAndTargetPaths(string propertyName = null)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                RaisePropertyChanged(propertyName);
            }
            //Validate if paths are not the same
            var pathEqualResult = new ValidationResult(TriggerViewModel.Path.Item != TargetRouterViewModel.BaseTargetPath.Item,
                "Source and Target path cannot be the same.");

            //Get all results for Trigger and Target. 
            //Change the results only if there is a difference
            SetPathValidationResults(nameof(TriggerViewModel), pathEqualResult);
            SetPathValidationResults(nameof(TargetRouterViewModel), pathEqualResult);
        }

        private void SetPathValidationResults(string propertyName, ValidationResult pathEqualResult)
        {
            var newResults = GetErrorsWithPathEqual(GetValidationResults(propertyName), pathEqualResult);

            if (GetErrors(propertyName).Count() != newResults.Count())
            {
                SetErrors(propertyName, newResults);
            }

            //Needs to be passed back to the source to show validation error
            switch (propertyName)
            {
                case (nameof(TriggerViewModel)):
                    //Get all errors and add pathEqual result if IsValid is false.
                    var triggerPathErrors = GetErrorsWithPathEqual(TriggerViewModel.Path
                        .GetErrors(nameof(TriggerViewModel.Path.Item))
                        .Where(vr => vr.ErrorContent != pathEqualResult.ErrorContent),
                        pathEqualResult);
                    TriggerViewModel.Path.SetErrors(nameof(TriggerViewModel.Path.Item), triggerPathErrors);
                    break;
                case (nameof(TargetRouterViewModel)):
                    var targetPathErrors = GetErrorsWithPathEqual(TargetRouterViewModel.BaseTargetPath
                        .GetErrors(nameof(TargetRouterViewModel.BaseTargetPath.Item))
                        .Where(vr => vr.ErrorContent != pathEqualResult.ErrorContent),
                        pathEqualResult);
                    TargetRouterViewModel.BaseTargetPath.SetErrors(nameof(TriggerViewModel.Path.Item), targetPathErrors);
                    break;
                default:
                    throw new ArgumentException($"Property name {propertyName} was not expected.");
            }
        }

        private IEnumerable<ValidationResult> GetErrorsWithPathEqual(IEnumerable<ValidationResult> currentErrors, ValidationResult pathEqualResult)
        {
            return currentErrors
                .Concat(new[] { pathEqualResult })
                .Where(r => !r.IsValid);
        }
    }
}
