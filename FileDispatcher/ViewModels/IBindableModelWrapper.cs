using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDispatcher.ViewModels
{
    /// <summary>
    /// Provides base members for wrapping model
    /// </summary>
    /// <typeparam name="T">Type of model to be wrapped</typeparam>
    public interface IBindableModelWrapper : INotifyPropertyChanged
    {
        /// <summary>
        /// When implemented should update the current state of wrapped model
        /// </summary>
        void UpdateModel();
    }
}
