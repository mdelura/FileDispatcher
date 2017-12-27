using System.Collections.Generic;
using FileDispatcher.Core;
using FileDispatcher.Core.Tasks;
using FileDispatcher.Core.Tasks.ViewModels;
using FileDispatcher.Core.ViewModels;
using FileDispatcherTests.Core.Tasks.TestBase;
using FileDispatcherTests.Core.Tasks.ViewModels.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileDispatcherTests.Core.Tasks.ViewModels
{
    [TestClass]
    public class TaskBaseViewModelTest : TaskBaseViewModelTestBase<TaskBaseViewModelMock, TaskBaseMock>
    {
        protected override IEnumerable<string> ExpectedPropertyChangedPropertyNames() => base.ExpectedPropertyChangedPropertyNames();

        protected override IEnumerable<string> ExpectedErrorsChangedPropertyNames() => base.ExpectedErrorsChangedPropertyNames();

        protected override void SetProperties(TaskBaseViewModelMock bindable, bool validValues) => base.SetProperties(bindable, validValues);
    }
}
