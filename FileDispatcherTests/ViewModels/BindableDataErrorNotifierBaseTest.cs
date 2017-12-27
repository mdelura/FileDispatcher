using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using FileDispatcher.ViewModels;
using FileDispatcherTests.ViewModels.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileDispatcherTests.ViewModels
{
    [TestClass]
    public class BindableDataErrorNotifierBaseTest : BindableDataErrorNotifierTestBase<BindableDataErrorNotifierMock>
    {
        [TestMethod]
        public void GetErrors_PropertyName_ReturnsExpectedErrorsContent()
        {
            //Arrange
            SetProperties(false);
            string expectedErrorsContent = BindableDataErrorNotifierMock.TextErrorsContent;
            //Act
            string actualErrorsContent = GetErrorsContent(nameof(BindableDataErrorNotifierMock.Text))
                .Single();
            //Assert
            Assert.AreEqual(expectedErrorsContent, actualErrorsContent);
        }

        [TestMethod]
        public void GetErrors_ReturnsExpectedErrorsContent()
        {
            //Arrange
            SetProperties(false);
            string[] expectedErrorsContent = new string[]
            {
                $"{nameof(BindableDataErrorNotifierMock.Text)}.{BindableDataErrorNotifierMock.TextErrorsContent}",
                $"{nameof(BindableDataErrorNotifierMock.Value)}.{BindableDataErrorNotifierMock.ValueErrorsContent }"
            };
            //Act
            string[] actualErrorsContent = bindable.GetErrors()
                .Select(v => v.ErrorContent.ToString())
                .ToArray();
            //Assert
            TestUtil.AssertCollectionsAreEqual(expectedErrorsContent, actualErrorsContent);
        }

        [TestMethod]
        public void SetErrors_SetsErrors()
        {
            //Arrange
            string propertyName = "Value";

            var errorResults = new[]
            {
                new ValidationResult(false, "Test error 1"),
                new ValidationResult(false, "Test error 2"),
                new ValidationResult(false, "Test error 3")
            };

            //Act
            bindable.SetErrors(propertyName, errorResults);

            //Assert
            CollectionAssert.AreEqual(errorResults, bindable.GetErrors(propertyName).ToArray());
        }

        protected override IEnumerable<string> ExpectedPropertyChangedPropertyNames()
        {
            return new string[]
            {
                nameof(BindableDataErrorNotifierMock.Text),
                nameof(BindableDataErrorNotifierMock.Value)
            };
        }
        protected override IEnumerable<string> ExpectedErrorsChangedPropertyNames() => ExpectedPropertyChangedPropertyNames();

        protected override void SetProperties(BindableDataErrorNotifierMock bindableDataErrorNotifier, bool validValues)
        {
            bindableDataErrorNotifier.Text = validValues ?
                BindableDataErrorNotifierMock.ValidText : BindableDataErrorNotifierMock.InvalidText;
            bindableDataErrorNotifier.Value = validValues ?
                BindableDataErrorNotifierMock.ValidValue : BindableDataErrorNotifierMock.InvalidValue; 
        }
    }
}
