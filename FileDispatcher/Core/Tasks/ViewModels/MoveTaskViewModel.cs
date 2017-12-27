using System.Collections.Generic;

namespace FileDispatcher.Core.Tasks.ViewModels
{
    public class MoveTaskViewModel : TargetableTaskBaseViewModel<MoveTask>
    {
        public MoveTaskViewModel(IEnumerable<string> existingTaskNames) : this(new MoveTask(new WatcherTrigger()), existingTaskNames)
        {
        }

        public MoveTaskViewModel(MoveTask task, IEnumerable<string> existingTaskNames) : base(task, existingTaskNames)
        {
        }
    }
}
