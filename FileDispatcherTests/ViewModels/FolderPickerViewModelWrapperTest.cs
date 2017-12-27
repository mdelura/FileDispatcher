using System;
using System.Collections.Generic;
using System.IO;
using FileDispatcher.ViewModels;
using FileDispatcherTests.ViewModels.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static FileDispatcherTests.TestUtil;

namespace FileDispatcherTests.ViewModels
{
    [TestClass]
    public class FolderPickerViewModelWrapperTest : BindableDataErrorNotifierTestBase<FolderPickerViewModel>
    {
        protected override IEnumerable<string> ExpectedErrorsChangedPropertyNames() => ExpectedPropertyChangedPropertyNames();

        protected override IEnumerable<string> ExpectedPropertyChangedPropertyNames()
        {
            yield return nameof(FolderPickerViewModel.Item);
        }

        protected override void SetProperties(FolderPickerViewModel bindable, bool validValues)
        {
            bindable.Item = validValues ? CreateTestDirectory() : GetNonExistingPath();
        }
    }
}
