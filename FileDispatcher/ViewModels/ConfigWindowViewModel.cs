using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using FileDispatcher.Core;
using FileDispatcher.Core.Tasks;
using FileDispatcher.Core.Tasks.ViewModels;
using FileDispatcher.Views;
using IWshRuntimeLibrary;
using Prism.Commands;
using Prism.Mvvm;
using IO = System.IO;

namespace FileDispatcher.ViewModels
{
    public class ConfigWindowViewModel : BindableBase
    {
        private static readonly Dictionary<string, Func<TaskBase, ITaskViewModel>> taskViewModelTypes;
        private static string appName;
        private static string startupShortcutPath;

        static ConfigWindowViewModel()
        {
            taskViewModelTypes = new Dictionary<string, Func<TaskBase, ITaskViewModel>>()
            {
                {nameof(CopyTask), (t) => t == null ? new CopyTaskViewModel(GetExistingTaskNames(t)) : new CopyTaskViewModel((CopyTask)t, GetExistingTaskNames(t)) },
                {nameof(DeleteTask), (t) => t == null ? new DeleteTaskViewModel(GetExistingTaskNames(t)) : new DeleteTaskViewModel((DeleteTask)t, GetExistingTaskNames(t)) },
                {nameof(ExtractTask), (t) => t == null ? new ExtractTaskViewModel(GetExistingTaskNames(t)) : new ExtractTaskViewModel((ExtractTask)t, GetExistingTaskNames(t)) },
                {nameof(MoveTask), (t) => t == null ? new MoveTaskViewModel(GetExistingTaskNames(t)) : new MoveTaskViewModel((MoveTask)t, GetExistingTaskNames(t))},
            };

            appName = Assembly.GetExecutingAssembly().GetName().Name;
            startupShortcutPath = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), $"{appName}.lnk");
        }

        public ConfigWindowViewModel()
        {
            //Setup tasks
            Tasks = new ObservableCollection<ITaskViewModel>(DispatchManager.Tasks
                .Select(t => taskViewModelTypes[t.GetType().Name].Invoke(t)));
            Tasks.CollectionChanged += OnTasksCollectionChanged;

            //Setup Commands
            _createTaskCommand = new DelegateCommand(OnCreateTaskExecute, OnCreateTaskCanExecute);
            _deleteSelectedTaskCommand = new DelegateCommand(OnDeleteSelectedTaskExecute, OnDeleteSelectedTaskCanExecute);
            _saveSelectedTaskCommand = new DelegateCommand(OnSaveSelectedExecute, OnSaveSelectedCanExecute);
        }

        public ObservableCollection<ITaskViewModel> Tasks { get; private set; }

        private void OnTasksCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var taskViewModel in e.NewItems.Cast<ITaskViewModel>())
                    {
                        taskViewModel.AddTaskToManager();
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var taskViewModel in e.OldItems.Cast<ITaskViewModel>())
                    {
                        taskViewModel.RemoveTaskFromManager();
                    }
                    break;
                default:
                    throw new NotImplementedException($"{e.Action} for {nameof(Tasks)} is not implemented.");
            }
        }

        private ITaskViewModel _selectedTask;
        public ITaskViewModel SelectedTask
        {
            get => _selectedTask;
            set
            {
                OnSelectedTaskChanging();
                SetProperty(ref _selectedTask, value);
                OnSelectedTaskChanged();
            }
        }

        private void OnSelectedTaskChanging()
        {
            if (_selectedTask != null)
            {
                _selectedTask.PropertyChanged -= OnSelectedTaskPropertyOrErrorChanged;
                _selectedTask.ErrorsChanged -= OnSelectedTaskPropertyOrErrorChanged;
            }
        }


        bool _selectedTaskEdited;
        private void OnSelectedTaskChanged()
        {
            _deleteSelectedTaskCommand.RaiseCanExecuteChanged();
            _saveSelectedTaskCommand.RaiseCanExecuteChanged();
            _selectedTaskEdited = false;
            if (_selectedTask != null)
            {
                _selectedTask.PropertyChanged += OnSelectedTaskPropertyOrErrorChanged;
                _selectedTask.ErrorsChanged += OnSelectedTaskPropertyOrErrorChanged;
            }
        }

        public IEnumerable<string> TaskTypes => taskViewModelTypes.Keys.AsEnumerable();

        private string _createTaskType;
        public string CreateTaskType
        {
            get => _createTaskType;
            set => SetCreateTaskType(value);

        }
        private void SetCreateTaskType(string value)
        {
            if (!taskViewModelTypes.Keys.Contains(value))
                throw new ArgumentException($"{value} is not a valid value for {nameof(CreateTaskType)}");

            SetProperty(ref _createTaskType, value, nameof(CreateTaskType));
            _createTaskCommand.RaiseCanExecuteChanged();
        }

        public bool OpenOnStartup
        {
            get => IO.File.Exists(startupShortcutPath);
            set => SetShortcut(startupShortcutPath, value);
        }

        #region Commands
        private DelegateCommand _createTaskCommand;
        public ICommand CreateTaskCommand => _createTaskCommand;

        private void OnCreateTaskExecute()
        {
            var createNewTaskWindow = new CreateNewTaskWindow();
            var createNewTaskViewModel = new CreateNewTaskWindowViewModel(taskViewModelTypes[_createTaskType].Invoke(null), Tasks);
            createNewTaskWindow.DataContext = createNewTaskViewModel;
            createNewTaskWindow.Show();
        }
        private bool OnCreateTaskCanExecute() => !string.IsNullOrEmpty(_createTaskType);

        private DelegateCommand _deleteSelectedTaskCommand;
        public ICommand DeleteSelectedTaskCommand => _deleteSelectedTaskCommand; 

        private void OnDeleteSelectedTaskExecute()
        {
            MessageBoxResult confirmationResult = ShowMessageBox($"Do you want to permanently delete task {_selectedTask.Name}?", "Delete task",
                MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (confirmationResult == MessageBoxResult.OK)
            {
                Tasks.Remove(_selectedTask);
            }
        }


        private bool OnDeleteSelectedTaskCanExecute() => _selectedTask != null;

        private DelegateCommand _saveSelectedTaskCommand;
        public ICommand SaveSelectedTaskCommand => _saveSelectedTaskCommand;

        private void OnSaveSelectedExecute()
        {
            _selectedTask.UpdateModel();
            DispatchManager.SaveTasks();
            _selectedTaskEdited = false;
            _saveSelectedTaskCommand.RaiseCanExecuteChanged();
        }
        private bool OnSaveSelectedCanExecute() => _selectedTaskEdited && (!_selectedTask?.HasErrors ?? false);
        #endregion

        private static IEnumerable<string> GetExistingTaskNames(TaskBase task)
        {
            return DispatchManager.Tasks
                .Select(t => t.Name)
                .Where(n => n != task?.Name);
        }

        private static void SetShortcut(string shortcutPath, bool openOnStartup)
        {
            if (openOnStartup)
            {
                try
                {
                    string startupPath = AppDomain.CurrentDomain.BaseDirectory;
                    string shortcutTarget = IO.Path.Combine(startupPath, $"{appName}.exe");
                    WshShell myShell = new WshShell();
                    WshShortcut myShortcut = (WshShortcut)myShell.CreateShortcut(shortcutPath);
                    myShortcut.TargetPath = shortcutTarget;
                    //Set the icon of the shortcut
                    myShortcut.IconLocation = shortcutTarget + ",0";
                    myShortcut.WorkingDirectory = startupPath;
                    myShortcut.Save();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                try
                {
                    if (IO.File.Exists(shortcutPath))
                        IO.File.Delete(shortcutPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        protected virtual MessageBoxResult ShowMessageBox(string message, string caption, MessageBoxButton buttons, MessageBoxImage image) => MessageBox.Show(message, caption, buttons, image);

        private void OnSelectedTaskPropertyOrErrorChanged(object sender, EventArgs e)
        {
            _selectedTaskEdited = true;
            _saveSelectedTaskCommand.RaiseCanExecuteChanged();
        }
    }
}