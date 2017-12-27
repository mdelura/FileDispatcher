using System;
using System.Collections.Generic;
using System.IO;
using FileDispatcher.Core;
using FileDispatcher.Core.Tasks;
using FileDispatcher.Core.Tasks.ViewModels;
using FileDispatcher.Core.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static FileDispatcherTests.TestUtil;

namespace FileDispatcherTests.Core.Tasks.ViewModels.TestBase
{
    [TestClass]
    public abstract class TargetableTaskBaseViewModelTestBase<TTaskViewModel, TTargetableTask> : TaskBaseViewModelTestBase<TTaskViewModel, TTargetableTask>
        where TTaskViewModel : TargetableTaskBaseViewModel<TTargetableTask>
        where TTargetableTask : TargetableTaskBase
    {
        protected override void AssertModelPropertyValuesAreAsExpected(TTaskViewModel bindable, TTargetableTask model)
        {
            Assert.AreEqual(bindable.TargetExistsBehaviour, model.TargetExistsBehaviour);
        }

        protected override IEnumerable<string> ExpectedPropertyChangedPropertyNames()
        {
            yield return nameof(TargetableTaskBaseViewModel<TTargetableTask>.TriggerViewModel);
            yield return nameof(TargetableTaskBaseViewModel<TTargetableTask>.TargetRouterViewModel);
            yield return nameof(TargetableTaskBaseViewModel<TTargetableTask>.TargetExistsBehaviour);
        }

        protected override IEnumerable<string> ExpectedErrorsChangedPropertyNames()
        {
            yield return nameof(TargetableTaskBaseViewModel<TTargetableTask>.TriggerViewModel);
            yield return nameof(TargetableTaskBaseViewModel<TTargetableTask>.TargetRouterViewModel);
        }

        protected override void SetProperties(TTaskViewModel bindable, bool validValues)
        {
            bindable.TriggerViewModel.Path.Item = validValues ? CreateTestDirectory() : GetNonExistingPath();
            bindable.TargetRouterViewModel.BaseTargetPath.Item = validValues ? CreateTestDirectory() : GetNonExistingPath();
            bindable.TargetExistsBehaviour = TargetExistsBehaviour.Rename;
        }

        protected override TTargetableTask CreateNonDefaultModel()
        {
            var model = CreateModel();
            model.TargetExistsBehaviour = TargetExistsBehaviour.Overwrite;

            return model;
        }
    }
}
