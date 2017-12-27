using System.Collections.Generic;
using FileDispatcher.Core;
using FileDispatcher.Core.Tasks;
using FileDispatcher.Core.Tasks.ViewModels;
using FileDispatcher.Core.ViewModels;
using FileDispatcherTests.Core.Tasks.ViewModels.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileDispatcherTests.Core.Tasks.ViewModels
{
    [TestClass]
    public class CopyTaskViewModelTest : TargetableTaskBaseViewModelTestBase<CopyTaskViewModel, CopyTask>
    {
        protected override void AssertModelPropertyValuesAreAsExpected(CopyTaskViewModel bindable, CopyTask model)
        {
            Assert.AreEqual(bindable.CopySubdirectories, model.CopySubdirectories);
        }

        protected override IEnumerable<string> ExpectedPropertyChangedPropertyNames()
        {
            foreach (var expected in base.ExpectedPropertyChangedPropertyNames())
            {
                yield return expected;
            }
            yield return nameof(CopyTask.CopySubdirectories);
        }

        protected override IEnumerable<string> ExpectedErrorsChangedPropertyNames()
        {
            return base.ExpectedErrorsChangedPropertyNames();
        }

        protected override void SetProperties(CopyTaskViewModel bindable, bool validValues)
        {
            base.SetProperties(bindable, validValues);
            bindable.CopySubdirectories = !bindable.CopySubdirectories;
        }

        protected override CopyTask CreateNonDefaultModel()
        {
            var copyTask = new CopyTask(new WatcherTrigger());
            copyTask.CopySubdirectories = !copyTask.CopySubdirectories;

            return copyTask;
        }
    }
}
