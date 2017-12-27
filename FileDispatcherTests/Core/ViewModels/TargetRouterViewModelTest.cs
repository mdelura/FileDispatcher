using System.Collections.Generic;
using FileDispatcher.Core;
using FileDispatcher.Core.ViewModels;
using FileDispatcherTests.ViewModels.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static FileDispatcherTests.TestUtil;

namespace FileDispatcherTests.ViewModels
{
    [TestClass]
    public class TargetRouterViewModelTest : BindableDataErrorNotifierModelWrapperTestBase<TargetRouterViewModel, TargetRouter>
    {
        protected override void AssertModelPropertyValuesAreAsExpected(TargetRouterViewModel bindable, TargetRouter model)
        {
            Assert.AreEqual(bindable.BaseTargetPath.Item, model.BaseTargetPath);
            Assert.AreEqual(bindable.MatchSubdirectory, model.MatchSubdirectory);
            Assert.AreEqual(bindable.MatchToSimilarFile, model.MatchToSimilarFile);
        }

        protected override TargetRouter CreateNonDefaultModel()
        {
            return new TargetRouter()
            {
                BaseTargetPath = CreateTestDirectory(),
                MatchSubdirectory = Match.Always,
                MatchToSimilarFile = Match.Always
            };
        }

        protected override IEnumerable<string> ExpectedErrorsChangedPropertyNames()
        {
            yield return nameof(TargetRouterViewModel.BaseTargetPath);
        }

        protected override IEnumerable<string> ExpectedPropertyChangedPropertyNames()
        {
            yield return nameof(TargetRouterViewModel.BaseTargetPath);
            yield return nameof(TargetRouterViewModel.MatchSubdirectory);
            yield return nameof(TargetRouterViewModel.MatchToSimilarFile);
        }

        protected override void SetProperties(TargetRouterViewModel bindable, bool validValues)
        {
            bindable.BaseTargetPath.Item = validValues ? CreateTestDirectory() : GetNonExistingPath();
            bindable.MatchSubdirectory = Match.Always;
            bindable.MatchToSimilarFile = Match.Always;
        }
    }
}
