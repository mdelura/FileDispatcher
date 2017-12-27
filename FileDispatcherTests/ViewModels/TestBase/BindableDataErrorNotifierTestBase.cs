using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using FileDispatcher.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static FileDispatcherTests.TestUtil;

namespace FileDispatcherTests.ViewModels.TestBase
{
    [TestClass]
    public abstract class BindableDataErrorNotifierTestBase<TBindableDataErrorNotifier> : BindableTestBase<TBindableDataErrorNotifier> 
        where TBindableDataErrorNotifier : BindableDataErrorNotifierBase
    {
        /// <summary>
        /// <see cref="List{string}"/> of property names provided by <see cref="BindableDataErrorNotifierBase.ErrorsChanged"/> event.
        /// </summary>
        protected List<string> errorsChangedPropertyNames;

        /// <summary>
        /// When implemented should set property values for <paramref name="bindable"/> to either valid values 
        /// if <paramref name="validValues"/> is <see langword="true"/> or to invalid values otherwise.
        /// </summary>
        /// <param name="bindable">Tested <see cref="BindableDataErrorNotifierBase"/> where the properties are set</param>
        /// <param name="validValues">Decide if properties should be set to valid or invalid values</param>
        protected abstract void SetProperties(TBindableDataErrorNotifier bindable, bool validValues);
        /// <summary>
        /// When implemented should provide property names expected when <see cref="BindableDataErrorNotifierBase.ErrorsChanged"/> event is raised by tested
        /// <see cref="BindableDataErrorNotifierBase"/>
        /// </summary>
        /// <returns>Expected errors changed property names</returns>
        protected abstract IEnumerable<string> ExpectedErrorsChangedPropertyNames();
        

        [TestInitialize]
        public override void TestInit()
        {
            base.TestInit();

            errorsChangedPropertyNames = new List<string>();
            bindable.ErrorsChanged += (s, e) => errorsChangedPropertyNames.Add(e.PropertyName);
        }

        [TestMethod]
        public void Properties_SetInvalidValues_RaisesErrorsChanged()
        {
            //Act
            SetProperties(false);
            //Assert
            AssertCollectionsAreEqual(ExpectedErrorsChangedPropertyNames(), errorsChangedPropertyNames);
        }

        [TestMethod]
        public void Properties_SetInvalidThenValidValues_RaisesErrorsChanged()
        {
            bindable.ErrorsChanged += (s, e) =>
            {
                System.Diagnostics.Debug.Print($"ErrorsChanged: {e.PropertyName}");
                System.Diagnostics.Debug.Print($"  Content:\r\n  {string.Join("\r\n  ", bindable.GetErrors(e.PropertyName).Select(v => v.ErrorContent))}");
            };
            //Arrange 
            var expectedErrorsChangedPropertyNames =
                ExpectedErrorsChangedPropertyNames()
                .Concat(ExpectedErrorsChangedPropertyNames());
            //Act
            SetProperties(false);
            SetProperties(true);

            //Assert
            AssertCollectionsAreEqual(expectedErrorsChangedPropertyNames, errorsChangedPropertyNames);
        }

        [TestMethod]
        public void Properties_SetValidValues_NoErrors()
        {
            //Act
            SetProperties(true);
            //Assert
            Assert.IsFalse(bindable.HasErrors);
        }

        [TestMethod]
        public void Properties_SetValidThenValidValues_ErrorsChangedNotRaised()
        {
            //Act
            SetProperties(true);
            SetProperties(true);
            //Assert
            Assert.IsFalse(errorsChangedPropertyNames.Any());
        }

        [TestMethod]
        public void Properties_SetInvalidValues_HasErrors()
        {
            //Act
            SetProperties(false);
            //Assert
            Assert.IsTrue(bindable.HasErrors);
        }

        [TestMethod]
        public void Properties_SetInvalidThenValidValues_NoErrors()
        {
            //Act
            SetProperties(bindable, false);
            SetProperties(bindable, true);
            //Assert
            Assert.IsFalse(bindable.HasErrors, $"Errors:\r\n{string.Join("\r\n", bindable.GetErrors().Select(r => r.ErrorContent))}");
        }

        protected override void SetProperties(TBindableDataErrorNotifier bindable) => SetProperties(bindable, true);

        /// <summary>
        /// Set property values for tested <see cref="BindableDataErrorNotifierBase"/> to valid values
        /// if <paramref name="validValues"/> is <see langword="true"/> or to invalid values otherwise.
        /// </summary>
        /// <param name="validValues">Decide if properties should be set to valid or invalid values</param>
        protected void SetProperties(bool validValues) => SetProperties(bindable, validValues);
        /// <summary>
        /// Get errors content as <see cref="string"/> from tested <see cref="BindableDataErrorNotifierBase"/>
        /// </summary>
        /// <param name="propertyName">Name of the property to get errors content for.</param>
        /// <returns>Errors content as an <see cref="IEnumerable{string}"/></returns>
        protected IEnumerable<string> GetErrorsContent(string propertyName)
        {
            return bindable
                .GetErrors(propertyName)
                .Cast<ValidationResult>()
                .Select(r => r.ErrorContent.ToString());
        }
    }
}
