using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileDispatcher.ViewModels;

namespace FileDispatcher.Core.ViewModels
{
    public abstract class TriggerViewModel<TTrigger> : BindableDataErrorNotifierModelWrapperBase<TTrigger> where TTrigger : ITrigger
    {

        public TriggerViewModel(TTrigger trigger) : base(trigger)
        {
            _enabled = trigger.Enabled;
        }

        private bool _enabled;

        public bool Enabled
        {
            get => _enabled;
            set => SetProperty(ref _enabled, value);
        }

        protected override void UpdateModel(TTrigger model)
        {
            model.Enabled = _enabled;
        }
    }
}
