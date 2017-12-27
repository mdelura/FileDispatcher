using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace FileDispatcher.ViewModels
{
    public abstract class BindableModelWrapperBase<T> : BindableBase, IBindableModelWrapper
    {
        private readonly T _model;

        public BindableModelWrapperBase(T model)
        {
            _model = model;
        }

        public void UpdateModel() => UpdateModel(_model);

        protected abstract void UpdateModel(T model);
    }
}
