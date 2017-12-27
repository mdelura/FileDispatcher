using System;
using System.Collections.Generic;
using FileDispatcher.Core;
using FileDispatcher.Core.Tasks;
using FileDispatcher.Core.Tasks.ViewModels;
using FileDispatcher.Core.ViewModels;
using FileDispatcherTests.ViewModels.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileDispatcherTests.Core.Tasks.ViewModels.TestBase
{
    [TestClass]
    public abstract class TaskBaseViewModelTestBase<TTaskViewModel, TTask> : BindableDataErrorNotifierModelWrapperTestBase<TTaskViewModel, TTask>
        where TTaskViewModel : TaskBaseViewModel<TTask>
        where TTask : TaskBase
    {
        protected const string existingTaskName = "Existing Task";

        protected WatcherTrigger watcherTrigger;
        protected string[] existingTaskNames;

        [TestInitialize]
        public override void TestInit()
        {
            watcherTrigger = new WatcherTrigger();
            existingTaskNames = new string[]
            {
                existingTaskName
            };
            base.TestInit();
        }

        protected override TTaskViewModel CreateBindable(TTask model) => (TTaskViewModel)Activator.CreateInstance(typeof(TTaskViewModel), model, existingTaskNames);

        protected override TTask CreateModel() => (TTask)Activator.CreateInstance(typeof(TTask), watcherTrigger);

        protected override void AssertModelPropertyValuesAreAsExpected(TTaskViewModel bindable, TTask model)
        {
            Assert.AreEqual(bindable.TriggerViewModel.Enabled, model.Trigger.Enabled);
        }

        protected override IEnumerable<string> ExpectedErrorsChangedPropertyNames()
        {
            yield return nameof(TaskBaseViewModel<TTask>.Name);
            yield return nameof(TaskBaseViewModel<TTask>.TriggerViewModel);
        }

        protected override IEnumerable<string> ExpectedPropertyChangedPropertyNames()
        {
            yield return nameof(TaskBaseViewModel<TTask>.Name);
            yield return nameof(TaskBaseViewModel<TTask>.TriggerViewModel);
            yield return nameof(TaskBaseViewModel<TTask>.TriggerViewModel);
        }

        protected override void SetProperties(TTaskViewModel bindable, bool validValues)
        {
            bindable.Name = validValues ? "Test task" : existingTaskName;
            bindable.TriggerViewModel.Path.Item = validValues ? TestUtil.CreateTestDirectory() : TestUtil.GetNonExistingPath();
            bindable.TriggerViewModel.Enabled = !bindable.TriggerViewModel.Enabled;
        }

        protected override TTask CreateNonDefaultModel()
        {
            var model = CreateModel();
            model.Name = "Test Name";
            ((WatcherTrigger)model.Trigger).Path = TestUtil.CreateTestDirectory();
            model.Trigger.Enabled = !model.Trigger.Enabled;

            return model;
        }
    }
}
