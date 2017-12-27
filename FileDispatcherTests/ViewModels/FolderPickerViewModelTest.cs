using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using FileDispatcher.ViewModels;
using FileDispatcherTests.ViewModels.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileDispatcherTests.ViewModels
{
    [TestClass]
    public class FolderPickerViewModelTest : BindableDataErrorNotifierTestBase<FolderPickerViewModel>
    {
        static string validPath;
        static string invalidPath;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            validPath = TestUtil.CreateTestDirectory();
            invalidPath = Path.Combine(validPath, Guid.NewGuid().ToString("N"));
        }

        protected override IEnumerable<string> ExpectedPropertyChangedPropertyNames()
        {
            yield return nameof(FolderPickerViewModel.Item);
        }

        protected override IEnumerable<string> ExpectedErrorsChangedPropertyNames() => ExpectedPropertyChangedPropertyNames();

        protected override void SetProperties(FolderPickerViewModel bindableDataErrorNotifier, bool validValues)
        {
            bindableDataErrorNotifier.Item = validValues ?
                validPath : invalidPath;
        }
    }
}
