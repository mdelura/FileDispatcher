using System;
using FileDispatcher.Core;
using FileDispatcher.Core.Tasks;

namespace FileDispatcherTests.Core.Tasks.TestBase
{
    public class TargetableTaskBaseMock : TargetableTaskBase
    {
        public TargetableTaskBaseMock(ITrigger trigger) : base(trigger)
        {
        }

        protected override DispatchedEventArgs PerformDispatch(string sourcePath)
        {
            throw new NotImplementedException();
        }
    }
}