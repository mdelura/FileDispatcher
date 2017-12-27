using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using FileDispatcher.Core;
using FileDispatcher.Core.Tasks;
using FileDispatcher.Core.Tasks.ViewModels;
using FileDispatcher.ViewModels;
using FileDispatcherTests.Core.Tasks.TestBase;
using FileDispatcherTests.Core.Tasks.ViewModels.TestBase;
using FileDispatcherTests.ViewModels.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FileDispatcherTests.ViewModels
{
    [TestClass]
    public class ConfigWindowViewModelTest : BindableTestBase<ConfigWindowViewModelTesting>
    {
        protected override IEnumerable<string> ExpectedPropertyChangedPropertyNames()
        {
            return new string[]
            {
                nameof(ConfigWindowViewModel.CreateTaskType),
                nameof(ConfigWindowViewModel.SelectedTask),
            };
        }

        protected override void SetProperties(ConfigWindowViewModelTesting bindable)
        {
            bindable.CreateTaskType = bindable.TaskTypes.Last();
            bindable.SelectedTask = Mock.Of<ITaskViewModel>();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateTaskType_SetInvalid_ShouldThrowArgumentException()
        {
            bindable.CreateTaskType = Guid.NewGuid().ToString();
        }

        [TestMethod]
        public void CreateTaskCommand_SetCreateTaskType_CanExecuteChangedAndReturnsTrue()
        {
            //Arrange
            bool canExecuteChanged = false;
            bindable.CreateTaskCommand.CanExecuteChanged += (s, e) => canExecuteChanged = true;
            
            //Act
            bindable.CreateTaskType = bindable.TaskTypes.Last();

            //Assert
            Assert.IsTrue(canExecuteChanged);
            Assert.IsTrue(bindable.CreateTaskCommand.CanExecute(null));
        }

        [TestMethod]
        public void DeleteSelectedTaskCommand_SetSelectedTask_CanExecuteChangedAndReturnsTrue()
        {
            bool canExecuteChanged = false;
            bindable.DeleteSelectedTaskCommand.CanExecuteChanged += (s, e) => canExecuteChanged = true;

            //Act
            bindable.SelectedTask = Mock.Of<ITaskViewModel>();

            //Assert
            Assert.IsTrue(canExecuteChanged);
            Assert.IsTrue(bindable.DeleteSelectedTaskCommand.CanExecute(null));
        }

        [TestMethod]
        public void SaveSelectedTaskCommand_SelectedTaskEdited_CanExecuteChangedAndReturnsTrue()
        {
            //Arrange
            var taskViewModelMock = new Mock<ITaskViewModel>();
            bool canExecuteChanged = false;
            bindable.SelectedTask = taskViewModelMock.Object;
            bindable.SaveSelectedTaskCommand.CanExecuteChanged += (s, e) => canExecuteChanged = true;

            //Act
            taskViewModelMock.Raise(m => m.PropertyChanged += null, new PropertyChangedEventArgs(nameof(ITaskViewModel.Name)));

            //Assert
            Assert.IsTrue(canExecuteChanged);
            Assert.IsTrue(bindable.SaveSelectedTaskCommand.CanExecute(null));
        }

        [TestMethod]
        public void DeleteSelectedTaskCommand_Execute_SelectedTaskIsDeleted()
        {
            //Arrange
            var taskViewModelMock = new Mock<ITaskViewModel>();
            bindable.Tasks.Add(taskViewModelMock.Object);
            bindable.SelectedTask = taskViewModelMock.Object;

            //Act
            bindable.DeleteSelectedTaskCommand.Execute(null);

            //Assert
            Assert.IsTrue(bindable.MessageBoxShown);
            Assert.IsFalse(bindable.Tasks.Any());
        }

        [TestMethod]
        public void SaveSelectedTaskCommand_Execute_ChangesAreSaved()
        {
            //Arrange
            string name = "Name";
            string changedName = "New name";

            TaskBaseMock taskMock = new TaskBaseMock(Mock.Of<WatcherTrigger>());
            taskMock.Name = name;

            TaskBaseViewModelMock taskViewModel = new TaskBaseViewModelMock(taskMock, new string[] { });

            bindable.SelectedTask = taskViewModel;


            taskViewModel.Name = changedName;

            //Act
            bindable.SaveSelectedTaskCommand.Execute(null);

            //Assert
            Assert.IsFalse(bindable.SaveSelectedTaskCommand.CanExecute(null));
            Assert.AreEqual(changedName, taskMock.Name);
        }
    }

    public class ConfigWindowViewModelTesting : ConfigWindowViewModel
    {
        public bool MessageBoxShown { get; private set; }
        protected override MessageBoxResult ShowMessageBox(string message, string caption, MessageBoxButton buttons, MessageBoxImage image)
        {
            MessageBoxShown = true;
            return MessageBoxResult.OK;
        }
    }
}
