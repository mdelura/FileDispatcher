using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Prism.Mvvm;

namespace FileDispatcher.ViewModels
{
    /// <summary>
    /// A base class for bindable View Models with base implementation of <see cref="INotifyDataErrorInfo"/>
    /// </summary>
    public abstract class BindableDataErrorNotifierBase : BindableBase, INotifyDataErrorInfo
    {
        private readonly ErrorsContainer<ValidationResult> _errorsContainer;

        public BindableDataErrorNotifierBase()
        {
            _errorsContainer = new ErrorsContainer<ValidationResult>(RaiseErrorsChanged);
        }

        #region INotifyDataErrorInfo Members
        public bool HasErrors => _errorsContainer.HasErrors;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Gets the validation errors for a specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property to retrieve validation errors for</param>
        /// <returns>The validation errors for the property</returns>
        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName) => _errorsContainer.GetErrors(propertyName);
        #endregion

        /// <summary>
        /// Gets all validation errors for all properties.
        /// </summary>
        /// <returns>The validation errors for all properties.</returns>
        public IEnumerable<ValidationResult> GetErrors()
        {
            return GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(pi => pi.Name != nameof(INotifyDataErrorInfo.HasErrors))
                .SelectMany(pi => GetErrors(pi.Name)
                    .Select(v => new ValidationResult(
                        v.IsValid, v.ErrorContent is string ? $"{pi.Name}.{v.ErrorContent}" : v.ErrorContent)));
        }

        /// <summary>
        /// Gets the validation errors for a specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property to retrieve validation errors for</param>
        /// <returns>The validation errors for the property</returns>
        public IEnumerable<ValidationResult> GetErrors(string propertyName) => _errorsContainer.GetErrors(propertyName);

        /// <summary>
        /// Sets the validation errors for specified property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="newValidationResults">New validation results.</param>
        public void SetErrors(string propertyName, IEnumerable<ValidationResult> newValidationResults) => _errorsContainer.SetErrors(propertyName, newValidationResults.Where(r => !r.IsValid));

        protected static NotImplementedException PropertyNotImplementedException(string propertyName) => new NotImplementedException($"Property not implemented: {propertyName}");

        protected virtual bool SetPropertyAndValidate<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            return base.SetProperty(ref storage, value, () => SetErrors(propertyName, GetValidationResults(propertyName)), propertyName);
        }

        protected virtual bool SetPropertyAndValidate<T>(ref T storage, T value, Action<bool> onValidated, [CallerMemberName] string propertyName = null)
        {
            return SetProperty(ref storage, value, () =>
            {
                SetErrors(propertyName, GetValidationResults(propertyName));
                onValidated?.Invoke(_errorsContainer.GetErrors(propertyName).Any());
            }, 
            propertyName);
        }

        protected abstract IEnumerable<ValidationResult> GetValidationResults(string propertyName);

        protected void RaisePropertyChangedAndValidate(string propertyName)
        {
            RaisePropertyChanged(propertyName);
            SetErrors(propertyName, GetValidationResults(propertyName));
        }

        protected void RaiseErrorsChanged(string propertyName) => ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }
}
