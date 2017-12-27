using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileDispatcher.Core.ViewModels;

namespace FileDispatcher.Core.Tasks.ViewModels
{
    public class CopyTaskViewModel : TargetableTaskBaseViewModel<CopyTask>
    {
        public CopyTaskViewModel(IEnumerable<string> existingTaskNames) : this(new CopyTask(new WatcherTrigger()), existingTaskNames)
        {
        }

        public CopyTaskViewModel(CopyTask task, IEnumerable<string> existingTaskNames) : base(task, existingTaskNames)
        {
            _copySubdirectories = task.CopySubdirectories;
        }

        private bool _copySubdirectories;
        public bool CopySubdirectories
        {
            get => _copySubdirectories;
            set => SetProperty(ref _copySubdirectories, value);
        }

        protected override void UpdateModel(CopyTask model)
        {
            base.UpdateModel(model);
            model.CopySubdirectories = _copySubdirectories;
        }
    }
}
