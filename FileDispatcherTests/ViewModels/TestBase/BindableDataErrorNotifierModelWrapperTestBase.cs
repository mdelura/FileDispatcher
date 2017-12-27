using System;
using FileDispatcher.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileDispatcherTests.ViewModels.TestBase
{
    [TestClass]
    public abstract class BindableDataErrorNotifierModelWrapperTestBase<TBindable, TModel> : BindableDataErrorNotifierTestBase<TBindable> 
        where TBindable : BindableDataErrorNotifierModelWrapperBase<TModel>
    {
        protected TModel model;

        /// <summary>
        /// When implemented should determine whether wrapped <see cref="TModel"/> properties were updated as expected
        /// </summary>
        /// <param name="model"><see cref="TModel"/> instance to be checked.</param>
        /// <returns><see langword="true"/> if all property values are correct, otherwise <see langword="false"/></returns>
        protected abstract void AssertModelPropertyValuesAreAsExpected(TBindable bindable, TModel model);

        /// <summary>
        /// When implemented should provide a <see cref="TModel"/> instance with non-default property values
        /// </summary>
        /// <returns></returns>
        protected abstract TModel CreateNonDefaultModel();

        [TestInitialize]
        public override void TestInit()
        {
            model = CreateModel();
            base.TestInit();
        }

        [TestMethod]
        public void UpdateModel_UpdatesModelPropertyValues()
        {
            //Arrange
            SetProperties();
            //Act
            bindable.UpdateModel();
            //Assert
            AssertModelPropertyValuesAreAsExpected();
        }

        [TestMethod]
        public void CreateFromModel_PropertiesAreEqual()
        {
            //Arrange
            TModel model = CreateNonDefaultModel();
            //Act
            TBindable bindableFromModel = CreateBindable(model);

            //Assert
            AssertModelPropertyValuesAreAsExpected(bindableFromModel, model);
        }

        /// <summary>
        /// Determine whether model properties were updated as expected
        /// </summary>
        /// <returns><see langword="true"/> if all property values are correct, otherwise <see langword="false"/></returns>
        protected void AssertModelPropertyValuesAreAsExpected() => AssertModelPropertyValuesAreAsExpected(bindable, model);

        protected virtual TModel CreateModel() => (TModel)Activator.CreateInstance(typeof(TModel));

        protected override TBindable CreateBindable() => CreateBindable(model);

        protected virtual TBindable CreateBindable(TModel model) => (TBindable)Activator.CreateInstance(typeof(TBindable), model);
    }
}
