using System.Collections.Generic;
using FileDispatcher.Core;
using FileDispatcher.Core.Tasks;
using FileDispatcher.Core.Tasks.ViewModels;
using FileDispatcherTests.Core.Tasks.ViewModels.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileDispatcherTests.Core.Tasks.ViewModels
{
    [TestClass]
    public class DeleteTaskViewModelTest : TaskBaseViewModelTestBase<DeleteTaskViewModel, DeleteTask>
    {
        protected override void AssertModelPropertyValuesAreAsExpected(DeleteTaskViewModel bindable, DeleteTask model)
        {
            Assert.AreEqual(bindable.DeleteRecursive, model.DeleteRecursive);
        }

        protected override DeleteTask CreateNonDefaultModel()
        {
            var deleteTask = new DeleteTask(new WatcherTrigger());
            deleteTask.DeleteRecursive = !deleteTask.DeleteRecursive;

            return deleteTask;
        }

        protected override IEnumerable<string> ExpectedPropertyChangedPropertyNames()
        {
            foreach (var expected in base.ExpectedPropertyChangedPropertyNames())
            {
                yield return expected;
            }
            yield return nameof(DeleteTaskViewModel.DeleteRecursive);
            yield return nameof(DeleteTaskViewModel.TriggerViewModel);
            yield return nameof(DeleteTaskViewModel.IsDeleteRecursiveEnabled);
        }

        protected override void SetProperties(DeleteTaskViewModel bindable, bool validValues)
        {
            base.SetProperties(bindable, validValues);
            bindable.DeleteRecursive = !bindable.DeleteRecursive;
            bindable.TriggerViewModel.WatchedElements = WatchedElements.Directory;
        }
    }
}
