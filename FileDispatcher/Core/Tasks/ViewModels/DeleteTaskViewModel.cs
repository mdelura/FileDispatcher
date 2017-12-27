using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileDispatcher.Core.ViewModels;

namespace FileDispatcher.Core.Tasks.ViewModels
{
    public class DeleteTaskViewModel : TaskBaseViewModel<DeleteTask>
    {
        public DeleteTaskViewModel(IEnumerable<string> existingTaskNames) : this(new DeleteTask(new WatcherTrigger()), existingTaskNames)
        {
        }

        public DeleteTaskViewModel(DeleteTask task, IEnumerable<string> existingTaskNames) : base(task, existingTaskNames)
        {
            TriggerViewModel.PropertyChanged += TriggerViewModel_PropertyChanged;
            _deleteRecursive = task.DeleteRecursive;
        }


        private bool _deleteRecursive;
        public bool DeleteRecursive
        {
            get => _deleteRecursive;
            set => SetProperty(ref _deleteRecursive, value);
        }

        public bool IsDeleteRecursiveEnabled => TriggerViewModel.WatchedElements.HasFlag(WatchedElements.Directory);

        protected override void UpdateModel(DeleteTask model)
        {
            base.UpdateModel(model);
            model.DeleteRecursive = _deleteRecursive;
        }

        private void TriggerViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TriggerViewModel.WatchedElements))
                RaisePropertyChanged(nameof(IsDeleteRecursiveEnabled));
        }
    }
}
