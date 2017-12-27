using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileDispatcher.Core;
using FileDispatcher.Core.Tasks.ViewModels;
using FileDispatcher.Core.ViewModels;
using FileDispatcherTests.Core.Tasks.TestBase;

namespace FileDispatcherTests.Core.Tasks.ViewModels.TestBase
{
    public class TargetableTaskBaseViewModelMock : TargetableTaskBaseViewModel<TargetableTaskBaseMock>
    {
        public TargetableTaskBaseViewModelMock(TargetableTaskBaseMock task) : base(task, new string[] { })
        {
        }
    }
}
