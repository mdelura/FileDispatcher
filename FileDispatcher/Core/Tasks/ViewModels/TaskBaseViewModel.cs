using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using FileDispatcher.Core.ViewModels;
using FileDispatcher.ViewModels;

namespace FileDispatcher.Core.Tasks.ViewModels
{
    public abstract class TaskBaseViewModel<TTask> : BindableDataErrorNotifierModelWrapperBase<TTask>, ITaskViewModel
        where TTask : TaskBase
    {
        private readonly TTask _task;
        private readonly IEnumerable<string> _existingTaskNames;

        public TaskBaseViewModel(TTask task, IEnumerable<string> existingTaskNames) : base(task)
        {
            _task = task;
            _existingTaskNames = existingTaskNames;
            _name = task.Name;
            _triggerViewModel = new WatcherTriggerViewModel((WatcherTrigger)task.Trigger);
            string triggerViewModelPropertyName = nameof(TriggerViewModel);
            _triggerViewModel.ErrorsChanged += (s, e) => SetErrors(triggerViewModelPropertyName, GetValidationResults(triggerViewModelPropertyName));
            _triggerViewModel.PropertyChanged += (s, e) => RaisePropertyChanged(nameof(TriggerViewModel));
        }

        private string _name;

        public string Name
        {
            get => _name;
            set => SetPropertyAndValidate(ref _name, value);
        }

        private WatcherTriggerViewModel _triggerViewModel;
        public WatcherTriggerViewModel TriggerViewModel => _triggerViewModel;

        public string TaskType => typeof(TTask).Name;

        public virtual bool HasRequiredFieldsFilled => !string.IsNullOrEmpty(_name) && _triggerViewModel.HasRequiredFieldsFilled;

        protected override void UpdateModel(TTask model)
        {
            model.Name = Name;
            _triggerViewModel.UpdateModel();
        }

        protected override IEnumerable<ValidationResult> GetValidationResults(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Name):
                    yield return new ValidationResult(!string.IsNullOrEmpty(Name), "Task name cannot be empty.");
                    yield return new ValidationResult(!_existingTaskNames.Contains(Name), "This task name is already present");
                    break;
                case nameof(TriggerViewModel):
                    foreach (var result in TriggerViewModel.GetErrors())
                    {
                        yield return result;
                    }
                    break;
            }
        }

        public void AddTaskToManager() => DispatchManager.AddTask(_task);

        public void RemoveTaskFromManager() => DispatchManager.RemoveTask(_task);
    }
}
