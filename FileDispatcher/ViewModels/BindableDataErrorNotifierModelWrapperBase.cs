using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDispatcher.ViewModels
{
    public abstract class BindableDataErrorNotifierModelWrapperBase<T> : BindableDataErrorNotifierBase, IBindableModelWrapper
    {
        private readonly T _model;

        public BindableDataErrorNotifierModelWrapperBase(T model)
        {
            _model = model;
        }

        public void UpdateModel()
        {
            if (HasErrors)
                throw new InvalidOperationException($"Update cannot be performed, validation errors are present.");
            UpdateModel(_model);
        }

        protected abstract void UpdateModel(T model);
    }
}
