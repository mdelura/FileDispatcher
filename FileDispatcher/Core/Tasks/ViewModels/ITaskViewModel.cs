using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileDispatcher.ViewModels;

namespace FileDispatcher.Core.Tasks.ViewModels
{
    public interface ITaskViewModel : IBindableModelWrapper, INotifyDataErrorInfo, IRequiredFieldsInfo
    {
        string Name { get; set; }

        string TaskType { get; }

        void AddTaskToManager();
        void RemoveTaskFromManager();
    }
}
