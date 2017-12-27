using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Mvvm;
using static FileDispatcherTests.TestUtil;

namespace FileDispatcherTests.ViewModels.TestBase
{
    [TestClass]
    public abstract class BindableTestBase<TBindable> where TBindable : BindableBase
    {
        /// <summary>
        /// An instance of <see cref="BindableBase"/> derived class to be tested.
        /// </summary>
        protected TBindable bindable;
        
        /// <summary>
        /// <see cref="List{string}"/> of property names provided by <see cref="BindableBase.PropertyChanged"/> event.
        /// </summary>
        protected List<string> propertyChangedPropertyNames;

        /// <summary>
        /// When implemented should set property values for <paramref name="bindable"/>.
        /// </summary>
        /// <param name="bindable">Tested <see cref="BindableBase"/> where the properties are set</param>
        protected abstract void SetProperties(TBindable bindable);
        /// <summary>
        /// When implemented should provide property names expected when <see cref="System.ComponentModel.INotifyPropertyChanged.PropertyChanged"/> event is raised by tested
        /// <see cref="BindableDataErrorNotifierBase"/>
        /// </summary>
        /// <returns>Expected property names</returns>
        protected abstract IEnumerable<string> ExpectedPropertyChangedPropertyNames();

        [TestInitialize]
        public virtual void TestInit()
        {
            bindable = CreateBindable();
            propertyChangedPropertyNames = new List<string>();
            bindable.PropertyChanged += (s, e) =>
            {
                propertyChangedPropertyNames.Add(e.PropertyName);
            };
        }

        [TestMethod]
        public void Properties_Set_RaisesPropertyChanged()
        {
            //Act
            SetProperties();
            //Assert
            AssertCollectionsAreEqual(ExpectedPropertyChangedPropertyNames(), propertyChangedPropertyNames);
        }

        /// <summary>
        /// Set property values for tested <see cref="BindableBase"/>.
        /// </summary>
        protected void SetProperties() => SetProperties(bindable);

        /// <summary>
        /// Initializes a new instance of <see cref="TBindable"/> to be tested.
        /// </summary>
        protected virtual TBindable CreateBindable() => (TBindable)Activator.CreateInstance(typeof(TBindable));
    }
}
