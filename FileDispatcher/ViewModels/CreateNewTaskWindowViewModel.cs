using System.Collections.Generic;
using System.Windows.Input;
using FileDispatcher.Core.Tasks.ViewModels;
using FileDispatcher.Views;
using Prism.Commands;
using Prism.Mvvm;

namespace FileDispatcher.ViewModels
{
    class CreateNewTaskWindowViewModel : BindableBase
    {
        ICollection<ITaskViewModel> _tasks;
        public CreateNewTaskWindowViewModel(ITaskViewModel taskViewModel, ICollection<ITaskViewModel> tasks)
        {
            CancelCommand = new DelegateCommand<CreateNewTaskWindow>((w) => w.Close());
            TaskViewModel = taskViewModel;
            _tasks = tasks;

            _createTaskCommand = new DelegateCommand<CreateNewTaskWindow>(CreateTask, (w) => !TaskViewModel.HasErrors && TaskViewModel.HasRequiredFieldsFilled);
            taskViewModel.PropertyChanged += (s, e) => _createTaskCommand.RaiseCanExecuteChanged();
        }

        private void CreateTask(CreateNewTaskWindow createNewTaskWindow)
        {
            TaskViewModel.UpdateModel();
            _tasks.Add(TaskViewModel);
            createNewTaskWindow.Close();
        }

        public ITaskViewModel TaskViewModel { get; private set; }

        private DelegateCommand<CreateNewTaskWindow> _createTaskCommand;
        public ICommand CreateTaskCommand => _createTaskCommand;

        public ICommand CancelCommand { get; private set; }
    }
}
